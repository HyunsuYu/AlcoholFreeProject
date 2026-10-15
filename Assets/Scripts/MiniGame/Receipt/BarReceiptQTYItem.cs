using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using TMPro;


public sealed class BarReceiptQTYItem : MonoBehaviour
{
    [SerializeField] private TMP_Text m_text_QTY;
    [SerializeField] private TMP_Text m_text_Description;
    [SerializeField] private TMP_Text m_text_UnitPrice;
    [SerializeField] private TMP_Text m_text_SubTotal;

    private InventoryControl.ItemType m_ItemType;


    internal InventoryControl.ItemType ItemType
    {
        get
        {
            return m_ItemType;
        }
    }

    internal void Render(in int qyt, in InventoryControl.ItemType itemType, in int unitPrice)
    {
        m_ItemType = itemType;

        m_text_QTY.text = qyt.ToString();

        m_text_Description.text = itemType.ToString();

        m_text_UnitPrice.text = unitPrice.ToString();
        m_text_SubTotal.text = (unitPrice * qyt).ToString();
    }
}