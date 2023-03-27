#define _G_LOG

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace ShipDock.Applications
{
    public class AniStateBehaviour : StateMachineBehaviour
    {
        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("是否应用程序状态机")]
#endif
        private bool m_ApplyFSM;

        [SerializeField]
#if ODIN_INSPECTOR
        [ShowIf("@this.m_ApplyFSM == true")]
#endif
        private int m_MotionCompleted;

        [SerializeField]
#if ODIN_INSPECTOR
        [ShowIf("@this.m_ApplyFSM == true")]
#endif
        private int m_MotionCompleteCheckMax = 1;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("动画状态参数"), Indent(1)]
#endif
        private AnimationSubgroup m_AniSubgroup = new AnimationSubgroup();

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("动画状态特效")]
#endif
        private AniStateFX[] m_AniStateEffects;

        private int mNoticeName;
        private string[] mParamName;
        private KeyValueList<string, int> mActivedEffectMapper;
        private IParamNotice<AniStateBehaviour> mNotice;

        public AnimationSubgroup Subgroup
        {
            get
            {
                return m_AniSubgroup;
            }
        }

        public AniStateFX[] AniStateFX
        {
            get
            {
                return m_AniStateEffects;
            }
        }

        public bool IsDuringState { get; private set; }

        public AniStateFX GetAniStateFX(string FXName)
        {
            AniStateFX result = default;

            if (mActivedEffectMapper != default && mActivedEffectMapper.ContainsKey(FXName))
            {
                int index = mActivedEffectMapper[FXName];
                result = m_AniStateEffects[index];
            }
            else { }

            return result;
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            IsDuringState = true;

            mNoticeName = animator.GetInstanceID();
            mNotice = Pooling<ParamNotice<AniStateBehaviour>>.From();

            mNotice.ParamValue = this;

            if (m_ApplyFSM)
            {
                mParamName = m_AniSubgroup.ParamName;
            }
            else { }

            m_MotionCompleted = 0;

            SendAniStateNotice();
            StateEffectsEntered();

        }

        private void StateEffectsEntered()
        {
            if (HasAniStateEffect())
            {
                bool isInitEffectMapper = mActivedEffectMapper == default;
                if (isInitEffectMapper)
                {
                    mActivedEffectMapper = new KeyValueList<string, int>();
                }
                else { }

                string name;
                AniStateFX item;
                int m = m_AniStateEffects.Length;
                for (int i = 0; i < m; i++)
                {
                    item = m_AniStateEffects[i];
                    item.Init();

                    if (isInitEffectMapper)
                    {
                        name = item.FXName;
                        mActivedEffectMapper[name] = i;
                    }
                    else { }
                }

                int max = m_AniStateEffects.Length;
                for (int i = 0; i < max; i++)
                {
                    m_AniStateEffects[i].UpdateFX();
                }
            }
            else { }
        }

        private bool HasAniStateEffect()
        {
            return m_AniStateEffects != default && m_AniStateEffects.Length > 0;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (HasAniStateEffect())
            {
                int max = m_AniStateEffects.Length;
                for (int i = 0; i < max; i++)
                {
                    m_AniStateEffects[i].UpdateFX();
                }
            }
            else { }

            IsDuringState = stateInfo.IsName(m_AniSubgroup.MotionName) && stateInfo.normalizedTime <= 1f;

            if (!IsDuringState)
            {
                if (mNotice != default)
                {
                    if (m_MotionCompleteCheckMax > 0)
                    {
                        m_MotionCompleted++;
                        if (m_MotionCompleted >= m_MotionCompleteCheckMax)
                        {
                            SendAniStateNotice();
                        }
                        else { }
                    }
                    else
                    {
                        SendAniStateNotice();
                    }
                }
                else { }
            }
            else { }
        }

        private void SendAniStateNotice()
        {
            if (mNotice != default)
            {
                mNoticeName.Broadcast(mNotice);
            }
            else { }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            int max = m_AniStateEffects.Length;
            for (int i = 0; i < max; i++)
            {
                m_AniStateEffects[i].CheckFXAutoClean();
            }

            IsDuringState = false;

            ReleaseNotice();
        }

        private void ReleaseNotice()
        {
            mNotice?.ToPool();
            mNotice = default;
        }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}

        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            base.OnStateMachineExit(animator, stateMachinePathHash);
        }

        public void Clean()
        {
            mActivedEffectMapper?.Clear();
            mActivedEffectMapper = default;

            if (HasAniStateEffect())
            {
                int max = m_AniStateEffects.Length;
                for (int i = 0; i < max; i++)
                {
                    m_AniStateEffects[i].Release();
                }
                m_AniStateEffects = default;
            }
            else { }

            ReleaseNotice();
        }
    }
}