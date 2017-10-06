using System;
using System.Collections.Generic;

namespace TestProject
{
    public sealed class Logger : IEquatable<Logger>
    {
        private readonly object lockObject = new object();

        private readonly List<string> list = new List<string>();

        private string str;

        public Logger()
        {
            list = new List<string>();
        }

        public Logger(IEnumerable<string> collection)
        {
            list.AddRange(collection);
        }

        public void Add(string str)
        {
            lock (lockObject)
            {
                list.Add(str);
                str = null;
            }
        }

        public void Clear()
        {
            lock (lockObject)
            {
                list.Clear();
                str = null;
            }
        }

        public bool Equals(Logger other)
        {
            if (other == null) return false;

            var str1 = ToString();
            var str2 = other.ToString();
            return str1 == str2;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Logger);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            lock (lockObject)
            {
                if (str == null)
                {
                    str = string.Join(string.Empty, list);
                }

                return str;
            }
        }
    }
}