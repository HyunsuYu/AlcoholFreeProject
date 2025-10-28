using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;


public sealed class BranchItem : MonoBehaviour
{
    [SerializeField] private Image m_image_SelectedState;

    [SerializeField] private TMP_InputField m_inputField_BranchText;

    private int m_branchIndex = -1;


    #region Unity Callbacks
    public void SelectThis()
    {
        ChiefUIManager.Instance.CurSelectedBranchIndex = m_branchIndex;
        ChiefUIManager.Instance.RenderAll();
    }

    public void TrySetBranchText()
    {
        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        SceneData curSceneData = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID];
        FrameData curFrameData = curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex];

        curFrameData.CurBasicData.CurBranchData.BranchTransitionTargets[m_branchIndex] = m_inputField_BranchText.text;

        curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex] = curFrameData;
        curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID] = curSceneData;
        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        ChiefUIManager.Instance.RenderAll();
    }
    #endregion

    internal void Render(in int targetBranchIndex)
    {
        m_branchIndex = targetBranchIndex;

        if (m_branchIndex == ChiefUIManager.Instance.CurSelectedBranchIndex)
        {
            m_image_SelectedState.color = ChiefUIManager.SelectedColor;
        }
        else
        {
            m_image_SelectedState.color = ChiefUIManager.BasicBlackColor;
        }

        FrameData curFrameData = PlotDataBuffer.Instance.GetFrameData(ChiefUIManager.Instance.CurSelectedSceneID, ChiefUIManager.Instance.CurSelectedFrameIndex);
        m_inputField_BranchText.SetTextWithoutNotify(curFrameData.CurBasicData.CurBranchData.BranchTransitionTargets[m_branchIndex]);
    }
}