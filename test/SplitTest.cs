using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makaretu.Collections
{
    /// <summary>
    ///   From https://github.com/tristanls/k-bucket/blob/master/test/split.js
    /// </summary>
    [TestClass]
    public class SplitTest
    {
        [TestMethod]
        public void OneContactDoesNotSplit()
        {
            var kBucket = new KBucket<Contact>();
            kBucket.Add(new Contact("a"));
            Assert.IsNull(kBucket.Root.Left);
            Assert.IsNull(kBucket.Root.Right);
            Assert.IsNotNull(kBucket.Root.Contacts);
        }

        [TestMethod]
        public void MaxContactsPerNodeDoesNotSplit()
        {
            var kBucket = new KBucket<Contact>();
            for (var i = 0; i < kBucket.ContactsPerBucket; ++i)
            {
                kBucket.Add(new Contact(i));
            }

            Assert.IsNull(kBucket.Root.Left);
            Assert.IsNull(kBucket.Root.Right);
            Assert.IsNotNull(kBucket.Root.Contacts);
        }

        [TestMethod]
        public void MaxContactsPerNodePlusOneDoetSplit()
        {
            var kBucket = new KBucket<Contact>();
            for (var i = 0; i < kBucket.ContactsPerBucket + 1; ++i)
            {
                kBucket.Add(new Contact(i));
            }

            Assert.IsNotNull(kBucket.Root.Left);
            Assert.IsNotNull(kBucket.Root.Right);
            Assert.IsNull(kBucket.Root.Contacts);
        }

        [TestMethod]
        public void SplitNodesContainsAllContacts()
        {
            var kBucket = new KBucket<Contact>
            {
                LocalContactId = new byte[] { 0x00 }
            };
            var contacts = new List<Contact>();
            for (var i = 0; i < kBucket.ContactsPerBucket + 1; ++i)
            {
                var contact = new Contact((byte)i);
                contacts.Add(contact);
                kBucket.Add(contact);
            }

            foreach (var contact in contacts)
            {
                Assert.IsTrue(kBucket.Contains(contact));
            }
        }

        [TestMethod]
        public void FarAway()
        {
            var kBucket = new KBucket<Contact>
            {
                LocalContactId = new byte[] { 0x00 }
            };
            for (var i = 0; i < kBucket.ContactsPerBucket + 1; ++i)
            {
                kBucket.Add(new Contact((byte)i));
            }

            // since localNodeId is 0x00, we expect every right node to be "far" and
            // therefore marked as "dontSplit = true"
            // there will be one "left" node and four "right" nodes (t.expect(5)) 
            traverse(kBucket.Root, false);
        }

        void traverse (Bucket<Contact> node, bool dontSplit)
        {
            if (node.Contacts == null)
            {
                traverse(node.Left, false);
                traverse(node.Right, true);
            }
            else
            {
                Assert.AreEqual(dontSplit, node.DontSplit);
            }
        }
    }
}
