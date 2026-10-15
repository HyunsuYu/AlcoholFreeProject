using CommonUtilLib.ThreadSafe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


public sealed class ChiefMapManager : SingleTonForGameObject<ChiefMapManager>
{
    [SerializeField] private GraphicRaycaster m_graphicRaycaster;
    [SerializeField] private EventSystem m_eventSystem;

    [SerializeField] private AudioSource m_audioSource_BGM;

    [Header("Map Place Btn")]
    [SerializeField] private MapPlaceButton[] m_placeButtons;
    [Range(0f, 1f)]
    [SerializeField] private float m_upSpeed;

    [Header("Check Popup")]
    private BackgroundResourceDesc.BackgroundType m_curSelectedPlace = BackgroundResourceDesc.BackgroundType.HomeRoom;

    [SerializeField] private TMP_Text m_text_PopupPlace;
    [SerializeField] private GameObject m_popup_CheckPopup;


    public void Awake()
    {
        SetInstance(this);

        ClosePopup();
    }
    public void Start()
    {
        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        QualitySettings.vSyncCount = curSettingData.ScreenSetting.BIsVSync ? 1 : 0;
        m_audioSource_BGM.volume = curSettingData.AudioSetting.BGMVolume * curSettingData.AudioSetting.MasterVolume;
    }
    public void Update()
    {
        if(m_popup_CheckPopup.activeSelf)
        {
            return;
        }

        foreach (var item in m_placeButtons)
        {
            item.BIsOnCursor = false;
        }

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        m_graphicRaycaster.Raycast(new PointerEventData(m_eventSystem)
        {
            position = Input.mousePosition
        }, raycastResults);

        foreach(var raycastResult in raycastResults)
        {
            MapPlaceButton button = raycastResult.gameObject.GetComponent<MapPlaceButton>();
            if(button != null)
            {
                button.BIsOnCursor = true;
                button.CurFloatingHeight += m_upSpeed;

                break;
            }
        }
    }

    internal BackgroundResourceDesc.BackgroundType CurSelectedPlace
    {
        set
        {
            m_curSelectedPlace = value;

            m_text_PopupPlace.text = $"< {ResourceDescBuffer.Instance.GetBackgroundName(value)} >";
        }
    }

    #region Unity Callbacks
    public void ConfirmPopup()
    {
        switch (m_curSelectedPlace)
        {
            case BackgroundResourceDesc.BackgroundType.Cafe:
                ChiefCharacterSelectionManager.CurSelectedCharacterType = CharacterResourceDesc.CharacterListType.Kahlua;
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    ChiefMiniGameManager.CurRecipe = RecipeSelectionControl.RecipeType.BlackRussian;
                }
                else
                {
                    ChiefMiniGameManager.CurRecipe = RecipeSelectionControl.RecipeType.FlatWhiteRussian;
                }
                break;

            case BackgroundResourceDesc.BackgroundType.Library:
                ChiefCharacterSelectionManager.CurSelectedCharacterType = CharacterResourceDesc.CharacterListType.HayaseuraTokusenGinjo;
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    ChiefMiniGameManager.CurRecipe = RecipeSelectionControl.RecipeType.Saketini;
                }
                else
                {
                    ChiefMiniGameManager.CurRecipe = RecipeSelectionControl.RecipeType.SamuraiRock;
                }
                break;

            case BackgroundResourceDesc.BackgroundType.PaintingRoom:
                ChiefCharacterSelectionManager.CurSelectedCharacterType = CharacterResourceDesc.CharacterListType.Absinthe;
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    ChiefMiniGameManager.CurRecipe = RecipeSelectionControl.RecipeType.DeathInTheAfternoon;
                }
                else
                {
                    ChiefMiniGameManager.CurRecipe = RecipeSelectionControl.RecipeType.Absinthe;
                }
                break;

            case BackgroundResourceDesc.BackgroundType.Restaurant:
                ChiefCharacterSelectionManager.CurSelectedCharacterType = CharacterResourceDesc.CharacterListType.CabernetSauvignon;
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    ChiefMiniGameManager.CurRecipe = RecipeSelectionControl.RecipeType.SplitzerLuge;
                }
                else
                {
                    ChiefMiniGameManager.CurRecipe = RecipeSelectionControl.RecipeType.Calimocho;
                }
                break;

            case BackgroundResourceDesc.BackgroundType.ClothingStore:
                ChiefCharacterSelectionManager.CurSelectedCharacterType = CharacterResourceDesc.CharacterListType.Hpnotiq;
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    ChiefMiniGameManager.CurRecipe = RecipeSelectionControl.RecipeType.HipKiss;
                }
                else
                {
                    ChiefMiniGameManager.CurRecipe = RecipeSelectionControl.RecipeType.IncredibleHulk;
                }
                break;
        }
        ChiefMainGameManager.Instance.Render(m_curSelectedPlace.ToString());
    }
    public void ClosePopup()
    {
        m_curSelectedPlace = BackgroundResourceDesc.BackgroundType.HomeRoom;

        m_popup_CheckPopup.SetActive(false);
    }
    #endregion

    internal void OpenPopup()
    {
        m_popup_CheckPopup.SetActive(true);
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}