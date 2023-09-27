#define _EDITOR_LOG_ENABLED

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ShipDock.Editors
{
    /// <summary>
    /// 
    /// 编辑器扩展基类
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public abstract class ShipDockEditor : EditorWindow
    {
        public const string TRUE = "true";
        public const string FALSE = "false";

        /// <summary>
        /// 初始化编辑器窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="title"></param>
        /// <param name="rect"></param>
        public static void InitEditorWindow<T>(string title, Rect rect = default) where T : ShipDockEditor
        {
            ShipDockEditor window = (rect != default) ? GetWindowWithRect<T>(rect, true, title) : GetWindow<T>(title);
            window.Preshow();
            window.Show();
        }

        /// <summary>编辑器中即将生效的字段集合</summary>
        protected KeyValueList<string, bool> mApplyValues;
        /// <summary>编辑器中即将生效的字符串集合</summary>
        protected KeyValueList<string, string> mApplyStrings;
        /// <summary>编辑器中的属性集合</summary>
        protected KeyValueList<string, ValueItem> mValueItemMapper;

        private List<int> mGUIFlagKeys;
        private List<string> mFlagKeys;
        private List<string> mFlagLabels;
        private List<bool> mConfigFlagValues;

        /// <summary>
        /// 预显示编辑器窗口
        /// </summary>
        public virtual void Preshow()
        {
            UpdateEditorAsset();
            InitConfigFlagAndValues();

            bool value;
            int max = mFlagKeys.Count;
            for (int i = 0; i < max; i++)
            {
                value = mConfigFlagValues[i];
                CheckClientConfigApplyed(mFlagKeys[i], ref value);
            }
        }

        /// <summary>
        /// 更新编辑器相关的配置资源，例如一些 .asset文件
        /// </summary>
        protected virtual void UpdateEditorAsset()
        {
            UpdateEditorConfigAssets();
        }

        protected virtual void UpdateEditorConfigAssets() { }

        private void InitConfigFlagAndValues()
        {
            mGUIFlagKeys = new List<int>();
            mFlagKeys = new List<string>();
            mFlagLabels = new List<string>();
            mConfigFlagValues = new List<bool>();
            mValueItemMapper = new KeyValueList<string, ValueItem>();

            mApplyValues = new KeyValueList<string, bool>();

            ReadyClientValues();
        }

        protected void CheckClientConfigApplyed(string flagKey, ref bool clientConfigFlag)
        {
            bool applyFlag = mApplyValues.IsContainsKey(flagKey) && mApplyValues[flagKey];
            if (applyFlag != clientConfigFlag)
            {
                mApplyValues[flagKey] = clientConfigFlag;
            }
            else { }
        }

        protected void AddFlagKeyValue(string flag, ref bool configValue, string label)
        {
            mFlagKeys.Add(flag);
            mFlagLabels.Add(label);
            mConfigFlagValues.Add(configValue);
        }

        protected void NeedUpdateFlagKeyValue(string flag, ref bool configValue)
        {
            int index = mFlagKeys.IndexOf(flag);
            if (index >= 0)
            {
                mGUIFlagKeys.Add(index);
                mConfigFlagValues[index] = configValue;
            }
            else { }
        }

        protected bool GetFlagKeyValue(string flag)
        {
            int index = mFlagKeys.IndexOf(flag);
            return (index >= 0) && mConfigFlagValues[index];
        }

        /// <summary>
        /// 设置编辑器字段值
        /// </summary>
        /// <param name="keyField"></param>
        /// <param name="value"></param>
        public void SetValueItem(string keyField, string value)
        {
            if (mValueItemMapper != default)
            {
                ValueItem valueItem = ValueItem.New(keyField, value);
                mValueItemMapper[keyField] = valueItem;
            }
            else { }
        }

        /// <summary>
        /// 获取编辑器字段值
        /// </summary>
        /// <param name="keyField"></param>
        /// <returns></returns>
        public ValueItem GetValueItem(string keyField)
        {
            return mValueItemMapper != default ? mValueItemMapper[keyField] : default;
        }

        [System.Diagnostics.Conditional("EDITOR_LOG_ENABLED")]
        private void CheckAndLog(int logType, string keyFieldItem = "", string keyFieldValue = "")
        {
            switch (logType)
            {
                case 0:
                    const string logAboutWriteKey = "Write key [{0}] = [{1}]";
                    Debug.Log(string.Format(logAboutWriteKey, keyFieldItem, keyFieldValue));
                    break;

                case 1:
                    const string logAboutReadKey = "Read value [{0}] from key [{1}]";
                    Debug.Log(string.Format(logAboutReadKey, keyFieldItem, keyFieldValue));
                    break;

                case 2:
                    const string logAboutEmptyValue = "Value is empty, it is create new value in key [{0}]";
                    Debug.Log(string.Format(logAboutEmptyValue, keyFieldItem));
                    break;
            }
        }

        /// <summary>
        /// 将值写入编辑器的缓存
        /// </summary>
        /// <param name="keyFields"></param>
        public void WriteValueItemDataToEditor(params string[] keyFields)
        {
            foreach (string name in keyFields)
            {
                string value = mValueItemMapper[name].Value;
                EditorPrefs.SetString(name, value);

                if (string.IsNullOrEmpty(value))
                {
                    CheckAndLog(2, name);
                }
                else
                {
                    CheckAndLog(0, name, value);
                }
            }
        }

        /// <summary>
        /// 从编辑器缓存中读取值
        /// </summary>
        /// <param name="keyFieldName"></param>
        public void ReadValueItemValueFromEditor(string keyFieldName)
        {
            string value = EditorPrefs.GetString(keyFieldName);
            SetValueItem(keyFieldName, value);

            if (string.IsNullOrEmpty(value))
            {
                CheckAndLog(2, keyFieldName);
            }
            else
            {
                CheckAndLog(1, value, keyFieldName);
            }
        }

        /// <summary>
        /// 使用编辑器字段值绘制 TextField 控件
        /// </summary>
        /// <param name="keyField"></param>
        /// <param name="title"></param>
        /// <param name="isLayoutH"></param>
        public void ValueItemTextField(string keyField, string title = "", bool isLayoutH = true)
        {
            CheckValueItemArea(ref title, isLayoutH);

            string input = GetValueItem(keyField).Value;
            string value = EditorGUILayout.TextField(input);
            GetValueItem(keyField).Change(value);

            CheckValueItemArea(ref title, isLayoutH, true);
        }

        /// <summary>
        /// 使用编辑器字段值绘制 Toggle 控件
        /// </summary>
        /// <param name="keyField"></param>
        /// <param name="title"></param>
        /// <param name="isLayoutH"></param>
        /// <returns></returns>
        public bool ValueItemTriggle(string keyField, string title = "", bool isLayoutH = true)
        {
            CheckValueItemArea(ref title, isLayoutH);

            ValueItem item = GetValueItem(keyField);
            bool value = item != default ? GetValueItem(keyField).Bool : false;
            bool toggle = EditorGUILayout.Toggle(value);
            SetValueItem(keyField, toggle.ToString());

            CheckValueItemArea(ref title, isLayoutH, true);
            return toggle;
        }

        /// <summary>
        /// 使用编辑器字段值绘制可编辑的 文本区域 控件
        /// </summary>
        /// <param name="keyField"></param>
        /// <param name="isReadFromEditor">初始化时是否从编辑器缓存获取值进行显示，且在值被修改后自动写入编辑器缓存</param>
        /// <param name="title"></param>
        /// <param name="isLayoutH"></param>
        public void ValueItemTextAreaField(string keyField, bool isReadFromEditor = false, string title = "", bool isLayoutH = true)
        {
            CheckValueItemArea(ref title, isLayoutH);

            if (isReadFromEditor)
            {
                ReadValueItemValueFromEditor(keyField);
            }
            else { }

            ValueItem valueItem = GetValueItem(keyField);
            if (valueItem != default)
            {
                string input = valueItem.Value;
                string value = EditorGUILayout.TextField(input);
                valueItem.Change(value);

                if (isReadFromEditor)
                {
                    WriteValueItemDataToEditor(keyField);
                }
                else { }
            }
            else { }
            CheckValueItemArea(ref title, isLayoutH, true);
        }

        /// <summary>
        /// 使用编辑器字段值绘制 Label 控件
        /// </summary>
        /// <param name="keyField"></param>
        /// <param name="title"></param>
        /// <param name="isLayoutH"></param>
        public void ValueItemLabel(string keyField, string title = "", bool isLayoutH = true)
        {
            CheckValueItemArea(ref title, isLayoutH);

            ValueItem item = GetValueItem(keyField);
            if (item != default)
            {
                string input = item.Value;
                EditorGUILayout.LabelField(input);
            }
            else { }

            CheckValueItemArea(ref title, isLayoutH, true);
        }

        /// <summary>
        /// 使用编辑器字段值绘制 按钮 控件
        /// </summary>
        /// <param name="keyField"></param>
        /// <param name="title"></param>
        /// <param name="formater"></param>
        public bool ValueItemButton(string keyField, string title = "", string formater = "")
        {
            bool result = false;

            ValueItem item = GetValueItem(keyField);

            bool flag = item != default;
            string buttonLabel = flag ? item.Value : title;

            if (string.IsNullOrEmpty(formater)) { }
            else
            {
                if (flag)
                {
                    buttonLabel = string.Format(formater, buttonLabel);
                }
                else { }
            }

            if (GUILayout.Button(buttonLabel))
            {
                result = true;
            }
            else { }

            return result;
        }

        /// <summary>
        /// 对编辑器扩展的控件进行区域排版
        /// </summary>
        /// <param name="title"></param>
        /// <param name="isLayoutH"></param>
        /// <param name="isCheckEnd"></param>
        public void CheckValueItemArea(ref string title, bool isLayoutH, bool isCheckEnd = false)
        {
            bool hasTitle = !string.IsNullOrEmpty(title);

            if (isCheckEnd)
            {
                if (hasTitle && isLayoutH)
                {
                    EditorGUILayout.EndHorizontal();
                }
                else { }
            }
            else
            {
                if (hasTitle)
                {
                    if (isLayoutH)
                    {
                        EditorGUILayout.BeginHorizontal();
                    }
                    else { }
                    EditorGUILayout.LabelField(title);
                }
                else { }
            }
        }

        protected void LayoutH(Action action)
        {
            EditorGUILayout.BeginHorizontal();
            action?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        protected void LayoutV(Action action)
        {
            EditorGUILayout.BeginVertical();
            action?.Invoke();
            EditorGUILayout.EndVertical();
        }

        protected void LayoutSpace(float width, bool expand = false)
        {
            EditorGUILayout.Space(width, expand);
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();
            CheckGUI();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 更新编辑器的 GUI 界面
        /// </summary>
        protected virtual void CheckGUI() { }
        
        protected void ConfirmPopup(string titleValue, string message, Action action = null, string ok = "好的", string cancel = "取消", string log = "")
        {
            bool result;
            if (string.IsNullOrEmpty(cancel))
            {
                result = EditorUtility.DisplayDialog(titleValue, message, ok);
            }
            else
            {
                result = EditorUtility.DisplayDialog(titleValue, message, ok, cancel);
            }

            if (result)
            {
                if (action != null)
                {
                    action.Invoke();
                    if (!string.IsNullOrEmpty(log))
                    {
                        Debug.Log(log);
                    }
                    else { }
                }
                else { }
            }
            else { }
        }

        /// <summary>
        /// 准备编辑器需要的字段
        /// </summary>
        protected abstract void ReadyClientValues();

        /// <summary>
        /// 更新编辑器需要的字段
        /// </summary>
        protected abstract void UpdateClientValues();
    }
}
