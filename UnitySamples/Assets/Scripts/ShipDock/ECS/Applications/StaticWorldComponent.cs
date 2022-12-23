using ShipDock.ECS;
using ShipDock.Notices;
using ShipDock.Tools;

namespace ShipDock.Applications
{
    public class WorldInteracter : LogicData, INotificationSender
    {
    }

    public abstract class StaticWorldComponent : DataComponent<WorldInteracter>
    {
        private int[] mWorldItemIDs;
        private int[] mItemGroupIDs;

        public StaticWorldComponent()
        {
            Name = "世界交互组件";
        }

        protected override void OnResetSuccessive(bool clearOnly = false)
        {
            base.OnResetSuccessive(clearOnly);

            Utils.Reclaim(ref mWorldItemIDs, clearOnly);
            Utils.Reclaim(ref mItemGroupIDs, clearOnly);
        }

        protected override void UpdateDataStretch(int dataSize)
        {
            base.UpdateDataStretch(dataSize);

            Utils.Stretch(ref mWorldItemIDs, dataSize);
            Utils.Stretch(ref mItemGroupIDs, dataSize);
        }

        #region 世界互动项
        public void WorldItemID(int entitas, int gbjInstanceID)
        {
            UpdateValidWithType(entitas, ref mWorldItemIDs, out _, gbjInstanceID);
        }

        public int GetWorldItemID(int entitas)
        {
            int worldItemID = GetDataValueWithType(entitas, ref mWorldItemIDs, out _);
            return worldItemID;
        }
        #endregion

        #region 分组 ID
        public void ItemGroupID(int entitas, int groupID)
        {
            UpdateValidWithType(entitas, ref mItemGroupIDs, out _, groupID);
        }

        public int GetItemGroupID(int entitas)
        {
            int itemGroupID = GetDataValueWithType(entitas, ref mItemGroupIDs, out _);
            return itemGroupID;
        }
        #endregion
    }
}