using ShipDock.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Peace
{
    public interface IHeadquartersUI { }

    public class UIHeadquarters : UIContainer, IHeadquartersUI
    {
        protected override void Awake()
        {
            base.Awake();


        }
    }
}