namespace ShipDock
{
    public abstract class Tenon : ITenon, IReclaim
    {
        protected bool mIsDataChanged;

        private bool mEnabled;
        private bool mIsDroped;
        private bool mIsBlock;
        private bool mIsDataChangeAutoReset;
        private int mTenonID = int.MaxValue;
        private int mTenonType = int.MaxValue;
        private int mInstanceID = int.MaxValue;
        private Tenons mTenons;

        public virtual int[] SystemIDs { get; } = new int[0];

        protected virtual float DeltaTime { get; set; }
        protected bool IsReclaim { get; private set; }

        public Tenon()
        {
            Reset();
        }

        public void Reclaim()
        {
            Clear();

            mTenonID = int.MaxValue;
            mTenonType = int.MaxValue;
            mInstanceID = int.MaxValue;
        }

        public void Clear()
        {
            if (IsReclaim)
            {
                //不重复执行回收
            }
            else
            {
                IsReclaim = true;
                mIsDataChangeAutoReset = false;

                RemoveFromSystems();
                ResetData();
                Purge();
                Reset();

                IsReclaim = false;
            }
        }

        protected abstract void Purge();

        protected virtual void Reset()
        {
            mTenons = default;
            mEnabled = true;
            mIsDroped = false;
        }

        public virtual void SetupTenon(Tenons tenons)
        {
            mTenons = tenons;
            SetEnabled(true);
            CreateData();
            ApplySystems();
        }

        private void ApplySystems()
        {
            if (mTenons != default)
            {
                int[] systemIDs = SystemIDs;
                int len = systemIDs.Length;
                for (int i = 0; i < len; i++)
                {
                    BindSystem(ref mTenons, systemIDs[i]);
                }
            }
            else { }
        }

        public void RemoveFromSystems()
        {
            if (mTenons != default)
            {
                int[] systemIDs = SystemIDs;
                int len = systemIDs.Length;
                for (int i = 0; i < len; i++)
                {
                    DebindSystem(ref mTenons, systemIDs[i]);
                }
            }
            else { }
        }

        protected virtual void BindSystem(ref Tenons tenons, int systemID) 
        {
            //Sample: tenons.BindSystem<T>(this, systemID);
        }

        protected virtual void DebindSystem(ref Tenons tenons, int systemID)
        {
            //Sample: tenons.DebindSystem<T>(this, systemID);
        }

        protected virtual void CreateData()
        {
            mIsBlock = false;
            mIsDataChanged = false;
        }

        protected virtual void ResetData() { }

        public virtual T GetData<T>() where T : struct//IECSData
        {
            return default;
        }

        protected virtual bool CheckAndKeepingDataChanged()
        {
            return false;
        }

        public virtual void DataValid()
        {
            mIsDataChanged = true;
        }

        public virtual void ResetDataChangedMark()
        {
            mIsDataChanged = false;
        }

        public void PerFrameInit(float deltaTime)
        {
            if (mIsDataChangeAutoReset)
            {
                //不处理自动重置数据变更标记的情况
            }
            else
            {
                if (mIsDataChanged && CheckAndKeepingDataChanged())
                {
                    //暂不处理不需要重置数据变更标记的情况
                }
                else
                {
                    mIsDataChanged = false;
                }
            }

            OnTenonFrameInit(deltaTime);
        }

        protected virtual void OnTenonFrameInit(float deltaTime) { }

        public void PerFrameReady(float deltaTime)
        {
            DeltaTime = deltaTime;
            OnTenonFrameReady(deltaTime);
        }

        protected virtual void OnTenonFrameReady(float deltaTime) { }

        public void PerFrameFixed(float deltaTime)
        {
            DeltaTime = deltaTime;
            OnTenonFrameFixed(deltaTime);
        }

        protected virtual void OnTenonFrameFixed(float deltaTime) { }

        public void PerFrame(float deltaTime)
        {
            DeltaTime = deltaTime;
            OnTenonFrame(deltaTime);
        }

        protected virtual void OnTenonFrame(float deltaTime) { }

        public void PerFrameLate()
        {
            OnTenonFrameLate();
        }

        protected virtual void OnTenonFrameLate() { }

        public void PerFrameEnd()
        {
            OnTenonFrameEnd();
        }

        protected virtual void OnTenonFrameEnd() { }

        public virtual void Drop()
        {
            mIsBlock = true;
            mIsDroped = true;
        }

        public void CancelDrop()
        {
            mIsBlock = false;
            mIsDroped = false;
        }

        public bool IsDroped()
        {
            return mIsDroped;
        }

        public bool IsBlock()
        {
            return mIsBlock;
        }

        public bool IsEnabled()
        {
            return mEnabled;
        }

        public void SetEnabled(bool value)
        {
            mIsBlock = !value;
            mEnabled = value;
        }

        public int GetTenonID()
        {
            return mTenonID;
        }

        public void SetTenonID(int value)
        {
            mTenonID = value;
        }

        public int GetInstanceIndex()
        {
            return mInstanceID;
        }

        public void SetInstanceIndex(int value)
        {
            mInstanceID = value;
        }

        public int GetTenonType()
        {
            return mTenonType;
        }

        public void SetTenonType(int value)
        {
            mTenonType = value;
        }

        public void SetDataChangeAutoReset(bool value)
        {
            mIsDataChangeAutoReset = value;
        }

        public bool IsDataChanged()
        {
            return mIsDataChanged;
        }
    }

}