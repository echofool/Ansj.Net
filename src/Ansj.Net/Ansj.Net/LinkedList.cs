using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Ansj.Net
{
    /*
     * http://docs.oracle.com/javase/7/docs/api/java/util/LinkedList.html
     */

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LinkedList<T> : ICollection<T>, IEnumerable<T>, ICollection, IEnumerable, ISerializable,
        IDeserializationCallback
    {
        private readonly System.Collections.Generic.LinkedList<T> _linkedList;

        public LinkedList() : this(new System.Collections.Generic.LinkedList<T>())
        {
        }

        public LinkedList(System.Collections.Generic.LinkedList<T> linkedList)
        {
            _linkedList = linkedList;
        }

        public T this[int index]
        {
            get { return FindNode(index).Value; }
            set { FindNode(index).Value = value; }
        }

        public LinkedListNode<T> First
        {
            get { return _linkedList.First; }
        }

        public LinkedListNode<T> Last
        {
            get { return _linkedList.Last; }
        }

        /// <summary>
        ///     获取 <see cref="T:System.Collections.ICollection" /> 中包含的元素数。
        /// </summary>
        /// <returns>
        ///     <see cref="T:System.Collections.ICollection" /> 中包含的元素数。
        /// </returns>
        public int Count
        {
            get { return _linkedList.Count; }
        }

        /// <summary>
        ///     从特定的 <see cref="T:System.Array" /> 索引处开始，将 <see cref="T:System.Collections.ICollection" /> 的元素复制到一个
        ///     <see cref="T:System.Array" /> 中。
        /// </summary>
        /// <param name="array">
        ///     作为从 <see cref="T:System.Collections.ICollection" /> 复制的元素的目标位置的一维 <see cref="T:System.Array" />。
        ///     <see cref="T:System.Array" /> 必须具有从零开始的索引。
        /// </param>
        /// <param name="index"><paramref name="array" /> 中从零开始的索引，将在此处开始复制。</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array" /> 为 null。</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index" /> 小于零。</exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="array" /> 是多维的。- 或 -源
        ///     <see cref="T:System.Collections.ICollection" /> 中的元素数目大于从 <paramref name="index" /> 到目标 <paramref name="array" />
        ///     末尾之间的可用空间。
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     源 <see cref="T:System.Collections.ICollection" /> 的类型无法自动转换为目标
        ///     <paramref name="array" /> 的类型。
        /// </exception>
        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection) _linkedList).CopyTo(array, index);
        }

        /// <summary>
        ///     获取一个可用于同步对 <see cref="T:System.Collections.ICollection" /> 的访问的对象。
        /// </summary>
        /// <returns>
        ///     可用于同步对 <see cref="T:System.Collections.ICollection" /> 的访问的对象。
        /// </returns>
        object ICollection.SyncRoot
        {
            get { return ((ICollection) _linkedList).SyncRoot; }
        }

        /// <summary>
        ///     获取一个值，该值指示是否同步对 <see cref="T:System.Collections.ICollection" /> 的访问（线程安全）。
        /// </summary>
        /// <returns>
        ///     如果对 <see cref="T:System.Collections.ICollection" /> 的访问是同步的（线程安全），则为 true；否则为 false。
        /// </returns>
        bool ICollection.IsSynchronized
        {
            get { return ((ICollection) _linkedList).IsSynchronized; }
        }

        /// <summary>
        ///     从 <see cref="T:System.Collections.Generic.ICollection`1" /> 中移除所有项。
        /// </summary>
        /// <exception cref="T:System.NotSupportedException"><see cref="T:System.Collections.Generic.ICollection`1" /> 是只读的。</exception>
        public void Clear()
        {
            _linkedList.Clear();
        }

        /// <summary>
        ///     确定 <see cref="T:System.Collections.Generic.ICollection`1" /> 是否包含特定值。
        /// </summary>
        /// <returns>
        ///     如果在 <see cref="T:System.Collections.Generic.ICollection`1" /> 中找到 <paramref name="item" />，则为 true；否则为 false。
        /// </returns>
        /// <param name="item">要在 <see cref="T:System.Collections.Generic.ICollection`1" /> 中定位的对象。</param>
        public bool Contains(T item)
        {
            return _linkedList.Contains(item);
        }

        /// <summary>
        ///     从特定的 <see cref="T:System.Array" /> 索引处开始，将 <see cref="T:System.Collections.Generic.ICollection`1" /> 的元素复制到一个
        ///     <see cref="T:System.Array" /> 中。
        /// </summary>
        /// <param name="array">
        ///     作为从 <see cref="T:System.Collections.Generic.ICollection`1" /> 复制的元素的目标位置的一维
        ///     <see cref="T:System.Array" />。<see cref="T:System.Array" /> 必须具有从零开始的索引。
        /// </param>
        /// <param name="arrayIndex"><paramref name="array" /> 中从零开始的索引，将在此处开始复制。</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array" /> 为 null。</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex" /> 小于 0。</exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="array" /> 是多维数组。- 或 -源
        ///     <see cref="T:System.Collections.Generic.ICollection`1" /> 中的元素数大于从 <paramref name="arrayIndex" /> 到目标
        ///     <paramref name="array" /> 结尾处之间的可用空间。- 或 -无法自动将类型 <paramref name="T" /> 强制转换为目标 <paramref name="array" /> 的类型。
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _linkedList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        ///     从 <see cref="T:System.Collections.Generic.ICollection`1" /> 中移除特定对象的第一个匹配项。
        /// </summary>
        /// <returns>
        ///     如果已从 <see cref="T:System.Collections.Generic.ICollection`1" /> 中成功移除 <paramref name="item" />，则为 true；否则为
        ///     false。如果在原始 <see cref="T:System.Collections.Generic.ICollection`1" /> 中没有找到 <paramref name="item" />，该方法也会返回 false。
        /// </returns>
        /// <param name="item">要从 <see cref="T:System.Collections.Generic.ICollection`1" /> 中移除的对象。</param>
        /// <exception cref="T:System.NotSupportedException"><see cref="T:System.Collections.Generic.ICollection`1" /> 是只读的。</exception>
        public bool Remove(T item)
        {
            return _linkedList.Remove(item);
        }

        void ICollection<T>.Add(T value)
        {
            Add(value);
        }

        /// <summary>
        ///     获取 <see cref="T:System.Collections.Generic.ICollection`1" /> 中包含的元素数。
        /// </summary>
        /// <returns>
        ///     <see cref="T:System.Collections.Generic.ICollection`1" /> 中包含的元素数。
        /// </returns>
        int ICollection<T>.Count
        {
            get { return Count; }
        }

        /// <summary>
        ///     获取一个值，该值指示 <see cref="T:System.Collections.Generic.ICollection`1" /> 是否为只读。
        /// </summary>
        /// <returns>
        ///     如果 <see cref="T:System.Collections.Generic.ICollection`1" /> 为只读，则为 true；否则为 false。
        /// </returns>
        bool ICollection<T>.IsReadOnly
        {
            get { return ((ICollection<T>) _linkedList).IsReadOnly; }
        }

        #region Implementation of IDeserializationCallback

        /// <summary>
        ///     在整个对象图形已经反序列化时运行。
        /// </summary>
        /// <param name="sender">开始回调的对象。当前未实现该参数的功能。</param>
        public void OnDeserialization(object sender)
        {
            _linkedList.OnDeserialization(sender);
        }

        #endregion

        #region Implementation of ISerializable

        /// <summary>
        ///     使用将目标对象序列化所需的数据填充 <see cref="T:System.Runtime.Serialization.SerializationInfo" />。
        /// </summary>
        /// <param name="info">要填充数据的 <see cref="T:System.Runtime.Serialization.SerializationInfo" />。</param>
        /// <param name="context">此序列化的目标（请参见 <see cref="T:System.Runtime.Serialization.StreamingContext" />）。</param>
        /// <exception cref="T:System.Security.SecurityException">调用方没有所要求的权限。</exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _linkedList.GetObjectData(info, context);
        }

        #endregion

        public bool Add(T value)
        {
            _linkedList.AddLast(value);
            return true;
        }

        public void Add(int index, T value)
        {
            var node = FindNode(index);
            _linkedList.AddAfter(node, value);
        }

        public bool AddAll(IEnumerable<T> source)
        {
            if (Count > 0)
            {
                return AddAll(Count - 1, source);
            }
            return AddAll(0, source);
        }

        public bool AddAll(int index, IEnumerable<T> source)
        {
            var node = FindNode(index);
            foreach (var value in source)
            {
                if (node == null)
                {
                    _linkedList.AddFirst(value);
                }
                else
                {
                    node = _linkedList.AddAfter(node, value);
                }
            }
            return true;
        }

        public void AddFirst(T value)
        {
            _linkedList.AddFirst(value);
        }

        public void AddLast(T value)
        {
            _linkedList.AddLast(value);
        }

        public LinkedList<T> Clone()
        {
            return (LinkedList<T>) MemberwiseClone();
        }

        public T Poll()
        {
            return PollFirst();
        }

        public T PollFirst()
        {
            if (Count == 0)
            {
                return default(T);
            }
            var value = First.Value;
            _linkedList.RemoveFirst();
            return value;
        }

        public T PollLast()
        {
            if (Count == 0)
            {
                return default(T);
            }
            var value = Last.Value;
            _linkedList.RemoveLast();
            return value;
        }

        public bool IsEmpty()
        {
            return Count == 0;
        }

        public int IndexOf(T value)
        {
            var linkedListNode = First;
            var @default = EqualityComparer<T>.Default;
            var index = 0;
            if (linkedListNode != null)
            {
                if (value == null)
                {
                    while (linkedListNode.Value != null)
                    {
                        linkedListNode = linkedListNode.Next;
                        if (linkedListNode == First)
                        {
                            return -1;
                        }
                        index++;
                    }
                    return index;
                }
                do
                {
                    if (@default.Equals(linkedListNode.Value, value))
                    {
                        return index;
                    }
                    linkedListNode = linkedListNode.Next;
                    index++;
                } while (linkedListNode != First);
            }
            return -1;
        }

        public int LastIndexOf(T value)
        {
            var index = Count;
            if (First == null)
            {
                return -1;
            }
            var linkedListNode = First.Previous;
            var linkedListNode1 = linkedListNode;
            var @default = EqualityComparer<T>.Default;
            if (linkedListNode1 != null)
            {
                if (value == null)
                {
                    while (linkedListNode1.Value != null)
                    {
                        linkedListNode1 = linkedListNode1.Previous;
                        if (linkedListNode1 == linkedListNode)
                        {
                            return -1;
                        }
                        index--;
                    }
                    return index;
                }
                do
                {
                    if (@default.Equals(linkedListNode1.Value, value))
                    {
                        return index;
                    }
                    linkedListNode1 = linkedListNode1.Previous;
                    index--;
                } while (linkedListNode1 != linkedListNode);
            }
            return -1;
        }

        public T Remove(int index)
        {
            var node = FindNode(index);
            _linkedList.Remove(node);
            return node != null ? node.Value : default(T);
        }

        public LinkedListNode<T> FindNode(int index)
        {
            if (index < 0 || index > Count - 1)
            {
                if (index == 0)
                {
                    return null;
                }
                throw new ArgumentOutOfRangeException("index");
            }
            var node = First;
            while (index > 0)
            {
                index--;
                node = node.Next;
            }
            return node;
        }

        public static implicit operator LinkedList<T>(System.Collections.Generic.LinkedList<T> linkedList)
        {
            return new LinkedList<T>(linkedList);
        }

        public static implicit operator System.Collections.Generic.LinkedList<T>(LinkedList<T> linkedList)
        {
            return linkedList != null ? linkedList._linkedList : null;
        }

        public static implicit operator List<T>(LinkedList<T> linkedList)
        {
            return linkedList.ToList();
        }

        public static implicit operator LinkedList<T>(List<T> list)
        {
            return new LinkedList<T>(new System.Collections.Generic.LinkedList<T>(list));
        }

        #region Implementation of IEnumerable

        /// <summary>
        ///     返回一个循环访问集合的枚举器。
        /// </summary>
        /// <returns>
        ///     可用于循环访问集合的 <see cref="T:System.Collections.Generic.IEnumerator`1" />。
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _linkedList.GetEnumerator();
        }

        /// <summary>
        ///     返回一个循环访问集合的枚举器。
        /// </summary>
        /// <returns>
        ///     可用于循环访问集合的 <see cref="T:System.Collections.IEnumerator" /> 对象。
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}