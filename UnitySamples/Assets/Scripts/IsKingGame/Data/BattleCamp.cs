
using System.Collections.Generic;

namespace IsKing
{
    /// <summary>
    /// 
    /// 战斗阵营
    /// 
    /// 管理阵营中的数据
    /// 
    /// </summary>
    public class BattleCamp
    {
        private BattleHeroController[] mBattleHeros;

        public BattleInfoController BattleInfoController { get; private set; }

        public BattleCamp()
        {
            mBattleHeros = new BattleHeroController[Consts.CAMP_HERO_MAX];
            BattleInfoController = new BattleInfoController(new BattleFields());
        }

        public void SetCampHero(int heroPos, BattleHeroController heroController)
        {
            mBattleHeros[heroPos] = heroController;
            heroController.SetCamp(this);
            BattleInfoController.CountTotalFields(ref heroController);
        }

        public void FillIntoBattleHeros(ref List<BattleHeroController> result)
        {
            int max = mBattleHeros.Length;
            for (int i = 0; i < max; i++)
            {
                if (mBattleHeros[i].IsGoIntoBattle)
                {
                    result.Add(mBattleHeros[i]);
                }
                else { }
            }
        }

        public void FillIdleBattleHeros(ref List<BattleHeroController> result)
        {
            int max = mBattleHeros.Length;
            for (int i = 0; i < max; i++)
            {
                if (!mBattleHeros[i].IsGoIntoBattle)
                {
                    result.Add(mBattleHeros[i]);
                }
                else { }
            }
        }
    }
}