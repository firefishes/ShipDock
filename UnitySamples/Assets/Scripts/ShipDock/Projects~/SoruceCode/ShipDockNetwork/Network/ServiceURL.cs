using System.Collections.Generic;

namespace ShipDock.Network
{
    public class ServiceURL
    {
        protected Dictionary<string, string> NetworkAPIMapper { get; set; }

        public string ServerURL { get; protected set; }

        public ServiceURL()
        {
            NetworkAPIMapper = new Dictionary<string, string>();
        }

        public string GetHttpURL(string requestName)
        {
            string result = default;
            if (NetworkAPIMapper.TryGetValue(requestName, out result))
            {
                result = ServerURL.Append(result);
            }
            else { }

            return result;
        }
    }
}
