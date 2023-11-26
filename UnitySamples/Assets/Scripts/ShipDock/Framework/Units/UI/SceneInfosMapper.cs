using UnityEngine;

namespace ShipDock
{

    public abstract class SceneInfosMapper<K, V> : KeyValueList<K, V>
    {
        [SerializeField]
        protected bool m_DisposeInfos;

        [SerializeField]
        public V[] infos;

        public virtual void InitInfos(V[] source = default)
        {
            if (source != default)
            {
                infos = source;
            }
            else { }

            V info;
            int max = infos.Length;
            for (int i = 0; i < max; i++)
            {
                info = infos[i];
                K key = GetInfoKey(ref info);
                Put(key, info);
                AfterInitItem(ref info);
            }
        }

        public override void Reclaim()
        {
            base.Reclaim();

            Utils.Reclaim(ref infos, true, m_DisposeInfos);
        }

        protected virtual void AfterInitItem(ref V item) { }

        public abstract K GetInfoKey(ref V item);

    }
}