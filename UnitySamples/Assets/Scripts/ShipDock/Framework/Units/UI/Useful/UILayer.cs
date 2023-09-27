using UnityEngine;

namespace ShipDock
{

    public class UILayer : MonoBehaviour
    {
        [SerializeField]
        private UILayerTypeEnum m_UILayerValue;

        public int UILayerValue
        {
            get
            {
                return (int)m_UILayerValue;
            }
        }
    }

}