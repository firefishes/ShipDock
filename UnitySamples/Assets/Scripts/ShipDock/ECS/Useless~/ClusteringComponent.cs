using ShipDock.ECS;

namespace ShipDock.Applications
{
    public class ClusteringComponent : DataComponent<ClusteringData>
    {
        protected override ILogicData CreateData()
        {
            ClusteringData data = (ClusteringData)base.CreateData();
            data.SetWorldGroupID(int.MaxValue);
            return new ClusteringData();
        }
    }

    public class ClusteringData : LogicData
    {
        public bool IsCenter { get; private set; }
        public bool IsGroupCached { get; set; }
        public bool IsDissolutionGroup { get; set; }
        public int WorldGroupID { get; private set; }
        public float ClusteringMag { get; set; }

        public void SetWorldGroupID(int id)
        {
            WorldGroupID = id;
        }

        public void MakeClusteringCenter(int gourpID)
        {
            IsCenter = true;
            WorldGroupID = gourpID;
        }

        public void DissolutionClustering()
        {
            if (IsDissolutionGroup)
            {
                IsCenter = false;
                WorldGroupID = int.MaxValue;
            }
        }
    }
}