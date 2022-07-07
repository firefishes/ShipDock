using ShipDock.Interfaces;
using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace ShipDock.Datas
{
    /// <summary>
    /// 
    /// 字段化数据
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public abstract class FieldableData : IFieldableData, IDataUnit, IDispose
    {
        private static int instanceIDSeed = 0;

        protected List<int> mIntFieldSource;
        protected List<float> mFloatFieldSource;
        protected List<string> mStringFieldSource;

        /// <summary>是否为另一个字段化数据的引用</summary>
        private bool mIsReference;
        /// <summary>整型值</summary>
        private int[] mIntValues;
        /// <summary>浮点型值</summary>
        private float[] mFloatValues;
        /// <summary>字符串型值</summary>
        private string[] mStringValues;
        /// <summary>有修改的值索引</summary>
        private List<bool> mHasChanges;
        /// <summary>所有数据的字段集合</summary>
        private List<int> mAllFields;
        /// <summary>值发生变化的回调</summary>
        private List<Action> mOnValuesChanged;

        public List<int> AllFieldNames
        {
            get
            {
                return mAllFields;
            }
        }

        /// <summary>唯一标识</summary>
        public int InstanceID { get; private set; }
        /// <summary>是否应用自动检测值的修改</summary>
        public bool ApplyAutoChangedCheck { get; set; } = true;
        public virtual List<int> IntFieldNames { get; protected set; }
        public virtual List<int> FloatFieldNames { get; protected set; }
        public virtual List<int> StringFieldNames { get; protected set; }

        public FieldableData() { }

        /// <summary>销毁</summary>
        public virtual void Dispose()
        {
            if (mIsReference)
            {
                mAllFields = null;
            }
            else { }

            Utils.Reclaim(ref mAllFields);
            Utils.Reclaim(ref mIntValues);
            Utils.Reclaim(ref mFloatValues);
            Utils.Reclaim(ref mStringValues);
            Utils.Reclaim(ref mHasChanges);
            Utils.Reclaim(ref mOnValuesChanged);

            mIsReference = false;
        }

        /// <summary>获取整型值的数据源</summary>
        public virtual List<float> GetFloatFieldSource()
        {
            return mFloatFieldSource;
        }

        /// <summary>获取浮点型值的数据源</summary>
        public virtual List<int> GetIntFieldSource()
        {
            return mIntFieldSource;
        }

        /// <summary>获取字符串型值的数据源</summary>
        public virtual List<string> GetStringFieldSource()
        {
            return mStringFieldSource;
        }

        /// <summary>填充数据</summary>
        protected void FillValues(bool newInstance = false)
        {
            if (newInstance)
            {
                instanceIDSeed++;
                SetInstanceID(instanceIDSeed);
            }
            else { }

            FillValuesByFields(IntFieldNames, ref mIntValues, GetIntFieldSource());
            FillValuesByFields(FloatFieldNames, ref mFloatValues, GetFloatFieldSource());
            FillValuesByFields(StringFieldNames, ref mStringValues, GetStringFieldSource());
            
            if (mAllFields == default)
            {
                mAllFields = new List<int>();
            }
            else { }

            int max = 0;
            AddFieldsToAllFieldNames(IntFieldNames, ref max);
            AddFieldsToAllFieldNames(FloatFieldNames, ref max);
            AddFieldsToAllFieldNames(StringFieldNames, ref max);
            
            mHasChanges = new List<bool>(max);
            for (int i = 0; i < max; i++)
            {
                mHasChanges.Add(default);
            }

            mOnValuesChanged = new List<Action>(max);
            for (int i = 0; i < max; i++)
            {
                mOnValuesChanged.Add(default);
            }

            AfterFilledData();
        }

        protected virtual void AfterFilledData()
        {
        }

        public void InitFromFieldableData<T>(ref T fieldableData, bool isReference = true) where T : FieldableData
        {
            if (fieldableData != default)
            {
                mIsReference = isReference;
                IntFieldNames = fieldableData.IntFieldNames;
                FloatFieldNames = fieldableData.FloatFieldNames;
                StringFieldNames = fieldableData.StringFieldNames;

                List<int> intFieldSourceRaw = fieldableData.GetIntFieldSource();
                List<float> floatFieldSourceRaw = fieldableData.GetFloatFieldSource();
                List<string> stringFieldSourceRaw = fieldableData.GetStringFieldSource();

                int max;
                if (isReference)
                {
                    mIntFieldSource = intFieldSourceRaw;
                }
                else
                {
                    max = intFieldSourceRaw.Count;
                    for (int i = 0; i < max; i++)
                    {
                        mIntFieldSource.Add(intFieldSourceRaw[i]);
                    }
                }

                if (isReference)
                {
                    mFloatFieldSource = floatFieldSourceRaw;
                }
                else
                {
                    max = floatFieldSourceRaw.Count;
                    for (int i = 0; i < max; i++)
                    {
                        mFloatFieldSource.Add(floatFieldSourceRaw[i]);
                    }
                }

                if (isReference)
                {
                    mStringFieldSource = stringFieldSourceRaw;
                }
                else
                {
                    max = stringFieldSourceRaw.Count;
                    for (int i = 0; i < max; i++)
                    {
                        mStringFieldSource.Add(stringFieldSourceRaw[i]);
                    }
                }

                FillValues(true);
            }
            else { }
        }

        /// <summary>
        /// 向字段集合增加预增字段位
        /// </summary>
        /// <param name="list"></param>
        /// <param name="totalFieldCount"></param>
        private void AddFieldsToAllFieldNames(List<int> list, ref int totalFieldCount)
        {
            int count = list != default ? list.Count : 0;
            if(count > 0)
            {
                totalFieldCount += count;
                mAllFields.Contact(list);
            }
            else { }
        }

        /// <summary>
        /// 通过数据源填充数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fields"></param>
        /// <param name="values"></param>
        /// <param name="willFillIn"></param>
        protected void FillValuesByFields<T>(List<int> fields, ref T[] values, List<T> willFillIn)
        {
            int max = fields != default ? fields.Count : 0;
            if (max > 0)
            {
                values = new T[max];
                for (int i = 0; i < max; i++)
                {
                    values[i] = willFillIn != default && (willFillIn.Count > i) ? willFillIn[i] : default;
                }
            }
            else { }
        }

        public void SetInstanceID(int value)
        {
            InstanceID = value;
        }

        public int GetIntData(int fieldName)
        {
            return GetData(fieldName, IntFieldNames, ref mIntValues);
        }

        public void SetIntData(int fieldName, int value)
        {
            SetData(fieldName, value, IntFieldNames, ref mIntValues);
        }

        public float GetFloatData(int fieldName)
        {
            return GetData(fieldName, FloatFieldNames, ref mFloatValues);
        }

        public void SetFloatData(int fieldName, float value)
        {
            SetData(fieldName, value, FloatFieldNames, ref mFloatValues);
        }

        public string GetStringData(int fieldName)
        {
            return GetData(fieldName, StringFieldNames, ref mStringValues);
        }

        public void SetStringData(int fieldName, string value)
        {
            SetData(fieldName, value, StringFieldNames, ref mStringValues);
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <param name="fieldNames"></param>
        /// <param name="values"></param>
        private void SetData(int fieldName, int value, List<int> fieldNames, ref int[] values)
        {
            int index = fieldNames.IndexOf(fieldName);

            if (ApplyAutoChangedCheck)
            {
                int old = values[index];
                if (!old.Equals(value))
                {
                    values[index] = value;
                    index = mAllFields.IndexOf(fieldName);
                    mHasChanges[index] = true;
                }
                else { }
            }
            else
            {
                values[index] = value;
            }
        }

        private void SetData(int fieldName, float value, List<int> fieldNames, ref float[] values)
        {
            int index = fieldNames.IndexOf(fieldName);

            if (ApplyAutoChangedCheck)
            {
                float old = values[index];
                if (!old.Equals(value))
                {
                    values[index] = value;
                    index = mAllFields.IndexOf(fieldName);
                    mHasChanges[index] = true;
                }
                else { }
            }
            else
            {
                values[index] = value;
            }
        }

        private void SetData(int fieldName, string value, List<int> fieldNames, ref string[] values)
        {
            int index = fieldNames.IndexOf(fieldName);

            if (ApplyAutoChangedCheck)
            {
                string old = values[index];
                if (!old.Equals(value))
                {
                    values[index] = value;
                    index = mAllFields.IndexOf(fieldName);
                    mHasChanges[index] = true;
                }
                else { }
            }
            else
            {
                values[index] = value;
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <param name="fieldNames"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public T GetData<T>(int fieldName, List<int> fieldNames, ref T[] values)
        {
            int index = fieldNames.IndexOf(fieldName);
            return values[index];
        }

        /// <summary>
        /// 检测字段是否有修改
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="applyReset"></param>
        /// <returns></returns>
        public bool HasFieldChanged(int fieldName, bool applyReset = true)
        {
            if (!ApplyAutoChangedCheck)
            {
                return true;
            }
            else { }

            int index = mAllFields.IndexOf(fieldName);
            bool result = mHasChanges[index];
            if (applyReset && result)
            {
                mHasChanges[index] = false;
            }
            else { }

            return result;
        }

        /// <summary>
        /// 设置值修改的回调
        /// </summary>
        /// <param name="actions"></param>
        public void SetValueChanged(params Action[] actions)
        {
            if (ApplyAutoChangedCheck)
            {
                int max = actions.Length;
                for (int i = 0; i < max; i++)
                {
                    mOnValuesChanged[i] = actions[i];
                }
            }
            else { }
        }

        /// <summary>
        /// 标记字段位为已修改
        /// </summary>
        /// <param name="actions"></param>
        public void SetValueChanged(int fieldName, Action method)
        {
            if (ApplyAutoChangedCheck)
            {
                int index = mAllFields.IndexOf(fieldName);
                mOnValuesChanged[index] = method;
            }
            else { }
        }

        /// <summary>
        /// 执行值被修改后的回调
        /// </summary>
        /// <param name="index"></param>
        public void AfterValueChange(int index)
        {
            if (ApplyAutoChangedCheck)
            {
                mOnValuesChanged[index]?.Invoke();
            }
            else { }

        }
    }
}