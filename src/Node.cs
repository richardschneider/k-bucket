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
    class Node
    {
        public List<IContact> Contacts = new List<IContact>();
        public bool DontSplit;
        public Node Left;
        public Node Right;

        public int DeepCount()
        {
            var n = Contacts.Count;
            if (Left != null)
                n += Left.DeepCount();
            if (Right != null)
                n += Right.DeepCount();

            return n;
        }

        public IEnumerable<IContact> AllContacts()
        {
            foreach (var contact in Contacts)
            {
                yield return contact;
            }
        }
    }
}
