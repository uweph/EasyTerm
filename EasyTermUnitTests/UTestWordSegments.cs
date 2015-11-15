using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyTermCore;

namespace EasyTermUnitTests
{
    [TestClass]
    public class UTestWordSegments
    {
        [TestMethod]
        public void SegmentWords()
        {
            WordSegments sw = new WordSegments("A  simple text");

            Assert.AreEqual(3, sw.Count);

            Assert.AreEqual(0, sw.GetWordStart(0));
            Assert.AreEqual(1, sw.GetWordEnd(0));
            
            Assert.AreEqual(3, sw.GetWordStart(1));
            Assert.AreEqual(9, sw.GetWordEnd(1));

            Assert.AreEqual(10, sw.GetWordStart(2));
            Assert.AreEqual(14, sw.GetWordEnd(2));
        }

        [TestMethod]
        public void EmptyString()
        {
            WordSegments sw = new WordSegments("");

            Assert.AreEqual(0, sw.Count);
        }

        [TestMethod]
        public void TabbedString()
        {
            WordSegments sw = new WordSegments("one\ttwo");

            Assert.AreEqual(2, sw.Count);

            Assert.AreEqual(0, sw.GetWordStart(0));
            Assert.AreEqual(3, sw.GetWordEnd(0));

            Assert.AreEqual(4, sw.GetWordStart(1));
            Assert.AreEqual(7, sw.GetWordEnd(1));
        }

        [TestMethod]
        public void Hyphen()
        {
            WordSegments sw = new WordSegments("one-two");

            Assert.AreEqual(2, sw.Count);

            Assert.AreEqual(0, sw.GetWordStart(0));
            Assert.AreEqual(3, sw.GetWordEnd(0));

            Assert.AreEqual(4, sw.GetWordStart(1));
            Assert.AreEqual(7, sw.GetWordEnd(1));
        }

    }
}
