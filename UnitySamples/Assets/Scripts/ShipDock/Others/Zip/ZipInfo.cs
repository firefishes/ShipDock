#define _LOG_UNPRESS_ZIP

using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace ShipDock
{
    public class ZipInfo : IUpdate
    {
        private int mSize;
        private byte[] mBytes;
        private ZipInputStream mZipInputStream;

        public bool IsCompleted { get; private set; }
        public string FileName { get; private set; }
        public int StreamReadSize { get; private set; }
        public FileStream FileStream { get; private set; }

        public bool IsUpdate { get; }
        public bool IsFixedUpdate { get; }
        public bool IsLateUpdate { get; }

        public ZipInfo(string fileName, int size, ZipInputStream zipStream)
        {
            StreamReadSize = size;
            mZipInputStream = zipStream;
            mBytes = new byte[size];

            FileName = fileName;
            FileStream = File.Create(fileName);
        }

        private void Uncompress()
        {
            if (!IsCompleted)
            {
                mSize = mZipInputStream.Read(mBytes, 0, mBytes.Length);

#if LOG_UNPRESS_ZIP
                bool isReadAll = mSize <= 0;
                if (isReadAll)
                {
                    "log:File {0} uncompress end.".Log(FileName);
                }
                else
                {
                    "log:Zip stream read size {0}".Log(mSize.ToString());
                }
#endif
                if (mSize <= 0)
                {
                    IsCompleted = true;
                    FileStream.Write(mBytes, 0, mSize);//解决读取不完整情况
                }
                else { }
            }
            else { }
        }

        public void OnUpdate(float dTime)
        {
            Uncompress();

            if (IsCompleted)
            {
                UpdaterNotice.RemoveUpdater(this);

                FileStream.Close();
                FileStream.Dispose();

                mZipInputStream = default;
            }
            else { }
        }

        public void AfterAddUpdate() { }

        public void AfterRemoveUpdate() { }

        public void OnLateUpdate() { }

        public void OnFixedUpdate(float dTime) { }
    }

}