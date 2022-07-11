using ShipDock.Config;
using ShipDock.Datas;
using ShipDock.Pooling;
using System.Collections.Generic;

namespace Peace
{
    /// <summary>
    /// 基础信息字段
    /// </summary>
    public abstract class BaseFields : FieldableData, IPoolable
    {
        /// <summary>
        /// 获取连接了新的整型字段的字段列表
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="vs"></param>
        /// <returns></returns>
        protected static List<int> GetNewIntFields(ref List<int> fields, List<int> vs)
        {
            if (fields == default)
            {
                List<int> defaultFields = FieldsConsts.IntFieldsBase;

                int max = defaultFields.Count;
                fields = new List<int>(max + vs.Count);
                for (int i = 0; i < max; i++)
                {
                    fields.Add(defaultFields[i]);
                }

                max = vs.Count;
                for (int i = 0; i < max; i++)
                {
                    fields.Add(vs[i]);
                }
            }
            else { }

            return fields;
        }

        /// <summary>
        /// 获取连接了新的文本字段的字段列表
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="vs"></param>
        /// <returns></returns>
        protected static List<int> GetNewStringFields(ref List<int> fields, List<int> vs)
        {
            if (fields == default)
            {
                List<int> defaultFields = FieldsConsts.StringFieldsBase;

                int max = defaultFields.Count;
                fields = new List<int>(max + vs.Count);
                for (int i = 0; i < max; i++)
                {
                    fields.Add(defaultFields[i]);
                }

                max = vs.Count;
                for (int i = 0; i < max; i++)
                {
                    fields.Add(vs[i]);
                }
            }
            else { }

            return fields;
        }

        public bool IsInited { get; private set; }
        public override List<int> IntFieldNames { get; protected set; } = FieldsConsts.IntFieldsBase;
        public override List<int> StringFieldNames { get; protected set; } = FieldsConsts.StringFieldsBase;

        public override void Dispose()
        {
            base.Dispose();

            Purge();

            IsInited = false;
        }

        protected virtual void Purge()
        {
        }

        public virtual void Revert()
        {
            Purge();
        }

        public abstract void ToPool();

        /// <summary>
        /// 通过配置对象进行初始化
        /// </summary>
        /// <param name="config"></param>
        public virtual void InitFieldsFromConfig(IConfig config)
        {
            int configID = config != default ? config.GetID() : -1;
            string nameValue = GetNameFieldSource(ref config);

            if (IsInited)
            {
                IDAdvanced();
                //客户端 ID
                SetIntData(FieldsConsts.F_ID, InstanceID);
                //服务端 ID
                SetIntData(FieldsConsts.F_S_ID, -1);
                //配置 ID
                SetIntData(FieldsConsts.F_CONF_ID, configID);
                //设置名称字段数据源
                SetStringData(FieldsConsts.F_NAME, nameValue);
            }
            else
            {
                mIntFieldSource = new List<int>()
                {
                    //客户端 ID
                    -1,
                    //服务端 ID
                    -1,
                    //配置 ID
                    configID,
                };

                //设置名称字段数据源
                mStringFieldSource = new List<string>
                {
                    nameValue,
                };
            }
        }

        protected override void AfterFilledData()
        {
            base.AfterFilledData();

            SetIntData(FieldsConsts.F_ID, InstanceID);

            IsInited = true;
        }

        public int GetID()
        {
            return GetIntData(FieldsConsts.F_ID);
        }

        public void SetSID(int sid)
        {
            SetIntData(FieldsConsts.F_S_ID, sid);
        }

        public void SetName(string name)
        {
            SetStringData(FieldsConsts.F_NAME, name);
        }

        /// <summary>
        /// 获取名称字段的数据源
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        protected virtual string GetNameFieldSource(ref IConfig config)
        {
            return string.Empty;
        }

        /// <summary>
        /// 使用默认值设置整型字段数据
        /// </summary>
        /// <param name="fields"></param>
        protected void SetDefaultIntData(List<int> fields)
        {
            if (IsInited)
            {
                if (fields != default)
                {
                    int max = fields.Count;
                    List<int> list = new List<int>(max);
                    for (int i = 0; i < max; i++)
                    {
                        list.Add(default);
                    }
                    mIntFieldSource.Contact(list);
                }
                else { }
            }
            else { }
        }
    }
}
