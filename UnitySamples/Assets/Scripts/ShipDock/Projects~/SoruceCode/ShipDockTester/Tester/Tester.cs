﻿
#define _ASSERT

using System;
using System.Collections.Generic;
using UnityEngine;
using AsserterMapper = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<ShipDock.Testers.Asserter>>;
using LogsMapper = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, ShipDock.Testers.LogItem>>;
using TesterIndxMapper = System.Collections.Generic.Dictionary<string, int>;
using TesterMapper = System.Collections.Generic.Dictionary<string, ShipDock.Testers.ITester>;

namespace ShipDock.Testers
{
    public interface ITester
    {
        string Name { get; set; }
    }

    public class Asserter
    {
        public string title;
        public string content;
    }

    public class LogItem
    {
        public bool enabled = true;
        public string format;
        public string logColor;
        public Action<string[]> onLoged;
        public LogEnabledSubgroup enabledSubgroup;
    }

    [Serializable]
    public class LogEnabledSubgroup
    {
        public string logID;
        public string decs;
        public string testerName;
        public bool enabled;
        public Action<string[]> onLoged;

        public void Commit()
        {
            Tester.Instance.LogEnabled(logID, enabled, testerName);
        }
    }

    public class Tester
    {
        private static Tester instance;

        public static Tester Instance
        {
            get
            {
                if (instance == default)
                {
                    instance = new Tester();
                }
                else { }
                return instance;
            }
        }

        public bool isShowLogCount;

        private int mLogCount;
        private ITester mDefaultTester;
        private TesterMapper mTesters;
        private UnityEngine.Object mLogSignTarget;

        private TesterIndxMapper mTesterIndexs;
        private AsserterMapper mAsserterMapper;
        private LogsMapper mLoggerMapper;
        private TesterMapper mTesterMapper;

        private Action<string, string[]> TestBrokers { get; set; }

        public Action<string, string> OnUnityAssert { get; set; }
        public Func<string, LogItem, LogEnabledSubgroup> OnLogItemAdded { get; set; }

        public Tester()
        {
            mTesterIndexs = new TesterIndxMapper();
            mAsserterMapper = new AsserterMapper();
            mLoggerMapper = new LogsMapper();
            mTesterMapper = new TesterMapper();
            mTesters = new TesterMapper();
        }

        public void Dispose()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
            TestBrokers = default;
            mDefaultTester = default;
            OnLogItemAdded = default;
            OnUnityAssert = default;

            mTesterIndexs?.Clear();
            mAsserterMapper?.Clear();
            mLoggerMapper?.Clear();
            mTesterMapper?.Clear();
            mTesters?.Clear();
        }

        public void Init(ITester defaultTester)
        {
            SetDefaultTester(defaultTester);
            Application.logMessageReceived += OnLogMessageReceived;
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void SetDefaultTester(ITester tester)
        {
            if(mDefaultTester == default)
            {
                mDefaultTester = tester;
                AddTester(mDefaultTester);

                AddLogger(mDefaultTester, "ast do not pass", "error: Asserter {0} do not pass in {1}");
                AddLogger(mDefaultTester, "ast not correct", "error: Tester correct is \"{0}\", it do not \"{1}\".");
                AddLogger(mDefaultTester, "tester hited", "Tester: [{0}] Assert coincide {1}/{2}. correct is {3}", "#7FE939");
                AddLogger(mDefaultTester, "all tester hited", "Tester: {0} All hit！", "#48DD22");
            }
            else { }
        }

        /// <summary>接收日志消息的回调函数</summary>
        private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Assert:
                    break;
                case LogType.Error:
                    break;
            }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void LogEnabled(string logID, bool value, string name)
        {
            if (mTesters.TryGetValue(name, out ITester tester))
            {
                LogEnabled(logID, value, tester);
            }
            else { }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void LogEnabled(string logID, bool value, ITester tester = default)
        {
            ITester target = tester ?? mDefaultTester;
            if (target == default)
            {
                return;
            }
            else { }

            string testerName = target.Name;
            if (mLoggerMapper.ContainsKey(testerName))
            {
                Dictionary<string, LogItem> list = mLoggerMapper[testerName];
                if (list.TryGetValue(logID, out LogItem item))
                {
                    item.enabled = value;
                }
                else
                {
                    list[logID] = new LogItem() { enabled = value };
                }
            }
            else { }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void AddTester(ITester tester)
        {
            if (tester == mDefaultTester)
            {
                return;
            }
            else { }

            bool emptyName = string.IsNullOrEmpty(tester.Name);
            Type type = tester.GetType();
            string name = emptyName ? type.Name : tester.Name;
            if (emptyName)
            {
                tester.Name = name;
            }
            else { }

            if (!mAsserterMapper.ContainsKey(name))
            {
                mTesters[name] = tester;
                mAsserterMapper[name] = new List<Asserter>();
                mLoggerMapper[name] = new Dictionary<string, LogItem>();
                if (!emptyName)
                {
                    tester.Name = name;
                }
                else { }
            }
            else { }
        }

        public ITester GetTester(string name)
        {
#if G_LOG
            mTesters.TryGetValue(name, out ITester tester);
            return tester;
#else
            return default;
#endif
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void AddLogger(string logID, string format, string logColor = "", Action<string[]> onLogedMethod = default)
        {
            if (mDefaultTester != default)
            {
                AddLogger(mDefaultTester, logID, format, logColor, onLogedMethod);
            }
            else { }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void AddLogger(ITester tester, string logID, string format, string logColor = "", Action<string[]> onLogedMethod = default)
        {
            if (!mTesterMapper.ContainsKey(logID))
            {
                mTesterMapper[logID] = tester;
            }
            else { }

            string testerName = tester.Name;
            CreateLogger(testerName, ref logID, ref format, tester, logColor, onLogedMethod);
        }

        [System.Diagnostics.Conditional("G_LOG")]
        private void CreateLogger(string testerName, ref string logID, ref string format, ITester tester = default, string logColor = "", Action<string[]> onLogedMethod = default)
        {
            if (!mLoggerMapper.ContainsKey(testerName))
            {
                mLoggerMapper[testerName] = new Dictionary<string, LogItem>();
            }
            else { }

            Dictionary<string, LogItem> list = mLoggerMapper[testerName];
            if (list.TryGetValue(logID, out LogItem item))
            {
                item.format = format;
                item.logColor = logColor;
                item.onLoged = onLogedMethod;
            }
            else
            {
                item = new LogItem()
                {
                    format = format,
                    logColor = logColor,
                    onLoged = onLogedMethod,
                };

                list[logID] = item;

                SetLogEnabledSetting(ref logID, ref item, tester);
            }
        }

        private void SetLogEnabledSetting(ref string logID, ref LogItem item, ITester tester)
        {
            LogEnabledSubgroup subgroup = OnLogItemAdded?.Invoke(logID, item);
            if (subgroup != default)
            {
                subgroup.testerName = tester != default ? tester.Name : "Empty";
                subgroup.onLoged = item.onLoged;

                item.enabledSubgroup = subgroup;
                item.enabled = subgroup.enabled;
            }
            else { }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void Log(string logID, UnityEngine.Object logSignTarget, params string[] args)
        {
            mLogSignTarget = logSignTarget;
            Log(logID, args);
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void Log(bool logFilters, params string[] args)
        {
            if(logFilters)
            {
                Log(string.Empty, args);
            }
            else { }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void Log(string logID, bool logFilters, params string[] args)
        {
            if (logFilters)
            {
                Log(logID, args);
            }
            else { }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void Log(params string[] args)
        {
            LogFromTester(string.Empty, args);
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void LogAndAssert(string logID, string title, string assertTarget, params string[] args)
        {
            if (mAsserterMapper.ContainsKey(title))
            {
                //LogFromTester(logID, args);
                Asserting(title, assertTarget);
            }
            else { }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void Log(string logID, params string[] args)
        {
            LogFromTester(logID, args);
        }

        [System.Diagnostics.Conditional("G_LOG")]
        private void LogFromTester(string logID, params string[] args)
        {
            ITester tester = mTesterMapper.ContainsKey(logID) ? mTesterMapper[logID] : default;
            ITester logOwner = tester;
            if (logOwner == default)
            {
                AddLogger(logID, logID);
                logOwner = tester ?? mDefaultTester;
            }
            else { }

            LogItem logger = default;
            if (logOwner != default)
            {
                string testName = logOwner.Name;
                logger = mLoggerMapper[testName][logID];
            }
            else
            {
                if (logID.Contains("{0}"))
                {
                    CreateLogger(string.Empty, ref logID, ref logID);
                    logger = mLoggerMapper[string.Empty][logID];
                }
                else
                {
                    string log = logID.Append(" - ");
                    int max = args.Length;
                    for (int i = 0; i < max; i++)
                    {
                        log = log.Append(args[i]);
                    }
                    Debug.Log(log);
                    return;
                }
            }
            CheckLogger(ref logID, ref logger, ref args);
        }

        [System.Diagnostics.Conditional("G_LOG")]
        private void CheckLogger(ref string logID, ref LogItem logger, ref string[] args)
        {
            if (!logger.enabled)
            {
                return;
            }
            else { }

            string log;
            logger.onLoged?.Invoke(args);
            if (isShowLogCount)
            {
                log = string.Format(logger.format, args).Append("(", mLogCount.ToString(), ")");
            }
            else
            {
                log = string.Format(logger.format, args);
            }
            if (mLogSignTarget != null)
            {
                DebugUtils.LogInColorAndSignIt(mLogSignTarget, log);
                mLogSignTarget = null;
            }
            else
            {
                if (string.IsNullOrEmpty(logger.logColor))
                {
                    DebugUtils.LogInColor(log);
                }
                else
                {
                    DebugUtils.LogInColor(logger.logColor, log);
                }
            }
            TestBrokers?.Invoke(logID, args);
            mLogCount++;
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void AddAsserter(string title, bool isIgnore, params string[] content)
        {
            if (isIgnore)
            {
                return;
            }
            else { }

            List<Asserter> list;
            if (mAsserterMapper.ContainsKey(title))
            {
                list = mAsserterMapper[title];
            }
            else
            {
                list = new List<Asserter>();
                mAsserterMapper[title] = list;
                mTesterIndexs[title] = 0;
            }
            Asserter result;
            int max = content.Length;
            for (int i = 0; i < max; i++)
            {
                result = new Asserter { title = title, content = content[i] };
                list.Add(result);
            }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void Asserting(string title, string target, bool moveNext = true)
        {
            if (mAsserterMapper.ContainsKey(title))
            {
                List<Asserter> list = mAsserterMapper[title];
                int max = list.Count;
                if (max != 0)
                {
                    int index = mTesterIndexs[title];
                    Asserter asserter = list[index];
                    string correct = asserter.content;

                    if (OnUnityAssert != default)
                    {
                        ShouldMoveNextAsserter(ref index, max, moveNext, ref title);
                        OnUnityAssert(target, correct);
                    }
                    else
                    {
                        bool result = target != correct;
                        if (result)
                        {
                            "ast do not pass".Log(asserter.title, index.ToString());
                            "ast not correct".Log(correct, target);
                        }
                        else
                        {
                            int next = index + 1;
                            "tester hited".Log(asserter.title, next.ToString(), max.ToString(), target);
                            ShouldMoveNextAsserter(ref index, max, moveNext, ref title);
                        }
                    }
                }
                else { }
            }
            else { }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        private void ShouldMoveNextAsserter(ref int index, int max, bool moveNext, ref string title)
        {
            if (moveNext)
            {
                index++;
                mTesterIndexs[title] = index;
            }
            else { }

            if (index >= max)
            {
                "all tester hited".Log(title);
                mAsserterMapper.Remove(title);
                mTesterIndexs.Remove(title);
            }
            else { }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void InjectTestValue<T>(ref T raw, T value, bool apply = false)
        {
            if(apply)
            {
                raw = value;
            }
            else { }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void SetTestBrokerHandler(Action<string, string[]> handler)
        {
            TestBrokers += handler;
        }
    }
}