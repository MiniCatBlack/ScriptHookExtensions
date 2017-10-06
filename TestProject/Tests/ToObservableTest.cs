using System;
using System.Collections;
using GTA.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniRx;

namespace TestProject
{
    [TestClass]
    public class ToObservableTest
    {
        public CoroutineManager Coroutine { get; } = new CoroutineManager();

        #region RunningTest

        [TestMethod]
        public void RunningTest()
        {
            var logger = new Logger();
            var expected = new Logger(new[] { "S", "1", "P", "2", "OnNext:Test", "P", "3", "OnCompleted", "P", "F" });

            Coroutine.ToObservable<string>(() => Coroutine1(logger)).Subscribe(new TestObserver<string>(logger));
            logger.Add("S");

            while (Coroutine.Count > 0)
            {
                Coroutine.Run();
                logger.Add("P");
            }
            logger.Add("F");

            Assert.AreEqual(expected, logger);
        }

        private IEnumerator Coroutine1(Logger logger)
        {
            logger.Add("1");
            yield return null;

            logger.Add("2");
            yield return "Test";

            logger.Add("3");
        }

        #endregion

        #region OnErrorTest

        [TestMethod]
        public void OnErrorTest()
        {
            var logger = new Logger();
            var expected = new Logger(new[] { "S", "1", "P", "OnError:TestException", "P", "F" });

            Coroutine.ToObservable<Unit>(() => Coroutine2(logger)).Subscribe(new TestObserver<Unit>(logger));
            logger.Add("S");

            while (Coroutine.Count > 0)
            {
                Coroutine.Run();
                logger.Add("P");
            }
            logger.Add("F");

            Assert.AreEqual(expected, logger);
        }

        private IEnumerator Coroutine2(Logger logger)
        {
            logger.Add("1");
            yield return null;

            throw new Exception("TestException");
        }

        #endregion

        #region UnsubscribeTest

        [TestMethod]
        public void UnsubscribeTest()
        {
            var logger = new Logger();
            var expected = new Logger(new[] { "S", "1", "OnNext:1", "P", "2", "OnNext:2", "P", "3", "OnNext:3", "P", "OnCompleted", "P", "F" });

            var subscription = Coroutine.ToObservable<int>(() => Coroutine3(logger)).Subscribe(new TestObserver<int>(logger));
            logger.Add("S");

            int count = 0;
            while (Coroutine.Count > 0)
            {
                Coroutine.Run();
                logger.Add("P");

                count++;
                if (count == 3) subscription.Dispose();
            }
            logger.Add("F");

            Assert.AreEqual(expected, logger);
        }

        private IEnumerator Coroutine3(Logger logger)
        {
            for (int i = 1; i <= 1000; i++)
            {
                logger.Add($"{i}");

                yield return i;
            }
        }

        #endregion

        #region MultipleSubscribeTest

        [TestMethod]
        public void MultipleSubscribeTest()
        {
            var logger = new Logger();
            var expected = new Logger(new[] { "S", "1", "1", "P", "2", "2", "P", "3", "OnCompleted", "3", "OnCompleted", "P", "F" });

            var observable = Coroutine.ToObservable<Unit>(() => Coroutine4(logger));
            observable.Subscribe(new TestObserver<Unit>(logger));
            observable.Subscribe(new TestObserver<Unit>(logger));
            logger.Add("S");

            while (Coroutine.Count > 0)
            {
                Coroutine.Run();
                logger.Add("P");
            }
            logger.Add("F");

            Assert.AreEqual(expected, logger);
        }

        private IEnumerator Coroutine4(Logger logger)
        {
            logger.Add("1");
            yield return null;

            logger.Add("2");
            yield return null;

            logger.Add("3");
        }

        #endregion

        #region HotMultipleSubscribeTest

        [TestMethod]
        public void HotMultipleSubscribeTest()
        {
            var logger = new Logger();
            var expected = new Logger(new[] { "S", "1", "P", "2", "P", "3", "OnCompleted", "OnCompleted", "P", "F" });

            var observable = Coroutine.ToObservable<Unit>(() => Coroutine5(logger)).Publish().RefCount();
            observable.Subscribe(new TestObserver<Unit>(logger));
            observable.Subscribe(new TestObserver<Unit>(logger));
            logger.Add("S");

            while (Coroutine.Count > 0)
            {
                Coroutine.Run();
                logger.Add("P");
            }
            logger.Add("F");

            Assert.AreEqual(expected, logger);
        }

        private IEnumerator Coroutine5(Logger logger)
        {
            logger.Add("1");
            yield return null;

            logger.Add("2");
            yield return null;

            logger.Add("3");
        }

        #endregion

        #region YielReturnExceptionTest

        [TestMethod]
        public void YieldReturnExceptionTest()
        {
            var logger = new Logger();
            var expected = new Logger(new[] { "S", "1", "OnError:TestException", "P", "F" });

            Coroutine.ToObservable<Unit>(() => Coroutine6(logger)).Subscribe(new TestObserver<Unit>(logger));
            logger.Add("S");

            while (Coroutine.Count > 0)
            {
                Coroutine.Run();
                logger.Add("P");
            }
            logger.Add("F");

            Assert.AreEqual(expected, logger);
        }

        private IEnumerator Coroutine6(Logger logger)
        {
            logger.Add("1");
            yield return new Exception("TestException");

            logger.Add("2");
        }

        #endregion

        #region ObserveExceptionTest

        public void ObserveExceptionTest()
        {
            var logger = new Logger();
            var expected = new Logger(new[] { "S", "1", "OnNext:TestException", "P", "2", "OnNext:TestException", "P", "3", "P", "4", "OnCompleted", "P", "F" });

            Coroutine.ToObservable<Exception>(() => Coroutine7(logger)).Subscribe(new TestObserver<Exception>(logger));
            logger.Add("S");

            while (Coroutine.Count > 0)
            {
                Coroutine.Run();
                logger.Add("P");
            }
            logger.Add("F");

            Assert.AreEqual(expected, logger);
        }

        private IEnumerator Coroutine7(Logger logger)
        {
            logger.Add("1");
            yield return new Exception("TestException");

            logger.Add("2");
            yield return new InvalidOperationException("TestException");

            logger.Add("3");
            yield return null;

            logger.Add("4");
        }

        #endregion

        #region DisposeTest

        [TestMethod]
        public void DisposeTest()
        {
            var logger = new Logger();
            var expected = new Logger(new[] { "S", "1", "P", "OnCompleted", "Disposed", "P", "F" });

            var cancellation = Coroutine.ToObservable<Exception>(() => new TestEnumerator8(logger)).Subscribe(new TestObserver<Exception>(logger));
            logger.Add("S");

            while (Coroutine.Count > 0)
            {
                Coroutine.Run();
                logger.Add("P");
                cancellation.Dispose();
            }
            logger.Add("F");

            Assert.AreEqual(expected, logger);
        }

        internal class TestEnumerator8 : IEnumerator, IDisposable
        {
            private readonly Logger logger;

            private int count;

            public TestEnumerator8(Logger logger)
            {
                this.logger = logger;
            }

            public bool MoveNext()
            {
                if (count < 3)
                {
                    count++;
                    logger.Add($"{count}");
                    return true;
                }

                logger.Add("4");
                return false;
            }

            public void Dispose()
            {
                logger.Add("Disposed");
            }

            public object Current => null;

            public void Reset() => throw new NotSupportedException();
        }

        #endregion
    }
}