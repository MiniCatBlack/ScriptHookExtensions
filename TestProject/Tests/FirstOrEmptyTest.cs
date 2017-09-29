//  https://github.com/TORISOUP/UniRx/blob/a36ba7e6deeb2a544f4fd59eb91628c969384edb/Tests/UniRx.Tests/Observable.PagingTest.cs

using System;
using System.Collections.Generic;
using GTA.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniRx;

namespace TestProject
{
    [TestClass]
    public class FirstOrEmptyTest
    {
        [TestMethod]
        public void FirstOrEmpty()
        {
            //  OnNext -> OnError
            var subject = new Subject<int>();
            var list = new List<Notification<int>>();
            {
                subject.FirstOrEmpty().Materialize().Subscribe(list.Add);

                subject.OnNext(10);
                subject.OnError(new Exception());

                Assert.AreEqual(10, list[0].Value);
                Assert.AreEqual(NotificationKind.OnCompleted, list[1].Kind);
            }

            //  OnError
            subject = new Subject<int>();
            list.Clear();
            {
                subject.FirstOrEmpty().Materialize().Subscribe(list.Add);

                subject.OnError(new Exception());

                Assert.AreEqual(NotificationKind.OnError, list[0].Kind);
            }

            //  OnCompleted
            subject = new Subject<int>();
            list.Clear();
            {
                subject.FirstOrEmpty().Materialize().Subscribe(list.Add);

                subject.OnCompleted();

                Assert.AreEqual(NotificationKind.OnCompleted, list[0].Kind);
            }

            //  (OnNext) -> OnCompleted
            subject = new Subject<int>();
            list.Clear();
            {
                subject.FirstOrEmpty(x => x % 2 == 0).Materialize().Subscribe(list.Add);

                subject.OnNext(9);
                subject.OnCompleted();

                Assert.AreEqual(NotificationKind.OnCompleted, list[0].Kind);
            }

            //  (OnNext) -> OnNext
            subject = new Subject<int>();
            list.Clear();
            {
                subject.FirstOrEmpty(x => x % 2 == 0).Materialize().Subscribe(list.Add);

                subject.OnNext(9);
                subject.OnNext(10);
                subject.OnCompleted();

                Assert.AreEqual(10, list[0].Value);
                Assert.AreEqual(NotificationKind.OnCompleted, list[1].Kind);
            }

            //  (OnNext) -> OnError
            subject = new Subject<int>();
            list.Clear();
            {
                subject.FirstOrEmpty(x => x % 2 == 0).Materialize().Subscribe(list.Add);

                subject.OnNext(9);
                subject.OnError(new Exception());

                Assert.AreEqual(NotificationKind.OnError, list[0].Kind);
            }
        }
    }
}