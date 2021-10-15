using ShipDock.Tools;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;
using UnityEngine;

namespace ShipDock.Applications
{

    /// <summary>
    /// 
    /// 基础类型引用子组
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    [Serializable]
    public class ValueSubgroup
    {
        /// <summary>节点字段名</summary>
#if ODIN_INSPECTOR
        [EnableIf("@this.editable")]
#endif
        public string keyField;

        /// <summary>基础值类型</summary>
#if ODIN_INSPECTOR
        [EnumPaging, ShowIf("@this.editable"), Indent(1)]
#endif
        public ValueItemType valueType;

#if ODIN_INSPECTOR
        /// <summary>是否启用修改</summary>
        [LabelText("修改"), ToggleLeft(), Indent(1)]
        public bool editable;
#endif

        /// <summary>是否设置了用于阻尼变化的时间</summary>
#if ODIN_INSPECTOR
        [Indent(1), ShowIf("@this.editable")]
#endif
        [SerializeField]
        private bool m_HasDampTimeValue;

        /// <summary>用于阻尼变化的时间</summary>
        [SerializeField]
#if ODIN_INSPECTOR
        [ShowIf("m_HasDampTimeValue", true), ShowIf("@this.editable"), Indent(1)]
#endif
        private float m_DampTime;

        /// <summary>字符串</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", ValueItemType.STRING), ShowIf("@this.editable"), Indent(1)]
#endif
        [SerializeField]
        private string m_Str;
#if ODIN_INSPECTOR
        [ShowIf("valueType", ValueItemType.DOUBLE), ShowIf("@this.editable"), Indent(1)]
#endif
        [SerializeField]
        public double m_DoubleValue;

        /// <summary>浮点值</summary>
#if ODIN_INSPECTOR
        [ShowIf("@this.valueType == ValueItemType.FLOAT || this.valueType == ValueItemType.INT"), ShowIf("@this.editable"), Indent(1)]
#endif
        [SerializeField]
        private float m_FloatValue;

        /// <summary>布尔值</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", ValueItemType.BOOL), ShowIf("@this.editable"), Indent(1)]
#endif
        [SerializeField]
        private bool m_TriggerValue;

        /// <summary>3D 向量值</summary>
#if ODIN_INSPECTOR
        [ShowIf("@this.valueType == ValueItemType.VECTOR_2 || this.valueType == ValueItemType.VECTOR_3"), ShowIf("@this.editable"), Indent(1)]
#endif
        [SerializeField]
        private Vector3 m_Vector;

        /// <summary>颜色值</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", ValueItemType.COLOR), ShowIf("@this.editable"), Indent(1)]
#endif
        [SerializeField]
        private Color m_Color;

        /// <summary>遮罩层</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", ValueItemType.LAYER_MASK), ShowIf("@this.editable"), Indent(1)]
#endif
        [SerializeField]
        private LayerMask m_LayerMask;

        /// <summary>基础值节点缓存</summary>
        private ValueItem mCached;

#if UNITY_EDITOR
        public void Sync()
        {
            //m_Str = str;
            //m_FloatValue = floatValue;
            //m_DoubleValue = doubleValue;
            //m_DampTime = dampTime;
            //m_TriggerValue = triggerValue;
        }
#endif

        #region 获取各类值对应的值对象
        public ValueItem GetFloat()
        {
            return ValueItem.New(keyField, m_FloatValue).SetDampTime(m_DampTime);
        }

        public ValueItem GetBool()
        {
            return ValueItem.New(keyField, m_TriggerValue).SetDampTime(m_DampTime);
        }

        public ValueItem GetString()
        {
            return ValueItem.New(keyField, m_Str).SetDampTime(m_DampTime);
        }

        private ValueItem GetInt()
        {
            return ValueItem.New(keyField, (int)m_FloatValue).SetDampTime(m_DampTime);
        }

        private ValueItem GetDouble()
        {
            return ValueItem.New(keyField, m_DoubleValue).SetDampTime(m_DampTime);
        }
        #endregion

        /// <summary>
        /// 根据类型获取代表基础类型数据的值对象
        /// </summary>
        /// <param name="isRefresh"></param>
        /// <returns></returns>
        public ValueItem Result(bool isRefresh = false)
        {
            if (isRefresh)
            {
                Clean();
            }
            else { }

            if (mCached == default)
            {
                ValueItem result;
                int type = (int)valueType;
                switch (type)
                {
                    case ValueItem.STRING:
                        result = GetString();
                        break;
                    case ValueItem.INT:
                        result = GetInt();
                        break;
                    case ValueItem.DOUBLE:
                        result = GetDouble();
                        break;
                    case ValueItem.BOOL:
                        result = GetBool();
                        break;
                    case ValueItem.FLOAT:
                        result = GetFloat();
                        break;
                    default:
                        result = ValueItem.New(keyField, string.Empty);
                        break;
                }
                mCached = result;
            }
            else { }

            return mCached;
        }

        /// <summary>
        /// 根据类型获取代表2D向量类型数据的值对象
        /// </summary>
        /// <returns></returns>
        public Vector2 GetV2()
        {
            return valueType == ValueItemType.VECTOR_2 ? new Vector2(m_Vector.x, m_Vector.y) : Vector2.zero;
        }

        /// <summary>
        /// 根据类型获取代表3D向量类型数据的值对象
        /// </summary>
        /// <returns></returns>
        public Vector3 GetV3()
        {
            return valueType == ValueItemType.VECTOR_3 ? m_Vector : Vector3.zero;
        }

        /// <summary>
        /// 根据类型获取代表颜色类型数据的值对象
        /// </summary>
        /// <returns></returns>
        public Color GetColor()
        {
            return valueType == ValueItemType.COLOR ? m_Color : Color.clear;
        }

        /// <summary>
        /// 根据类型获取代表遮罩层类型数据的值对象
        /// </summary>
        /// <returns></returns>
        public LayerMask GetLayerMask()
        {
            return valueType == ValueItemType.LAYER_MASK ? m_LayerMask : default;
        }

        /// <summary>
        /// 清除值
        /// </summary>
        public void Clean()
        {
            mCached = default;
        }
    }
}