using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public sealed class MusicPreprocessor : MonoBehaviour
{
    [SerializeField] private MusicResourceDesc m_musicResourceDesc;


    public void Start()
    {
        foreach (BackgroundResourceDesc.BackgroundType backgroundType in Enum.GetValues(typeof(BackgroundResourceDesc.BackgroundType)))
        {
            MusicDataBuffer.Instance.TrySaveBackgroundMusic(m_musicResourceDesc.BackgroundMusicClips[(int)backgroundType], backgroundType);
        }
        Debug.Log("Background music clips saved successfully.");

        foreach (CharacterResourceDesc.CharacterListType characterListType in Enum.GetValues(typeof(CharacterResourceDesc.CharacterListType)))
        {
            if (characterListType == CharacterResourceDesc.CharacterListType.MainCharacter || characterListType == CharacterResourceDesc.CharacterListType.Description)
            {
                continue; // Skip MainCharacter and Description
            }

            MusicDataBuffer.Instance.TrySaveCharacterMusic(m_musicResourceDesc.CharacterMusics[(int)characterListType].CharacterMusicClips[0], characterListType, 0);
        }
        Debug.Log("Character music clips for lower likeability saved successfully.");

        foreach (MusicDataBuffer.SFXMusicType sfxMusicType in Enum.GetValues(typeof(MusicDataBuffer.SFXMusicType)))
        {
            if (sfxMusicType == MusicDataBuffer.SFXMusicType.None)
            {
                continue; // Skip None
            }
            MusicDataBuffer.Instance.TrySaveSFXMusic(m_musicResourceDesc.SFXMusicClips[(int)(sfxMusicType - 1)], sfxMusicType);
        }
        Debug.Log("SFX music clips saved successfully.");
    }
}