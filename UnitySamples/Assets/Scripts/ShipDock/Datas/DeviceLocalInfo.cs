using System;
using System.Collections.Generic;

namespace ShipDock.Datas
{
    [Serializable]
    public class DeviceLocalInfo
    {
        public bool has_allowed_microphone;
        public bool has_agree_privacy;
        public bool has_allowed_camera;
        public List<string> all_accounts = new List<string>();

        public DeviceLocalInfo() { }
    }
}