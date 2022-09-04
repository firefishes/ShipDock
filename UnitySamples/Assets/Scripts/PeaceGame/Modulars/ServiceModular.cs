using System.Collections;
using System.Collections.Generic;
using ShipDock.Notices;
using UnityEngine;
using ProtoBuf;
using User;
using ShipDock.Pooling;
using System;
using ShipDock.Tools;

namespace Peace
{
    public class Protocal : IPoolable
    {
        private byte[] mData;
        private System.IO.MemoryStream mMS;

        public Protocal()
        {
        }

        public void Revert()
        {
            Utils.Reclaim(ref mData);
        }

        public void ToPool()
        {
            Pooling<Protocal>.To(this);
        }

        public void Encode<T>(T proto) where T : IExtensible
        {
            mMS = new System.IO.MemoryStream();

            //BinaryFormatter bm = new BinaryFormatter();
            //bm.Serialize(ms, p);
            Serializer.Serialize(mMS, proto);

            mData = mMS.ToArray();

            Debug.Log(mData.Length);
        }

        public T Decode<T>() where T : IExtensible
        {
            mMS = new System.IO.MemoryStream(mData);
            
            // BinaryFormatter bm = new BinaryFormatter();
            //T proto = (T)bm.Deserialize(ms);
            T proto = Serializer.Deserialize<T>(mMS);

            return proto;
        }
    }

    public class ProtocalNotice : ParamNotice<Protocal>
    {
        protected override void Purge()
        {
            Pooling<Protocal>.To(ParamValue);

            base.Purge();
        }

        public override void ToPool()
        {
            Pooling<ProtocalNotice>.To(this);
        }
    }

    public class ServiceModular : BaseModular
    {
        public ServiceModular() : base(Consts.M_SERVICE)
        {
        }

        protected override void SettleMessageQueue(int message, INoticeBase<int> notice)
        {
            switch (message)
            {
                case Consts.MSG_S_INIT_PLAYER:
                    ProtocalNotice serviceDataNotice = notice as ProtocalNotice;
                    //IServiceCaller caller = serviceDataNotice.ParamValue;

                    //IServiceData serviceData = Consts.D_SERVICE.GetData<IServiceData>();
                    //caller.InitIDAdvanced(serviceData.IDAdvanced);
                    InitPlayer initPlayer = serviceDataNotice.ParamValue.Decode<InitPlayer>();
                    Debug.Log(initPlayer.IsNewPlayer);
                    break;
            }
        }
    }

}