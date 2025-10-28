using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using CommonUtilLib.ThreadSafe;


public class RenderTargetControl : SingleTonForGameObject<RenderTargetControl>
{
    [Header("Main Components")]
    [SerializeField] private Camera m_camera_GameCamera;

    [Header("Render Target UI Rect")]
    [SerializeField] private RectTransform m_rectTransform_RenderTargetLayout;


    public void Awake()
    {
        SetInstance(this);
    }

    internal void UpdateRenderTargetSize()
    {
        //Debug.Log($"UpdateRenderTargetSize: {m_rectTransform_RenderTargetLayout.rect.width}, {m_rectTransform_RenderTargetLayout.rect.height}");
        //m_camera_GameCamera.targetTexture = new RenderTexture((int)m_rectTransform_RenderTargetLayout.rect.width, (int)m_rectTransform_RenderTargetLayout.rect.height, 0);
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}