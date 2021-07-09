using System.Collections.Generic;
using UnityEngine.Networking;

namespace ShipDock.Network
{
    public static class HttpUtility
    {
        public const string KEY_EQ = "=";
        public const string KEY_LK = "&";

        public static string GetOriginalDataString(Dictionary<string, string> data)
        {
            string formData = string.Empty;
            if (data != null && data.Count != 0)
            {
                string k;
                KeyValuePair<string, string> item;
                Dictionary<string, string>.Enumerator enumer = data.GetEnumerator();

                int max = data.Count;
                int last = max - 1;
                for (int i = 0; i < max; i++)
                {
                    enumer.MoveNext();
                    item = enumer.Current;

                    k = item.Key;
                    if (i < last)
                    {
                        formData = formData.Append(k, KEY_EQ, UnityWebRequest.EscapeURL(data[k]), KEY_LK);
                    }
                    else
                    {
                        formData = formData.Append(k, KEY_EQ, UnityWebRequest.EscapeURL(data[k]));
                    }
                }
            }
            return formData;
        }
    }
}
