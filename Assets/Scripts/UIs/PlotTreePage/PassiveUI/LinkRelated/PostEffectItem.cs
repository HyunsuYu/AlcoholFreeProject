using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;


public class PostEffectItem : MonoBehaviour
{
    [SerializeField] private GlobalVariableDesc m_globalVariableDesc;

    [SerializeField] private TMP_Dropdown m_dropdown_GlobalVariableList;
    [SerializeField] private TMP_Dropdown m_dropdown_LinkConditionChangeType;
    [SerializeField] private TMP_InputField m_inputField_CardinalValue;

    private string m_curGlobalVariable;

    private bool m_bisInitialized = false;


    #region Unity Callbacks
    public void TrySetTargetGlobalVariable()
    {
        if (!m_bisInitialized)
        {
            return;
        }

        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        PlotData.LinkData linkData = curPlotData.LinkTable[PlotTreeActiveUIControl.Instance.CurSelectedLinkID];

        string targetGlobalVariable = m_globalVariableDesc.GlobalVariables[m_dropdown_GlobalVariableList.value];
        if (linkData.PostEffectTable.ContainsKey(targetGlobalVariable))
        {
            m_dropdown_GlobalVariableList.SetValueWithoutNotify(Array.IndexOf(m_globalVariableDesc.GlobalVariables, m_curGlobalVariable));
            return;
        }

        linkData.PostEffectTable.Add(targetGlobalVariable, linkData.PostEffectTable[m_curGlobalVariable]);
        linkData.PostEffectTable.Remove(m_curGlobalVariable);

        curPlotData.LinkTable[PlotTreeActiveUIControl.Instance.CurSelectedLinkID] = linkData;
        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        PlotTreeRightToolBarControl.Instance.Render();
    }
    public void TrySetLinkConditionChangeType()
    {
        if (!m_bisInitialized)
        {
            return;
        }

        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        PlotData.LinkData linkData = curPlotData.LinkTable[PlotTreeActiveUIControl.Instance.CurSelectedLinkID];

        linkData.PostEffectTable[m_curGlobalVariable] = new Tuple<PlotData.LinkData.LinkConditionChangeType, int>((PlotData.LinkData.LinkConditionChangeType)m_dropdown_LinkConditionChangeType.value, linkData.PostEffectTable[m_curGlobalVariable].Item2);

        curPlotData.LinkTable[PlotTreeActiveUIControl.Instance.CurSelectedLinkID] = linkData;
        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        PlotTreeRightToolBarControl.Instance.Render();
    }
    public void TrySetCardinalValue()
    {
        if (!m_bisInitialized)
        {
            return;
        }
        else if (string.IsNullOrEmpty(m_inputField_CardinalValue.text))
        {
            m_inputField_CardinalValue.SetTextWithoutNotify(PlotDataBuffer.Instance.PlotData.Value.LinkTable[PlotTreeActiveUIControl.Instance.CurSelectedLinkID].PostEffectTable[m_curGlobalVariable].Item2.ToString());
            return;
        }

        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        PlotData.LinkData linkData = curPlotData.LinkTable[PlotTreeActiveUIControl.Instance.CurSelectedLinkID];

        linkData.PostEffectTable[m_curGlobalVariable] = new Tuple<PlotData.LinkData.LinkConditionChangeType, int>(linkData.PostEffectTable[m_curGlobalVariable].Item1, int.Parse(m_inputField_CardinalValue.text));

        curPlotData.LinkTable[PlotTreeActiveUIControl.Instance.CurSelectedLinkID] = linkData;
        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        PlotTreeRightToolBarControl.Instance.Render();
    }

    public void RemoveItem()
    {
        if (!m_bisInitialized)
        {
            return;
        }

        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        PlotData.LinkData linkData = curPlotData.LinkTable[PlotTreeActiveUIControl.Instance.CurSelectedLinkID];

        linkData.PostEffectTable.Remove(m_curGlobalVariable);

        curPlotData.LinkTable[PlotTreeActiveUIControl.Instance.CurSelectedLinkID] = linkData;
        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        PlotTreeRightToolBarControl.Instance.Render();
    }
    #endregion

    internal void Render(in string targetGlobalVariable, in PlotData.LinkData.LinkConditionChangeType targetLinkConditionChangeType, in int targetCardinalValue)
    {
        m_bisInitialized = false;

        m_curGlobalVariable = targetGlobalVariable;

        m_dropdown_GlobalVariableList.options.Clear();
        foreach (string globalVariable in m_globalVariableDesc.GlobalVariables)
        {
            m_dropdown_GlobalVariableList.options.Add(new TMP_Dropdown.OptionData(globalVariable));
        }
        m_dropdown_GlobalVariableList.SetValueWithoutNotify(Array.IndexOf(m_globalVariableDesc.GlobalVariables, targetGlobalVariable));

        m_dropdown_LinkConditionChangeType.options.Clear();
        foreach (PlotData.LinkData.LinkConditionChangeType changeType in Enum.GetValues(typeof(PlotData.LinkData.LinkConditionChangeType)))
        {
            m_dropdown_LinkConditionChangeType.options.Add(new TMP_Dropdown.OptionData(changeType.ToString()));
        }
        m_dropdown_LinkConditionChangeType.SetValueWithoutNotify((int)targetLinkConditionChangeType);

        m_inputField_CardinalValue.SetTextWithoutNotify(targetCardinalValue.ToString());

        m_bisInitialized = true;
    }
}