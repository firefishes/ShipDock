using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elimlnate
{
    public class GridGravityEnterEffect : GridEffect
    {
        private ElimlnateCore GamePlayCore { get; set; }

        protected override IEffectInfo<ElimlnateGrid, GridEffectParam> Create(ref ElimlnateGrid target)
        {
            TweenEffect tween = new TweenEffect
            {
                EffectMethod = OnEffect
            };
            return tween;
        }

        private void OnEffect(ElimlnateGrid target, TweenEffectBase<GridEffectParam> tw, GridEffectParam param)
        {
            if (GamePlayCore == default)
            {
                GamePlayCore = ElimlnateCore.Instance;
            }
            else { }

            if (!param.IsInited)
            {
                Vector2Int pos = target.GridPos;
                param.IsInited = true;

                ElimlnateGrid grid1 = GamePlayCore.BoardGrids.GetGridByRowColumn(pos.x, pos.y - 1);
                ElimlnateGrid grid2 = GamePlayCore.BoardGrids.GetGridByRowColumn(pos.x - 1, pos.y - 1);
                ElimlnateGrid grid3 = GamePlayCore.BoardGrids.GetGridByRowColumn(pos.x + 1, pos.y - 1);

                if (!grid2.IsObstacle || !grid3.IsObstacle)
                {

                }
                else { }
            }
            else
            {
            }


        }
    }
}
