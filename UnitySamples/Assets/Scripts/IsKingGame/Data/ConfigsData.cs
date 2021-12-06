using ShipDock.Datas;
using ShipDock.Tools;

namespace IsKing
{
    public class ConfigsData : DataProxy, IDataExtracter
    {
        private KeyValueList<int, IGameItemCollections> mGameItemCollections;

        public ConfigsData() : base(Consts.D_CONFIGS)
        {
            mGameItemCollections = new KeyValueList<int, IGameItemCollections>();
        }

        public void Init()
        {
            this.DataProxyLink(Consts.D_PLAYER);
        }

        public void LoadItems(int type, IGameItemCollections collections)
        {
            mGameItemCollections[type] = collections;
            collections.InitCollections();
        }

        public T GetGameItem<T>(int itemType, int id) where T : IGameItem
        {
            IGameItem result = default;
            IGameItemCollections collections = mGameItemCollections[itemType];
            if (collections != default)
            {
                switch (itemType)
                {
                    case Consts.ITEM_HERO:
                        result = (collections as HeroCollections).GetItem(id);
                        break;
                    case Consts.ITEM_SKILL:
                        result = (collections as SkillCollections).GetItem(id);
                        break;
                    case Consts.ITEM_SKILL_EFFECT:
                        result = (collections as SkillEffectCollections).GetItem(id);
                        break;
                }
            }
            else { }

            return (T)result;
        }

        public void OnDataProxyNotify(IDataProxy data, int DCName)
        {
            if (data is PlayerData playerData)
            {
                switch (DCName)
                {
                    case Consts.DN_GET_HEROS_ITEMS:
                        int[] heroIDs = default;
                        heroIDs = playerData.Events.DispatchWithParam(PlayerData.N_GET_NEW_HEROS, heroIDs);
                        int max = heroIDs.Length;
                        HeroItem item;
                        for (int i = 0; i < max; i++)
                        {
                            item = GetGameItem<HeroItem>(Consts.ITEM_HERO, heroIDs[i]);
                            "log".Log("Add new hero");
                            playerData.Heros.AddHero(item.id, ref item);
                        }
                        break;
                }
            }
        }
    }

}