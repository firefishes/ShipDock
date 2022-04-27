using ShipDock.Applications;
using ShipDock.Notices;
using UnityEngine;

namespace IsKing
{
    public class BattleInfoController : InfoController<BattleFields>
    {
        private bool mIsFinishCycle;
        private CommonUpdaters mCommonUpdaters;

        public BattleInfoController(BattleFields info) : base(info)
        {
            mCommonUpdaters = new CommonUpdaters();

            AddListener(Consts.N_START_COLLECT_INTELLIGENTAL, OnStartCollectIntelligental);
        }

        public void ExitBattle()
        {
            RemoveListener(Consts.N_START_COLLECT_INTELLIGENTAL, OnStartCollectIntelligental);
        }

        /// <summary>
        /// 统计完整战斗属性
        /// </summary>
        /// <param name="heroController"></param>
        public void CountTotalFields(ref BattleHeroController heroController)
        {
            int troops = Info.GetIntData(Consts.FN_TROOPS);
            HeroFields heroInfo = heroController.Info;
            troops += heroInfo.GetIntData(Consts.FN_TROOPS);

            Info.SetIntData(Consts.FN_TROOPS, troops);
            Info.SetIntData(Consts.FN_TROOPS_MAX, troops);

            float intelligential = Info.GetFloatData(Consts.FN_INTELLIGENTIAL);
            intelligential += heroInfo.GetFloatData(Consts.FN_INTELLECT);

            Info.SetFloatData(Consts.FN_INTELLIGENTIAL, intelligential);
            Info.SetFloatData(Consts.FN_INTELLIGENTIAL_MAX, intelligential);
            Info.SetFloatData(Consts.FN_INTELLIGENTIAL_DELTA, intelligential * 0.002f);
            Info.SetFloatData(Consts.FN_MORALE, 50f);
            Info.SetFloatData(Consts.FN_MORALE_MAX, 100f);
        }

        private void OnStartCollectIntelligental(INoticeBase<int> param)
        {
            mCommonUpdaters.AddUpdate(UpdateIntelligental);
        }

        public void ResetFinishCycleFlag()
        {
            mIsFinishCycle = false;
        }

        /// <summary>
        /// 更新情报收集进度
        /// </summary>
        /// <param name="time"></param>
        private void UpdateIntelligental(int time)
        {
            if (mIsFinishCycle)
            {
                return;
            }
            else { }

            float cur = Info.GetFloatData(Consts.FN_INTELLIGENTIAL);
            float max = Info.GetFloatData(Consts.FN_INTELLIGENTIAL_MAX);
            float delta = Info.GetFloatData(Consts.FN_INTELLIGENTIAL_DELTA);

            cur += delta;

            mIsFinishCycle = cur >= max;
            if (mIsFinishCycle)
            {
                cur -= max;
                Info.SetFloatData(Consts.FN_INTELLIGENTIAL, cur);
                Dispatch(Consts.N_INTELLIGENTAL_FINISHED);//情报收集完成，发牌
                ResetFinishCycleFlag();
            }
            else
            {
                Info.SetFloatData(Consts.FN_INTELLIGENTIAL, cur);
            }

            Dispatch(Consts.N_INTELLIGENTAL_UPDATE);

        }
    }
}