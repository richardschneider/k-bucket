using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Makaretu.Collections
{
    /// <summary>
    ///   Implementation of a Kademlia DHT k-bucket used for storing contact (peer node) information.
    /// </summary>
    /// <remarks>
    ///   All public methods and properties are thead-safe.
    /// </remarks>
    public class KBucket : ICollection<IContact>
    {
        Bucket root = new Bucket();
        readonly ReaderWriterLockSlim rwlock = new ReaderWriterLockSlim();
        byte[] localContactId;

        /// <summary>
        ///   The number of contacts allowed in a bucket.
        /// </summary>
        /// <value>
        ///   This is the 'K' in KBucket.  Defaults to 20.
        /// </value>
        public int ContactsPerBucket { get; set; } = 20;

        /// <summary>
        ///   The ID of the local contact/peer.
        /// </summary>
        /// <value>
        ///   Defaults to 20 random bytes.
        /// </value>
        public byte[] LocalContactId {
            get
            {
                if (localContactId == null)
                {
                    localContactId = new byte[20];
                    new Random().NextBytes(localContactId);
                }
                return localContactId;
            }
            set
            {
                localContactId = value;
            }
        }

        /// <summary>
        ///   Finds the XOR distance between the two contacts.
        /// </summary>
        public int Distance(IContact a, IContact b)
        {
            Validate(a);
            Validate(b);
            return Distance(a.Id, b.Id);
        }

        /// <summary>
        ///   Finds the XOR distance between the two contact IDs.
        /// </summary>
        public int Distance(byte[] a, byte[] b)
        {
            var distance = 0;
            var i = 0;
            var min = Math.Min(a.Length, b.Length);
            var max = Math.Max(a.Length, b.Length);
            for (; i < min; ++i)
            {
                distance = distance * 256 + (a[i] ^ b[i]);
            }
            for (; i < max; ++i)
            {
                distance = distance * 256 + 255;
            }
            return distance;
        }

        /// <summary>
        ///   Gets the closest contacts to the provided contact.
        /// </summary>
        /// <param name="contact"></param>
        /// <returns>
        ///   An ordered sequence of contact, sorted by closeness. 
        /// </returns>
        /// <remarks>
        ///   "Closest" is the XOR metric of the contact.
        /// </remarks>
        public IEnumerable<IContact> Closest(IContact contact)
        {
            Validate(contact);
            return this
                .Select(c => new { distance = Distance(c.Id, contact.Id), contact = c })
                .OrderBy(a => a.distance)
                .Select(a => a.contact);
        }

        /// <inheritdoc />
        public int Count => root.DeepCount();

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public void Add(IContact item)
        {
            Validate(item);

            rwlock.EnterWriteLock();
            try
            {
                _Add(item);
            }
            finally
            {
                rwlock.ExitWriteLock();
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            root = new Bucket();
        }

        /// <inheritdoc />
        public bool Contains(IContact item)
        {
            Validate(item);

            rwlock.EnterReadLock();
            try
            {
                return _Get(item.Id) != null;
            }
            finally
            {
                rwlock.ExitReadLock();
            }
        }

        /// <inheritdoc />
        public void CopyTo(IContact[] array, int arrayIndex)
        {
            foreach (var contact in this)
            {
                array[arrayIndex++] = contact;
            }
        }

        /// <inheritdoc />
        public IEnumerator<IContact> GetEnumerator()
        {
            rwlock.EnterReadLock();
            try
            {
                foreach (var contact in root.AllContacts())
                {
                    yield return contact;
                }
            }
            finally
            {
                rwlock.ExitReadLock();
            }
        }

        /// <inheritdoc />
        public bool Remove(IContact item)
        {
            Validate(item);

            rwlock.EnterWriteLock();
            try
            {
                return _Remove(item.Id);
            }
            finally
            {
                rwlock.ExitWriteLock();
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void Validate(IContact contact)
        {
            if (contact == null)
                throw new ArgumentNullException("contact");
            if (contact.Id == null || contact.Id.Length == 0)
                throw new ArgumentNullException("contact.Id");
        }

        void _Add(IContact contact)
        {
            var bitIndex = 0;
            var node = root;

            while (node.Contacts == null)
            {
                // this is not a leaf node but an inner node with 'low' and 'high'
                // branches; we will check the appropriate bit of the identifier and
                // delegate to the appropriate node for further processing
                node = _DetermineNode(node, contact.Id, bitIndex++);
            }

            // check if the contact already exists
            if (node.Contains(contact))
            {
                _Update(node, contact);
                return;
            }

            if (node.Contacts.Count < ContactsPerBucket)
            {
                node.Contacts.Add(contact);
                return;
            }

            // the bucket is full
            if (node.DontSplit)
            {
                // we are not allowed to split the bucket
                // we need to ping the first this.numberOfNodesToPing
                // in order to determine if they are alive
                // only if one of the pinged nodes does not respond, can the new contact
                // be added (this prevents DoS flodding with new invalid contacts)

                // TODO: this.emit('ping', node.contacts.slice(0, this.numberOfNodesToPing), contact)
                return;
            }

            _Split(node, bitIndex);
            _Add(contact);
        }

        /**
           * Splits the node, redistributes contacts to the new nodes, and marks the
           * node that was split as an inner node of the binary tree of nodes by
           * setting this.root.contacts = null
           *
           * @param  {Object} node     node for splitting
           * @param  {Number} bitIndex the bitIndex to which byte to check in the
           *                           Uint8Array for navigating the binary tree
           */
        void _Split(Bucket node, int bitIndex)
        {
            node.Left = new Bucket();
            node.Right = new Bucket();

            // redistribute existing contacts amongst the two newly created nodes
            foreach (var contact in node.Contacts)
            {
                _DetermineNode(node, contact.Id, bitIndex)
                    .Contacts.Add(contact);
            }

            node.Contacts = null; // mark as inner tree node

            // don't split the "far away" node
            // we check where the local node would end up and mark the other one as
            // "dontSplit" (i.e. "far away")
            var detNode = _DetermineNode(node, LocalContactId, bitIndex);
            var otherNode = node.Left == detNode ? node.Right : node.Left;
            // TODO: otherNode.DontSplit = true;
        }

        private void _Update(Bucket node, IContact contact)
        {
            // TODO
        }

        /// <summary>
        ///   Determines whether the id at the bitIndex is 0 or 1.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="id"></param>
        /// <param name="bitIndex"></param>
        /// <returns>
        ///   Left leaf if `id` at `bitIndex` is 0, right leaf otherwise
        /// </returns>
        Bucket _DetermineNode(Bucket node, byte[]id, int bitIndex)
        {

            // id's that are too short are put in low bucket (1 byte = 8 bits)
            // (bitIndex >> 3) finds how many bytes the bitIndex describes
            // bitIndex % 8 checks if we have extra bits beyond byte multiples
            // if number of bytes is <= no. of bytes described by bitIndex and there
            // are extra bits to consider, this means id has less bits than what
            // bitIndex describes, id therefore is too short, and will be put in low
            // bucket
            var bytesDescribedByBitIndex = bitIndex >> 3;
            var bitIndexWithinByte = bitIndex % 8;
            if ((id.Length <= bytesDescribedByBitIndex) && (bitIndexWithinByte != 0))
            {
                return node.Left;
            }

            // byteUnderConsideration is an integer from 0 to 255 represented by 8 bits
            // where 255 is 11111111 and 0 is 00000000
            // in order to find out whether the bit at bitIndexWithinByte is set
            // we construct (1 << (7 - bitIndexWithinByte)) which will consist
            // of all bits being 0, with only one bit set to 1
            // for example, if bitIndexWithinByte is 3, we will construct 00010000 by
            // (1 << (7 - 3)) -> (1 << 4) -> 16
            var byteUnderConsideration = id[bytesDescribedByBitIndex];
            if (0 != (byteUnderConsideration & (1 << (7 - bitIndexWithinByte))))
            {
                return node.Right;
            }

            return node.Left;
        }

        /**
   * Get a contact by its exact ID.
   * If this is a leaf, loop through the bucket contents and return the correct
   * contact if we have it or null if not. If this is an inner node, determine
   * which branch of the tree to traverse and repeat.
   *
   * @param  {Uint8Array} id The ID of the contact to fetch.
   * @return {Object|Null}   The contact if available, otherwise null
   */
        IContact _Get(byte[] id)
        {
            var bitIndex = 0;

            var node = root;
            while (node.Contacts == null)
            {
                node = _DetermineNode(node, id, bitIndex++);
            }

            // index of uses contact id for matching
            return node.Get(id);
        }

        bool _Remove(byte[] id)
        {
            var bitIndex = 0;

            var node = root;
            while (node.Contacts == null)
            {
                node = _DetermineNode(node, id, bitIndex++);
            }

            // index of uses contact id for matching
            var index = node.IndexOf(id);
            if (0 <= index)
            {
                node.Contacts.RemoveAt(index);
                return true;
            }

            return false;
        }

    }
}
