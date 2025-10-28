using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public sealed class ScreenTransitionItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform m_rectTransform;

    [SerializeField] private Image m_image_SelectedState;

    [Header("For Node Movement")]
    private Vector3 m_nodeStartPos;
    private bool m_bisNodeSelected = false;

    [Header("For Hide Content")]
    [SerializeField] private RectTransform m_rectTransform_InnerContent;

    [SerializeField] private GameObject m_layout_MainContent;

    [SerializeField] private RectTransform m_rectTransform_HideIcon;

    [Header("For Input Part")]
    [SerializeField] private PlotTreeNodeLinkSpot m_inputPart;

    private bool m_bisContentHidden = false;
    [SerializeField] private Transform m_transform_HiddenInputSpot;

    private int m_curScreenTransitionNodeIndex = -1;


    public void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ChiefUIManager.Instance.CurPageType != TotalEdgyToolBarControl.PageType.PlotTree)
        {
            return;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            return;
        }

        if (!m_bisNodeSelected)
        {
            m_nodeStartPos = m_rectTransform.position;
            m_bisNodeSelected = true;
        }
        else
        {
            m_bisNodeSelected = false;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (ChiefUIManager.Instance.CurPageType != TotalEdgyToolBarControl.PageType.PlotTree)
        {
            return;
        }
        else if (Input.GetMouseButton(2))
        {
            return;
        }

        if (m_bisNodeSelected)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0.0f; // Ensure z is zero for 2D movement
            m_rectTransform.position = mousePos;

            var plotData = PlotDataBuffer.Instance.PlotData.Value;
            //var enterEventTable = plotData.EnterEventTable;

            //enterEventTable[m_curEnterEventName] = new SerializableVector3(m_rectTransform.position);
            //plotData.EnterEventTable = enterEventTable;

            var curScreenTransitionNodePoses = plotData.ScreenTransitionNodePos;
            curScreenTransitionNodePoses[m_curScreenTransitionNodeIndex] = new SerializableVector3(m_rectTransform.position);

            plotData.ScreenTransitionNodePos = curScreenTransitionNodePoses;

            PlotDataBuffer.Instance.PlotData = plotData;

            PlotDataBuffer.Instance.TrySaveData();

            //PlotTreeActiveUIControl.Instance.RenderLinkLine();

            RerenderLinkLine();
        }
        else
        {
            m_rectTransform.position = m_nodeStartPos;
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (ChiefUIManager.Instance.CurPageType != TotalEdgyToolBarControl.PageType.PlotTree)
        {
            return;
        }

        if (m_bisNodeSelected)
        {
            m_bisNodeSelected = false;
        }
    }

    internal bool BIsContentHidden
    {
        get
        {
            return m_bisContentHidden;
        }
    }
    internal Transform HiddenInputSpot
    {
        get
        {
            return m_transform_HiddenInputSpot;
        }
    }

    internal PlotTreeNodeLinkSpot InputPartSpot
    {
        get
        {
            return m_inputPart;
        }
    }

    #region Unity Callbacks
    public void HideContent()
    {
        if (m_bisContentHidden)
        {
            m_layout_MainContent.SetActive(true);

            m_rectTransform.sizeDelta = new Vector2()
            {
                x = 10.25f,
                y = 8.25f
            };
            m_rectTransform_InnerContent.sizeDelta = new Vector2()
            {
                x = 10.0f,
                y = 8.0f
            };

            m_rectTransform_HideIcon.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);

            m_bisContentHidden = false;
        }
        else
        {
            m_layout_MainContent.SetActive(false);

            m_rectTransform.sizeDelta = new Vector2()
            {
                x = 10.25f,
                y = 3.25f
            };
            m_rectTransform_InnerContent.sizeDelta = new Vector2()
            {
                x = 10.0f,
                y = 3.0f
            };

            m_rectTransform_HideIcon.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

            m_bisContentHidden = true;
        }

        Invoke("RerenderLinkLine", 0.1f);
    }

    public void SelectThisNode()
    {
        if (ChiefUIManager.Instance.CurPageType != TotalEdgyToolBarControl.PageType.PlotTree)
        {
            return;
        }
        else if (m_bisNodeSelected)
        {
            return;
        }

        PlotTreeActiveUIControl.Instance.CurScreenTransitionNodeIndex = m_curScreenTransitionNodeIndex;

        PlotTreeActiveUIControl.Instance.Render();
    }

    #endregion

    internal void Render(in int targetScreenTransitionNodeIndex)
    {
        m_curScreenTransitionNodeIndex = targetScreenTransitionNodeIndex;

        if (m_curScreenTransitionNodeIndex == PlotTreeActiveUIControl.Instance.CurScreenTransitionNodeIndex)
        {
            m_image_SelectedState.color = new Color(0.1803922f, 0.4862745f, 0.8196079f, 1.0f);
        }
        else
        {
            m_image_SelectedState.color = new Color(0.1686275f, 0.1686275f, 0.1686275f, 1.0f);
        }

        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        m_rectTransform.position = curPlotData.ScreenTransitionNodePos[m_curScreenTransitionNodeIndex].ToVector3();

        #region Input Part
        Guid[] inputLinkIDs = PlotDataInterface.FindScreenTranstionNodeInputLinks();
        m_inputPart.Render(m_curScreenTransitionNodeIndex, inputLinkIDs);
        #endregion
    }

    private void RerenderLinkLine()
    {
        Guid[] inputLinkIDs = PlotDataInterface.FindScreenTranstionNodeInputLinks();
        if(inputLinkIDs == null || inputLinkIDs.Length == 0)
        {
            return;
        }

        foreach (Guid inputLinkID in inputLinkIDs)
        {
            PlotTreeActiveUIControl.Instance.GetLinkLineItem(inputLinkID).SetToLinkLineSpot(inputLinkID);
        }
    }
}