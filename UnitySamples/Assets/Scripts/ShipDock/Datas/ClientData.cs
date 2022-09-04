using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Datas
{
    public static class ClientDataConsts
    {
        public const string DEVICE_INFO = "DeviceInfo";
        public const string LAST_ACCOUNT_ID = "LastAccountID";
        public const string PLAYER_INFO = "PlayerInfo";
    }

    public interface IClientData
    {
        void Init();
        void FlushInfos();
        ClientLocalInfo GetClientInfo();
    }

    public class ClientData<DeviceT, ClientT> : IClientData where DeviceT : DeviceLocalInfo, new() where ClientT : ClientLocalInfo, new()
    {
        public bool IsInited { get; private set; }
        public DeviceT DeviceInfo { get; private set; }
        public ClientT ClientInfo { get; private set; }

        public ClientData() { }

        public void Init()
        {
            if (IsInited)
            {
                return;
            }
            else { }

            IsInited = true;

            InitClientInfo();
            InitDeviceInfo();
        }

        private void InitDeviceInfo()
        {
            string deviceInfoKey = GetDeviceInfoKey();
            string infoRaw = GetLocalStringData(deviceInfoKey);
            if (string.IsNullOrEmpty(infoRaw))
            {
                DeviceInfo = new DeviceT();
            }
            else
            {
                DeviceInfo = JsonUtility.FromJson<DeviceT>(infoRaw);
                if (DeviceInfo.all_accounts == default)
                {
                    DeviceInfo.all_accounts = new List<string>();
                }
                Debug.Log(string.Format("Last device info init success, account id is {0}", infoRaw));
            }
        }

        private void InitClientInfo()
        {
            string lastAccountID = GetLocalStringData(ClientDataConsts.LAST_ACCOUNT_ID);
            if (string.IsNullOrEmpty(lastAccountID))
            {
                ClientInfo = new ClientT();
                Debug.Log("New client info init success");
            }
            else
            {
                InitClientInfoFromLast(ref lastAccountID);
                Debug.Log(string.Format("Last client info init success, account id is {0}", lastAccountID));
            }
        }

        private void InitClientInfoFromLast(ref string lastAccountID)
        {
            UpdateLocalData(ClientDataConsts.LAST_ACCOUNT_ID, lastAccountID);

            if (ClientInfo != default)
            {
                if (ClientInfo.accountID != lastAccountID)
                {
                    ClientInfo.CheckInfoPatch();

                    string oldInfoKey = GetClientInfoKey();
                    string infoRaw = JsonUtility.ToJson(ClientInfo);
                    UpdateLocalData(oldInfoKey, infoRaw);
                    ClientInfo = default;

                    RefillClientInfoByAccountID(ref lastAccountID);
                }
                else { }
            }
            else
            {
                RefillClientInfoByAccountID(ref lastAccountID);
            }
        }

        private void RefillClientInfoByAccountID(ref string lastAccountID)
        {
            string infoKey = ClientDataConsts.PLAYER_INFO.Append(lastAccountID);
            string infoRaw = GetLocalStringData(infoKey);

            ClientInfo = JsonUtility.FromJson<ClientT>(infoRaw);
            Debug.Log(string.Format("Last client info init success, account id is {0}", infoRaw));

            if (ClientInfo == default)
            {
                ClientInfo = new ClientT()
                {
                    accountID = lastAccountID,
                };
                ClientInfo.CheckInfoPatch();
            }
            else { }
        }

        /// <summary>
        /// 更新本地字符串数据
        /// </summary>
        public void UpdateLocalData(string keyName, string data, bool checkUnique = false)
        {
            if (string.IsNullOrEmpty(keyName))
            {
                Debug.Log(string.Format("Player prefs {0} do not exist", keyName));
            }
            else
            {
                if (checkUnique && HasKey(keyName) && (data == PlayerPrefs.GetString(keyName)))
                {
                    Debug.Log(string.Format("Player prefs key {0}'s value must be unique.", keyName));
                    return;
                }
                else { }

                PlayerPrefs.SetString(keyName, data);
                Debug.Log(string.Format("Player prefs set string value.. {0} >> {1}", keyName, data));
            }
        }

        /// <summary>
        /// 更新本地字符串数据
        /// </summary>
        public void UpdateLocalDataInt(string keyName, int data, bool checkUnique = false)
        {
            if (string.IsNullOrEmpty(keyName))
            {
                Debug.Log(string.Format("Player prefs {0} do not exist", keyName));
            }
            else
            {
                if (checkUnique && HasKey(keyName) && (data == PlayerPrefs.GetInt(keyName)))
                {
                    Debug.Log(string.Format("Player prefs key {0}'s value must be unique.", keyName));
                    return;
                }
                else { }

                PlayerPrefs.SetInt(keyName, data);
                Debug.Log(string.Format("Player prefs set int value.. {0} >> {1}", keyName, data));
            }
        }

        /// <summary>
        /// 更新本地字符串数据
        /// </summary>
        public void UpdateLocalDataFloat(string keyName, int data, bool checkUnique = false)
        {
            if (!string.IsNullOrEmpty(keyName))
            {
                if (checkUnique && HasKey(keyName) && (data == PlayerPrefs.GetFloat(keyName)))
                {
                    Debug.Log(string.Format("Player prefs key {0}'s value must be unique.", keyName));
                    return;
                }
                else { }

                PlayerPrefs.SetFloat(keyName, data);
                Debug.Log(string.Format("Player prefs set float value.. {0} >> {1}", keyName, data));
            }
            else
            {
                Debug.Log(string.Format("Player prefs {0} do not exist", keyName));
            }
        }

        /// <summary>
        /// 获取本地字符串类型的数据
        /// </summary>
        public string GetLocalStringData(string keyName)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(keyName) && HasKey(keyName))
            {
                result = PlayerPrefs.GetString(keyName);
            }
            else { }

            return result;
        }

        /// <summary>
        /// 获取本地整型数据
        /// </summary>
        public int GetLocalIntData(string keyName)
        {
            int result = 0;
            if (!string.IsNullOrEmpty(keyName) && HasKey(keyName))
            {
                result = PlayerPrefs.GetInt(keyName);
            }
            else { }

            return result;
        }

        /// <summary>
        /// 获取本地浮点型数据
        /// </summary>
        public float GetLocalFloatData(string keyName)
        {
            float result = 0f;
            if (!string.IsNullOrEmpty(keyName) && HasKey(keyName))
            {
                result = PlayerPrefs.GetFloat(keyName);
            }
            else { }

            return result;
        }

        public void DeleteKey(string keyName)
        {
            PlayerPrefs.DeleteKey(keyName);
        }

        /// <summary>
        /// 本地是否存在该字段
        /// </summary>
        public bool HasKey(string keyName)
        {
            return PlayerPrefs.HasKey(keyName);
        }

        public void FlushInfos()
        {
            FlushClientInfo();
            FlushDeviceInfo();
            PlayerPrefs.Save();
        }

        public void FlushDeviceInfo()
        {
            if (ClientInfo != default && !string.IsNullOrEmpty(ClientInfo.accountID))
            {
                if (DeviceInfo.all_accounts.Contains(ClientInfo.accountID)) { }
                else
                {
                    DeviceInfo.all_accounts.Add(ClientInfo.accountID);
                }
            }
            else { }

            string json = JsonUtility.ToJson(DeviceInfo);
            string deviceInfoKey = GetDeviceInfoKey();
            UpdateLocalData(deviceInfoKey, json);
        }

        public void FlushClientInfo()
        {
            string json = JsonUtility.ToJson(ClientInfo);
            string clientInfoKey = GetClientInfoKey();
            UpdateLocalData(clientInfoKey, json);
            UpdateLocalData(ClientDataConsts.LAST_ACCOUNT_ID, ClientInfo.accountID);
        }

        public void DeleteClientInfo()
        {
            string clientInfoKey = GetClientInfoKey();
            DeleteKey(clientInfoKey);
            UpdateLocalData(ClientDataConsts.LAST_ACCOUNT_ID, string.Empty);
        }

        public void CreateNewClient()
        {
            FlushInfos();
            DeleteKey(ClientDataConsts.LAST_ACCOUNT_ID);
            InitClientInfo();
        }

        private string GetDeviceInfoKey()
        {
            return ClientDataConsts.DEVICE_INFO.Append(ClientInfo.accountID);
        }

        private string GetClientInfoKey()
        {
            return ClientDataConsts.PLAYER_INFO.Append(ClientInfo.accountID);
        }

        public ClientLocalInfo GetClientInfo()
        {
            return ClientInfo;
        }
    }
}
