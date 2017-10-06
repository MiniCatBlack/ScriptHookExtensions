using System;
using System.Collections;
using GTA.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniRx;

namespace TestProject
{
    [TestClass]
    public class ToYieldInstructionTest
    {
        public CoroutineManager Coroutine { get; } = new CoroutineManager();

        #region RunningTest

        [TestMethod]
        public void RunningTest()
        {
            var logger = new Logger();
            var expected = new Logger(new[] { "S", "1", "P", "Subscribe", "P", "OnNext:Test", "P", "OnNext:Test", "P", "OnNext:Test", "OnCompleted", "P", "Result:Test", "P", "2", "P", "F" });
            var subject = new Subject<string>();

            Coroutine.Start(Coroutine1(logger, subject.AsObservable()));
            logger.Add("S");

            while (Coroutine.Count > 0)
            {
                Coroutine.Run();
                logger.Add("P");

                subject.OnNext("Test");
            }
            logger.Add("F");

            Assert.AreEqual(expected, logger);
        }

        private IEnumerator Coroutine1(Logger logger, UniRx.IObservable<string> stream)
        {
            logger.Add("1");
            yield return null;

            logger.Add("Subscribe");
            var yieldInstruction = stream.Take(3).Do(new TestObserver<string>(logger)).ToYieldInstruction();
            yield return yieldInstruction;

            if (yieldInstruction.HasError)
            {
                logger.Add($"Error:{yieldInstruction.Error.Message}");
                yield break;
            }

            if (yieldInstruction.HasResult)
            {
                logger.Add($"Result:{yieldInstruction.Result}");
            }
            yield return null;

            logger.Add("2");
        }

        #endregion

        #region OnErrorTest

        [TestMethod]
        public void OnErrorTest()
        {
            var logger = new Logger();
            var expected = new Logger(new[] { "S", "1", "P", "Subscribe", "OnError:TestException", "P", "Error:TestException", "P", "F" });
            var subject = new Subject<string>();

            Coroutine.Start(Coroutine2(logger, subject.AsObservable()));
            logger.Add("S");

            while (Coroutine.Count > 0)
            {
                Coroutine.Run();
                logger.Add("P");

                subject.OnError(new Exception("TestException"));
            }
            logger.Add("F");

            Assert.AreEqual(expected, logger);
        }

        private IEnumerator Coroutine2(Logger logger, UniRx.IObservable<string> stream)
        {
            logger.Add("1");
            yield return null;

            logger.Add("Subscribe");
            var yieldInstruction = stream.Do(new TestObserver<string>(logger)).ToYieldInstruction();
            yield return yieldInstruction;

            if (yieldInstruction.HasError)
            {
                logger.Add($"Error:{yieldInstruction.Error.Message}");
                yield break;
            }

            if (yieldInstruction.HasResult)
            {
                logger.Add($"Result:{yieldInstruction.Result}");
            }
            yield return null;

            logger.Add("2");
        }

        #endregion
    }
}