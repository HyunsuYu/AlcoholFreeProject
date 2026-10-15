using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public sealed class MapPlaceButton : MonoBehaviour
{
    [SerializeField] private BackgroundResourceDesc.BackgroundType m_backgroundType;

    private RectTransform m_rectTransform;
    private Vector2 m_initPos;

    [SerializeField] private float m_targetFloatHeight;
    private float m_curFloatingHeight;

    [Range(0f, 1f)]
    [SerializeField] private float m_dowmSpeed;
    private bool m_bisOnCursor = false;

    [Header("State Icon")]
    [SerializeField] private GameObject m_gameObject_StateIcon;


    public void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();
        m_initPos = m_rectTransform.anchoredPosition;
    }
    public void Update()
    {
        if(!m_bisOnCursor)
        {
            CurFloatingHeight -= m_dowmSpeed;
        }

        m_rectTransform.anchoredPosition = new Vector2()
        {
            x = m_initPos.x,
            y = Mathf.Lerp(m_initPos.y, m_initPos.y + m_targetFloatHeight, m_curFloatingHeight)
        };

        if(m_gameObject_StateIcon != null)
        {
            if (m_curFloatingHeight >= 0.8f)
            {
                m_gameObject_StateIcon.SetActive(true);
            }
            else
            {
                m_gameObject_StateIcon.SetActive(false);
            }
        }
    }

    internal float CurFloatingHeight
    {
        get
        {
            return m_curFloatingHeight;
        }
        set
        {
            if (0.0f <= value && value <= 1.0f)
            {
                m_curFloatingHeight = value;
            }
        }
    }
    internal bool BIsOnCursor
    {
        set
        {
            m_bisOnCursor = value;
        }
    }

    #region Unity Callbacks
    public void SelectPlace()
    {
        ChiefMapManager.Instance.CurSelectedPlace = m_backgroundType;
        ChiefMapManager.Instance.OpenPopup();
    }
    #endregion
}