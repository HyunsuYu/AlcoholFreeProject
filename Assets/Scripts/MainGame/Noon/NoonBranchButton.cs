using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;


public sealed class NoonBranchButton : MonoBehaviour
{
    [SerializeField] private int m_branchIndex = 0;


    public void SelectThisBranch()
    {
        NoonCanvasControl.CurSelectedBranchIndex = m_branchIndex;

        EventSystem.current.SetSelectedGameObject(null);
    }
}