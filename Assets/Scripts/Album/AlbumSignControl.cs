using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public sealed class AlbumSignControl : MonoBehaviour
{
    private Animator m_animator;


    public void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    #region Unity Callbacks
    public void ReplaySign()
    {
        Render();
    }
    #endregion

    internal void Render()
    {
        switch (ChiefAlbumManager.Instance.CurSelectedCharacter)
        {
            case CharacterResourceDesc.CharacterListType.Absinthe:
                m_animator.SetTrigger("AbsintheSign");
                break;

            case CharacterResourceDesc.CharacterListType.CabernetSauvignon:
                m_animator.SetTrigger("CabernetSauvignonSign");
                break;

            case CharacterResourceDesc.CharacterListType.HayaseuraTokusenGinjo:
                m_animator.SetTrigger("HayaseuraTokusenGinjoSign");
                break;

            case CharacterResourceDesc.CharacterListType.Hpnotiq:
                m_animator.SetTrigger("HpnotiqSign");
                break;

            case CharacterResourceDesc.CharacterListType.Kahlua:
                m_animator.SetTrigger("KahluaSign");
                break;
        }
    }
}