using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IsKing
{
    public class BattleHeroController : InfoController<HeroFields>
    {
        public bool IsGoIntoBattle { get; private set; }
        public BattleCamp Camp { get; private set; }

        public BattleHeroController(HeroFields info) : base(info)
        {
        }

        public void SetCamp(BattleCamp camp)
        {
            Camp = camp;
        }

        public void GoIntoBattle()
        {
            IsGoIntoBattle = true;
        }
    }

}