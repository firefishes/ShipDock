using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShipDock
{
    public abstract class ECSSystem : IECSSystem
    {
        protected int mSearchResultsMax;
        protected List<EntitySearchResult> mSearchResults;

        public abstract int SystemID { get; }
        public Action<int, int> OnSearchResults { get; set; }

        public abstract void Execute();

        protected virtual void DuringExecute<T>(T data) where T : struct//IECSData
        {
        }

        public virtual void Init()
        {
        }

        public void SetSearchResult(int max, List<EntitySearchResult> searchResults)
        {
            mSearchResultsMax = max;
            mSearchResults = searchResults;

            Parallel.For(0, mSearchResultsMax, i =>
            {
                EntitySearchResult searchResult = mSearchResults[i];
                //OnSearchedResult(searchResult.componentID, searchResult.info.entity);
                OnSearchResults?.Invoke(searchResult.componentID, searchResult.info.entity);
            });

            OnSearchResults = default;
            mSearchResults = default;
        }

        protected T GetComponentDataByEntity<T>(int componentID, int entity, out bool isValid) where T : struct
        {
            IECSComponent<T> componentBase = (IECSComponent<T>)ECS.Instance.GetComponentByBase(componentID);
            T data = componentBase.GetEntityData(entity, out isValid);
            return data;
        }
    }

}