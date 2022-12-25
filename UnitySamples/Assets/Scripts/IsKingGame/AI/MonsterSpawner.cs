using System;
using ShipDock.Applications;
using ShipDock.ECS;
using ShipDock.Loader;
using ShipDock.Notices;
using ShipDock.Tools;
using UnityEngine;

namespace IsKing
{
    public class MonsterSpawner : MonoBehaviour
    {
        public static int allMonster = 0;

        [SerializeField]
        private ResPrefabBridge m_Res;

        [SerializeField]
        private int m_AllMonsters;

        [SerializeField]
        private float m_Time = 1f;
        [SerializeField]
        private float m_SpwanGapTime = 0.2f;

        [SerializeField]
        private int m_Count = 1;
        [SerializeField]
        private int m_Preload = 1;

        private float mTime;
        //private float mSpwanGapTime;
        //private bool mWillSpawnMonster;

        private int mCount;
        private bool mIsCreating;

        public int SpawnerIndex { get; set; }

        // Start is called before the first frame update
        void Start()
        {
            mTime = m_Time;
            mIsCreating = true;

            GameObject item;
            AssetBundles abs = ShipDockApp.Instance.ABs;
            GameObject monster = abs.Get<GameObject>("is_king_monsters/roles", "Monster");
            for (int i = 0; i < m_Preload; i++)
            {
                item = Instantiate(monster);
                //item = ShipDockApp.Instance.AssetsPooling.FromPool(1, ref monster, default);
                ShipDockApp.Instance.AssetsPooling.ToPool(m_Res.PoolID, item, default, false);
            }
            //Consts.N_MONSTER_COLLECTED.Add(OnMonsterCollected);
        }

        private void OnMonsterCollected(INoticeBase<int> obj)
        {
            ParamNotice<int> notice = obj as ParamNotice<int>;
            if (notice.ParamValue == SpawnerIndex)
            {
                m_Count++;
            }
            else { }
        }

        private void ChangeSpawnPosition()
        {
            Vector3 pos = Consts.N_GET_HERO_POS.BroadcastWithParam(new Vector3(), true);

            int direction = UnityEngine.Random.Range(0, 4);
            Vector3 spawnPos = default;
            switch (direction)
            {
                case 0:
                    spawnPos = new Vector3(UnityEngine.Random.Range(-20f, -15f), UnityEngine.Random.Range(-30f, 30f), pos.z);
                    break;
                case 1:
                    spawnPos = new Vector3(UnityEngine.Random.Range(15f, 20f), UnityEngine.Random.Range(-30f, 30f), pos.z);
                    break;
                case 2:
                    spawnPos = new Vector3(UnityEngine.Random.Range(-15f, 15f), UnityEngine.Random.Range(30f, 35f), pos.z);
                    break;
                case 3:
                    spawnPos = new Vector3(UnityEngine.Random.Range(-15f, 15f), UnityEngine.Random.Range(-35f, -30f), pos.z);
                    break;
            }
            transform.localPosition = spawnPos;
        }

        // Update is called once per frame
        void Update()
        {
            m_AllMonsters = allMonster;

            if (mIsCreating && m_Count > 0)
            {
                mTime -= Time.deltaTime;

                if (mTime <= 0f)
                {
                    mTime = m_Time;

                    TimeUpdater timeUpdater = TimeUpdater.New(m_SpwanGapTime, () =>
                    {
                        if (allMonster > 1)
                        {
                            return;
                        }
                        else { }

                        ChangeSpawnPosition();
                        GameObject monster = m_Res.CreateAsset(true);

                        monster.transform.SetParent(transform.parent);
                        monster.transform.localScale = Vector3.one;// * 0.5f;
                        monster.transform.localRotation = Quaternion.Euler(0f, 180f, 3f);

                        Vector3 pos = transform.localPosition;// + Utils.RandomPositionOnCircle(10f, false);
                        pos.Set(pos.x, pos.y, 3f);
                        monster.transform.localPosition = pos;

                        monster.gameObject.GetInstanceID().BroadcastWithParam(EntityComponent.ENTITY_SETUP, true);

                        allMonster++;
                        //m_Count--;

                        //ECSContext ecs = ShipDockApp.Instance.ECSContext;
                        //ILogicEntitas allEntitas = ecs.CurrentContext.AllEntitas;
                        //WorldResourceComp comp = allEntitas.GetComponentFromEntitas<WorldResourceComp>(Consts.entitasMain, Consts.COMP_WORLD_RES);
                        //comp.AddMonsterToActive(monster.gameObject.GetInstanceID());

                        mIsCreating = true;

                    }, default, m_Count);

                    timeUpdater.IsAutoDispose = true;
                    mIsCreating = false;
                }
                else { }
            }
            else { }

        }
    }

}