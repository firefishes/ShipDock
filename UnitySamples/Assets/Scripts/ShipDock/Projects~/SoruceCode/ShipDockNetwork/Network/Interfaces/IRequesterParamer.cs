using LitJson;
using System.Collections.Generic;

namespace ShipDock.Network
{
    public interface IRequesterParamer
    {
        Dictionary<string, string> RequestParam { get; set; }
    }

    public interface IRequesterJsonParamer
    {
        JsonData RequestParam { get; set; }
    }
}
