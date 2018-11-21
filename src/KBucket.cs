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
        Node root = new Node();
        readonly ReaderWriterLockSlim rwlock = new ReaderWriterLockSlim();

        /// <summary>
        ///   Finds the XOR distance between the two contacts.
        /// </summary>
        public int Distance(IContact a, IContact b)
        {
            throw new NotImplementedException();
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
            root = new Node();
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
            rwlock.EnterReadLock();
            try
            {
                foreach (var contact in this)
                {
                    array[arrayIndex++] = contact;
                }
            }
            finally
            {
                rwlock.ExitReadLock();
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
