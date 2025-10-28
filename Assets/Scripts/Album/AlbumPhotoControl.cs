using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;


public sealed class AlbumPhotoControl : MonoBehaviour
{
    [Serializable] public struct CharacterCutScenePhoto
    {
        public string CharacterSpeak;
        public Sprite[] Photos;
        public Color BtnColor;

        public Sprite Background;
        public Sprite BackgroundDeco;
    }


    [SerializeField] private Animator m_animator_Photo;
    [SerializeField] private Image[] m_image_Photos;

    [SerializeField] private CharacterCutScenePhoto[] m_characterCutScenePhotos;
    [SerializeField] private float[] m_photoAnimationDelays;

    [SerializeField] private TMP_Text[] m_text_CharacterSpeaks;
    [SerializeField] private float m_characterSpeakDelay;

    [Header("Btns")]
    [SerializeField] private GameObject m_gameObject_LeftCharacterBtn;
    [SerializeField] private GameObject m_gameObject_RightCharacterBtn;

    [SerializeField] private Image m_image_LeftCharacterInnerColor;
    [SerializeField] private Image m_image_RightCharacterInnerColor;

    [Header("Background")]
    [SerializeField] private Image m_image_Background;
    [SerializeField] private Image m_image_BackgroundDeco;

    [Header("Zoomed Photo")]
    [SerializeField] private Image m_image_ZoomedPhoto;

    [SerializeField] private ScreenBrushTransition m_screenTransition_Brush;
    [SerializeField] private CustomRenderTexture m_renderTexture;

    [SerializeField] private Texture m_defaultStateTex = null;
    private Sprite m_targetSprite = null;

    [SerializeField] private float m_transitionDuration = 2.0f;

    private bool m_bisTransitionEnd = true;


    #region Unity Callbacks
    public void OpenLeftCharacteInfo()
    {
        if(ChiefAlbumManager.Instance.CurSelectedCharacter == CharacterResourceDesc.CharacterListType.Absinthe)
        {
            return;
        }
        else if (!m_bisTransitionEnd)
        {
            return;
        }

        ChiefAlbumManager.Instance.CurSelectedCharacter--;
        ChiefAlbumManager.Instance.RenderAll();

        Graphics.CopyTexture(m_renderTexture, m_defaultStateTex);
    }
    public void OpenRightCharacterInfo()
    {
        if (ChiefAlbumManager.Instance.CurSelectedCharacter == CharacterResourceDesc.CharacterListType.Kahlua)
        {
            return;
        }
        else if (!m_bisTransitionEnd)
        {
            return;
        }

        ChiefAlbumManager.Instance.CurSelectedCharacter++;
        ChiefAlbumManager.Instance.RenderAll();

        Graphics.CopyTexture(m_renderTexture, m_defaultStateTex);
    }

    public void OpenZoomedPhoto(int photoIndex)
    {
        if(!m_bisTransitionEnd)
        {
            return;
        }

        Graphics.CopyTexture(m_renderTexture, m_defaultStateTex);

        m_image_ZoomedPhoto.sprite = null;
        if (m_image_ZoomedPhoto.gameObject.activeSelf)
        {
            m_image_ZoomedPhoto.gameObject.SetActive(false);

            m_screenTransition_Brush.ClearSettings();
        }
        else
        {
            switch(photoIndex)
            {
                case 0:
                    m_targetSprite = ResourceDescBuffer.Instance.CutSceneResourceDesc.CutSceneDatas[(int)ChiefAlbumManager.Instance.CurSelectedCharacter].CurSceneSprite;
                    break;

                case 1:
                    switch (ChiefAlbumManager.Instance.CurSelectedCharacter)
                    {
                        case CharacterResourceDesc.CharacterListType.Absinthe:
                            m_targetSprite = ResourceDescBuffer.Instance.GetBackgroundSprite(BackgroundResourceDesc.BackgroundType.PaintingRoom);
                            break;

                        case CharacterResourceDesc.CharacterListType.CabernetSauvignon:
                            m_targetSprite = ResourceDescBuffer.Instance.GetBackgroundSprite(BackgroundResourceDesc.BackgroundType.Restaurant);
                            break;

                        case CharacterResourceDesc.CharacterListType.HayaseuraTokusenGinjo:
                            m_targetSprite = ResourceDescBuffer.Instance.GetBackgroundSprite(BackgroundResourceDesc.BackgroundType.HealthRoom);
                            break;

                        case CharacterResourceDesc.CharacterListType.Hpnotiq:
                            m_targetSprite = ResourceDescBuffer.Instance.GetBackgroundSprite(BackgroundResourceDesc.BackgroundType.Mall);
                            break;

                        case CharacterResourceDesc.CharacterListType.Kahlua:
                            m_targetSprite = ResourceDescBuffer.Instance.GetBackgroundSprite(BackgroundResourceDesc.BackgroundType.Cafe);
                            break;
                    }
                    break;
            }

            m_screenTransition_Brush.StartTransition(m_transitionDuration, m_defaultStateTex, m_targetSprite.texture);

            m_image_ZoomedPhoto.gameObject.SetActive(true);
            Invoke(nameof(RenderFinalZoomResult), m_transitionDuration);

            m_bisTransitionEnd = false;
        }
    }
    #endregion

    internal void Render()
    {
        if(ChiefAlbumManager.Instance.CurSelectedCharacter == CharacterResourceDesc.CharacterListType.Absinthe)
        {
            m_gameObject_LeftCharacterBtn.SetActive(false);
            m_gameObject_RightCharacterBtn.SetActive(true);
        }
        else if(ChiefAlbumManager.Instance.CurSelectedCharacter == CharacterResourceDesc.CharacterListType.Kahlua)
        {
            m_gameObject_LeftCharacterBtn.SetActive(true);
            m_gameObject_RightCharacterBtn.SetActive(false);
        }
        else
        {
            m_gameObject_LeftCharacterBtn.SetActive(true);
            m_gameObject_RightCharacterBtn.SetActive(true);
        }

        m_image_LeftCharacterInnerColor.color = m_characterCutScenePhotos[(int)ChiefAlbumManager.Instance.CurSelectedCharacter - (int)CharacterResourceDesc.CharacterListType.Absinthe].BtnColor;
        m_image_RightCharacterInnerColor.color = m_characterCutScenePhotos[(int)ChiefAlbumManager.Instance.CurSelectedCharacter + (int)CharacterResourceDesc.CharacterListType.Absinthe].BtnColor;

        m_image_Background.sprite = m_characterCutScenePhotos[(int)ChiefAlbumManager.Instance.CurSelectedCharacter].Background;
        m_image_BackgroundDeco.sprite = m_characterCutScenePhotos[(int)ChiefAlbumManager.Instance.CurSelectedCharacter].BackgroundDeco;

        m_image_Photos[0].sprite = ResourceDescBuffer.Instance.CutSceneResourceDesc.CutSceneDatas[(int)ChiefAlbumManager.Instance.CurSelectedCharacter].CurSceneSprite;
        switch(ChiefAlbumManager.Instance.CurSelectedCharacter)
        {
            case CharacterResourceDesc.CharacterListType.Absinthe:
                m_image_Photos[1].sprite = ResourceDescBuffer.Instance.GetBackgroundSprite(BackgroundResourceDesc.BackgroundType.PaintingRoom);
                break;

            case CharacterResourceDesc.CharacterListType.CabernetSauvignon:
                m_image_Photos[1].sprite = ResourceDescBuffer.Instance.GetBackgroundSprite(BackgroundResourceDesc.BackgroundType.Restaurant);
                break;

            case CharacterResourceDesc.CharacterListType.HayaseuraTokusenGinjo:
                m_image_Photos[1].sprite = ResourceDescBuffer.Instance.GetBackgroundSprite(BackgroundResourceDesc.BackgroundType.HealthRoom);
                break;

            case CharacterResourceDesc.CharacterListType.Hpnotiq:
                m_image_Photos[1].sprite = ResourceDescBuffer.Instance.GetBackgroundSprite(BackgroundResourceDesc.BackgroundType.Mall);
                break;

            case CharacterResourceDesc.CharacterListType.Kahlua:
                m_image_Photos[1].sprite = ResourceDescBuffer.Instance.GetBackgroundSprite(BackgroundResourceDesc.BackgroundType.Cafe);
                break;
        }

        m_text_CharacterSpeaks[0].text = "{fade d=" + m_characterSpeakDelay + "}" + m_characterCutScenePhotos[(int)ChiefAlbumManager.Instance.CurSelectedCharacter].CharacterSpeak + "{/fade}";

        string infoStr = "{fade d=" + m_characterSpeakDelay + "}" + ResourceDescBuffer.Instance.GetCharacterResourceDesc(ChiefAlbumManager.Instance.CurSelectedCharacter).CharacterName + " 이벤트 준비중입니다...{/fade}";
        m_text_CharacterSpeaks[1].text = infoStr;
        m_text_CharacterSpeaks[2].text = infoStr;

        m_animator_Photo.SetTrigger("PhotoMovement");
    }

    private void RenderFinalZoomResult()
    {
        m_image_ZoomedPhoto.sprite = m_targetSprite;
        m_targetSprite = null;

        m_bisTransitionEnd = true;
    }
}