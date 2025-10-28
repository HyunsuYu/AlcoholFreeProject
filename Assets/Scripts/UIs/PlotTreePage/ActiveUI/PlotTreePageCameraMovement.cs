using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public sealed class PlotTreePageCameraMovement : MonoBehaviour
{
    private Transform m_transform_MainCamera;

    [SerializeField] private float m_moveSpeed = 0.05f;
    [SerializeField] private float m_zoomSpeed = 0.1f;

    private Vector3 m_startMousePos;


    public void Awake()
    {
        m_transform_MainCamera = Camera.main.transform;
    }
    public void Update()
    {
        if(ChiefUIManager.Instance.CurPageType == TotalEdgyToolBarControl.PageType.PlotTree)
        {
            if(Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
            {
                m_transform_MainCamera.position = new Vector3(0, 0, -10);
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                m_transform_MainCamera.position += Vector3.up * m_moveSpeed;
            }
            if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                m_transform_MainCamera.position += Vector3.down * m_moveSpeed;
            }
            if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                m_transform_MainCamera.position += Vector3.left * m_moveSpeed;
            }
            if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                m_transform_MainCamera.position += Vector3.right * m_moveSpeed;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    m_startMousePos = Input.mousePosition;
                }
                if (Input.GetMouseButton(0))
                {
                    Vector3 delta = -(Input.mousePosition - m_startMousePos);
                    m_startMousePos = Input.mousePosition;
                    Vector3 newPos = m_transform_MainCamera.position + new Vector3(delta.x * m_moveSpeed, delta.y * m_moveSpeed, 0);
                    m_transform_MainCamera.position = newPos;
                }
                if( Input.GetMouseButtonUp(0))
                {
                    m_startMousePos = Vector3.zero;
                }
            }
            else if(Input.GetMouseButton(2))
            {
                if (Input.GetMouseButtonDown(2))
                {
                    m_startMousePos = Input.mousePosition;
                }
                if (Input.GetMouseButton(2))
                {
                    Vector3 delta = -(Input.mousePosition - m_startMousePos);
                    m_startMousePos = Input.mousePosition;
                    Vector3 newPos = m_transform_MainCamera.position + new Vector3(delta.x * m_moveSpeed, delta.y * m_moveSpeed, 0);
                    m_transform_MainCamera.position = newPos;
                }
                if (Input.GetMouseButtonUp(2))
                {
                    m_startMousePos = Vector3.zero;
                }
            }

            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - Input.mouseScrollDelta.y * m_zoomSpeed, 1.0f, 100.0f);
        }
    }
}