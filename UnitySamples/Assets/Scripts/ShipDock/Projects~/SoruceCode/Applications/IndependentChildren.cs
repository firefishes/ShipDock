using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Application
{
    public class IndependentChildren
    {
        private Transform mContainer;
        private List<Transform> mIndependentChild;

        public IndependentChildren(Transform transform)
        {
            mContainer = transform;
            mIndependentChild = new List<Transform>();
        }

        public void Clean(bool isDeactive)
        {
            Transform item;
            int max = mIndependentChild.Count;
            for (int i = 0; i < max; i++)
            {
                item = mIndependentChild[i];

                if (item != default)
                {
                    if (item.parent == mContainer)
                    {
                        item.SetParent(default);
                    }
                    else { }
                }
                else { }
            }
            mIndependentChild.Clear();
            mIndependentChild = default;
        }

        public void AddChild(Transform trans)
        {
            if (mIndependentChild != default && !mIndependentChild.Contains(trans))
            {
                mIndependentChild.Add(trans);
            }
            else { }
        }

        public void RemoveChild(Transform trans)
        {
            mIndependentChild?.Remove(trans);
        }
    }
}
