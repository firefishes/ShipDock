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
        bool ShowWaiting { get; set; }
        JsonData RequestParam { get; set; }
    }
}
