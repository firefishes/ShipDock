using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Scriptables
{
    [Serializable]
    public class ConfigableItems
    {
        [SerializeField]
        private int m_ItemType;
        [SerializeField]
        private ScriptableObject m_Collections;

        public int ItemType()
        {
            return m_ItemType;
        }

        public ScriptableObject Collections()
        {
            return m_Collections;
        }
    }

    public class ConfigableItemsComponent : MonoBehaviour
    {
        [SerializeField]
        private List<ConfigableItems> m_Configables;

        public List<ConfigableItems> GetConfigableItems()
        {
            return m_Configables;
        }
    }

}