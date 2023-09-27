#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;

namespace ShipDock
{
    /// <summary>
    /// 
    /// 资源版本配置项
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    [Serializable]
    public class ResVersion
    {
        public static ResVersion CreateNew(string resName, int resVersion, long resFileSize)
        {
            ResVersion result = new ResVersion
            {
                name = resName,
                version = resVersion,
                file_size = resFileSize
            };
            return result;
        }

#if ODIN_INSPECTOR
        [LabelText("资源名")]
#endif
        public string name;

#if ODIN_INSPECTOR
        [LabelText("版本")]
#endif
        public int version;

        [UnityEngine.HideInInspector]
#if ODIN_INSPECTOR
        [LabelText("文件大小")]
#endif
        public long file_size;

#if ODIN_INSPECTOR
        [UnityEngine.HideInInspector]
        [LabelText("有限检测Steamming目录")]
#endif
        public bool isSteamingAsset = true;

        public string Url { get; set; }
    }

    [Serializable]
    public class ResUpdating : ResVersion
    {
    }
}