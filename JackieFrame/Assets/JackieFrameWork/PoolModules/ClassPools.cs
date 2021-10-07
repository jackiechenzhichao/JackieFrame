using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace fs
{
    /// <summary>
    /// 对象池
    /// 1.使用对象池，需要把原有对象的集合清空(list,dictionary等)
    /// @author hannibal
    /// @time 2016-7-24
    /// </summary>
    public sealed class ClassPools<T> where T : new()
    {
        private readonly Stack<T> m_stack = new Stack<T>();

        public int CountAll { get; private set; }
        public int CountActive { get { return CountAll - CountInactive; } }
        public int CountInactive { get { return m_stack.Count; } }
        
        public T Spawn()
        {
            T element;
            if (m_stack.Count == 0)
            {
                element = new T();
                CountAll++;
            }
            else
            {
                element = m_stack.Pop();
            }
            return element;
        }

        public void Despawn(T element)
        {
            if (m_stack.Count > 0 && ReferenceEquals(m_stack.Peek(), element))
                Debuger.LogError("Internal error. Trying to destroy object that is already released to pool.");

            m_stack.Push(element);
        }
    }

    public sealed class CommonClassPools<T> where T : new()
    {
        private static readonly ClassPools<T> m_objectPool = new ClassPools<T>();

        public static T Spawn()
        {
            return m_objectPool.Spawn();
        }

        public static void Despawn(T element)
        {
            m_objectPool.Despawn(element);
        }
    }
}