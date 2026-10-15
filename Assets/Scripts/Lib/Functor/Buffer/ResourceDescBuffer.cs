using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CommonUtilLib.ThreadSafe;
using Unity.VisualScripting;


public sealed class ResourceDescBuffer : SingleTonForGameObject<ResourceDescBuffer>
{
    [SerializeField] private BackgroundResourceDesc m_backgroundResourceDesc;
    [SerializeField] private CharacterResourceDesc[] m_characterResourceDescs;
    [SerializeField] private CutSceneResourceDesc m_cutSceneResourceDesc;
    [SerializeField] private MusicResourceDesc m_musicResourceDesc;

    [SerializeField] private CharacterDisplayConfig m_characterDisplayConfig;


    public void Awake()
    {
        SetInstance(this);
    }

    internal CutSceneResourceDesc CutSceneResourceDesc
    {
        get
        {
            return m_cutSceneResourceDesc;
        }
    }
    internal MusicResourceDesc MusicResourceDesc
    {
        get
        {
            return m_musicResourceDesc;
        }
    }

    internal CharacterDisplayConfig CharacterDisplayConfig
    {
        get
        {
            return m_characterDisplayConfig;
        }
    }

    internal Sprite GetBackgroundSprite(in BackgroundResourceDesc.BackgroundType backgroundType)
    {
        if (m_backgroundResourceDesc == null)
        {
            throw new System.NullReferenceException("BackgroundResourceDesc is null.");
        }

        return m_backgroundResourceDesc.BackgroundSprites[(int)backgroundType];
    }
    internal string GetBackgroundName(in BackgroundResourceDesc.BackgroundType backgroundType)
    {
        if (m_backgroundResourceDesc == null)
        {
            throw new System.NullReferenceException("BackgroundResourceDesc is null.");
        }

        return m_backgroundResourceDesc.BackgorundNames[(int)backgroundType];
    }

    internal CharacterResourceDesc GetCharacterResourceDesc(in CharacterResourceDesc.CharacterListType characterListType)
    {
        if (m_characterResourceDescs == null || m_characterResourceDescs.Length == 0)
        {
            throw new System.NullReferenceException("CharacterResourceDescs is null or empty.");
        }
        if ((int)characterListType < 0 || (int)characterListType >= m_characterResourceDescs.Length)
        {
            throw new System.ArgumentOutOfRangeException(nameof(characterListType), 
                $"CharacterListType {characterListType} is out of range. " +
                $"Valid range is from 0 to {m_characterResourceDescs.Length - 1}.");
        }

        return m_characterResourceDescs[(int)characterListType];
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}