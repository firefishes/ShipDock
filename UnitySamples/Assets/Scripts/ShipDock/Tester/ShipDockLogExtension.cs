﻿#define G_LOG

using ShipDock;
using UnityEngine;

public static class ShipDockLogExtension
{
    [System.Diagnostics.Conditional("G_LOG")]
    public static void Log(this string logID, params string[] args)
    {
        Tester.Instance.Log(logID, args);
    }

    [System.Diagnostics.Conditional("G_LOG")]
    public static void Log(this string target, bool logFilters, params string[] args)
    {
        Tester.Instance.Log(target, logFilters, args);
    }

    [System.Diagnostics.Conditional("G_LOG")]
    public static void AssertLog(this string target, string title, string assertTarget, params string[] args)
    {
        Tester.Instance.LogAndAssert(target, title, assertTarget, args.Length == 0 ? new string[] { assertTarget } : args);
    }

    [System.Diagnostics.Conditional("G_LOG")]
    public static void AssertLog(this string target, bool logFilters, string title, string assertTarget, params string[] args)
    {
        if (logFilters)
        {
            target.AssertLog(title, assertTarget, args);
        }
        else { }
    }

    [System.Diagnostics.Conditional("G_LOG")]
    public static void Assert(this string target, string assertTarget, params string[] args)
    {
        Tester.Instance.Asserting(target, assertTarget);
        if (args.Length > 0)
        {
            target.Log(args);
        }
        else { }
    }

    [System.Diagnostics.Conditional("G_LOG")]
    public static void Log(this object target, params string[] args)
    {
        target.ToString().Log(args);
    }

    [System.Diagnostics.Conditional("G_LOG")]
    public static void LogAndLocated(this Object target, string logID, params string[] args)
    {
        Tester.Instance.Log(logID, target, args);
    }

    [System.Diagnostics.Conditional("G_LOG")]
    public static void LogWithoutType(this object target, bool filters, params string[] args)
    {
        string.Empty.Log(filters, args);
    }
}
