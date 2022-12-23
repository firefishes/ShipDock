
using Spine;
using Spine.Unity;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Joypie
{
    public class USpineController : MonoBehaviour
    {

        private Action<string> startEvent = null;
        private Action<string> interruptEvent = null;
        private Action<string> disposeEvent = null;
        private Action<string> endEvent = null;
        private Action<string> completeEvent = null;
        private Action<string, string> eventEvent = null;



        private SkeletonAnimation m_SkeletonAnimation = null;
        public SkeletonAnimation skeletonAnimation
        {
            get
            {
                if (m_SkeletonAnimation == null)
                {
                    m_SkeletonAnimation = (SkeletonAnimation)this.gameObject.GetComponent(typeof(SkeletonAnimation));
                    m_SkeletonAnimation.AnimationState.Start += OnStart;
                    m_SkeletonAnimation.AnimationState.Interrupt += OnInterrupt;
                    m_SkeletonAnimation.AnimationState.Dispose += OnDispose;
                    m_SkeletonAnimation.AnimationState.End += OnEnd;
                    m_SkeletonAnimation.AnimationState.Complete += OnComplete;
                    m_SkeletonAnimation.AnimationState.Event += OnEvent;
                }
                return m_SkeletonAnimation;
            }
        }


        private MeshRenderer m_meshRenderer;

        public MeshRenderer meshRenderer
        {
            get
            {
                if (m_meshRenderer == null)
                {
                    m_meshRenderer = this.GetComponent<MeshRenderer>();
                }
                return m_meshRenderer;
            }
        }




        public void PlayAnim(int layer, string animationName, bool loop)
        {
            if (skeletonAnimation != null)
            {
                TrackEntry m_TrackEntry = skeletonAnimation.AnimationState.SetAnimation(layer, animationName, loop);
            }
        }

        public void PlayEmptyAnim(int layer, float mixDuration = 0)
        {
            if (skeletonAnimation != null)
            {
                TrackEntry m_TrackEntry = skeletonAnimation.AnimationState.SetEmptyAnimation(layer, mixDuration);
            }
        }

        public void AddAnim(int layer, string animationName, bool loop, float delay = 0)
        {
            if (skeletonAnimation != null)
            {
                TrackEntry m_TrackEntry = skeletonAnimation.AnimationState.AddAnimation(layer, animationName, loop, delay);
            }
        }

        public void AddEmptyAnim(int layer, float mixDuration = 0, float delay = 0)
        {
            if (skeletonAnimation != null)
            {
                TrackEntry m_TrackEntry = skeletonAnimation.AnimationState.AddEmptyAnimation(layer, mixDuration, delay);
            }
        }

        public void ClearTrack(int layer)
        {
            if (skeletonAnimation != null)
                skeletonAnimation.AnimationState.ClearTrack(layer);
        }

        public void ClearTracks()
        {
            if (skeletonAnimation != null)
                skeletonAnimation.AnimationState.ClearTracks();
        }

        void OnStart(TrackEntry trackEntry)
        {
            if (startEvent != null)
                startEvent(trackEntry.Animation.Name);
        }

        void OnInterrupt(TrackEntry trackEntry)
        {
            if (interruptEvent != null)
                interruptEvent(trackEntry.Animation.Name);
        }
        void OnDispose(TrackEntry trackEntry)
        {
            if (disposeEvent != null)
                disposeEvent(trackEntry.Animation.Name);
        }


        void OnEnd(TrackEntry trackEntry)
        {
            if (endEvent != null)
                endEvent(trackEntry.Animation.Name);
        }

        void OnComplete(TrackEntry trackEntry)
        {
            if (completeEvent != null)
                completeEvent(trackEntry.Animation.Name);
        }

        void OnEvent(TrackEntry trackEntry, Spine.Event e)
        {
            if (eventEvent != null)
                eventEvent(trackEntry.Animation.Name, e.Data.Name);
        }


        public void AddOnStart(Action<string> luafunc)
        {
            if (skeletonAnimation != null)
                startEvent = luafunc;
        }

        public void AddOnInterrupt(Action<string> luafunc)
        {
            if (skeletonAnimation != null)
                interruptEvent = luafunc;
        }
        public void AddOnDispose(Action<string> luafunc)
        {
            if (skeletonAnimation != null)
                disposeEvent = luafunc;
        }


        public void AddOnEnd(Action<string> luafunc)
        {
            if (skeletonAnimation != null)
                endEvent = luafunc;
        }

        public void AddOnComplete(Action<string> luafunc)
        {
            if (skeletonAnimation != null)
                completeEvent = luafunc;
        }

        public void AddOnEvent(Action<string, string> luafunc)
        {
            if (skeletonAnimation != null)
                eventEvent = luafunc;
        }


        public void SetColor(float r, float g, float b, float a)
        {
            skeletonAnimation.skeleton.A = a;
            skeletonAnimation.skeleton.R = r;
            skeletonAnimation.skeleton.G = g;
            skeletonAnimation.skeleton.B = b;
        }


        public void SetColorA(float a)
        {
            skeletonAnimation.skeleton.A = a;
        }


        public void SetColorR(float r)
        {
            skeletonAnimation.skeleton.R = r;
        }

        public void SetColorG(float g)
        {
            skeletonAnimation.skeleton.G = g;
        }

        public void SetColorB(float b)
        {
            skeletonAnimation.skeleton.B = b;
        }


        public Color GetColor()
        {
            Color color = new Color(0, 0, 0, 0);
            if (skeletonAnimation != null)
            {
                color.a = skeletonAnimation.skeleton.A;
                color.r = skeletonAnimation.skeleton.R;
                color.g = skeletonAnimation.skeleton.G;
                color.b = skeletonAnimation.skeleton.B;
            }
            return color;
        }


        public void SetTimeScale(float tiem)
        {
            if (skeletonAnimation)
            {
                skeletonAnimation.timeScale = tiem;
            }
        }

        public float GetAnimDurationTime(string animationName)
        {
            float d = 0;
            if (this.skeletonAnimation)
            {
                Spine.Animation ani = this.skeletonAnimation.AnimationState.GetAnimation(animationName);
                if (ani != null)
                {
                    d = ani.Duration;
                }
            }
            return d;
        }

        public float GetAnimationCurrentTime(int layer)
        {
            float result = 0f;
            if (skeletonAnimation != null)
            {
                Spine.AnimationState state = skeletonAnimation.AnimationState;
                if (state != null)
                {
                    TrackEntry track = state.GetCurrent(layer);
                    result = track.AnimationTime;
                }
                else { }
            }
            else { }
            return result;
        }

        public bool IsEqualsAnim(string animationName)
        {
            if (skeletonAnimation)
            {
                return skeletonAnimation.AnimationState.IsEqualsAnim(animationName);
            }
            return false;
        }

        public void SetSortingLayer(int sortingOrder, int sortingLayerID)
        {
            meshRenderer.sortingOrder = sortingOrder;
            meshRenderer.sortingLayerID = sortingLayerID;
        }

        public void GetSortingLayer(out int sortingOrder, out int sortingLayerID)
        {
           sortingOrder = meshRenderer.sortingOrder;
           sortingLayerID =  meshRenderer.sortingLayerID;
        }

    }
}
