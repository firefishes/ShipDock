namespace ShipDock
{
    public interface IScriptableItem
    {
        int GetID();
        void SetID(int id);
        void AutoFill();
        //JsonData ToJSON();
    }
}