
using UnityEngine;
using System;
using System.Text;
using UnityEngine.UI;
#if !RELEASE
using System.Runtime.InteropServices;
using UnityEngine.Profiling;
#endif

namespace ShipDock.Testers
{
    /// <summary>调试工具类</summary>
    public class DebugUtils
    {

        /// <summary>是否为调试模式</summary>
        public static bool isDebug = false;

        //private static readonly int logLine = 0;
        private static int logLineMax = 0;
        private static Text logText;

        /// <summary>打印某个对象的内存地址</summary>
        public static void LogMemory(System.Object debugObject, string log = "", UnityEngine.Object obj = null)
        {
#if !RELEASE
            if (!isDebug)
            {
                return;
            }
            
            GCHandle gcHandle = GCHandle.Alloc(debugObject, GCHandleType.Pinned);
            IntPtr addr = gcHandle.AddrOfPinnedObject();
            //logContent.Append(log);
            //logContent.Append("0x");
            //logContent.Append(addr.ToString());
#endif
        }

        /// <summary>调试射线</summary>
        public static void DebugDrawRay(Vector3 origin, Vector3 direction)
        {
#if !RELEASE
            if (!isDebug)
            {
                return;
            }

            Ray ray = new Ray(origin, direction);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 2000))
            {
                Debug.DrawLine(origin, hit.point, Color.green);
            }
#endif
        }

        public static void LogMemInfo(ref string content)
        {
#if !RELEASE
            int MUnit = 1000000;
            StringBuilder sbd = new StringBuilder();
            content = "Mono Used: ".Append((Profiler.GetMonoUsedSizeLong() / MUnit).ToString(), "M\n");
            content = content.Append("All Memory: ", (Profiler.GetTotalAllocatedMemoryLong() / MUnit).ToString(), "M\n");
            content = content.Append("Un Used Reserved: ", (Profiler.GetTotalUnusedReservedMemoryLong() / MUnit).ToString(), "M\n");
#endif
        }

        public static void SetLogDebugColor(string colorValue)
        {
            LOG_COLOR_DEBUG = colorValue;
        }

        public static void SetLogWarningColor(string colorValue)
        {
            LOG_COLOR_WARNING = colorValue;
        }

        public static void SetLogTodoColor(string colorValue)
        {
            LOG_COLOR_TODO = colorValue;
        }

        public static void SetLogErrorColor(string colorValue)
        {
            LOG_COLOR_ERROR = colorValue;
        }

        public static void SetLogDefaultColor(string colorValue)
        {
            LOG_COLOR_DEFAULT = colorValue;
        }

        public static Color GetLogColor(string type)
        {
            string htmlColor = LOG_COLOR_DEFAULT;
            switch (type)
            {
                case "l":
                    htmlColor = LOG_COLOR_DEBUG;
                    break;
                case "w":
                    htmlColor = LOG_COLOR_WARNING;
                    break;
                case "e":
                    htmlColor = LOG_COLOR_ERROR;
                    break;
                case "t":
                    htmlColor = LOG_COLOR_TODO;
                    break;
            }
            ColorUtility.TryParseHtmlString(htmlColor, out Color color);
            return color;
        }

        private const string COLOR_FORMAT = "<color=\"{0}\">{1}</color>";
        private const string COLOR_SYBMBOL = "#";
        private const string COLOR_LOG_MATCHER_TODO = "todo:";
        private const string COLOR_LOG_MATCHER_WARNING = "warning:";
        private const string COLOR_LOG_MATCHER_ERROR = "error:";
        private const string COLOR_LOG_MATCHER_LOG = "log:";
        private const string COLOR_LOG_MATCHER_TESTER = "Tester:";

        private static string LOG_COLOR_TODO = "#FF4DFF";
        private static string LOG_COLOR_WARNING = "#E78D08";
        private static string LOG_COLOR_DEBUG = "#00BEEEFF";
        private static string LOG_COLOR_ERROR = "#EE0000";
        private static string LOG_COLOR_DEFAULT = "#8A8A8A";
        private static string colorInLog = string.Empty;
        private static string colorValueInLog = string.Empty;
        private static string firstElment = string.Empty;
        private static UnityEngine.Object logObjTarget;

        public static void LogInColorAndSignIt(UnityEngine.Object logTarget, params string[] contents)
        {
            logObjTarget = logTarget;
            LogInColor(contents);
        }

        public static void LogInColor(params string[] contents)
        {
            colorInLog = string.Empty;
            firstElment = contents[0].ToString();

            bool isSetColor = (contents[0] != null) && (firstElment.IndexOf(COLOR_SYBMBOL) != -1);
            int start = isSetColor ? 1 : 0;
#if !COLOR_LOG
            start = 0;
#endif
            int max = contents.Length;
            for (int i = start; i < max; i++)
            {
                colorInLog += contents[i];
            }

            colorValueInLog = isSetColor ? firstElment : string.Empty;
#if !COLOR_LOG
            colorValueInLog = string.Empty;
#endif
            bool isTODO = colorInLog.StartsWith(COLOR_LOG_MATCHER_TODO);
            bool isWarning = colorInLog.StartsWith(COLOR_LOG_MATCHER_WARNING);
            bool isLog = colorInLog.StartsWith(COLOR_LOG_MATCHER_LOG);
            bool isError = colorInLog.StartsWith(COLOR_LOG_MATCHER_ERROR);
            bool isTester = colorInLog.StartsWith(COLOR_LOG_MATCHER_TESTER);
            if (string.IsNullOrEmpty(colorValueInLog))
            {
                if (isTODO)
                {
                    colorValueInLog = LOG_COLOR_TODO;
                }
                else if (isWarning)
                {
                    colorValueInLog = LOG_COLOR_WARNING;
                }
                else if (isError)
                {
                    colorValueInLog = LOG_COLOR_ERROR;
                }
                else if (isLog)
                {
                    colorValueInLog = LOG_COLOR_DEBUG;
                }
                else
                {
                    colorValueInLog = LOG_COLOR_DEFAULT;
                }
            }

#if COLOR_LOG
            colorInLog = string.Format(COLOR_FORMAT, colorValueInLog, colorInLog);
#else
            if (isTODO || isError || isWarning)
            {
                colorInLog = string.Format(COLOR_FORMAT, colorValueInLog, colorInLog);
            }
            if (isSetColor)
            {
                colorInLog = colorInLog.Remove(0, 7);//去掉自定义的颜色
            }
#endif
            if(logObjTarget != null)
            {
                Debug.Log(colorInLog, logObjTarget);
                logObjTarget = null;
            }
            else
            {
                Debug.Log(colorInLog);
            }
        }

        public static void SetLogTextUI(ref Text target)
        {
            logText = target;
        }

        public static void SetLogMaxLine(int value)
        {
            logLineMax = value;
        }
    }
}