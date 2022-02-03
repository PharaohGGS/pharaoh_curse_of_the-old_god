using System;

namespace Pharaoh.Tools.DesignPatterns
{
    public class Singleton<T> : object where T : Singleton<T>, new()
    {
        protected Singleton(){}

        protected static T _instance;
        public static T Instance => _instance ??= new T();
    }
}