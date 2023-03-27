namespace ShipDock
{
    public class WorldMovementSystem : LogicSystem
    {
        public int WorldComponentName { get; set; }

        public override void Execute(int entitas, int componentName, ILogicData data)
        {
            if (componentName == WorldComponentName)
            {

            }
            else { }
        }
    }
}
