using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock
{
    /// <summary>
    /// 
    /// 音效管理器
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class SoundEffects
    {
        /// <summary>
        /// 构建音效播放列表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="group"></param>
        /// <param name="clips"></param>
        public static void BuildSoundPlayList(ref List<SoundItem> list, int group, params AudioClip[] clips)
        {
            if (list != default)
            {
                AudioClip clip;
                int max = clips.Length;
                for (int i = 0; i < max; i++)
                {
                    clip = clips[i];
                    SoundItem item = new SoundItem()
                    {
                        clip = clip,
                        playName = clip.name,
                        isBGM = false,
                        group = group,
                    };
                    list.Add(item);
                }
            }
            else { }
        }

        /// <summary>
        /// 构建背景音乐播放列表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="group"></param>
        /// <param name="clips"></param>
        public static void BuildBGMPlayList(ref List<SoundItem> list, int group, params AudioClip[] clips)
        {
            if (list != default)
            {
                AudioClip clip;
                int max = clips.Length;
                for (int i = 0; i < max; i++)
                {
                    clip = clips[i];
                    SoundItem item = new SoundItem()
                    {
                        clip = clip,
                        playName = clip.name,
                        isBGM = true,
                        group = group,
                    };
                    list.Add(item);
                }
            }
            else { }
        }

        /// <summary>已使用的音轨数量</summary>
        private int mSoundTrackCount;
        /// <summary>主音轨</summary>
        private SoundInfo mBGMInfo;
        private SoundTrack mBGMSoundTrack;
        /// <summary>背景音乐声源</summary>
        private AudioSource mBGMAudioSource;
        /// <summary>正在播放的音轨</summary>
        private Queue<SoundTrack> mPlayingTracks;
        /// <summary>生源对象池</summary>
        private Stack<AudioSource> mAudioSourcePool;
        /// <summary>音轨池</summary>
        private Stack<SoundTrack> mTrackPool;
        /// <summary>即将停止的声音名</summary>
        private List<string> mWillStopSounds;
        /// <summary>声音信息映射</summary>
        private KeyValueList<string, SoundInfo> mSounds;
        /// <summary>声音播放完成后的回调函数</summary>
        private Action mPlayCompleteCallback;
        /// <summary>音频组件根节点</summary>
        private GameObject mPlayerRoot;
        #region 临时引用
        private SoundTrack mTrackTemp;
        private AudioSource mSourceTemp;
        #endregion

        public bool IsBGMMute { get; private set; }
        public bool IsSoundMute { get; private set; }
        public float VolumnBGM { get; private set; }
        public float VolumnSound { get; private set; }

        public SoundEffects()
        {
            mWillStopSounds = new List<string>();
            mSounds = new KeyValueList<string, SoundInfo>();
            mPlayingTracks = new Queue<SoundTrack>();
            mTrackPool = new Stack<SoundTrack>();
            mAudioSourcePool = new Stack<AudioSource>();
        }

        public void Clean()
        {
        }

        public void Init()
        {
            if (mPlayerRoot == default)
            {
                mPlayerRoot = new GameObject()
                {
                    name = "SoundEffects"
                };
                GameObject item = new GameObject
                {
                    name = "BGMTrack"
                };
                item.transform.SetParent(mPlayerRoot.transform);
                mBGMAudioSource = item.AddComponent<AudioSource>();
                mBGMAudioSource.loop = true;
            }
            else { }
        }

        /// <summary>
        /// 设置背景音乐音量
        /// </summary>
        /// <param name="vol"></param>
        public void SetVolumnBGM(float vol)
        {
            vol = Mathf.Min(1f, vol);
            vol = Mathf.Max(0f, vol);
            VolumnBGM = vol;
            bool prevBGMMute = IsBGMMute;
            IsBGMMute = vol <= 0f;

            if (mBGMAudioSource != default)
            {
                mBGMAudioSource.volume = VolumnBGM;
                if(prevBGMMute != IsBGMMute)
                {
                    if (IsBGMMute)
                    {
                        mBGMAudioSource.Pause();
                    }
                    else
                    {
                        mBGMAudioSource.Play();
                    }
                }
                else { }
            }
            else { }
        }

        /// <summary>
        /// 设置音效音量
        /// </summary>
        /// <param name="vol"></param>
        public void SetVolumnSound(float vol)
        {
            vol = Mathf.Min(1f, vol);
            vol = Mathf.Max(0f, vol);
            VolumnSound = vol;
            IsSoundMute = vol <= 0f;
        }

        public void AddSound(string name, AudioClip clip, bool isBGM = false, int groupName = int.MaxValue)
        {
            SoundInfo info;
            if (mSounds.ContainsKey(name))
            {
                info = mSounds[name];
            }
            else
            {
                info = new SoundInfo();
                mSounds[name] = info;
            }
            info.clip = clip;
            info.groupName = groupName;
            info.isBGM = isBGM;
        }

        public void RemoveSound(int groupName)
        {
            List<string> names = new List<string>();

            int max = mSounds.Size;
            List<string> keys = mSounds.Keys;
            List<SoundInfo> values = mSounds.Values;
            for (int i = 0; i < max; i++)
            {
                if (values[i].groupName == groupName)
                {
                    names.Add(keys[i]);
                }
                else {  }
            }

            max = names.Count;
            for (int i = 0; i < max; i++)
            {
                RemoveSound(names[i]);
            }
        }

        public void RemoveSound(string name)
        {
            SoundInfo info = mSounds.GetValue(name, true);
            if (info != default)
            {
                if (info.isBGM)
                {
                    if (mBGMAudioSource != default && info.clip == mBGMAudioSource.clip)
                    {
                        StopBGM();
                    }
                    else { }
                }
                else
                {
                    StopSound(name);
                }
                info.clip = default;
            }
            else { }
        }

        public void PlayBGM(string name, Action onBGMCompleted = default)
        {
            SoundInfo info = mSounds[name];
            if (info != default)
            {
                if (info.isBGM)
                {
                    mBGMInfo = info;
                    if (mBGMSoundTrack == default)
                    {
                        mBGMSoundTrack = new SoundTrack();
                    }
                    else { }
                    mBGMAudioSource.volume = VolumnBGM;
                    PlaySoundTrack(ref name, ref mBGMInfo, ref mBGMAudioSource, ref mBGMSoundTrack, onBGMCompleted, false, IsBGMMute);
                }
                else
                {
                    "error:Audio clip {0} is short effect sound what you will play".Log(name);
                }
            }
            else { }
        }

        public void StopBGM()
        {
            mBGMAudioSource.Stop();
            mBGMAudioSource.clip = default;
        }

        public void PlaySound(string name, Action onCompleted = default)
        {
            if (IsSoundMute)
            {
                return;
            }
            else { }

            SoundInfo info = mSounds[name];
            if (info != default)
            {
                if (!info.isBGM)
                {
                    AudioSource player;
                    SoundTrack track = default;
                    if (mAudioSourcePool.Count > 0)
                    {
                        track = mTrackPool.Pop();
                        player = mAudioSourcePool.Pop();
                    }
                    else
                    {
                        CreateNewSoundTrack(out player, out track);
                    }

                    player.volume = VolumnSound;
                    PlaySoundTrack(ref name, ref info, ref player, ref track, onCompleted, true, IsSoundMute);
                }
                else
                {
                    "error:Audio clip {0} is bgm what you will play".Log(name);
                }
            }
            else { }
        }

        private void PlaySoundTrack(ref string name, ref SoundInfo info, ref AudioSource player, ref SoundTrack track, Action onCompleted = default, bool isPlayingStacking = true, bool isMute = false)
        {
            if (track != default)
            {
                player.clip = info.clip;

                if (!isMute)
                {
                    player.Play();
                }
                else { }

                track.source = player;
                track.soundInfoName = name;
                track.clipLength = info.clip.length;
                track.onPlayCompleted = onCompleted;

                if (isPlayingStacking)
                {
                    mPlayingTracks.Enqueue(track);
                }
                else { }
            }
            else { }
        }

        private void CreateNewSoundTrack(out AudioSource player, out SoundTrack track)
        {
            GameObject item = new GameObject
            {
                name = "SoundTrack_".Append(mSoundTrackCount.ToString())
            };
            item.transform.SetParent(mPlayerRoot.transform);
            mSoundTrackCount++;

            player = item.AddComponent<AudioSource>();
            track = new SoundTrack();
        }

        public void StopSound(string name)
        {
            mWillStopSounds.Add(name);
        }

        public void Update()
        {
            int max = mPlayingTracks.Count;
            if (max > 0)
            {
                while (max > 0)
                {
                    mTrackTemp = mPlayingTracks.Dequeue();
                    mSourceTemp = mTrackTemp.source;
                    CheckTrackPlayedTime();
                    max--;
                }
                mTrackTemp = default;
                mSourceTemp = default;
            }
            else { }

            mWillStopSounds.Clear();

            mPlayCompleteCallback?.Invoke();
            mPlayCompleteCallback = default;
        }

        private void CheckTrackPlayedTime()
        {
            float lenDelta = Time.deltaTime * Time.timeScale;
            if (mTrackTemp != default && mSourceTemp != default)
            {
                mSourceTemp.volume = VolumnSound;
                bool isNipOff = mWillStopSounds.Contains(mTrackTemp.soundInfoName);
                if (mTrackTemp.clipLength > 0f && !isNipOff)
                {
                    mTrackTemp.clipLength -= lenDelta;
                }
                else
                {
                    mTrackTemp.clipLength = 0f;
                }

                TrackChecked(isNipOff);
            }
            else { }

            if (!IsBGMMute && mBGMSoundTrack != default && mBGMInfo != default && mBGMInfo.clip != default)
            {
                mBGMSoundTrack.source.volume = VolumnBGM;
                mBGMSoundTrack.clipLength -= lenDelta;
                if (mBGMSoundTrack.clipLength <= 0f)
                {
                    mBGMSoundTrack.clipLength += mBGMInfo.clip.length;
                    mBGMSoundTrack.onPlayCompleted?.Invoke();
                }
                else { }
            }
            else { }
        }

        private void TrackChecked(bool isNipOff)
        {
            if (mTrackTemp.clipLength > 0f)
            {
                mPlayingTracks.Enqueue(mTrackTemp);
            }
            else
            {
                if (isNipOff)
                {
                    mTrackTemp.source.Stop();
                }
                else
                {
                    mPlayCompleteCallback += mTrackTemp.onPlayCompleted;
                }

                mSourceTemp.Stop();
                mSourceTemp.clip = default;

                mTrackTemp.soundInfoName = string.Empty;
                mTrackTemp.source = default;
                mTrackTemp.onPlayCompleted = default;
                mTrackTemp.clipLength = 0f;

                mAudioSourcePool.Push(mSourceTemp);
                mTrackPool.Push(mTrackTemp);
                mSourceTemp = default;
            }
        }

        public void SetPlayList(params SoundItem[] items)
        {
            SoundItem item;
            int max = items != default ? items.Length : 0;
            if (max > 0)
            {
                for (int i = 0; i < max; i++)
                {
                    item = items[i];
                    AddSound(item.playName, item.clip, item.isBGM, item.group);
                    item.clip = default;
                }
                Array.Clear(items, 0, items.Length);
            }
            else { }
        }
    }
}
