using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pharaoh.Sets
{
    /// <summary>
    /// https://github.com/roboryantron/Unite2017/blob/master/Assets/Code/Sets/RuntimeSet.cs
    /// </summary>
    /// <typeparam description="T"></typeparam>
    public abstract class RuntimeSet<T> : ScriptableObject
    {
        private readonly List<T> _items = new List<T>();

        public void Add(T item)
        {
            if (!_items.Contains(item))
            {
                _items.Add(item);
            }
        }

        public void Remove(T item)
        {
            if (_items.Contains(item))
            {
                _items.Remove(item);
            }
        }
    }
}