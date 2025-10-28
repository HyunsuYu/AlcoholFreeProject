using CommonUtilLib.ThreadSafe;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public sealed class InventoryControl : SingleTonForGameObject<InventoryControl>
{
    public enum InventoryTabType : byte
    {
        Alcohol,
        NonAlcohol,
        SubIngredient,
        Garnish,
        Ice,
        Glass,
        Tool,
        Action
    }
    public enum ItemType
    {
        None,

        // Alcohol
        Absinthe,
        Kahlua,
        DryGin,
        Vodka,
        Cognac,
        Sake,
        Hipnotic,
        RedWine,
        Champagne,

        // Non Alcohol
        Water,
        Milk,
        Espresso,
        SparklingWater,
        Coke,
        Milgis,

        // Sub Ingredient
        LumpSugar,
        LimeJuice,

        // Garnish
        LemonSlice,
        OliveSkewers,
        LimeSlice,
        SilverSpoonWithSugarCube,

        // Ice
        Ice,
        BarIce,
        SphereIce,
        CubeIce,

        // Glass
        AbsintheGlass,
        OnTheRocksGlass,
        CocktailGlass,
        WineGlass,
        HighballGlass,
        OldFashionedGlass,

        // Tool
        Shaker,

        // Action
        SpoonShaking,
        HandShaking,
    }

    [Serializable] public struct ItemUIs
    {
        public GameObject Layout_Inventory;

        [Header("Prefabs")]
        public GameObject Prefab_InventoryItem;

        [Header("Item Parent Transforms")]
        public Transform Transform_AlcoholItemParent;
        public Transform Transform_NonAlcoholItemParent;
        public Transform Transform_SubIngredientItemParent;
        public Transform Transform_GarnishItemParent;
        public Transform Transform_GlassItemParent;
        public Transform Transform_IceItemParent;
        public Transform Transform_ToolItemParent;
        public Transform Transform_ActionItemParent;

        [Header("Liqure Placeholders")]
        public InventoryLiqurePlaceholder[] InventoryLiqurePlaceholders;

        [Header("Empty Glass Placeholder")]
        public GameObject Layout_EmptyGlassPlaceholder;
        public GameObject Layout_EmptyShakerPlaceholder;
    }


    [SerializeField] private GameObject[] m_layout_InventoryTabs;
    [SerializeField] private InventoryPageBtnItem[] m_inventoryPageBtnItems;

    [SerializeField] private ItemUIs m_itemUIs;

    [Header("Item Info")]
    [SerializeField] private GraphicRaycaster m_graphicRaycaster;
    [SerializeField] private EventSystem m_eventSystem;

    [SerializeField] private RectTransform m_rectTransform_LiqureInfo;
    [SerializeField] private TMP_Text m_text_LiqureInfo;

    [SerializeField] private Vector3 m_LiqureInfoPopupDistance;

    private InventoryTabType m_curInventoryPageType = InventoryTabType.Alcohol;

    private bool m_isInitialized = false;

    private int m_curSelectedPlaceholderIndex = -1;


    public void Awake()
    {
        SetInstance(this);
    }
    //public void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Escape))
    //    {
    //        if (m_itemUIs.Layout_Inventory.activeSelf)
    //        {
    //            CloseInventoryTab();
    //        }
    //        else
    //        {
    //            OpenInventoryTab();
    //        }
    //    }
    //}

    internal InventoryTabType CurInventoryPageType
    {
        get
        {
            return m_curInventoryPageType;
        }
        set
        {
            if (m_curInventoryPageType != value)
            {
                m_curInventoryPageType = value;
                Render();
            }
        }
    }

    internal int CurSelectedPlaceholderIndex
    {
        get
        {
            return m_curSelectedPlaceholderIndex;
        }
        set
        {
            m_curSelectedPlaceholderIndex = value;
        }
    }
    internal InventoryLiqurePlaceholder[] InventoryLiqurePlaceholders
    {
        get
        {
            return m_itemUIs.InventoryLiqurePlaceholders;
        }
    }

    #region Unity Callbacks
    public void TryAddIce()
    {
        if (ChiefMiniGameManager.Instance.CurSelectedBottle != null)
        {
            ChiefMiniGameManager.Instance.CurSelectedBottle.AddIngredient(ItemType.Ice);
            if (!ChiefMiniGameManager.Instance.CurSelectedBottle.BisShaker)
            {
                ChiefMiniGameManager.Instance.CurSelectedBottle.GetComponent<GlassItem>().AddIngredientWaveScale();
            }
        }
    }

    public void OpenSubIngredientPage()
    {
        CurInventoryPageType = InventoryTabType.SubIngredient;
        OpenInventoryTab();
    }
    public void OpenIcePage()
    {
        CurInventoryPageType = InventoryTabType.Ice;
        OpenInventoryTab();
    }
    public void OpenGlassPage()
    {
        CurInventoryPageType = InventoryTabType.Glass;
        OpenInventoryTab();
    }
    public void OpenToolPage()
    {
        CurInventoryPageType = InventoryTabType.Tool;
        OpenInventoryTab();
    }

    public void CloseInventoryTab()
    {
        m_itemUIs.Layout_Inventory.SetActive(false);
    }
    #endregion

    internal void Render()
    {
        foreach(var btnItem in m_inventoryPageBtnItems)
        {
            btnItem.Render();
        }

        if(!m_isInitialized)
        {
            m_isInitialized = true;
            foreach (ItemType curItemType in Enum.GetValues(typeof(ItemType)))
            {
                if (curItemType == ItemType.None)
                {
                    continue;
                }

                switch (MiniGameItemResourceInterface.Instance.GetInventoryTabType(curItemType))
                {
                    case InventoryTabType.Alcohol:
                        {
                            InventoryItem inventoryItem = Instantiate(m_itemUIs.Prefab_InventoryItem, m_itemUIs.Transform_AlcoholItemParent).GetComponent<InventoryItem>();
                            inventoryItem.Init(curItemType, m_graphicRaycaster, m_eventSystem, m_rectTransform_LiqureInfo, m_text_LiqureInfo, m_LiqureInfoPopupDistance);
                        }
                        break;

                    case InventoryTabType.NonAlcohol:
                        {
                            InventoryItem inventoryItem = Instantiate(m_itemUIs.Prefab_InventoryItem, m_itemUIs.Transform_NonAlcoholItemParent).GetComponent<InventoryItem>();
                            inventoryItem.Init(curItemType, m_graphicRaycaster, m_eventSystem, m_rectTransform_LiqureInfo, m_text_LiqureInfo, m_LiqureInfoPopupDistance);
                        }
                        break;

                    case InventoryTabType.SubIngredient:
                        {
                            InventoryItem inventoryItem = Instantiate(m_itemUIs.Prefab_InventoryItem, m_itemUIs.Transform_SubIngredientItemParent).GetComponent<InventoryItem>();
                            inventoryItem.Init(curItemType, m_graphicRaycaster, m_eventSystem, m_rectTransform_LiqureInfo, m_text_LiqureInfo, m_LiqureInfoPopupDistance);
                        }
                        break;

                    case InventoryTabType.Garnish:
                        {
                            InventoryItem inventoryItem = Instantiate(m_itemUIs.Prefab_InventoryItem, m_itemUIs.Transform_GarnishItemParent).GetComponent<InventoryItem>();
                            inventoryItem.Init(curItemType, m_graphicRaycaster, m_eventSystem, m_rectTransform_LiqureInfo, m_text_LiqureInfo, m_LiqureInfoPopupDistance);
                        }
                        break;

                    case InventoryTabType.Ice:
                        {
                            InventoryItem inventoryItem = Instantiate(m_itemUIs.Prefab_InventoryItem, m_itemUIs.Transform_IceItemParent).GetComponent<InventoryItem>();
                            inventoryItem.Init(curItemType, m_graphicRaycaster, m_eventSystem, m_rectTransform_LiqureInfo, m_text_LiqureInfo, m_LiqureInfoPopupDistance);
                        }
                        break;

                    case InventoryTabType.Glass:
                        {
                            InventoryItem inventoryItem = Instantiate(m_itemUIs.Prefab_InventoryItem, m_itemUIs.Transform_GlassItemParent).GetComponent<InventoryItem>();
                            inventoryItem.Init(curItemType, m_graphicRaycaster, m_eventSystem, m_rectTransform_LiqureInfo, m_text_LiqureInfo, m_LiqureInfoPopupDistance);
                        }
                        break;

                    case InventoryTabType.Tool:
                        {
                            InventoryItem inventoryItem = Instantiate(m_itemUIs.Prefab_InventoryItem, m_itemUIs.Transform_ToolItemParent).GetComponent<InventoryItem>();
                            inventoryItem.Init(curItemType, m_graphicRaycaster, m_eventSystem, m_rectTransform_LiqureInfo, m_text_LiqureInfo, m_LiqureInfoPopupDistance);
                        }
                        break;

                    case InventoryTabType.Action:
                        {
                            InventoryItem inventoryItem = Instantiate(m_itemUIs.Prefab_InventoryItem, m_itemUIs.Transform_ActionItemParent).GetComponent<InventoryItem>();
                            inventoryItem.Init(curItemType, m_graphicRaycaster, m_eventSystem, m_rectTransform_LiqureInfo, m_text_LiqureInfo, m_LiqureInfoPopupDistance);
                        }
                        break;
                }
            }
        }
    }
    internal void OpenInventoryTab()
    {
        for (int index = 0; index < Enum.GetNames(typeof(InventoryTabType)).Length; index++)
        {
            if ((InventoryTabType)index == m_curInventoryPageType)
            {
                m_layout_InventoryTabs[index].SetActive(true);
            }
            else
            {
                m_layout_InventoryTabs[index].SetActive(false);
            }
        }
        m_itemUIs.Layout_Inventory.SetActive(true);
    }
    internal void RenderInventoryLiqurePlaceholders()
    {
        foreach(var placeholder in m_itemUIs.InventoryLiqurePlaceholders)
        {
            placeholder.Render((ItemType)placeholder.CurItemType);
        }
    }

    internal void CloseEmptyGlassPlaceholder()
    {
        m_itemUIs.Layout_EmptyGlassPlaceholder.SetActive(false);
    }
    internal void CloseEmptyShakerPlaceholder()
    {
        m_itemUIs.Layout_EmptyShakerPlaceholder.SetActive(false);
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}