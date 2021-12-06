using System.Globalization;
using UnityEngine;

namespace ShipDock.Tools
{
    public enum ColorType
    {
        RRGGBB,
        RRGGBBAA
    }

    /// <summary>
    /// Code Reference from：https://blog.csdn.net/qq_33520289/article/details/106171554
    /// </summary>
    public static class ColorUtil
    {
        private const string hexRegex = "^#?(?:[0-9a-fA-F]{3,4}){1,2}$";

        public static string ColorToHex(Color32 color, ColorType colorType)
        {
            long num = 0;
            string hexStr = "";

            if (colorType == ColorType.RRGGBB)
            {
                num = 0xFFFFFF & (ColorRGBAToInt(color) >> 8);
                hexStr = "#" + num.ToString("X6");
            }
            else
            {
                num = 0xFFFFFFFF & (ColorRGBAToInt(color));
                hexStr = "#" + num.ToString("X8");
            }

            return hexStr;
        }

        public static bool HexToColor(string hex, out Color32 color, ColorType colorType)
        {
            // Check if this is a valid hex string (# is optional)
            if (System.Text.RegularExpressions.Regex.IsMatch(hex, hexRegex))
            {
                int startIndex = hex.StartsWith("#") ? 1 : 0;

                color = Color.black;
                if (colorType == ColorType.RRGGBBAA) //#RRGGBBAA
                {
                    color = new Color32(byte.Parse(hex.Substring(startIndex, 2), NumberStyles.AllowHexSpecifier),
                        byte.Parse(hex.Substring(startIndex + 2, 2), NumberStyles.AllowHexSpecifier),
                        byte.Parse(hex.Substring(startIndex + 4, 2), NumberStyles.AllowHexSpecifier),
                        byte.Parse(hex.Substring(startIndex + 6, 2), NumberStyles.AllowHexSpecifier));
                }
                else if (colorType == ColorType.RRGGBB) //#RRGGBB
                {
                    color = new Color32(byte.Parse(hex.Substring(startIndex, 2), NumberStyles.AllowHexSpecifier),
                        byte.Parse(hex.Substring(startIndex + 2, 2), NumberStyles.AllowHexSpecifier),
                        byte.Parse(hex.Substring(startIndex + 4, 2), NumberStyles.AllowHexSpecifier),
                        255);
                }
                return true;
            }
            else
            {
                color = new Color32();
                return false;
            }
        }

        public static int ColorRGBAToInt(Color c)
        {
            int retVal = 0;
            retVal |= Mathf.RoundToInt(c.r * 255f) << 24;
            retVal |= Mathf.RoundToInt(c.g * 255f) << 16;
            retVal |= Mathf.RoundToInt(c.b * 255f) << 8;
            retVal |= Mathf.RoundToInt(c.a * 255f);

            return retVal;
        }
    }
}
