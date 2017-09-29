//  https://github.com/TORISOUP/UniRx/blob/13ea83b38c1bc3bede700a7da17e60f67c3c5f08/Tests/UniRx.Tests/ResetAfterTest.cs

using System;
using GTA.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniRx;

namespace TestProject
{
    [TestClass]
    public class ResetAfterTest
    {
        [TestMethod]
        public void ResetAfter()
        {
            //should publish default value
            var results = Observable.Concat(
                    Observable.Return(1),
                    Observable.Return(2).Delay(TimeSpan.FromSeconds(0.5)),
                    Observable.Return(3).Delay(TimeSpan.FromSeconds(1.1))
                )
                .ResetAfter(TimeSpan.FromSeconds(1))
                .Materialize()
                .ToArray()
                .Wait();

            Assert.AreEqual(5, results.Length);
            Assert.AreEqual(1, results[0].Value);
            Assert.AreEqual(2, results[1].Value);
            Assert.AreEqual(0, results[2].Value);   //default value
            Assert.AreEqual(3, results[3].Value);
            Assert.AreEqual(NotificationKind.OnCompleted, results[4].Kind);
        }

        [TestMethod]
        public void ResetAfter2()
        {
            //  should measure time from the last message
            var results = Observable.Concat(
                    Observable.Return(1).Delay(TimeSpan.FromSeconds(0.1)),
                    Observable.Return(2).Delay(TimeSpan.FromSeconds(0.1)),
                    Observable.Return(3).Delay(TimeSpan.FromSeconds(1.1)), // after 1 second from previous message
                    Observable.Return(4).Delay(TimeSpan.FromSeconds(0.1))
                )
                .ResetAfter(TimeSpan.FromSeconds(1))
                .Materialize()
                .ToArray()
                .Wait();

            Assert.AreEqual(6, results.Length);
            Assert.AreEqual(1, results[0].Value);
            Assert.AreEqual(2, results[1].Value);
            Assert.AreEqual(0, results[2].Value);   //default value
            Assert.AreEqual(3, results[3].Value);
            Assert.AreEqual(4, results[4].Value);
            Assert.AreEqual(NotificationKind.OnCompleted, results[5].Kind);
        }


        [TestMethod]
        public void ResetAfter3()
        {
            //should publish default value even if last message is the same as default value 
            var results = Observable.Concat(
                    Observable.Return(0),
                    Observable.Return(5).Delay(TimeSpan.FromSeconds(1.5))
                )
                .ResetAfter(TimeSpan.FromSeconds(1))
                .Materialize()
                .ToArray()
                .Wait();

            Assert.AreEqual(4, results.Length);
            Assert.AreEqual(0, results[0].Value);
            Assert.AreEqual(0, results[1].Value);   //default value
            Assert.AreEqual(5, results[2].Value);
            Assert.AreEqual(NotificationKind.OnCompleted, results[3].Kind);
        }

        [TestMethod]
        public void ResetAfter4()
        {
            //should be able to set any value as default
            var results = Observable.Concat(
                    Observable.Return("first"),
                    Observable.Return("second").Delay(TimeSpan.FromSeconds(1.5))
                )
                .ResetAfter("default", TimeSpan.FromSeconds(1))
                .Materialize()
                .ToArray()
                .Wait();

            Assert.AreEqual(4, results.Length);
            Assert.AreEqual("first", results[0].Value);
            Assert.AreEqual("default", results[1].Value);   //default value
            Assert.AreEqual("second", results[2].Value);
            Assert.AreEqual(NotificationKind.OnCompleted, results[3].Kind);
        }

        [TestMethod]
        public void ResetAfter5()
        {
            //should publish OnCompleted immediately when parent observer is finished
            var results = Observable.Return(1)
                .ResetAfter(TimeSpan.FromSeconds(1))
                .Materialize()
                .ToArray()
                .Wait();

            Assert.AreEqual(2, results.Length);
            Assert.AreEqual(1, results[0].Value);
            Assert.AreEqual(NotificationKind.OnCompleted, results[1].Kind);
        }


        [TestMethod]
        public void ResetAfter6()
        {
            //should publish OnError immediately
            var results = Observable.Return(1)
                .Concat(Observable.Throw<int>(new Exception("error occurred.")))
                .ResetAfter(TimeSpan.FromSeconds(1))
                .Materialize()
                .ToArray()
                .Wait();

            Assert.AreEqual(2, results.Length);
            Assert.AreEqual(1, results[0].Value);
            Assert.AreEqual(NotificationKind.OnError, results[1].Kind);
        }
    }
}