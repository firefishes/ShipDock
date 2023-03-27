﻿using System;

namespace ShipDock
{
    static public class StringUtils
    {
        public const char SPLIT_CHAR = ',';
        public const char DOT_CHAR = '.';
        public const string DOT = ".";
        public const string PATH_SYMBOL = "/";
        public const char PATH_SYMBOL_CHAR = '/';

        public static string GetQualifiedClassName(object target, bool isFullName = false)
        {
            if(target == null)
            {
                return string.Empty;
            }
            Type type = target.GetType();
            return isFullName ? type.FullName : type.Name;
        }
    }

}
