using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;


public class GlobalVariableConditionItem : MonoBehaviour
{
    [SerializeField] private GlobalVariableDesc m_globalVariableDesc;

    [SerializeField] private TMP_Dropdown m_dropdown_GlobalVariableList;
    [SerializeField] private TMP_Dropdown m_dropdown_LinkConditionCompareType;
    [SerializeField] private TMP_InputField m_inputField_CardinalValue;

    private string m_curGlobalVariable;

    private bool m_bisInitialized = false;


    #region Unity Callbacks
    public void TrySetTargetGlobalVariable()
    {
        if(!m_bisInitialized)
        {
            return;
        }

        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        PlotData.LinkData linkData = curPlotData.LinkTable[PlotTreeActiveUIControl.Instance.CurSelectedLinkID];

        string targetGlobalVariable = m_globalVariableDesc.GlobalVariables[m_dropdown_GlobalVariableList.value];
        if(linkData.GlobalVariableConditionTable.ContainsKey(targetGlobalVariable))
        {
            m_dropdown_GlobalVariableList.SetValueWithoutNotify(Array.IndexOf(m_globalVariableDesc.GlobalVariables, m_curGlobalVariable));
            return;
        }

        linkData.GlobalVariableConditionTable.Add(targetGlobalVariable, linkData.GlobalVariableConditionTable[m_curGlobalVariable]);
        linkData.GlobalVariableConditionTable.Remove(m_curGlobalVariable);

        curPlotData.LinkTable[PlotTreeActiveUIControl.Instance.CurSelectedLinkID] = linkData;
        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        PlotTreeRightToolBarControl.Instance.Render();
    }
    public void TrySetLinkConditionCompareType()
    {
        if (!m_bisInitialized)
        {
            return;
        }

        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        PlotData.LinkData linkData = curPlotData.LinkTable[PlotTreeActiveUIControl.Instance.CurSelectedLinkID];

        linkData.GlobalVariableConditionTable[m_curGlobalVariable] = new Tuple<PlotData.LinkData.LinkConditionCompareType, int>((PlotData.LinkData.LinkConditionCompareType)m_dropdown_LinkConditionCompareType.value, linkData.GlobalVariableConditionTable[m_curGlobalVariable].Item2);

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
            m_inputField_CardinalValue.SetTextWithoutNotify(PlotDataBuffer.Instance.PlotData.Value.LinkTable[PlotTreeActiveUIControl.Instance.CurSelectedLinkID].GlobalVariableConditionTable[m_curGlobalVariable].Item2.ToString());
            return;
        }

        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        PlotData.LinkData linkData = curPlotData.LinkTable[PlotTreeActiveUIControl.Instance.CurSelectedLinkID];

        linkData.GlobalVariableConditionTable[m_curGlobalVariable] = new Tuple<PlotData.LinkData.LinkConditionCompareType, int>(linkData.GlobalVariableConditionTable[m_curGlobalVariable].Item1, int.Parse(m_inputField_CardinalValue.text));

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

        linkData.GlobalVariableConditionTable.Remove(m_curGlobalVariable);

        curPlotData.LinkTable[PlotTreeActiveUIControl.Instance.CurSelectedLinkID] = linkData;
        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        PlotTreeRightToolBarControl.Instance.Render();
    }
    #endregion

    internal void Render(in string targetGlobalVariable, in PlotData.LinkData.LinkConditionCompareType targetLinkConditionCompareType, in int targetCardinalValue)
    {
        m_bisInitialized = false;

        m_curGlobalVariable = targetGlobalVariable;

        m_dropdown_GlobalVariableList.options.Clear();
        foreach(string globalVariable in m_globalVariableDesc.GlobalVariables)
        {
            m_dropdown_GlobalVariableList.options.Add(new TMP_Dropdown.OptionData(globalVariable));
        }
        m_dropdown_GlobalVariableList.SetValueWithoutNotify(Array.IndexOf(m_globalVariableDesc.GlobalVariables, targetGlobalVariable));

        m_dropdown_LinkConditionCompareType.options.Clear();
        foreach(PlotData.LinkData.LinkConditionCompareType compareType in Enum.GetValues(typeof(PlotData.LinkData.LinkConditionCompareType)))
        {
            m_dropdown_LinkConditionCompareType.options.Add(new TMP_Dropdown.OptionData(compareType.ToString()));
        }
        m_dropdown_LinkConditionCompareType.SetValueWithoutNotify((int)targetLinkConditionCompareType);

        m_inputField_CardinalValue.SetTextWithoutNotify(targetCardinalValue.ToString());

        m_bisInitialized = true;
    }
}