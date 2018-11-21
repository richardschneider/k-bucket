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

        /// <summary>
        ///   The number of contacts allowed in a bucket.
        /// </summary>
        /// <value>
        ///   This is the 'K' in KBucket.  Defaults to 20.
        /// </value>
        public int ContactsPerBucket { get; set; } = 20;

        /// <summary>
        ///   Finds the XOR distance between the two contacts.
        /// </summary>
        public int Distance(IContact a, IContact b)
        {
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
        public IEnumerable<IContact> FindClosest(IContact contact)
        {
            throw new NotImplementedException();
        }

        #region ICollection
        /// <inheritdoc />
        public int Count => root.DeepCount();

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public void Add(IContact item)
        {
            rwlock.EnterWriteLock();
            try
            {
                // TODO
                root.Contacts.Add(item);
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
            rwlock.EnterReadLock();
            try
            {
                // TODO
                throw new NotImplementedException();
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
            rwlock.EnterWriteLock();
            try
            {
                // TODO
                throw new NotImplementedException();
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
        #endregion
    }
}
