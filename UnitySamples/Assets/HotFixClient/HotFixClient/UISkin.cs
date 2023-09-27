using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShipDock
{
    public class UISkin : HotFixBase
    {
        public bool IsTheme { get; private set; }
        public HotFixerComponent HotFixer { get; private set; }
        public bool IsApplyAfterReady { get; set; }

        private Image mImage;
        private Sprite mSkin;
        private ResSpriteBridge mRes;
        private List<UISkin> mThemeSkins;

        public override void ShellInited(MonoBehaviour target)
        {
            HotFixer = target as HotFixerComponent;

            IsTheme = HotFixer.GetDataField("IsTheme").Result().Bool;
            if (IsTheme)
            {
                ValueSubgroup subgroup = HotFixer.GetDataField("IsApplyAfterReady");
                IsApplyAfterReady = subgroup != default ? subgroup.Result().Bool : true;

                GameObject theme = HotFixer.GetSceneNode("Theme").value;
                mThemeSkins = new List<UISkin>();

                List<HotFixerComponent> list = new List<HotFixerComponent>();
                Utils.GetComponentsInAllChildren(ref list, theme.transform);

                int noticeName;
                HotFixerComponent skinItem;
                int max = list.Count;
                for (int i = 0; i < max; i++)
                {
                    skinItem = list[i];

                    noticeName = skinItem.gameObject.GetInstanceID();
                    noticeName.Add(OnSkinReady);

                    skinItem.RunHotFix();
                }
            }
            else
            {
                mImage = HotFixer.GetSceneNode("Image").image;
                GameObject skin = HotFixer.GetSceneNode("Skin").value;

                mRes = skin.GetComponent<ResSpriteBridge>();
                mRes.CreateRaw();

                UpdateSkin();
            }
        }

        public void ApplyTheme()
        {
            if (IsTheme)
            {
                int max = mThemeSkins.Count;
                for (int i = 0; i < max; i++)
                {
                    mThemeSkins[i].UpdateSkin();
                }
            }
            else { }
        }

        public void CleanTheme()
        {
            if (IsTheme)
            {
                int max = mThemeSkins.Count;
                for (int i = 0; i < max; i++)
                {
                    mThemeSkins[i].ClearSkin();
                }
            }
            else { }
        }

        public void UpdateSkin()
        {
            if (mSkin == default)
            {
                mSkin = mRes.CreateAsset();
            }
            else { }

            mImage.overrideSprite = mSkin;
        }

        public void ClearSkin()
        {
            mImage.overrideSprite = default;
        }

        public INoticeBase<int> GetReadyNotice()
        {
            ReadyID = HotFixer.HotFixerReadID;
            return new ParamNotice<UISkin>
            {
                ParamValue = this,
            };
        }

        private void OnSkinReady(INoticeBase<int> param)
        {
            if (param is ParamNotice<UISkin> notice)
            {
                notice.Name.Remove(OnSkinReady);
                
                mThemeSkins.Add(notice.ParamValue);

                if (IsApplyAfterReady)
                {
                    notice.ParamValue.UpdateSkin();
                }
                else { }
            }
            else { }
        }

        public override void FixedUpdate()
        {
        }

        public override void LateUpdate()
        {
        }

        public override void Update()
        {
        }
    }
}
