#define XLSX

using Excel;
using ShipDock.Applications;
using ShipDock.Tools;
using System.Data;
using System.IO;
using System.Text;
using UnityEngine;

namespace ShipDock.Editors
{
    /// <summary>
    /// 
    /// Excel表格转配置文件工具
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class ExcelTool
    {
        /// <summary>
        /// 配置转换的相关设置
        /// </summary>
        public class ExeclSetting
        {
            /// <summary>配置数据的起始行列定义</summary>
            public ExeclDefination dataStart;
            /// <summary>类信息（类名等）的行列定义</summary>
            public ExeclDefination classDefine;
            /// <summary>生成的配置文件名</summary>
            public ExeclDefination generateFileName;
            /// <summary>类模板信息（映射对象、静态类等）的行列定义</summary>
            public ExeclDefination classType;
            /// <summary>id字段名的行列定义</summary>
            public ExeclDefination IDFieldName;
            /// <summary>字段类型的行列定义</summary>
            public ExeclDefination dataType;
            /// <summary>字段名的行列定义</summary>
            public ExeclDefination keyFieldDef;
            /// <summary>注释的行列定义</summary>
            public ExeclDefination noteFieldDef;
        }

        public struct ExeclDefination
        {
            public int row;
            public int column;

            public ExeclDefination(int rowValue, int colValue)
            {
                row = rowValue;
                column = colValue;
            }
        }

        public static ExeclSetting Setting { get; set; }

        /// <summary>
        /// 初始化表格中特殊单元格的含义
        /// </summary>
        private static void InitExeclDefs()
        {
            Setting = new ExeclSetting
            {
                classDefine = new ExeclDefination(0, 0),
                generateFileName = new ExeclDefination(1, 0),
                classType = new ExeclDefination(0, 1),
                IDFieldName = new ExeclDefination(0, 2),
                dataStart = new ExeclDefination(5, 1),
                dataType = new ExeclDefination(1, 0),
                keyFieldDef = new ExeclDefination(3, 0),
                noteFieldDef = new ExeclDefination(4, 0),
            };
        }

        /// <summary>
        /// 类文件模板信息
        /// </summary>
        public class ClassTemplateInfo
        {
            public StringBuilder sb;
            public DataRowCollection collect;
            public int notesRow;
            public int dataStartCol;
            public string className;
            public string typeValue;
            public string IDName;
            /// <summary>类名标记</summary>
            public string rplClsName = "%cls%";
            /// <summary>id字段名标记</summary>
            public string idKeyName = "%id%";
            /// <summary>字段类型标记</summary>
            public string typeName = "%type%";
            /// <summary>字段名标记</summary>
            public string fieldName = "%fieldName%";
            /// <summary>解析配置的代码段落标记</summary>
            public string parseCode = "%parseCode%";
            /// <summary>其他代码段落标记（类的字段等代码）</summary>
            public string codes = "%codes%";
            /// <summary>注释标记</summary>
            public string notes = "%notes%";
        }

        /// <summary>生成配置文件的基础路径</summary>
        public static string configFileBasePath = string.Empty;
        /// <summary>生成配置文件的相对路径</summary>
        public static string configFileRelativePath = "configs/";
        /// <summary>生成配置文件代码类的基础路径</summary>
        public static string configCSharpCodeFileBasePath = string.Empty;
        /// <summary>生成配置文件代码类的相对路径</summary>
        public static string configCSharpCodeFileRelativePath = "/HotFix~/StaticConfigs/";

        public static string GetCSharpCodeFilePath(string className)
        {
            string relativePath = configCSharpCodeFileRelativePath.Append(className, ".cs");
            string result = string.IsNullOrEmpty(configCSharpCodeFileBasePath) ? Application.dataPath : configCSharpCodeFileBasePath;
            result = result.Append(relativePath);
            return result;
        }

        public static string GetConfigFilePath(string fileName, out string relativeName)
        {
            relativeName = configFileRelativePath.Append(fileName.ToLower(), ".bytes");
            string result = string.IsNullOrEmpty(configFileBasePath) ? AppPaths.DataPathResDataRoot : configFileBasePath;
            result = result.Append(relativeName);
            return result;
        }

        /// <summary>
        /// 读取表数据，生成对应的数组
        /// </summary>
        /// <param name="filePath">excel文件全路径</param>
        /// <returns>Item数组</returns>
        public static void CreateItemArrayWithExcel(string filePath, out string relativeName, ref string log)
        {
            InitExeclDefs();
            log += filePath + "开始解析... \r\n";

            DataRowCollection collect = ReadExcel(filePath, out int rowSize, out int colSize);//获得表数据
            ExeclDefination defination = Setting.classDefine;
            int col = defination.column, row = defination.row;
            string className = collect[row][col].ToString();//类名

            defination = Setting.generateFileName;
            col = defination.column;
            row = defination.row;
            string fileName = collect[row][col].ToString();//生成的配置文件名
            fileName = string.IsNullOrEmpty(fileName) ? className : fileName;

            defination = Setting.classType;
            col = defination.column;
            row = defination.row;
            string classType = collect[row][col].ToString();//类模板的类别

            defination = Setting.IDFieldName;
            col = defination.column;
            row = defination.row;
            string IDName = collect[row][col].ToString();//id字段的名称

            defination = Setting.dataStart;
            int dataStartRow = defination.row;
            int dataStartCol = defination.column;

            Debug.Log(rowSize);

            string cellData;
            for (int i = dataStartRow; i < rowSize; i++)
            {
                cellData = collect[i][dataStartCol].ToString();
                if (string.IsNullOrEmpty(cellData))
                {
                    rowSize = i;//找到数据起始列为空的一行，作为数据最终行
                    Debug.Log(rowSize);
                    break;
                }
                else { }
            }

            StringBuilder sb = new StringBuilder();

            int typeRow = Setting.dataType.row;//类型名所在行
            int keyRow = Setting.keyFieldDef.row;//字段名所在行
            int notesRow = Setting.noteFieldDef.row;//注释所在行

            DataRow rowData = collect[typeRow];
            object colData = rowData[dataStartCol];
            object IDTypeCell = collect[typeRow][dataStartCol];

            string typeValue = GetCSharpForTypeValueCode(IDTypeCell.ToString());
            ClassTemplateInfo info = new ClassTemplateInfo
            {
                sb = sb,
                collect = collect,
                className = className,
                notesRow = notesRow,
                dataStartCol = dataStartCol,
                typeValue = typeValue,
                IDName = IDName,
            };

            switch (classType)
            {
                case "mapper":
                    TranslateConfigCSharpClassCode(info);
                    break;
                case "const"://常量类
                    sb.Append("namespace StaticConfig");
                    sb.Append("{");
                    sb.Append("    public static class %cls%".Replace(info.rplClsName, className));
                    sb.Append("    {");
                    sb.Append("    }");
                    sb.Append("}");
                    break;
            }
            string field, notes, valueInitCode;
            string CSharpScript = sb.ToString();
            sb.Clear();

            object item;
            string parserCode = IDName.Append(GetCSharpForSetValueCode(IDTypeCell.ToString()));//获取到翻译后的 id 赋值语句

            for (int i = dataStartCol + 1; i < colSize; i++)//从id列顺延下一个字段开始构建剩余的脚本
            {
                item = collect[typeRow][i];//类型
                typeValue = item.ToString();

                if (string.IsNullOrEmpty(typeValue))
                {
                    colSize = i;//读取到无效的表头，此前的所有字段为完整的类成员字段，并更新到列数
                    break;
                }
                else { }

                typeValue = GetCSharpForTypeValueCode(typeValue);

                item = collect[keyRow][i];//字段名
                field = item.ToString();
                notes = collect[notesRow][i].ToString();
                valueInitCode = collect[typeRow][i].ToString();

                sb.Append("/// <summary>");
                sb.Append("\r\n        /// %notes%")
                    .Replace(info.notes, notes);
                sb.Append("\r\n        /// <summary>\r\n        ");
                sb.Append("public %type% %fieldName%;\r\n")
                    .Replace(info.typeName, typeValue)
                    .Replace(info.fieldName, field);
                sb.Append("        ");

                parserCode = parserCode.Append(field, GetCSharpForSetValueCode(valueInitCode));//获取到翻译后的其他成员字段赋值语句
            }

            CSharpScript = CSharpScript.Replace(info.codes, sb.ToString());//合成类成员字段的声明语句
            CSharpScript = CSharpScript.Replace(info.parseCode, parserCode);//合成类成员字段的初始化赋值语句
            sb.Clear();

            object ID;
            string IDValue;
            DataRow itemRow;
            ByteBuffer byteBuffer = ByteBuffer.Allocate(0);
            int dataRealSize = rowSize - dataStartRow;
            byteBuffer.WriteInt(dataRealSize);
            for (int i = dataStartRow; i < rowSize; i++)
            {
                itemRow = collect[i];
                ID = itemRow[dataStartCol];

                IDValue = ID.ToString();
                if (string.IsNullOrEmpty(IDValue))
                {
                    Debug.LogWarning("Config ID invalid in row " + i + ", it do not allow empty, config bytes parse will be break.");
                    break;
                }
                else
                {
                    Debug.Log("Writing in ID = " + IDValue.ToString());
                    for (int j = dataStartCol; j < colSize; j++)//从id列顺延下一个字段开始构建剩余的脚本
                    {
                        item = collect[typeRow][j];

                        typeValue = item.ToString();//字段类型
                        field = itemRow[j].ToString();//字段数据

                        WriteField(ref byteBuffer, typeValue, field, collect[keyRow][j].ToString());
                    }
                }
            }
            Debug.Log("Write finished.. length = " + byteBuffer.GetCapacity());

            string path = GetCSharpCodeFilePath(className);
            FileOperater.WriteUTF8Text(CSharpScript, path);//生成代码

            path = GetConfigFilePath(fileName, out relativeName);
            FileOperater.WriteBytes(byteBuffer.ToArray(), path);//生成配置数据文件
        }

        /// <summary>
        /// 将类信息转为基本的C#类代码
        /// </summary>
        /// <param name="info"></param>
        private static void TranslateConfigCSharpClassCode(ClassTemplateInfo info)
        {
            StringBuilder sb = info.sb;
            int row = info.notesRow;
            int col = info.dataStartCol;
            object notes = info.collect[row][col];

            sb.Append("using ShipDock.Config;\r\n\r\n");
            sb.Append("using ShipDock.Tools;\r\n\r\n");
            sb.Append("namespace StaticConfig\r\n");
            sb.Append("{\r\n");
            sb.Append("    public partial class %cls% : IConfig\r\n".Replace(info.rplClsName, info.className));
            sb.Append("    {\r\n");
            sb.Append("        /// <summary>\r\n");
            sb.Append("        /// %notes%\r\n").Replace(info.notes, notes.ToString());
            sb.Append("        /// <summary>\r\n");
            sb.Append("        public %type% %id%;\r\n\r\n").Replace(info.typeName, info.typeValue).Replace(info.idKeyName, info.IDName);//ID
            sb.Append("        %codes%\r\n");//此处容纳其他代码
            sb.Append("        public string CRCValue { get; }\r\n\r\n");
            sb.Append("        public %type% GetID()\r\n").Replace(info.typeName, info.typeValue);
            sb.Append("        {\r\n");
            sb.Append("            return id;\r\n");
            sb.Append("        }\r\n\r\n");
            sb.Append("        public void Parse(ByteBuffer buffer)\r\n");
            sb.Append("        {\r\n");
            sb.Append("            %parseCode%\r\n");
            sb.Append("        }\r\n\r\n");
            sb.Append("    }\r\n");
            sb.Append("}\r\n");
        }

        /// <summary>
        /// 将配置数据按照值对的形式写入二进制数据
        /// </summary>
        /// <param name="byteBuffer"></param>
        /// <param name="typeValue"></param>
        /// <param name="field"></param>
        /// <param name="propName"></param>
        private static void WriteField(ref ByteBuffer byteBuffer, string typeValue, string field, string propName)
        {
            switch(typeValue)
            {
                case "int32":
                    Debug.Log("Write in int32, " + propName + " = " + int.Parse(field));
                    byteBuffer.WriteInt(int.Parse(field));
                    break;
                case "string":
                    Debug.Log("Write in string, " + propName + " = " + field);
                    byteBuffer.WriteString(field);
                    break;
                case "float":
                    Debug.Log("Write in float, " + propName + " = " + float.Parse(field));
                    byteBuffer.WriteFloat(float.Parse(field));
                    break;
                case "bool":
                    int value = field == "True" || field == "TRUE" ? 1 : 0;
                    Debug.Log("Write in bool, field is " + field);
                    Debug.Log("Write in bool, " + propName + " = " + value);
                    byteBuffer.WriteInt(value);
                    break;
            }
        }

        /// <summary>
        /// 根据单元格定义的类型获取C#代码中的成员字段类型
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private static string GetCSharpForTypeValueCode(string v)
        {
            string result;
            switch (v)
            {
                case "int32":
                    result = "int";
                    break;
                case "string":
                    result = "string";
                    break;
                case "float":
                    result = "float";
                    break;
                case "bool":
                    result = "bool";
                    break;
                default:
                    result = "object";
                    break;
            }
            return result;
        }

        /// <summary>
        /// 根据单元格定义的类型获取C#代码中为成员字段赋值的代码
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private static string GetCSharpForSetValueCode(string v)
        {
            string result = string.Empty;
            switch (v)
            {
                case "int32":
                    result = " = buffer.ReadInt();\r\n";
                    break;
                case "string":
                    result = " = buffer.ReadString();\r\n";
                    break;
                case "float":
                    result = " = buffer.ReadFloat();\r\n";
                    break;
                case "bool":
                    result = " = buffer.ReadInt() != 0;\r\n";
                    break;
            }
            return result.Append("            ");
        }

        /// <summary>
        /// 读取excel文件内容
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="rowSize">行数</param>
        /// <param name="colSize">列数</param>
        /// <returns></returns>
        static DataRowCollection ReadExcel(string filePath, out int rowSize, out int colSize, int sheetIndex = 0)
        {
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
#if XLSX
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);//xlsx
#else
            IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);//xls
#endif
            Debug.Log(excelReader.IsValid);
            DataSet result = excelReader.AsDataSet();
            DataTable dataTable = result.Tables[sheetIndex];//下标0表示excel文件中第一张表的数据
            rowSize = dataTable.Rows.Count;
            colSize = dataTable.Columns.Count;
            return dataTable.Rows;
        }
    }
}