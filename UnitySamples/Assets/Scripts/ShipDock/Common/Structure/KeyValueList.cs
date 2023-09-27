using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock
{
    /// <summary>
    /// 
    /// 自制哈希类，支持以对象为键名
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class KeyValueList<K, V> : IReclaim
    {
        #region 字段
        private Dictionary<K, int> mMapper;
        #endregion

        #region 属性
        public int Capacity { get; private set; }
        public List<K> Keys { get; private set; }
        public List<V> Values { get; private set; }

        public int Size
        {
            get
            {
                return (Keys != null) ? Keys.Count : 0;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return Size < 1;
            }
        }
        #endregion

        #region 构造函数及初始化
        public KeyValueList()
        {
            Init();
        }

        public KeyValueList(int capacity)
        {
            Init(capacity);
        }

        public void Reinit(int capacity = 0)
        {
            Init(capacity);
        }

        /// <summary>初始化</summary>
        protected void Init(int capacity = 0)
        {
            if (Keys != null)
            {
                Keys.Clear();
            }
            else { }
            
            if (Values != null)
            {
                Values.Clear();
            }
            else { }

            if (capacity != 0)
            {
                Keys = new List<K>(capacity);
                Values = new List<V>(capacity);
            }
            else
            {
                Keys = new List<K>();
                Values = new List<V>();
            }
            Capacity = Keys.Capacity;
        }

        public void Clone(ref K[] k, ref V[] v)
        {
            Clear();
            Keys = new List<K>(k);
            Values = new List<V>(v);
            mMapper = new Dictionary<K, int>();
        }

        public void Clone(ref KeyValueList<K, V> target, bool isClear = false)
        {
            if (mMapper != default)
            {
                target.ApplyMapper();
            }
            else { }

            if(target == default)
            {
                target = new KeyValueList<K, V>();
            }
            else { }

            int max = Keys.Count;
            for (int i = 0; i < max; i++)
            {
                target[Keys[i]] = Values[i];
            }

            if(isClear)
            {
                Clear();
            }
            else { }
        }
        #endregion

        #region 销毁
        public virtual void Reclaim()
        {
            Clear();
        }

        public void Reclaim(bool isDisposeItems)
        {
            if (isDisposeItems)
            {
                IReclaim item;
                int max = Values.Count;
                for (int i = 0; i < max; i++)
                {
                    if (Values[i] is IReclaim)
                    {
                        item = Values[i] as IReclaim;
                        item.Reclaim();
                    }
                    else if (Values[i] is GameObject)
                    {
                        Object.DestroyImmediate(Values[i] as GameObject);
                    }
                    else if (Values[i] is IPoolable)
                    {
                        (Values[i] as IPoolable).Revert();
                    }
                    else { }
                }
            }
            else { }

            Reclaim();
        }

        public virtual void Clear(bool isTrimExcess = false)
        {
            Values.Clear();
            Keys.Clear();
            mMapper?.Clear();

            if(isTrimExcess)
            {
                TrimExcess();
            }
            else { }
        }
        #endregion

        #region 键值对管理
        public void ApplyMapper()
        {
            mMapper = new Dictionary<K, int>();
            
            List<K> keys = Keys;
            int count = keys.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    mMapper[keys[i]] = i;
                }
            }
            else { }
        }

        /// <summary>添加所有</summary>
        public void PutAll(ref KeyValueList<K, V> map)
        {
            if (map == null)
            {
                return;
            }
            else { }

            Clear();

            int max = map.Size;
            if (max > 0)
            {
                K key;
                List<K> list = map.Keys;
                for (int i = 0; i < max; i++)
                {
                    key = list[i];
                    Put(key, map[key]);
                }
            }
            else { }
        }

        /// <summary>检查是否包含某个属性</summary>
        public bool IsContainsKey(K key)
        {
            return KeyIndex(key) != -1;
        }

        /// <summary>检查是否包含某个值</summary>
        public bool IsContainsValue(V value)
        {
            return ValueIndex(value) != -1;
        }

        public bool TryGetValue(K key, out V value)
        {
            bool result = IsContainsKey(key);
            value = result ? this[key] : default;
            return result;
        }

        public int KeyIndex(K target)
        {
            int result;
            if (mMapper == default)
            {
                result = (Keys != null) ? Keys.IndexOf(target) : -1;
            }
            else
            {
                bool flag = mMapper.TryGetValue(target, out result);
                result = flag ? result : -1;
            }
            return result;
        }

        private int ValueIndex(V target)
        {
            return Values != null ? Values.IndexOf(target) : -1;
        }

        /// <summary>获取和设置值</summary>
        public V this[K key]
        {
            get
            {
                return GetValue(key);
            }
            set
            {
                if (Values == null)
                {
                    Init();
                }
                else { }

                Put(key, value);
            }
        }

        public V Put(K key, V value)
        {
            V result = default;
            int index = KeyIndex(key);
            if (index == -1)
            {
                index = Keys.Count;
                Values.Add(value);
                Keys.Add(key);

                if (mMapper != default)
                {
                    mMapper[key] = index;
                }
                else { }
            }
            else
            {
                result = Values[index];
                Values[index] = value;
            }

            return result;
        }

        /// <summary>移除数据</summary>
        public V Remove(K key)
        {
            int index = KeyIndex(key);
            bool hasKey = index != -1;
            if ((key == null) || !hasKey)
            {
                return default;
            }
            else { }

            V result = hasKey ? Values[index] : default;
            if (hasKey)
            {
                Keys.RemoveAt(index);
                Values.RemoveAt(index);
            }
            else { }

            if (mMapper != default)
            {
                bool flag = mMapper.TryGetValue(key, out _);
                if (flag)
                {
                    mMapper.Remove(key);
                }
                else { }
            }
            else { }
            return result;
        }

        private bool CheckValid()
        {
            return Keys != default && Values != default;
        }

        /// <summary>通过数据获取键名</summary>
        public K GetKey(V value)
        {
            if (!CheckValid())
            {
                return default;
            }
            else { }

            K result = default;
            int max = Keys.Count;
            for (int i = 0; i < max; i++)
            {
                result = Keys[i];
                if (Values[i].Equals(value))
                {
                    return result;
                }
                else { }
            }
            return result;
        }

        /// <summary>通过键名获取数据</summary>
        public V GetValue(K key, bool isDelete = false)
        {
            if (!CheckValid())
            {
                return default;
            }
            else { }

            V result;
            if (mMapper == default)
            {
                int index = KeyIndex(key);
                bool hasKey = index != -1;
                if (!hasKey)
                {
                    return default;
                }
                else { }

                result = Values[index];
                if (isDelete)
                {
                    Keys.RemoveAt(index);
                    Values.RemoveAt(index);
                }
                else { }
            }
            else
            {
                bool flag = mMapper.TryGetValue(key, out int index);
                result = flag ? Values[index] : default;
            }
            return result;
        }

        /// <summary>通过索引获取数据</summary>
        public V GetValueByIndex(int index, bool isDelete = false)
        {
            if (!CheckValid())
            {
                return default;
            }
            else { }

            V result = default;
            if (index < Keys.Count)
            {
                K key = Keys[index];
                result = GetValue(key, isDelete);
            }
            else { }
            return result;
        }

        public V TryGet(K key)
        {
            return IsContainsKey(key) ? GetValue(key) : default;
        }

        public bool ContainsKey(K key)
        {
            bool result;
            if (mMapper == default)
            {
                result = Keys != null && Keys.Contains(key);
            }
            else
            {
                result = mMapper.TryGetValue(key, out _);
            }
            return result;
        }

        public bool ContainsValue(V value)
        {
            return Values != null && Values.Contains(value);
        }

        public void TrimExcess()
        {
            Keys.TrimExcess();
            Values.TrimExcess();
            mMapper?.TrimExcess();
        }
        #endregion

    }

    public class StringIntValueList : KeyValueList<string, int>
    {
        public int Change(string key, int value, int defaultValue = 0, bool checkRemovable = true, int removeLowFromValue = 0)
        {
            if(!IsContainsKey(key))
            {
                this[key] = defaultValue;
            }
            else { }

            int result = this[key] += value;
            if(checkRemovable && (result <= removeLowFromValue))
            {
                Remove(key);
            }
            else { }

            return result;
        }
    }
}