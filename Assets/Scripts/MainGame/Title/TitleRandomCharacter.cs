using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


public sealed class TitleRandomCharacter : MonoBehaviour
{
    [SerializeField] private Image m_image_Character;
    [SerializeField] private Image m_image_Background;

    [SerializeField] private Sprite[] m_CharacterSprites;
    [SerializeField] private Sprite[] m_BackgroundSprites;


    public void Awake()
    {
        int selectedCharacterIndex = UnityEngine.Random.Range(0, m_CharacterSprites.Length);

        m_image_Character.sprite = m_CharacterSprites[selectedCharacterIndex];
        m_image_Background.sprite = m_BackgroundSprites[selectedCharacterIndex];
    }
}