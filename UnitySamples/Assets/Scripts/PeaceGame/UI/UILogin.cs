using ShipDock.UI;
using ShipDock.UIControls;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Peace
{
    public interface ILoginUI
    {
        void CheckLoadGameEnabled(bool enabled);
    }

    public class UILogin : UIContainer, ILoginUI
    {
        private UIButton mNewGameBtn;
        private UIButton mLoadGameBtn;

        protected override void Awake()
        {
            base.Awake();

            NodesControl.ReferenceButton("NewGameBtn", out Button btn);
            NodesControl.GetLabel("NewGameLabel", out Text label);
            mNewGameBtn = new UIButton(btn, OnNewGame, "NEW GAME", label);

            NodesControl.ReferenceButton("LoadGameBtn", out btn);
            NodesControl.GetLabel("LoadGameLabel", out label);
            mLoadGameBtn = new UIButton(btn, OnLoadGame, "LOAD GAME", label);
        }

        private void OnLoadGame(UIButton arg0)
        {
        }

        private void OnNewGame(UIButton btn)
        {
            Debug.Log("NewGame");
            this.Dispatch(UILoginModular.UIM_LGOIN_NEW_GAME);
        }

        public void CheckLoadGameEnabled(bool enabled)
        {
            mLoadGameBtn.Interactable = enabled;
        }
    }

}