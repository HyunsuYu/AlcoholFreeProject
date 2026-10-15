using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


public sealed class InventoryPageBtnItem : MonoBehaviour
{
    [SerializeField] private InventoryControl.InventoryTabType m_inventoryTabType;

    [SerializeField] private Transform m_transform_FrontItemParent;
    [SerializeField] private Transform m_transform_BackItemParent;

    [SerializeField] private Image m_image_SelectedState;
    [SerializeField] private Sprite[] m_sprite_SelectedState;


    public void Start()
    {
        Render();
    }

    public void OpenThisPage()
    {
        InventoryControl.Instance.CurInventoryPageType = m_inventoryTabType;
        InventoryControl.Instance.OpenInventoryTab();
    }

    internal void Render()
    {
        #region Lagacy
        //if (InventoryControl.Instance.CurInventoryPageType == m_inventoryTabType)
        //{
        //    transform.SetParent(m_transform_FrontItemParent);
        //    m_image_SelectedState.sprite = m_sprite_SelectedState[1];
        //}
        //else
        //{
        //    transform.SetParent(m_transform_BackItemParent);
        //    m_image_SelectedState.sprite = m_sprite_SelectedState[0];
        //}

        //gameObject.SetActive(true);
        //switch (InventoryControl.Instance.CurInventoryPageType)
        //{
        //    case InventoryControl.InventoryTabType.Alcohol:
        //    case InventoryControl.InventoryTabType.NonAlcohol:
        //        if (m_inventoryTabType != InventoryControl.InventoryTabType.Alcohol && m_inventoryTabType != InventoryControl.InventoryTabType.NonAlcohol)
        //        {
        //            gameObject.SetActive(false);
        //        }
        //        break;

        //    case InventoryControl.InventoryTabType.SubIngredient:
        //    case InventoryControl.InventoryTabType.Garnish:
        //    case InventoryControl.InventoryTabType.Action:
        //        if (m_inventoryTabType != InventoryControl.InventoryTabType.SubIngredient && m_inventoryTabType != InventoryControl.InventoryTabType.Garnish && m_inventoryTabType != InventoryControl.InventoryTabType.Action)
        //        {
        //            gameObject.SetActive(false);
        //        }
        //        break;

        //    case InventoryControl.InventoryTabType.Ice:
        //        if (m_inventoryTabType != InventoryControl.InventoryTabType.Ice)
        //        {
        //            gameObject.SetActive(false);
        //        }
        //        break;

        //    case InventoryControl.InventoryTabType.Glass:
        //        if (m_inventoryTabType != InventoryControl.InventoryTabType.Glass)
        //        {
        //            gameObject.SetActive(false);
        //        }
        //        break;

        //    case InventoryControl.InventoryTabType.Tool:
        //        if (m_inventoryTabType != InventoryControl.InventoryTabType.Tool)
        //        {
        //            gameObject.SetActive(false);
        //        }
        //        break;
        //}
        #endregion
    }
}