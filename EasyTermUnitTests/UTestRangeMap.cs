using System;
using DnEasyTerm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyTermUnitTests
{
    [TestClass]
    public class UTestRangeMap
    {
        [TestMethod]
        public void RangeBefore()
        {
            RangeMap map = new RangeMap();

            map.AddRange(20, 3);
            map.AddRange(15, 3);
            map.AddRange(5, 6);

            Assert.AreEqual(3,  map.Ranges.Count);
            Assert.AreEqual(5,  map.Ranges[0].From);
            Assert.AreEqual(10,  map.Ranges[0].To);
            Assert.AreEqual(15,  map.Ranges[1].From);
            Assert.AreEqual(17, map.Ranges[1].To);
            Assert.AreEqual(20,  map.Ranges[2].From);
            Assert.AreEqual(22, map.Ranges[2].To);

        }

        [TestMethod]
        public void RangeAfter()
        {
            RangeMap map = new RangeMap();

            map.AddRange(5, 6);
            map.AddRange(15, 3);
            map.AddRange(20, 3);

            Assert.AreEqual(3,  map.Ranges.Count);
            Assert.AreEqual(5,  map.Ranges[0].From);
            Assert.AreEqual(10,  map.Ranges[0].To);
            Assert.AreEqual(15,  map.Ranges[1].From);
            Assert.AreEqual(17, map.Ranges[1].To);
            Assert.AreEqual(20,  map.Ranges[2].From);
            Assert.AreEqual(22, map.Ranges[2].To);
        }

        [TestMethod]
        public void RangeBetween()
        {
            RangeMap map = new RangeMap();

            map.AddRange(5, 6);
            map.AddRange(30, 3);
            map.AddRange(15, 3);

            Assert.AreEqual(3,  map.Ranges.Count);
            Assert.AreEqual(5,  map.Ranges[0].From);
            Assert.AreEqual(10,  map.Ranges[0].To);
            Assert.AreEqual(15,  map.Ranges[1].From);
            Assert.AreEqual(17, map.Ranges[1].To);
            Assert.AreEqual(30,  map.Ranges[2].From);
            Assert.AreEqual(32, map.Ranges[2].To);

            map.AddRange(20, 5);
            Assert.AreEqual(4,  map.Ranges.Count);
            Assert.AreEqual(20,  map.Ranges[2].From);
            Assert.AreEqual(24, map.Ranges[2].To);

        }

        [TestMethod]
        public void RangeOverlap1()
        {
            RangeMap map = new RangeMap();
            map.AddRange(10, 11);
            map.AddRange(30, 11);
            map.AddRange(50, 11);

            map.AddRange(5, 44);

            Assert.AreEqual(2, map.Ranges.Count);
            Assert.AreEqual(5, map.Ranges[0].From);
            Assert.AreEqual(48, map.Ranges[0].To);
            Assert.AreEqual(50, map.Ranges[1].From);
            Assert.AreEqual(60, map.Ranges[1].To);

        }
    
        [TestMethod]
        public void RangeOverlap2()
        {
            RangeMap map = new RangeMap();
            map.AddRange(10, 11);
            map.AddRange(30, 11);
            map.AddRange(50, 11);

            map.AddRange(5, 25);

            Assert.AreEqual(2, map.Ranges.Count);
            Assert.AreEqual(5, map.Ranges[0].From);
            Assert.AreEqual(40, map.Ranges[0].To);
            Assert.AreEqual(50, map.Ranges[1].From);
            Assert.AreEqual(60, map.Ranges[1].To);

        }

        [TestMethod]
        public void RangeOverlap3()
        {
            RangeMap map = new RangeMap();
            map.AddRange(10, 11);
            map.AddRange(30, 11);
            map.AddRange(50, 11);

            map.AddRange(15, 41);

            Assert.AreEqual(1, map.Ranges.Count);
            Assert.AreEqual(10, map.Ranges[0].From);
            Assert.AreEqual(60, map.Ranges[0].To);

        }

        [TestMethod]
        public void RangeOverlap4()
        {
            RangeMap map = new RangeMap();
            map.AddRange(10, 11);
            map.AddRange(30, 11);
            map.AddRange(50, 11);

            map.AddRange(21, 9);

            Assert.AreEqual(2, map.Ranges.Count);
            Assert.AreEqual(10, map.Ranges[0].From);
            Assert.AreEqual(40, map.Ranges[0].To);
            Assert.AreEqual(50, map.Ranges[1].From);
            Assert.AreEqual(60, map.Ranges[1].To);

        }

        [TestMethod]
        public void RangeOverlap5()
        {
            RangeMap map = new RangeMap();
            map.AddRange(10, 11);
            map.AddRange(22, 9);
            map.AddRange(32, 9);

            map.AddRange(21, 11);

            Assert.AreEqual(1, map.Ranges.Count);
            Assert.AreEqual(10, map.Ranges[0].From);
            Assert.AreEqual(40, map.Ranges[0].To);

        }

        [TestMethod]
        public void CheckOverlapWithEmptyRanges()
        {
            RangeMap map = new RangeMap();

            Assert.IsFalse(map.OverlapsRange(5, 5));

        }

        [TestMethod]
        public void CheckOverlapStartBeforeRange()
        {
            RangeMap map = new RangeMap();
            map.AddRange(10, 11); // - 20
            map.AddRange(30, 11); // - 40
            map.AddRange(50, 11); // - 60

            // Start before first range
            Assert.IsFalse(map.OverlapsRange(5, 5)); // - 9
            Assert.IsTrue(map.OverlapsRange(5, 6));  // - 10
            Assert.IsTrue(map.OverlapsRange(5, 21)); // - 25
            Assert.IsTrue(map.OverlapsRange(5, 80)); // - 84


            // Start before last range
            Assert.IsFalse(map.OverlapsRange(45, 5)); // - 49
            Assert.IsTrue(map.OverlapsRange(45, 6));  // 
            Assert.IsTrue(map.OverlapsRange(45, 21)); // 
            Assert.IsTrue(map.OverlapsRange(45, 80)); // 

        }

        [TestMethod]
        public void CheckOverlapStartInRange()
        {
            RangeMap map = new RangeMap();
            map.AddRange(10, 11); // - 20
            map.AddRange(30, 11); // - 40
            map.AddRange(50, 11); // - 60

            // Start before first range
            Assert.IsTrue(map.OverlapsRange(15, 2)); 
            Assert.IsTrue(map.OverlapsRange(15, 7)); 
            Assert.IsTrue(map.OverlapsRange(15, 15));
            Assert.IsTrue(map.OverlapsRange(15, 80));


            // Start in last range
            Assert.IsTrue(map.OverlapsRange(55, 2)); 
            Assert.IsTrue(map.OverlapsRange(55, 7)); 
            Assert.IsTrue(map.OverlapsRange(55, 15));
            Assert.IsTrue(map.OverlapsRange(55, 80));


        }

        [TestMethod]
        public void CheckOverlapStartAfterRange()
        {
            RangeMap map = new RangeMap();
            map.AddRange(10, 11); // - 20
            map.AddRange(30, 11); // - 40
            map.AddRange(50, 11); // - 60

            // Start before first range
            Assert.IsTrue(map.OverlapsRange(60, 2)); 
            Assert.IsFalse(map.OverlapsRange(61, 7)); 
            Assert.IsFalse(map.OverlapsRange(100, 7)); 
        }

    }
}
