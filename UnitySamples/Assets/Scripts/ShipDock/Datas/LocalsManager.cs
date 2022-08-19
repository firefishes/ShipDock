using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Datas
{
    /// <summary>
    /// 
    /// 本地化多语言管理器
    /// 
    /// </summary>
    public class Locals
    {
        public AssetBundle languageAssetBundle;

        private int mPairKeyIndex;
        private int mPairValueIndex;
        private char mPairSpliter = '=';
        private string mKey;
        private string mValue;
        private string mTail;
        private string[] mKeyPair;
        private Dictionary<string, string> mLanguage;

        public string Local { get; private set; }

        public Locals()
        {
            mPairKeyIndex = 0;
            mPairValueIndex = 1;
        }

        public void Reclaim()
        {
            Local = string.Empty;
            if (mLanguage != null)
            {
                mLanguage.Clear();
                mLanguage = null;
            }
            else { }
        }
        
        public void SetLocal(string localKey = "")
        {
            if (!string.IsNullOrEmpty(localKey))
            {
                SetLocalName(localKey);
            }
            else { }

            if (languageAssetBundle != default)
            {
                string fileName = "local_".Append(Local, ".txt");
                TextAsset data = languageAssetBundle.LoadAsset<TextAsset>(fileName);
                string localsData = data.text;
                FillLanguagesData(ref localsData, AddLocalsLanguageData);
            }
            else { }
        }

        public void SetLocalName(string localKey)
        {
            Local = localKey;
        }

        public void SetLocal<K>(Dictionary<K, string> data, string localKey = "")
        {
            if (!string.IsNullOrEmpty(localKey))
            {
                SetLocalName(localKey);
            }
            else { }

            if (mLanguage == default)
            {
                mLanguage = new Dictionary<string, string>();
            }
            else { }

            if (data != default)
            {
                FillLanguagesData(ref data, AddLocalsLanguageData);
            }
            else { }
        }

        public void InitDefaultLanguages()
        {
            if (mLanguage == default)
            {
                if (mLanguage == default)
                {
                    mLanguage = new Dictionary<string, string>();
                }
                else { }

                TextAsset asset = Resources.Load<TextAsset>("local_default");//如果本地化数据未初始化则先从默认的本地化文本获取数据
                if (asset != default)
                {
                    string data = asset.text;
                    FillLanguagesData(ref data, AddDefaultLanguageData);
                }
                else { }
            }
            else { }
        }

        public string Language(string id, params string[] formats)
        {
            InitDefaultLanguages();

            string result = IsContainsLanguageID(ref id) ? mLanguage[id] : id;

            int max = formats.Length;
            if ((result != id) && (max > 0))
            {
                result = string.Format(result, formats);
            }
            else { }

            return result;
        }

        private void FillLanguagesData(ref string data, Action onAddLanguageData)
        {
            mTail = string.IsNullOrEmpty(Local) ? string.Empty : "_".Append(Local);

            string[] languagesData = data.Split('\n');
            int max = languagesData.Length;
            for (int i = 0; i < max; i++)
            {
                mKeyPair = languagesData[i].Split(mPairSpliter);

                if (IsInvalidPair(mKeyPair.Length))
                {
                    continue;
                }
                else { }

                mKey = mKeyPair[mPairKeyIndex].Trim();
                mValue = mKeyPair[mPairValueIndex].Trim();
                onAddLanguageData?.Invoke();
            }
        }

        private void FillLanguagesData<K>(ref Dictionary<K, string> data, Action onAddLanguageData)
        {
            mTail = string.IsNullOrEmpty(Local) ? string.Empty : "_".Append(Local);

            int max = data.Count;
            Dictionary<K, string>.Enumerator enumerator = data.GetEnumerator();
            for (int i = 0; i < max; i++)
            {
                enumerator.MoveNext();

                mKey = enumerator.Current.Key.ToString();
                mValue = enumerator.Current.Value;
                onAddLanguageData?.Invoke();
            }
            enumerator.Dispose();
        }

        private void AddLocalsLanguageData()
        {
            if (!string.IsNullOrEmpty(mKey))
            {
                mLanguage[mKey] = mValue;
            }
            else { }
        }

        private void AddDefaultLanguageData()
        {
            if (IsTailWithLocalSign(ref mKey, ref mTail))
            {
                mKey = mKey.Substring(0, mKey.Length - mTail.Length);
                mLanguage[mKey] = mValue;
            }
            else { }
        }

        private bool IsInvalidPair(int len)
        {
            return len < 2;
        }

        private bool IsTailWithLocalSign(ref string key, ref string tail)
        {
            if (string.IsNullOrEmpty(tail))
            {
                return true;
            }
            else { }

            return key.IndexOf(tail, StringComparison.Ordinal) != -1;
        }

        private bool IsContainsLanguageID(ref string id)
        {
            return mLanguage != null && mLanguage.ContainsKey(id);
        }
    }

}