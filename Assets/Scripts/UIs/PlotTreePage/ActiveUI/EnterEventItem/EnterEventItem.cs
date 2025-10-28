using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public sealed class EnterEventItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform m_rectTransform;

    private string m_curEnterEventName;

    [Header("For Selected State")]
    [SerializeField] private Image m_image_SelectedState;

    [Header("For Enter Event Dropdown")]
    //[SerializeField] private TMP_Dropdown m_dropdown_EnterEvent;
    [SerializeField] private TMP_Text m_text_EnterEventName;

    [Header("For Output Part")]
    [SerializeField] private PlotTreeNodeLinkSpot m_outputPart;

    [Header("For Node Movement")]
    private Vector3 m_nodeStartPos;
    private bool m_bisNodeSelected = false;

    [Header("For Hide Content")]
    [SerializeField] private RectTransform m_rectTransform_InnerContent;

    [SerializeField] private GameObject m_layout_MainContent;

    [SerializeField] private RectTransform m_rectTransform_HideIcon;

    private bool m_bisContentHidden = false;

    [SerializeField] private Transform m_transform_HiddenOutputSpot;


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
            var enterEventTable = plotData.EnterEventTable;

            enterEventTable[m_curEnterEventName] = new SerializableVector3(m_rectTransform.position);
            plotData.EnterEventTable = enterEventTable;

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
    internal Transform HiddenOutputSpot
    {
        get
        {
            return m_transform_HiddenOutputSpot;
        }
    }

    internal PlotTreeNodeLinkSpot OutputPartSpot
    {
        get
        {
            return m_outputPart;
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

        Invoke("RerenderLinkLine", 0.25f);
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

        PlotTreeActiveUIControl.Instance.CurSelectedNodeID = Guid.Empty;
        PlotTreeActiveUIControl.Instance.CurSelectedEnterEventName = m_curEnterEventName;

        ChiefUIManager.Instance.RenderAll();
        //PlotTreeActiveUIControl.Instance.Render();
    }
    #endregion

    internal void Render(in string targetEnterEventName)
    {
        m_curEnterEventName = targetEnterEventName;

        if (m_curEnterEventName == PlotTreeActiveUIControl.Instance.CurSelectedEnterEventName)
        {
            m_image_SelectedState.color = new Color(0.1803922f, 0.4862745f, 0.8196079f, 1.0f);
        }
        else
        {
            m_image_SelectedState.color = new Color(0.1686275f, 0.1686275f, 0.1686275f, 1.0f);
        }

        #region Dropdown
        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;

        //string prevSelectedEnterEventName = m_dropdown_EnterEvent.options[m_dropdown_EnterEvent.value].text;

        //m_dropdown_EnterEvent.options.Clear();
        //foreach(string enterEventName in curPlotData.EnterEventTable.Keys)
        //{
        //    m_dropdown_EnterEvent.options.Add(new TMP_Dropdown.OptionData(enterEventName));
        //}

        //int prevSelectedItemIndex = m_dropdown_EnterEvent.options.IndexOf(new TMP_Dropdown.OptionData(m_curEnterEventName));
        //if(prevSelectedItemIndex == -1)
        //{
        //    m_dropdown_EnterEvent.SetValueWithoutNotify(0);
        //}
        //else
        //{
        //    m_dropdown_EnterEvent.SetValueWithoutNotify(prevSelectedItemIndex);
        //}

        m_text_EnterEventName.text = m_curEnterEventName;
        #endregion

        m_rectTransform.position = curPlotData.EnterEventTable[m_curEnterEventName].ToVector3();

        #region Output Part
        Guid[] outputLinkIDs = PlotDataInterface.FindOutputLinks(m_curEnterEventName);
        m_outputPart.Render(m_curEnterEventName, outputLinkIDs);
        #endregion
    }

    private void RerenderLinkLine()
    {
        Guid[] outputLinkIDs = PlotDataInterface.FindOutputLinks(m_curEnterEventName);
        if(outputLinkIDs == null || outputLinkIDs.Length == 0)
        {
            return;
        }

        foreach (Guid outputLinkID in outputLinkIDs)
        {
            PlotTreeActiveUIControl.Instance.GetLinkLineItem(outputLinkID).SetFromLinkLineSpot(outputLinkID);
        }
    }
}