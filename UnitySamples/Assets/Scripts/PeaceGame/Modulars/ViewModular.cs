using ShipDock.Datas;
using ShipDock.Notices;
using ShipDock.Tools;
using System;

namespace Peace
{
    public class ViewModular : BaseModular, IDataExtracter
    {
        private class UIInfo
        {
            private Action<string, string[]> mOnActive;
            private string[] mABs;

            public UIInfo(Action<string, string[]> method, string[] ABs = default)
            {
                mABs = ABs;
                mOnActive = method;
            }

            public void Active(ref string nextUIModular)
            {
                mOnActive?.Invoke(nextUIModular, mABs);
            }
        }

        private UIData mUIData;
        private KeyValueList<string, UIInfo> mUIInfos;

        public ViewModular() : base(Consts.M_VIEW)
        {
            mUIData = Consts.D_UI.GetData<UIData>();
            mUIData.Register(this);

            mUIInfos = new KeyValueList<string, UIInfo>()
            {
                [Consts.UM_HEADQUARTERS] = new UIInfo((name, abs) => 
                {
                    name.LoadAndOpenUI(OnUIOpen<UIHeadquartersModular>(), abs);
                }),
            };
        }

        public void OnDataProxyNotify(IDataProxy data, int DCName)
        {
            switch (DCName)
            {
                case Consts.DN_NEXT_UI_MODULAR:
                    Consts.UM_LOADING.OpenUI<UILoadingModular>();

                    UIData UIData = Consts.D_UI.GetData<UIData>();
                    string nextUIModular = UIData.NextUIModular;

                    mUIInfos[nextUIModular]?.Active(ref nextUIModular);

                    break;
            }
        }

        private Action<T> OnUIOpen<T>()
        {
            return (T UIModular) =>
            {
                Consts.UM_LOADING.Close();
            };
        }

        protected override void SettleMessageQueue(int message, INoticeBase<int> notice)
        {
        }
    }
}