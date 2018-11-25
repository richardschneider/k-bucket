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
    ///   From https://github.com/tristanls/k-bucket/blob/master/test/determineNode.js
    /// </summary>
    [TestClass]
    public class DetermineNodeTest
    {
        static readonly Bucket<Contact> left = new Bucket<Contact>();
        static readonly Bucket<Contact> right = new Bucket<Contact>();
        static readonly Bucket<Contact> root = new Bucket<Contact> { Left = left, Right = right };

        [TestMethod]
        public void Tests()
        {
            var kBucket = new KBucket<Contact>();
            Bucket<Contact> actual;

            actual = kBucket._DetermineNode(root, new byte[] { 0x00 }, 0);
            Assert.AreSame(left, actual);

            actual = kBucket._DetermineNode(root, new byte[] { 0x40 }, 0);
            Assert.AreSame(left, actual);

            actual = kBucket._DetermineNode(root, new byte[] { 0x40 }, 1);
            Assert.AreSame(right, actual);

            actual = kBucket._DetermineNode(root, new byte[] { 0x40 }, 2);
            Assert.AreSame(left, actual);

            actual = kBucket._DetermineNode(root, new byte[] { 0x40 }, 9);
            Assert.AreSame(left, actual);

            actual = kBucket._DetermineNode(root, new byte[] { 0x41 }, 7);
            Assert.AreSame(right, actual);

            actual = kBucket._DetermineNode(root, new byte[] { 0x41, 0x00 }, 7);
            Assert.AreSame(right, actual);

            actual = kBucket._DetermineNode(root, new byte[] { 0x00, 0x41, 0x00 }, 15);
            Assert.AreSame(right, actual);
        }
    }
}
