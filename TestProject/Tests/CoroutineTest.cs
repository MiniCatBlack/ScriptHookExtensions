using System;
using System.Collections;
using GTA.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniRx;

namespace TestProject
{
    [TestClass]
    public class CoroutineTest
    {
        private CoroutineManager Coroutine { get; } = new CoroutineManager();

        #region RunningTest

        [TestMethod]
        public void RunningTest()
        {
            var logger = new Logger();
            var expected = new Logger(new[] { "S", "1", "P", "2", "P", "3", "P", "F" });

            Coroutine.Start(Coroutine1(logger));
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
            yield return null;

            logger.Add("3");
        }

        #endregion

        #region MultipleRunningTest

        [TestMethod]
        public void MultipleRunningTest()
        {
            var logger = new Logger();
            var expected = new Logger(new[] { "S", "1", "1", "P", "2", "2", "P", "3", "3", "P", "F" });

            Coroutine.Start(Coroutine2A(logger));
            Coroutine.Start(Coroutine2B(logger));
            logger.Add("S");

            Assert.AreEqual(2, Coroutine.Count);

            while (Coroutine.Count > 0)
            {
                Coroutine.Run();
                logger.Add("P");
            }
            logger.Add("F");

            Assert.AreEqual(expected, logger);
        }

        private IEnumerator Coroutine2A(Logger logger)
        {
            logger.Add("1");
            yield return null;

            logger.Add("2");
            yield return null;

            logger.Add("3");
        }

        private IEnumerator Coroutine2B(Logger logger)
        {
            logger.Add("1");
            yield return null;

            logger.Add("2");
            yield return null;

            logger.Add("3");
        }

        #endregion

        #region InnerCoroutineTest

        [TestMethod]
        public void InnerCoroutineTest()
        {
            var logger = new Logger();
            var expected = new Logger(new[] { "S", "1A", "P", "2A", "P", "1B", "P", "2B", "P", "3B", "P", "3A", "P", "F" });

            Coroutine.Start(Coroutine3A(logger));
            logger.Add("S");

            while (Coroutine.Count > 0)
            {
                Coroutine.Run();
                logger.Add("P");
            }
            logger.Add("F");

            Assert.AreEqual(expected, logger);
        }

        private IEnumerator Coroutine3A(Logger logger)
        {
            logger.Add("1A");
            yield return null;

            logger.Add("2A");
            yield return null;

            yield return Coroutine.Start(Coroutine3B(logger));

            logger.Add("3A");
        }

        private IEnumerator Coroutine3B(Logger logger)
        {
            logger.Add("1B");
            yield return null;

            logger.Add("2B");
            yield return null;

            logger.Add("3B");
        }

        #endregion

        #region DisposeTest

        [TestMethod]
        public void DisposeTest()
        {
            var logger = new Logger();
            var expected = new Logger(new[] { "S", "A", "P", "B", "P", "1", "P", "2", "P", "3", "P", "4", "Disposed", "P", "C", "P", "D", "P", "F" });

            Coroutine.Start(Coroutine4(logger));
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
            logger.Add("A");
            yield return null;

            logger.Add("B");
            yield return null;

            yield return Coroutine.Start(new TestEnumerator4(logger));

            logger.Add("C");
            yield return null;

            logger.Add("D");
        }

        internal class TestEnumerator4 : IEnumerator, IDisposable
        {
            private readonly Logger logger;

            private int count;

            public TestEnumerator4(Logger logger)
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

        #region CoroutineHandleTest

        [TestMethod]
        public void CoroutineHandleTest()
        {
            var logger = new Logger();
            var expected = new Logger(new[] { "S", "1", "P", "P", "2", "P", "P", "F" });

            var handle = Coroutine.Start(Coroutine5(logger));
            logger.Add("S");

            Coroutine.Run();
            logger.Add("P");
            Assert.AreEqual(CoroutineState.Running, handle.State);

            handle.Pause();
            Coroutine.Run();
            logger.Add("P");
            Assert.AreEqual(CoroutineState.Pausing, handle.State);

            handle.Resume();
            Coroutine.Run();
            logger.Add("P");
            Assert.AreEqual(CoroutineState.Running, handle.State);

            handle.Kill();
            Coroutine.Run();
            logger.Add("P");
            Assert.AreEqual(CoroutineState.Finished, handle.State);

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
    }
}