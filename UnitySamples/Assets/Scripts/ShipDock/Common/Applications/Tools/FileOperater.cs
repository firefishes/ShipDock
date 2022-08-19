﻿using ShipDock.Interfaces;
using ShipDock.Tools;
using System.IO;
using System.Text;
using UnityEngine;

namespace ShipDock.Applications
{
    public class FileCommandInfo : IReclaim
    {
        public enum FileCommandInfoReadType
        {
            Chars = 0,
            Bytes,
        }

        public byte[] DataBytes;
        public char[] DataChars;
        public string FileFullName;
        public string RelativePath;
        public FileCommandInfoReadType FileReadType = FileCommandInfoReadType.Bytes;

        public void Reclaim()
        {
            DataBytes = null;
            DataChars = null;
            FileFullName = string.Empty;
            RelativePath = string.Empty;
        }
    }

    public class FileOperater
    {
        public static void DeleteFileDirection(string directionName)
        {
            if (!string.IsNullOrEmpty(directionName))
            {
                if (Directory.Exists(directionName))
                {
                    Directory.Delete(directionName, true);//注意：这里参数"true"表示可以删除非空目录
                }
                else { }
            }
            else { }
        }

        public static void WriteUTF8Text(string content, string filePath, FileOperater operater = default)
        {
            FileOperater fileOperater = (operater == null) ? new FileOperater() : operater;
            FileCommandInfo fileCommandInfo = new FileCommandInfo
            {
                FileFullName = filePath,
                FileReadType = FileCommandInfo.FileCommandInfoReadType.Bytes,
                DataBytes = Encoding.UTF8.GetBytes(content)
            };
            fileOperater.WriteFile(ref fileCommandInfo);
            fileCommandInfo.Reclaim();
        }

        public static void WriteBytes(string content, string filePath, FileOperater operater = default)
        {
            byte[] vs = Encoding.UTF8.GetBytes(content);
            WriteBytes(vs, filePath, operater);
        }

        public static void WriteBytes(byte[] vs, string filePath, FileOperater operater = default)
        {
            FileOperater fileOperater = (operater == null) ? new FileOperater() : operater;
            FileCommandInfo fileCommandInfo = new FileCommandInfo
            {
                FileFullName = filePath,
                FileReadType = FileCommandInfo.FileCommandInfoReadType.Bytes,
                DataBytes = vs
            };
            fileOperater.WriteFile(ref fileCommandInfo);
            fileCommandInfo.Reclaim();
        }

        public static string ReadUTF8Text(string filePath, FileOperater operater = null)
        {
            FileOperater fileOperater = (operater == null) ? new FileOperater() : operater;
            FileCommandInfo fileCommandInfo = new FileCommandInfo
            {
                FileFullName = filePath,
                FileReadType = FileCommandInfo.FileCommandInfoReadType.Bytes
            };
            fileOperater.ReadFile(ref fileCommandInfo);

            byte[] bytes = fileCommandInfo.DataBytes;
            string result = (bytes == null) ? string.Empty : Encoding.UTF8.GetString(bytes);
            fileCommandInfo.Reclaim();

            return result;
        }

        public static byte[] ReadBytes(string filePath, FileOperater operater = default)
        {
            FileOperater fileOperater = (operater == null) ? new FileOperater() : operater;
            FileCommandInfo fileCommandInfo = new FileCommandInfo
            {
                FileFullName = filePath,
                FileReadType = FileCommandInfo.FileCommandInfoReadType.Bytes
            };
            fileOperater.ReadFile(ref fileCommandInfo);

            byte[] bytes = fileCommandInfo.DataBytes;
            fileCommandInfo.Reclaim();

            return bytes;
        }

        public bool IsDirectoryExistsOrCreateIt(ref string fileFullName, bool isCreateDirect = true)
        {
            bool result = Directory.Exists(GetDirectoryName(fileFullName));
            if (!result)
            {
                if (isCreateDirect)
                {
                    Directory.CreateDirectory(GetDirectoryName(fileFullName));
                }
            }
            return result;
        }

        public bool IsFileExists(ref string fileFullName)
        {
            return File.Exists(fileFullName);
        }

        public void ReadFile(ref FileCommandInfo fileInfo)
        {
            if (fileInfo == null)
            {
                return;
            }

            string fileFullName = fileInfo.FileFullName;
            IsDirectoryExistsOrCreateIt(ref fileFullName);

            if (!File.Exists(fileFullName))
            {
#if UNITY_EDITOR
                switch(fileInfo.FileReadType)
                {
                    case FileCommandInfo.FileCommandInfoReadType.Chars:
                        WriteUTF8Text(string.Empty, fileFullName);
                        break;
                    case FileCommandInfo.FileCommandInfoReadType.Bytes:
                        WriteBytes(new byte[] { }, fileFullName);
                        break;
                }
#else
                Debug.Log("error: File is not exists, path is ".Append(fileFullName));
                return;
#endif
            }

            using (FileStream fs = new FileStream(fileFullName, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    switch (fileInfo.FileReadType)
                    {
                        case FileCommandInfo.FileCommandInfoReadType.Chars:
                            char[] chars = new char[fs.Length];
                            br.Read(chars, 0, chars.Length);
                            fileInfo.DataChars = chars;
                            break;
                        case FileCommandInfo.FileCommandInfoReadType.Bytes:
                            byte[] bytes = new byte[fs.Length];
                            br.Read(bytes, 0, bytes.Length);
                            fileInfo.DataBytes = bytes;
                            break;
                    }
                }
            }
        }

        /// <summary>保存数据</summary>
        public void WriteFile(ref FileCommandInfo fileInfo)
        {
            if (fileInfo == null)
            {
                return;
            }

            string fileFullName = fileInfo.FileFullName;
            IsDirectoryExistsOrCreateIt(ref fileFullName);

            using (FileStream fs = new FileStream(fileFullName, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    Debug.Log("Writing......");
                    bw.Write(fileInfo.DataBytes);//写入
                    Debug.Log("Writing Finished. File name is ".Append(fileFullName));
                    bw.Flush();//清空缓冲区
                    bw.Close();//关闭流
                }
                fs.Close();
            }
        }

        /// <summary>获取文件的目录</summary>
        public string GetDirectoryName(string fileFullName)
        {
            return fileFullName.Substring(0, fileFullName.LastIndexOf(StringUtils.PATH_SYMBOL_CHAR));
        }
    }
}