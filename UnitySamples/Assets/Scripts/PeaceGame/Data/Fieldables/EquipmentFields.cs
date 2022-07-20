using ShipDock.Config;
using ShipDock.Pooling;
using ShipDock.Tools;
using StaticConfig;
using System.Collections.Generic;

namespace Peace
{
    public class EquipmentFields : BaseFields
    {
        private static List<int> newIntFields;

        public override List<int> IntFieldNames { get; protected set; } = GetNewIntFields(ref newIntFields, FieldsConsts.IntFieldsEquipment);

        public int EquipmentType { get; private set; }
        public int Assess { get; private set; }
        public int ApplyTo { get; private set; }

        /// <summary>容量组控件</summary>
        private VolumeGroupControl mVolumeGroupControl;

        public override void ToPool()
        {
            Pooling<EquipmentFields>.To(this);
        }

        protected override void Init()
        {
            if (IsInited)
            {
                mIntFieldSource.Clear();

                IDAdvanced();
                AfterFilledData();
            }
            else
            {
                //初始化容量组件
                mVolumeGroupControl = new VolumeGroupControl();

                SetDefaultIntData(FieldsConsts.IntFieldsEquipment);

                FillValues(true);
            }
        }

        protected override void AfterFilledData()
        {
            base.AfterFilledData();

            ValueVolumeGroup group = mVolumeGroupControl.VolumeGroup;
            //新建精密属性容量
            group.AddValueVolume(FieldsConsts.F_PRECISE);

            PeaceEquipment peaceEquipment = Config as PeaceEquipment;
            int[] properties = (peaceEquipment != default) ? peaceEquipment.GetEquipmentPropertyValues() : default;

            int max = (properties != default) ? properties.Length : 0;
            if (max > 0)
            {
                List<int> list = FieldsConsts.IntFieldsEquipment;
                for (int i = 0; i < max; i++)
                {
                    SetIntData(list[i], properties[i]);
                }
            }
            else { }

            if (peaceEquipment != default)
            {
                Assess = peaceEquipment.assess;
                ApplyTo = peaceEquipment.applyTo;
                EquipmentType = peaceEquipment.equipmentType;

                SetPrecise(0, peaceEquipment.precise);
            }
            else { }
        }

        public void SetPrecise(int current, int max = -1)
        {
            mVolumeGroupControl.SetVolumeData<BaseFields>(FieldsConsts.F_PRECISE, current, default, max);
        }

        public int GetPrecise()
        {
            return mVolumeGroupControl.GetVolumeData<BaseFields>(FieldsConsts.F_PRECISE);
        }
    }
}