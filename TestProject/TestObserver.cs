using System;
using UniRx;

namespace TestProject
{
    public class TestObserver<T> : UniRx.IObserver<T>
    {
        private readonly Logger logger;

        public TestObserver(Logger logger)
        {
            this.logger = logger;
        }

        public void OnNext(T value)
        {
            logger.Add($"OnNext:{value}");
        }

        public void OnError(Exception error)
        {
            logger.Add($"OnError:{error}");
        }

        public void OnCompleted()
        {
            logger.Add("OnCompleted");
        }
    }
}