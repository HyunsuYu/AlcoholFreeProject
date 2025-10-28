using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public sealed class CharacterSelectionPopup : MonoBehaviour
{
    private static string m_selectedCharacter = string.Empty;

    [SerializeField] private GameObject[] m_layout_OffTargets;
    [SerializeField] private GameObject m_layout_Contents;


    #region Unity Callbacks
    public void ClosePopup()
    {
        ChiefCharacterSelectionManager.CurSelectedCharacterType = CharacterResourceDesc.CharacterListType.MainCharacter;
        ChiefCharacterSelectionManager.Instance.BIsPopupActive = false;
    }
    #endregion

    internal static string SelectedCharacter
    {
        get
        {
            return m_selectedCharacter;
        }
    }

    internal void StartSrory()
    {
        switch(ChiefCharacterSelectionManager.CurSelectedCharacterType)
        {
            case CharacterResourceDesc.CharacterListType.Absinthe:
                m_selectedCharacter = "Absinthe";
                //ChiefMainGameManager.Instance.Render("Absinthe");
                break;

            case CharacterResourceDesc.CharacterListType.HayaseuraTokusenGinjo:
                m_selectedCharacter = "HayaseuraTokusenGinjo";
                //ChiefMainGameManager.Instance.Render("HayaseuraTokusenGinjo");
                break;

            case CharacterResourceDesc.CharacterListType.Kahlua:
                m_selectedCharacter = "Kahlua";
                //ChiefMainGameManager.Instance.Render("Kahlua");
                break;

            case CharacterResourceDesc.CharacterListType.Hpnotiq:
                m_selectedCharacter = "Hpnotiq";
                //ChiefMainGameManager.Instance.Render("Hpnotiq");
                break;

            case CharacterResourceDesc.CharacterListType.CabernetSauvignon:
                m_selectedCharacter = "CabernetSauvignon";
                //ChiefMainGameManager.Instance.Render("CabernetSauvignon");
                break;
        }
        //ChiefMainGameManager.Instance.Render("Tutorial");

        foreach (GameObject target in m_layout_OffTargets)
        {
            target.SetActive(false);
        }
        m_layout_Contents.SetActive(true);
    }
}