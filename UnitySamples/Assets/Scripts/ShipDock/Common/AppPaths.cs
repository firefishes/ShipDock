using UnityEngine;

namespace ShipDock
{
    public static class AppPaths
    {
        /// <summary>资源根目录</summary>
        public static string resData = "res_data";
        /// <summary>配置根目录</summary>
        public static string confData = "conf_data";
        /// <summary>资源输出根目录</summary>
        public static string outputPath = "res_data_output~/";
        /// <summary>资源路径</summary>
        public static string resDataRoot = resData.Append(StringUtils.PATH_SYMBOL);
        /// <summary>配置路径</summary>
        public static string confDataRoot = confData.Append(StringUtils.PATH_SYMBOL);

        /// <summary>项目资源包输出路径</summary>
        public static string ABBuildOutput { get; } = Application.dataPath.Append(StringUtils.PATH_SYMBOL, outputPath);
        /// <summary>项目资源包输出根路径</summary>
        public static string ABBuildOutputRoot { get; } = Application.dataPath.Append(StringUtils.PATH_SYMBOL, outputPath, resDataRoot);
        /// <summary>项目资源包输出根路径（临时）</summary>
        public static string ABBuildOutputTempRoot { get; } = Application.dataPath.Append(StringUtils.PATH_SYMBOL, outputPath, resDataRoot);//需要做字符串格式化
        /// <summary>Steamming资源根路径</summary>
        public static string StreamingResDataRoot { get; } = Application.streamingAssetsPath.Append(StringUtils.PATH_SYMBOL, resDataRoot);
        /// <summary>项目资源根路径</summary>
        public static string DataPathResDataRoot { get; } = Application.dataPath.Append(StringUtils.PATH_SYMBOL, resDataRoot);
        /// <summary>项目配置根路径</summary>
        public static string DataPathConfDataRoot { get; } = Application.dataPath.Append(StringUtils.PATH_SYMBOL, confDataRoot);
        /// <summary>私有目录根路径</summary>
        public static string PersistentResDataRoot { get; set; } = Application.persistentDataPath.Append(StringUtils.PATH_SYMBOL, resDataRoot);


    }

}