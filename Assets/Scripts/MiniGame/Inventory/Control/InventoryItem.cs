using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public sealed class InventoryItem : MonoBehaviour
{
    [SerializeField] private Image m_image_MainImage;
    [SerializeField] private TextMeshProUGUI m_text_ItemName;

    [SerializeField] private GameObject m_prefab_GlassItem;
    [SerializeField] private GameObject m_prefab_Shaker;

    private InventoryControl.ItemType m_curItemType;

    private GraphicRaycaster m_graphicRaycaster;
    private EventSystem m_eventSystem;

    private RectTransform m_rectTransform_LiqureInfo;
    private TMP_Text m_text_LiqureInfo;

    private Vector3 m_LiqureInfoPopupDistance;


    public void FixedUpdate()
    {
        if (m_curItemType == InventoryControl.ItemType.None)
        {
            return;
        }

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        m_graphicRaycaster.Raycast(new PointerEventData(m_eventSystem)
        {
            position = Input.mousePosition
        }, raycastResults);

        bool bisOnLiqureHolder = false;
        foreach (RaycastResult result in raycastResults)
        {
            var targetInventoryItem = result.gameObject.transform.parent.gameObject.GetComponent<InventoryItem>();
            if (result.gameObject.GetComponent<Button>() != null && targetInventoryItem != null)
            {
                bisOnLiqureHolder = true;
            }

            if (result.gameObject.GetComponent<Button>() != null && targetInventoryItem != null && targetInventoryItem.m_curItemType == m_curItemType)
            {
                m_rectTransform_LiqureInfo.gameObject.SetActive(true);
                m_rectTransform_LiqureInfo.anchoredPosition = Input.mousePosition - new Vector3(960.0f, 540.0f, 0.0f) + m_LiqureInfoPopupDistance;

                m_text_LiqureInfo.text = MiniGameItemResourceInterface.Instance.GetItemDescription(m_curItemType);
                return;
            }
        }

        if (!bisOnLiqureHolder)
        {
            m_rectTransform_LiqureInfo.gameObject.SetActive(false);
        }
    }

    #region Unity Callbacks
    public void TryUseItem()
    {
        switch(MiniGameItemResourceInterface.Instance.GetInventoryTabType(m_curItemType))
        {
            case InventoryControl.InventoryTabType.Alcohol:
            case InventoryControl.InventoryTabType.NonAlcohol:
                foreach(var placeholder in InventoryControl.Instance.InventoryLiqurePlaceholders)
                {
                    if (placeholder.CurItemType == m_curItemType)
                    {
                        InventoryControl.Instance.CloseInventoryTab();
                        m_rectTransform_LiqureInfo.gameObject.SetActive(false);
                        return;
                    }
                }

                InventoryControl.Instance.InventoryLiqurePlaceholders[InventoryControl.Instance.CurSelectedPlaceholderIndex].Render(m_curItemType);
                ChiefMiniGameManager.LiqurePlaceholderTable[InventoryControl.Instance.CurSelectedPlaceholderIndex] = m_curItemType;
                InventoryControl.Instance.CloseInventoryTab();
                m_rectTransform_LiqureInfo.gameObject.SetActive(false);
                break;

            case InventoryControl.InventoryTabType.SubIngredient:
            case InventoryControl.InventoryTabType.Garnish:
            case InventoryControl.InventoryTabType.Action:
                if (ChiefMiniGameManager.Instance.CurSelectedBottle != null)
                {
                    Debug.Log($"Adding ingredient: {m_curItemType}");
                    ChiefMiniGameManager.Instance.CurSelectedBottle.AddIngredient(m_curItemType);
                    if (!ChiefMiniGameManager.Instance.CurSelectedBottle.BisShaker)
                    {
                        ChiefMiniGameManager.Instance.CurSelectedBottle.GetComponent<GlassItem>().AddIngredientWaveScale();
                    }
                }
                InventoryControl.Instance.CloseInventoryTab();
                m_rectTransform_LiqureInfo.gameObject.SetActive(false);
                break;

            case InventoryControl.InventoryTabType.Ice:
                if (ChiefMiniGameManager.Instance.CurSelectedBottle != null)
                {
                    ChiefMiniGameManager.Instance.CurSelectedBottle.AddIngredient(m_curItemType);
                    if (!ChiefMiniGameManager.Instance.CurSelectedBottle.BisShaker)
                    {
                        ChiefMiniGameManager.Instance.CurSelectedBottle.GetComponent<GlassItem>().AddIngredientWaveScale();
                    }
                }
                InventoryControl.Instance.CloseInventoryTab();
                m_rectTransform_LiqureInfo.gameObject.SetActive(false);
                break;

            case InventoryControl.InventoryTabType.Glass:
            case InventoryControl.InventoryTabType.Tool:
                if(m_curItemType == InventoryControl.ItemType.Shaker)
                {
                    ShakerItem shakerItem = Instantiate(m_prefab_Shaker, new Vector3(-1.75f, -2.5f, 0.0f), Quaternion.identity).GetComponent<ShakerItem>();
                    shakerItem.gameObject.name = "ShakerItem";
                    DontDestroyOnLoad(shakerItem.gameObject);
                    //shakerItem.Init();

                    InventoryControl.Instance.CloseEmptyShakerPlaceholder();
                    ChiefMiniGameManager.Instance.AddBottle(shakerItem);
                }
                else
                {
                    GlassItem glassItem = Instantiate(m_prefab_GlassItem, new Vector3(-1.75f, -2.5f, 0.0f), Quaternion.identity).GetComponent<GlassItem>();
                    glassItem.gameObject.name = "glassItem";
                    DontDestroyOnLoad(glassItem.gameObject);
                    glassItem.Init((GlassItem.GlassType)m_curItemType);

                    if(ChiefMiniGameManager.Instance.CurShakerItem != null)
                    {
                        foreach(var ingredient in ChiefMiniGameManager.Instance.CurShakerItem.AddedIngredients)
                        {
                            glassItem.AddIngredient(ingredient, true);
                        }
                        glassItem.InvisibleIngredientIndex = ChiefMiniGameManager.Instance.CurShakerItem.AddedIngredients.Count;
                    }

                    InventoryControl.Instance.CloseEmptyGlassPlaceholder();
                    ChiefMiniGameManager.Instance.AddBottle(glassItem);
                }

                InventoryControl.Instance.CloseInventoryTab();
                m_rectTransform_LiqureInfo.gameObject.SetActive(false);
                break;
        }

        ChiefMiniGameManager.Instance.RenderBottles();
    }
    #endregion

    internal void Init(in InventoryControl.ItemType targetItemType, in GraphicRaycaster graphicRaycaster, in EventSystem eventSystem, in RectTransform reftTransform_LiqureInfo, in TMP_Text text_LiqureInfo, in Vector3 liqureInfoPopupDistance)
    {
        m_curItemType = targetItemType;

        m_graphicRaycaster = graphicRaycaster;
        m_eventSystem = eventSystem;

        m_rectTransform_LiqureInfo = reftTransform_LiqureInfo;
        m_text_LiqureInfo = text_LiqureInfo;

        m_LiqureInfoPopupDistance = liqureInfoPopupDistance;

        m_image_MainImage.sprite = MiniGameItemResourceInterface.Instance.GetItemTitleSprite(m_curItemType);
        m_text_ItemName.text = MiniGameItemResourceInterface.Instance.GetItemName(m_curItemType);
    }
}