using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CommonUtilLib.ThreadSafe;


public sealed class PlotTreeLeftToolBarControl : SingleTonForGameObject<PlotTreeLeftToolBarControl>
{
    [SerializeField] private GameObject m_prefab_PassiveEnterEventItem;
    [SerializeField] private Transform  m_transform_PassiveEnterEventItemParent;

    private Dictionary<string, PassiveEnterEventItem> m_itemTable = new Dictionary<string, PassiveEnterEventItem>();


    public void Awake()
    {
        SetInstance(this);
    }

    internal void Render()
    {
        List<string> unreusedEnterEventItems = new List<string>(m_itemTable.Keys);
        foreach (string enterEventName in PlotDataBuffer.Instance.PlotData.Value.EnterEventTable.Keys)
        {
            if(m_itemTable.ContainsKey(enterEventName))
            {
                m_itemTable[enterEventName].Render(enterEventName);
                unreusedEnterEventItems.Remove(enterEventName);
            }
            else
            {
                PassiveEnterEventItem newItem = Instantiate(m_prefab_PassiveEnterEventItem, m_transform_PassiveEnterEventItemParent).GetComponent<PassiveEnterEventItem>();
                newItem.Render(enterEventName);

                m_itemTable.Add(enterEventName, newItem);
            }
        }

        foreach (string unusedEnterEventName in unreusedEnterEventItems)
        {
            Destroy(m_itemTable[unusedEnterEventName].gameObject);
            m_itemTable.Remove(unusedEnterEventName);
        }
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}