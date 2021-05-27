#define _LOG_UNPRESS_ZIP

using ICSharpCode.SharpZipLib.Zip;
using ShipDock.Tools;
using System;
using System.IO;

namespace ShipDock.Applications
{
    /// <summary>
    /// 
    /// Zip 解压缩
    /// 
    /// 技术引用CSDN博主「C-h-h」的原创文章，https://blog.csdn.net/chh19941125/article/details/104984884
    /// 
    /// author C-h-h
    /// modify by Minghua.ji
    /// 
    /// </summary>
    public class ZipHelper
    {
        public string ZipID { get; private set; }
        public string SavePath { get; set; }

        /// <summary> 
        /// 解压功能(下载后直接解压压缩文件到指定目录) 
        /// </summary> 
        public bool SaveZip(string ID, byte[] zipByte, string password)
        {
            bool result = true;

            ZipID = ID;

            FileStream fs = null;
            ZipEntry ent = null;
            ZipInputStream zipStream = null;
            string fileName;

            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }
            else { }

            try
            {
                //直接使用 将byte转换为Stream，省去先保存到本地在解压的过程
                Stream stream = new MemoryStream(zipByte);
                zipStream = new ZipInputStream(stream);

                if (!string.IsNullOrEmpty(password))
                {
                    zipStream.Password = password;
                }
                else { }

                byte[] data;
                int fixSize = 2048;
                int size;
                while ((ent = zipStream.GetNextEntry()) != null)
                {
                    if (!string.IsNullOrEmpty(ent.Name))
                    {
                        fileName = SavePath.Append(StringUtils.PATH_SYMBOL, ent.Name);

                        #region Android
                        //fileName = fileName.Replace('\\', '/');

                        if (fileName.EndsWith("/"))
                        {
                            Directory.CreateDirectory(fileName);
                            continue;
                        }
                        else { }
                        #endregion

                        fs = File.Create(fileName);

#if LOG_UNPRESS_ZIP
                        "log:Start uncompress zip, file name is {0}".Log(fileName);
#endif
                        size = fixSize;
                        data = new byte[size];
                        while (true)
                        {
                            size = zipStream.Read(data, 0, data.Length);
                            if (size > 0)
                            {
#if LOG_UNPRESS_ZIP
                                "log:Zip stream read size {0}".Log(size.ToString());
#endif
                                //fs.Write(data, 0, data.Length);
                                fs.Write(data, 0, size);//解决读取不完整情况
                            }
                            else
                            {
#if LOG_UNPRESS_ZIP
                                "log:File {0} uncompress end.".Log(fileName);
#endif
                                break;
                            }
                        }
                    }
                    else
                    {
                        "error:Next Entry name is empty..".Log();
                    }
                }
            }
            catch (Exception e)
            {
                "error:Zip exception {0}".Log(e.ToString());
                result = false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
                else { }

                if (zipStream != null)
                {
                    zipStream.Close();
                    zipStream.Dispose();
                }
                else { }

                if (ent != null)
                {
                    ent = null;
                }
                else { }

                GC.Collect();
                GC.Collect(1);
            }
            return result;
        }
    }
}
