using ShipDock.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Elimlnate
{
    public class UIEliminatePlay : UI
    {
        [SerializeField]
        private GameObject m_OperateCellItemRenderer;
        [SerializeField]
        private LayoutGroup m_GridOperateLayout;

        public GameObject OperateCellItemRenderer
        {
            get
            {
                return m_OperateCellItemRenderer;
            }
        }

        public LayoutGroup GridOperateLayout
        {
            get
            {
                return m_GridOperateLayout;
            }
        }

        public override void UpdateUI()
        {
        }

        protected override void Purge()
        {
        }
    }
}
