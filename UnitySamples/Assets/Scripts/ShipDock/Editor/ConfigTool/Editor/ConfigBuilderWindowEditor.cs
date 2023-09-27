using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static ShipDock.Editors.ExcelTool;

namespace ShipDock.Editors
{
    public class ConfigBuilderPopupEditor : ShipDockEditor
    {
        /// <summary>
        /// 生成配置文件
        /// </summary>
        [MenuItem("ShipDock/Generate Configs")]
        public static ConfigBuilderPopupEditor GenerateConfigCodeAndFile()
        {
            return Popup();
        }

        public static ConfigBuilderPopupEditor Popup()
        {
            InitEditorWindow<ConfigBuilderPopupEditor>("构建配置文件");
            return focusedWindow as ConfigBuilderPopupEditor;
        }

        private ExeclSetting mDefaultExeclSetting = new ExeclSetting
        {
            classDefine = new ExeclDefination(0, 0),
            generateFileName = new ExeclDefination(1, 0),
            classType = new ExeclDefination(0, 1),
            IDFieldName = new ExeclDefination(0, 2),
            dataType = new ExeclDefination(1, 0),
            keyFieldDef = new ExeclDefination(3, 0),
            noteFieldDef = new ExeclDefination(4, 0),
            dataStart = new ExeclDefination(5, 1),
        };

        private string mHasEdite = "hasEdite";
        private string mShouldEdite = "shouldEdite";
        private string mRowClassDefine = "rowClassDefine";
        private string mColClassDefine = "colClassDefine";
        private string mRowGenFileNameDefine = "rowGenFileNameDefine";
        private string mColGenFileNameDefine = "colGenFileNameDefine";
        private string mRowClassTypeDefine = "rowClassTypeDefine";
        private string mColClassTypeDefine = "colClassTypeDefine";
        private string mRowIDFieldDefine = "rowIDFieldDefine ";
        private string mColIDFieldDefine = "colIDFieldDefine ";
        private string mRowDataTypeDefine = "rowDataTypeDefine";
        private string mColDataTypeDefine = "colDataTypeDefine";
        private string mRowKeyFieldDefine = "rowKeyFieldDefine";
        private string mColKeyFieldDefine = "colKeyFieldDefine";
        private string mRowNotesDefine = "rowNotesDefine";
        private string mColNotesDefine = "colNotesDefine";
        private string mRowDataStartDefine = "rowDataStartDefine";
        private string mColDataStartDefine = "colDataStartDefine";
        private string mCustomCSFilePath = "customCSFilePath";
        private string mCustomCSFileRelativePath = "customCSFileRelativePath";
        private string mCustomCSFilePathToShow = "customCSFilePathToShow";

        protected override void ReadyClientValues()
        {
            ReadValueItemValueFromEditor(mHasEdite);
            ReadValueItemValueFromEditor(mShouldEdite);

            ReadValueItemValueFromEditor(mRowClassDefine);
            ReadValueItemValueFromEditor(mColClassDefine);

            ReadValueItemValueFromEditor(mRowGenFileNameDefine);
            ReadValueItemValueFromEditor(mColGenFileNameDefine);

            ReadValueItemValueFromEditor(mRowClassTypeDefine);
            ReadValueItemValueFromEditor(mColClassTypeDefine);

            ReadValueItemValueFromEditor(mRowIDFieldDefine);
            ReadValueItemValueFromEditor(mColIDFieldDefine);

            ReadValueItemValueFromEditor(mRowDataTypeDefine);
            ReadValueItemValueFromEditor(mColDataTypeDefine);

            ReadValueItemValueFromEditor(mCustomCSFilePath);
            ReadValueItemValueFromEditor(mCustomCSFileRelativePath);
        }

        protected override void UpdateClientValues() { }

        private void FillSettingFieldValues(string title, ref string keyField, int defaultValue, out int value)
        {
            bool hasEdite = GetValueItem(mHasEdite).Bool;

            ValueItem valueItem = GetValueItem(keyField);
            value = hasEdite ? valueItem.Int : defaultValue;

            if (hasEdite) { }
            else
            {
                SetValueItem(keyField, value.ToString());
            }

            bool shouldEdite = GetValueItem(mShouldEdite).Bool;
            if (shouldEdite)
            {
                ValueItemTextField(keyField, title);
            }
            else { }
        }

        protected override void CheckGUI()
        {
            base.CheckGUI();

            ValueItemTriggle(mShouldEdite, "修改导出设置");

            RefreshConfigPathSetting();
            RefreshConfigParseSettings();
            RefreshButtonPanel();
        }

        private void RefreshButtonPanel()
        {
            bool shouldEdite = GetValueItem(mShouldEdite).Bool;

            if (shouldEdite)
            {
                string customCSFilePath = default;
                string customCSFileRelativePath = default;

                if (ValueItemButton(string.Empty, "Reset To Default Setting"))
                {
                    ConfirmPopup("还原导出设置", "确定还原此导出设置为默认？", () => {
                        SetValueItem(mHasEdite, FALSE);

                        ResetSetting();

                        configCSharpCodeFileBasePath = string.Empty;
                        configCSharpCodeFileRelativePath = DEFAULT_CSHARP_CODE_RELATE_PATH;

                        customCSFilePath = configCSharpCodeFileBasePath;
                        customCSFileRelativePath = configCSharpCodeFileRelativePath;
                        SetValueItem(mCustomCSFilePath, customCSFilePath);
                        SetValueItem(mCustomCSFileRelativePath, customCSFileRelativePath);

                        WriteValueItemDataToEditor(mCustomCSFilePath, mCustomCSFileRelativePath);

                    });
                }
                else { }

                if (ValueItemButton(string.Empty, "Change Config Setting"))
                {
                    ConfirmPopup("修改导出设置", "确定修改此导出设置？", () => {
                        SetValueItem(mHasEdite, TRUE);

                        customCSFilePath = GetValueItem(mCustomCSFilePath).Value;
                        customCSFileRelativePath = GetValueItem(mCustomCSFileRelativePath).Value;

                        configCSharpCodeFileBasePath = customCSFilePath;
                        configCSharpCodeFileRelativePath = customCSFileRelativePath;

                        SetValueItem(mCustomCSFilePath, customCSFilePath);
                        SetValueItem(mCustomCSFileRelativePath, customCSFileRelativePath);

                        WriteValueItemDataToEditor(mCustomCSFilePath, mCustomCSFileRelativePath);

                        SetValueItem(mShouldEdite, FALSE);
                    });
                }
                else { }
            }
            else
            {
                LayoutV(() => { LayoutSpace(50f); });
                if (ValueItemButton(string.Empty, "Start Build Config"))
                {
                    string path, relativeName;
                    string[] strs = Selection.assetGUIDs;
                    string log = string.Empty;
                    List<string> relativeNames = new List<string>();
                    foreach (var item in strs)
                    {
                        path = AssetDatabase.GUIDToAssetPath(item);
                        CreateItemArrayWithExcel(path, out relativeName, ref log);
                        relativeNames.Add(relativeName.ToLower());
                    }
                    AssetDatabase.Refresh();
                    Close();
                }
                else { }
            }
        }

        private void RefreshConfigPathSetting()
        {
            string filePath = GetValueItem(mCustomCSFilePath).Value;
            string vCustomCSFilePath = string.IsNullOrEmpty(filePath) ? configCSharpCodeFileBasePath : filePath;

            filePath = GetValueItem(mCustomCSFileRelativePath).Value;
            string vCustomCSFileRelativePath = string.IsNullOrEmpty(filePath) ? configCSharpCodeFileRelativePath : filePath;

            bool shouldEdite = GetValueItem(mShouldEdite).Bool;
            if (shouldEdite)
            {
                SetValueItem(mCustomCSFilePath, vCustomCSFilePath);
                ValueItemLabel(string.Empty, "自定义 C# 类完整路径: ");
                ValueItemTextField(mCustomCSFilePath, string.Empty);

                if (string.IsNullOrEmpty(vCustomCSFilePath))
                {
                    SetValueItem(mCustomCSFileRelativePath, vCustomCSFileRelativePath);
                    ValueItemLabel(string.Empty, "自定义 C# 类相对路径: ");
                    ValueItemTextField(mCustomCSFileRelativePath, string.Format("{0}", Application.dataPath));
                }
                else { }
            }
            else
            {
                string filePathToShow = string.IsNullOrEmpty(vCustomCSFilePath) ? Application.dataPath.Append(vCustomCSFileRelativePath) : vCustomCSFilePath;
                ValueItemLabel(string.Empty, "自定义 C# 将保存于: ");
                ValueItemLabel(string.Empty, filePathToShow);
            }
        }

        private void RefreshConfigParseSettings()
        {
            LayoutH(() =>
            {
                LayoutV(() =>
                {
                    FillSettingFieldValues("类名 - 行", ref mRowClassDefine, mDefaultExeclSetting.classDefine.row, out int rowClsDef);
                    FillSettingFieldValues("类名 - 列", ref mColClassDefine, mDefaultExeclSetting.classDefine.column, out int colClsDef);
                });

                LayoutV(() =>
                {
                    FillSettingFieldValues("类模板 - 行", ref mRowClassTypeDefine, mDefaultExeclSetting.classType.row, out int rowClsTypeDef);
                    FillSettingFieldValues("类模板 - 列", ref mColClassTypeDefine, mDefaultExeclSetting.classType.column, out int colClsTypeDef);
                });
            });

            LayoutH(() =>
            {
                LayoutV(() =>
                {
                    FillSettingFieldValues("配置文件名 - 行", ref mRowGenFileNameDefine, mDefaultExeclSetting.generateFileName.row, out int rowGenFileNameDef);
                    FillSettingFieldValues("配置文件名 - 列", ref mColGenFileNameDefine, mDefaultExeclSetting.generateFileName.column, out int colGenFileNameDef);
                });
                LayoutV(() =>
                {
                    FillSettingFieldValues("ID字段起始 - 行", ref mRowIDFieldDefine, mDefaultExeclSetting.IDFieldName.row, out int rowIDFieldDef);
                    FillSettingFieldValues("ID字段起始 - 列", ref mColIDFieldDefine, mDefaultExeclSetting.IDFieldName.column, out int colIDFieldDef);
                });
            });

            LayoutH(() =>
            {
                LayoutV(() =>
                {
                    FillSettingFieldValues("成员类型起始 - 行", ref mRowDataTypeDefine, mDefaultExeclSetting.dataType.row, out int rowDataTypeDef);
                    FillSettingFieldValues("成员类型起始 - 列", ref mColDataTypeDefine, mDefaultExeclSetting.dataType.column, out int colDataTypeDef);
                });

                LayoutV(() =>
                {
                    FillSettingFieldValues("成员字段名起始 - 行", ref mRowKeyFieldDefine, mDefaultExeclSetting.keyFieldDef.row, out int rowKeyFieldDef);
                    FillSettingFieldValues("成员字段名起始 - 列", ref mColKeyFieldDefine, mDefaultExeclSetting.keyFieldDef.column, out int colKeyFieldDef);
                });
            });

            LayoutH(() =>
            {
                LayoutV(() =>
                {
                    FillSettingFieldValues("注释起始 - 行", ref mRowNotesDefine, mDefaultExeclSetting.noteFieldDef.row, out int rowNotesDef);
                    FillSettingFieldValues("注释起始 - 列", ref mColNotesDefine, mDefaultExeclSetting.noteFieldDef.column, out int colNotesDef);
                });

                LayoutV(() =>
                {
                    FillSettingFieldValues("数据起始 - 行", ref mRowDataStartDefine, mDefaultExeclSetting.dataStart.row, out int rowDataStartDef);
                    FillSettingFieldValues("数据起始 - 列", ref mColDataStartDefine, mDefaultExeclSetting.dataStart.column, out int colDataStartDef);
                });
            });
        }

        private void ResetSetting()
        {
            Setting = mDefaultExeclSetting;
        }
    }
}
