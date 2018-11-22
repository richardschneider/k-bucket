using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makaretu.Collections
{
    /// <summary>
    ///   A node in the <see cref="KBucket{T}"/>.
    /// </summary>
    class Bucket<T>
        where T: class, IContact
    {
        public List<T> Contacts = new List<T>();
        public bool DontSplit;
        public Bucket<T> Left;
        public Bucket<T> Right;

        public bool Contains(T item)
        {
            if (Contacts == null)
            {
                return false;
            }
            return Contacts.Any(c => c.Id.SequenceEqual(item.Id));
        }

        public T Get(byte[] id)
        {
            return Contacts?.FirstOrDefault(c => c.Id.SequenceEqual(id));
        }

        public int IndexOf(byte[] id)
        {
            if (Contacts == null)
                return -1;
            return Contacts.FindIndex(c => c.Id.SequenceEqual(id));
        }

        public int DeepCount()
        {
            var n = 0;
            if (Contacts != null)
                n += Contacts.Count;
            if (Left != null)
                n += Left.DeepCount();
            if (Right != null)
                n += Right.DeepCount();

            return n;
        }

        public IEnumerable<T> AllContacts()
        {
            if (Contacts != null)
            {
                foreach (var contact in Contacts)
                {
                    yield return contact;
                }
            }
            if (Left != null)
            {
                foreach (var contact in Left.AllContacts())
                {
                    yield return contact;
                }
            }
            if (Right != null)
            {
                foreach (var contact in Right.AllContacts())
                {
                    yield return contact;
                }
            }
        }
    }
}
