using System.Collections.Generic;
using UnityEngine.Networking;

namespace ShipDock.Network
{
    class HttpUtility
    {
        public static string GetOriginalDataString(Dictionary<string, string> data)
        {
            string formData = "";
            if (data != null && data.Count != 0)
            {
                foreach (string k in data.Keys)
                {
                    formData = formData + k + "=" + UnityWebRequest.EscapeURL(data[k]) + "&";
                }
                formData = formData.Substring(0, formData.Length - 1);
            }
            return formData;
        }
    }
}
