using System;

namespace ShipDock
{
    public interface IResolvableConfig : IReclaim
    {
        void Create(IServersHolder servers);
        int TypeID { get; }
        int InterfaceID { get; }
        int AliasID { get; }
        Type Type { get; }
        Type InterfaceType { get; }
        string Alias { get; }
    }
}