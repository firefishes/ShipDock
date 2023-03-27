using System.Collections.Generic;

namespace ShipDock
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
                if (string.IsNullOrEmpty(ServerURL))
                {
                    UnityEngine.Debug.LogError("ServiceURL is null, please check Macro definition contians 'RELEASE || GRAY', 'TEST' or 'DEV'");
                }
                else { }

                result = ServerURL.Append(result);
            }
            else { }

            return result;
        }
    }
}
