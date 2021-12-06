using ShipDock.Tools;
using System.Collections.Generic;

namespace IsKing
{
    public class PlayerHerosController
    {
        private int[] mNewHeros;
        private KeyValueList<int, HeroFields> mHeros;
        private List<HeroFields> mTeamHeros;

        public PlayerHerosController()
        {
            mTeamHeros = new List<HeroFields>();
            mHeros = new KeyValueList<int, HeroFields>();
        }

        public void Init(ref IsKingClientInfo clientInfo)
        {
            if (clientInfo.heros == default || clientInfo.heros.Length == 0)
            {
                mNewHeros = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
                Consts.D_PLAYER.DataNotify(Consts.DN_GET_HEROS_ITEMS);
            }
            else
            {
                SyncHerosFromClient(ref clientInfo);
            }
        }

        public void SyncHerosFromClient(ref IsKingClientInfo clientInfo)
        {
            HeroItem[] heroItems = clientInfo.heros;
            int max = heroItems.Length;
            HeroItem item;
            for (int i = 0; i < max; i++)
            {
                item = heroItems[i];
                "log".Log("Sync existed hero");
                AddHero(item.id, ref item);
            }
        }

        public void SyncHerosToClient(ref IsKingClientInfo clientInfo)
        {
            int max = mHeros.Size;
            HeroItem item;
            HeroFields fields;
            clientInfo.heros = new HeroItem[max];
            for (int i = 0; i < max; i++)
            {
                item = new HeroItem();
                fields = mHeros.GetValueByIndex(i);
                fields.ParseToHeroItem(ref item);
                clientInfo.heros[i] = item;
            }
        }

        public void SyncHerosFromService()
        {
            //TODO 从服务端同步将领数据
        }

        public void AddHero(int sid, ref HeroItem item)
        {
            HeroFields heroFields = new HeroFields();
            heroFields.InitFormItem(ref item);
            mHeros[sid] = heroFields;

            //先直接放入队伍
            mTeamHeros.Add(heroFields);
        }

        public int[] GetNewHeroIDs()
        {
            return mNewHeros;
        }

        public List<HeroFields> GetTeamHeros()
        {
            return mTeamHeros;//队伍将领
        }
    }

}