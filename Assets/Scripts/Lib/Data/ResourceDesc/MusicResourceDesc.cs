using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


[CreateAssetMenu(fileName = "MusicResourceDesc", menuName = "ResourceDesc/MusicResourceDesc", order = 1)]
public sealed class MusicResourceDesc : ScriptableObject
{
    public enum SFXMusicType : byte
    {
        None,
        Walking,
        Bell,
        Wind,
        Heartbeat,
        MessageBeep
    }

    [Serializable] public struct CharacterMusicData
    {
        public enum CharacterMusicType : byte
        {
            LowerLikeability = 0,
            MiddleLikeability = 1,
            HigherLikeability = 2
        }


        /// <summary>
        /// CharacterMusicClips의 크기는 CharacterMusicType의 크기와 같아야 한다
        /// </summary>
        public AudioClip[] CharacterMusicClips;
    }


    /// <summary>
    /// CharacterMusics의 크기는 CharacterResourceDesc.CharacterListType의 크기와 같아야 한다
    /// </summary>
    public CharacterMusicData[] CharacterMusics;

    /// <summary>
    /// BackgroundMusicClips의 크기는 BackgroundResourceDesc.BackgroundType의 크기와 같아야 한다
    /// </summary>
    public AudioClip[] BackgroundMusicClips;

    /// <summary>
    /// SFXMusicClips의 크기는 SFXMusicType의 크기와 같아야 한다
    /// </summary>
    public AudioClip[] SFXMusicClips;
}