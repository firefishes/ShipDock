using ShipDock.Applications;
using ShipDock.ECS;
using ShipDock.Tools;
using UnityEngine;

namespace IsKing
{
    public class MonsterSpawner : MonoBehaviour
    {
        [SerializeField]
        private ResPrefabBridge m_Res;

        [SerializeField]
        private float m_Time = 1f;
        [SerializeField]
        private float m_SpwanGapTime = 0.2f;

        [SerializeField]
        private int m_Count = 1;

        private float mTime;
        //private float mSpwanGapTime;
        //private bool mWillSpawnMonster;

        // Start is called before the first frame update
        void Start()
        {
            mTime = m_Time;
            mIsCreating = true;
        }

        private void ChangeSpawnPosition()
        {
            Vector3 pos = Consts.N_GET_HERO_POS.BroadcastWithParam(new Vector3(), true);
            pos.Set(pos.x + Random.Range(-60f, 60f), pos.y + Random.Range(-100f, 100f), pos.z);
            transform.localPosition = pos;
        }

        private int mCount;
        private bool mIsCreating;

        // Update is called once per frame
        void Update()
        {
            if (mIsCreating)
            {
                mTime -= Time.deltaTime;

                if (mTime <= 0f)
                {
                    mTime = m_Time;

                    TimeUpdater timeUpdater = TimeUpdater.New(m_SpwanGapTime, () =>
                    {
                        ChangeSpawnPosition();
                        GameObject monster = m_Res.CreateAsset(true);

                        monster.transform.SetParent(transform.parent);
                        monster.transform.localScale = Vector3.one;// * 0.5f;
                        monster.transform.localRotation = Quaternion.Euler(0f, 180f, 3f);

                        Vector3 pos = transform.localPosition;// + Utils.RandomPositionOnCircle(10f, false);
                        pos.Set(pos.x, pos.y, 3f);
                        monster.transform.localPosition = pos;

                        monster.gameObject.GetInstanceID().BroadcastWithParam(EntityComponent.ENTITY_SETUP, true);

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