using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.UI
{
    public class ResourcesUI
    {
        public bool isUnique;
        public string resName;
        public GameObject ui;

        public Dictionary<string, ResourcesUI> MapperOwner { get; private set; }
        public IUIStack StackBinded { get; private set; }

        public ResourcesUI(Dictionary<string, ResourcesUI> mapper)
        {
            MapperOwner = mapper;
        }

        public void Clear()
        {
            MapperOwner = default;
            StackBinded = default;
            ui = default;
            resName = default;
        }

        public void BindToUIStack(IUIStack stack)
        {
            StackBinded = stack;
            StackBinded.OnExit += OnStackExit;
        }

        private void OnStackExit(bool isDestroy)
        {
            if (isDestroy)
            {
                MapperOwner.Remove(resName);
                Clear();
            }
            else { }
        }

    }
}
