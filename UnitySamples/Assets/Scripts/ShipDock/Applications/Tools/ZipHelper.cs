#define _LOG_UNPRESS_ZIP

using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;

namespace ShipDock
{
    /// <summary>
    /// 
    /// Zip 解压缩
    /// 
    /// 技术引用CSDN博主「C-h-h」的原创文章，https://blog.csdn.net/chh19941125/article/details/104984884
    /// 引用的库文件：I18N.CJK.dll，I18N.dll，I18N.West.dll，Mono.Data.Tds.dll，System.Data.dll
    /// 
    /// author C-h-h
    /// modify by Minghua.ji
    /// 
    /// </summary>
    public class ZipHelper : IUpdate
    {
        public enum ZipOperation
        {
            None = 0,
            Decompress,
            compress,
        }

        private string mPassword;
        private ZipEntry mEnt;
        private ZipInputStream mZipInputStream;

        public ZipOperation Operation { get; private set; } = ZipOperation.None;
        public string ZipID { get; private set; }
        public string SavePath { get; set; }
        public bool IsCompleted { get; private set; }
        public bool IsUpdate { get; } = true;
        public bool IsFixedUpdate { get; }
        public bool IsLateUpdate { get; }
        public Action OnCompleted { get; set; }
        public FileStream FileStream { get; private set; }

        public void AddUpdate() { }

        public void RemoveUpdate() { }

        public void OnLateUpdate() { }

        public void OnFixedUpdate(int dTime) { }

        public void OnUpdate(int dTime)
        {
            if (Operation == ZipOperation.Decompress)
            {
                DuringDecompress();
            }
            else
            {
            }
        }

        private void DuringDecompress()
        {
            if (!IsCompleted)
            {
                try
                {
                    int size;
                    int fixSize = 2048;
                    byte[] data;
                    string fileName;
                    while ((mEnt = mZipInputStream.GetNextEntry()) != null)
                    {
                        if (!string.IsNullOrEmpty(mEnt.Name))
                        {
                            fileName = Path.Combine(SavePath, mEnt.Name);

                            #region Android
                            fileName = fileName.Replace('\\', '/');

                            "log:Start uncompress zip, file name is {0}".Log(fileName);

                            if (fileName.EndsWith(StringUtils.PATH_SYMBOL))
                            {
                                Directory.CreateDirectory(fileName);
                                continue;
                            }
                            else { }
                            #endregion

                            FileStream = File.Create(fileName);

                            size = fixSize;
                            data = new byte[size];
                            while (true)
                            {
                                size = mZipInputStream.Read(data, 0, data.Length);

#if LOG_UNPRESS_ZIP
                                bool isReadAll = size <= 0;
                                if (isReadAll)
                                {
                                    "log:File {0} uncompress end.".Log(fileName);
                                }
                                else
                                {
                                    "log:Zip stream read size {0}".Log(size.ToString());
                                }
#endif
                                if (size > 0)
                                {
                                    FileStream.Write(data, 0, size);//解决读取不完整情况
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            "error".Log("Next Entry name is empty..");
                        }
                    }
                }
                catch (Exception e)
                {
                    "error:Zip exception {0}".Log(e.ToString());
                }
                finally
                {
                    if (FileStream != null)
                    {
                        FileStream.Close();
                        FileStream.Dispose();
                    }
                    else { }

                    if (mZipInputStream != null)
                    {
                        mZipInputStream.Close();
                        mZipInputStream.Dispose();
                    }
                    else { }

                    if (mEnt != null)
                    {
                        mEnt = null;
                    }
                    else { }

                    GC.Collect();
                    GC.Collect(1);

                    IsCompleted = true;
                    Operation = ZipOperation.None;
                    UpdaterNotice.SceneCallLater((t) => { OnCompleted?.Invoke(); });

                }
            }
            else { }
        }

        /// <summary> 
        /// 解压功能(下载后直接解压压缩文件到指定目录) 
        /// </summary> 
        public void SaveZip(string ID, byte[] zipByte, string password)
        {
            Operation = ZipOperation.Decompress;
            ZipID = ID;
            mPassword = password;

            //直接使用 将byte转换为Stream，省去先保存到本地在解压的过程
            Stream stream = new MemoryStream(zipByte);
            mZipInputStream = new ZipInputStream(stream);

            if (!string.IsNullOrEmpty(mPassword))
            {
                mZipInputStream.Password = mPassword;
            }
            else { }

            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }
            else { }

            UpdaterNotice.AddUpdater(this);
        }
    }
}
