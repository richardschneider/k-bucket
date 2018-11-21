using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makaretu.Collections
{
    /// <summary>
    ///   A node in the <see cref="KBucket"/>.
    /// </summary>
    class Bucket
    {
        public List<IContact> Contacts = new List<IContact>();
        public bool DontSplit;
        public Bucket Left;
        public Bucket Right;

        public bool Contains(IContact item)
        {
            if (Contacts == null)
            {
                return false;
            }
            return Contacts.Any(c => c.Id.SequenceEqual(item.Id));
        }

        public IContact Get(byte[] id)
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

        public IEnumerable<IContact> AllContacts()
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
