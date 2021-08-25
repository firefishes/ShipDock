using ShipDock.Applications;
using ShipDock.Tools;
using UnityEngine;

namespace Elimlnate
{
    public class GridSkillPreviewEffect : GridEffect
    {
        private KeyValueList<ElimlnateGrid, GameObject> mRangeSigns;

        public int RangeSignResPoolName { get; set; }
        public bool TweenEffectApplyUpdate { get; set; } = true;

        public GridSkillPreviewEffect()
        {
            mRangeSigns = new KeyValueList<ElimlnateGrid, GameObject>();
        }

        protected override void Purge()
        {
            base.Purge();

            mRangeSigns?.Clear();
        }

        protected override IEffectInfo<ElimlnateGrid, GridEffectParam> Create(ref ElimlnateGrid target)
        {
            TweenEffect tween = new TweenEffect
            {
                EffectMethod = OnEffect,
                ApplyUpdate = TweenEffectApplyUpdate,
            };
            tween.Param.Speed = new Vector3(0f, 0f, 20f);
            return tween;
        }

        private void OnEffect(ElimlnateGrid target, TweenEffectBase<GridEffectParam> info, GridEffectParam param)
        {
            if (mRangeSigns.ContainsKey(target))
            {
                Transform trans = mRangeSigns[target].transform;
                ApplyPriviewSign(ref trans, ref target, ref param);
            }
            else
            {
                ShipDockApp.Instance.Effects.CreateEffect(RangeSignResPoolName, out GameObject effect);

                CreatePreviewSign(ref target, ref effect, ref param);

                if (effect != default)
                {
                    mRangeSigns[target] = effect;
                }
                else { }
            }
        }

        protected virtual void CreatePreviewSign(ref ElimlnateGrid target, ref GameObject effect, ref GridEffectParam param)
        {
            if (effect == default)
            {
                return;
            }
            else { }

            Transform trans = effect.transform;
            target.AlignmentInGrid(trans, true, false);
            trans.localPosition += new Vector3(-0.1f, -0.09f, 0f);

            if (param.IsInited)
            {
                param.IsInited = false;
                trans.SetParent(default);
            }
            else { }
        }

        protected virtual void ApplyPriviewSign(ref Transform trans, ref ElimlnateGrid grid, ref GridEffectParam param)
        {
            Vector3 end = trans.localEulerAngles;
            end = new Vector3(end.x, end.y, end.z + param.Speed.z * Time.deltaTime);
            trans.localEulerAngles = end;
        }

        protected override void AfterStop(ElimlnateGrid target)
        {
            base.AfterStop(target);

            GameObject res = mRangeSigns.GetValue(target, true);
            res?.SetActive(false);
        }
    }
}
