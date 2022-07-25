using ShipDock.Notices;
using ShipDock.Pooling;
using UnityEngine;

namespace Peace
{
    public class BattleModular : BaseModular
    {
        public BattleModular() : base(Consts.M_BATTLE)
        {
        }

        protected override void InitCustomHandlers()
        {
            base.InitCustomHandlers();
        }

        protected override void SettleMessageQueue(int message, INoticeBase<int> notice)
        {
            switch(message)
            {
                case Consts.MSG_GAME_READY:
                    int count = Pooling<ParamNotice<string>>.Instance.UsedCount;
                    //Debug.Log(count);
                    IParamNotice<string> msgNotice = notice as IParamNotice<string>;
                    string s = msgNotice.ParamValue;
                    //Debug.Log(s);
                    msgNotice.ToPool();
                    //msgNotice.ParamValue = msgNotice.ParamValue.Append("£¡");

                    //AddMessageToQueue(Consts.MSG_GAME_READY, msgNotice);

                    break;
            }
        }
    }
}
