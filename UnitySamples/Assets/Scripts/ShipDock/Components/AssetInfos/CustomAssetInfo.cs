#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using ShipDock.Tools;
using System;
using UnityEditor;
using UnityEngine;

namespace ShipDock.Loader
{
    [Serializable]
    public class CustomAssetInfo
    {
#if ODIN_INSPECTOR
        [EnableIf("@false")]
#endif
        public string assetName;

#if ODIN_INSPECTOR
        [EnableIf("@false")]
#endif
        public string assetPath;

#if ODIN_INSPECTOR
        [ShowIf("@ShouldEdite && IsValid"), LabelText("修改")]
#endif
        [SerializeField]
        private bool m_EditEnabled = true;

        [SerializeField]
        private bool m_Valid = true;

#if ODIN_INSPECTOR
        private void OnAssetTypeChanged()
        {
            Refresh();
        }

        private void OnAssetChanged()
        {
            mIsAssetChanged = true;
            OnAssetTypeChanged();
            mIsAssetChanged = false;
        }

        [ShowIf("@ShouldEditeEnabled()"), OnValueChanged("OnAssetTypeChanged"), Indent(1)]
#endif
        public CustomAssetType assetType;

#if ODIN_INSPECTOR
        [ShowIf("@ShouldEditeEnabled()"), ShowIf("assetType", CustomAssetType.GAME_OBJECT), OnValueChanged("OnAssetChanged"), Indent(1)]
#endif
        public GameObject asset;

#if ODIN_INSPECTOR
        [ShowIf("@ShouldEditeEnabled()"), ShowIf("assetType", CustomAssetType.TEXTURE_2D), OnValueChanged("OnAssetChanged"), Indent(1)]
#endif
        public Texture2D tex2D;

#if ODIN_INSPECTOR
        [ShowIf("@ShouldEditeEnabled()"), ShowIf("assetType", CustomAssetType.SPRITE), OnValueChanged("OnAssetChanged"), Indent(1)]
#endif
        public Texture2D sprite;

#if ODIN_INSPECTOR
        [ShowIf("@this.ShouldEditeEnabled()"), ShowIf("assetType", CustomAssetType.AUDIO_CLIP), OnValueChanged("OnAssetChanged"), Indent(1)]
#endif
        public AudioClip audioClip;

#if ODIN_INSPECTOR
        [ShowIf("@ShouldEditeEnabled()"), ShowIf("assetType", CustomAssetType.TEXT_ASSET), OnValueChanged("OnAssetChanged"), Indent(1)]
#endif
        public TextAsset textData;

#if ODIN_INSPECTOR
        [ShowIf("@ShouldEditeEnabled()"), ShowIf("assetType", CustomAssetType.ASSET_BUNDLE), OnValueChanged("OnAssetChanged"), Indent(1)]
#endif
        public AssetBundle assetBundle;

        private bool mIsAssetChanged;
        private CustomAssetType mPrevAssetType;
        private string[] mAllCustomAssetType;

        public bool ShouldEdite { get; private set; } = true;

        public bool IsValid
        {
            get
            {
                return m_Valid;
            }
        }

        public CustomAssetInfo()
        {
            mAllCustomAssetType = Enum.GetNames(typeof(CustomAssetType));
        }

        public void SyncInfoFromAssetType()
        {
            RefreshAssetByAssetType(assetPath, assetType);
        }

        public void Refresh()
        {
            assetPath = GetAssetPatyByAssetType(assetType);

            if (mIsAssetChanged || (mPrevAssetType != assetType) || string.IsNullOrEmpty(assetName))
            {
                RefreshAssetName();
            }
            else { }

            mPrevAssetType = assetType;
        }

        private void RefreshAssetName()
        {
            string[] splited = assetPath.Split(StringUtils.PATH_SYMBOL_CHAR);
            string value = splited.GetLast();
            assetName = value.Split(StringUtils.DOT_CHAR)[0];
        }

        public void RefreshAssetByAssetType(string assetPathValue, CustomAssetType assetTypeValue)
        {
            if (string.IsNullOrEmpty(assetPathValue))
            {
                return;
            }
            else { }

#if UNITY_EDITOR
            UnityEngine.Object res = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPathValue);
            if (res != default)
            {
                switch (assetTypeValue)
                {
                    case CustomAssetType.GAME_OBJECT:
                        asset = res as GameObject;
                        break;
                    case CustomAssetType.TEXTURE_2D:
                        tex2D = res as Texture2D;
                        break;
                    case CustomAssetType.SPRITE:
                        sprite = res as Texture2D;
                        break;
                    case CustomAssetType.AUDIO_CLIP:
                        audioClip = res as AudioClip;
                        break;
                    case CustomAssetType.TEXT_ASSET:
                        textData = res as TextAsset;
                        break;
                    case CustomAssetType.ASSET_BUNDLE:
                        assetBundle = res as AssetBundle;
                        break;
                }
            }
            else
            {
                Debug.Log("Do not have res ".Append(assetPathValue));
            }
#endif
        }

        private string GetAssetPatyByAssetType(CustomAssetType assetType)
        {
            string result = string.Empty;
            UnityEngine.Object res = default;
            switch (assetType)
            {
                case CustomAssetType.GAME_OBJECT:
                    res = asset;
                    break;
                case CustomAssetType.TEXTURE_2D:
                    res = tex2D;
                    break;
                case CustomAssetType.SPRITE:
                    res = sprite;
                    break;
                case CustomAssetType.AUDIO_CLIP:
                    res = audioClip;
                    break;
                case CustomAssetType.TEXT_ASSET:
                    res = textData;
                    break;
                case CustomAssetType.ASSET_BUNDLE:
                    res = assetBundle;
                    break;
            }

            if (res != default)
            {
#if UNITY_EDITOR
                result = AssetDatabase.GetAssetPath(res);
#endif
            }
            else
            {
                result = string.Empty;
            }
            return result;
        }

        public CustomAssetInfoSource GetInfoJsonSource()
        {
            if (string.IsNullOrEmpty(assetName))
            {
                throw new Exception("Asset name is null");
            }
            else { }

            CustomAssetInfoSource result = new CustomAssetInfoSource()
            {
                valid = m_Valid,
                assetName = assetName,
                assetPath = assetPath,
                assetType = assetType,
            };

            int max = mAllCustomAssetType.Length;
            result.otherAssetPaths = new string[max];
            for (int i = 0; i < max; i++)
            {
                result.otherAssetPaths[i] = GetAssetPatyByAssetType((CustomAssetType)i);
            }

            return result;
        }

        public void SetEditable(bool flag)
        {
            ShouldEdite = flag;
        }

        private bool ShouldEditeEnabled()
        {
            return ShouldEdite && m_EditEnabled && IsValid;
        }

        public void SetValid(bool flag)
        {
            m_Valid = flag;
        }
    }

    public class CustomAssetInfoSource
    {
        public bool valid;
        public string assetName;
        public string assetPath;
        public CustomAssetType assetType;
        public string[] otherAssetPaths;
    }

}