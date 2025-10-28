using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


public sealed class ScreenBrushTransition : MonoBehaviour
{
    [SerializeField] private Image m_image_RenderTarget;
    [SerializeField] private Material m_ScreenBrushTransitionMat;
    
    [SerializeField] private LineRenderer m_lineRenderer;

    private bool m_bisStart = false;
    private float m_timer = 0.0f;

    private float m_timeDuration = -1.0f;

    private AudioSource m_bgmAudioSource = null;
    private AudioSource m_sfxAudioSource = null;

    private SettingData? m_curSettingData = null;

    private Vector4[] m_routes = new Vector4[1000];


    public void FixedUpdate()
    {
        if(m_bisStart)
        {
            m_timer += Time.fixedDeltaTime;

            if(m_timer >= m_timeDuration)
            {
                ClearSettings();
            }

            int curRouteCount = (int)(m_lineRenderer.positionCount * (m_timer / m_timeDuration));
            for(int index = 0; index < curRouteCount; index++)
            {
                Vector3 curPos = Camera.main.WorldToScreenPoint(m_lineRenderer.GetPosition(index));
                m_routes[index] = new Vector4()
                {
                    x = curPos.x / Camera.main.pixelWidth,
                    y = curPos.y / Camera.main.pixelHeight
                };
            }

            m_image_RenderTarget.material.SetVectorArray("_Routes", m_routes);
            m_image_RenderTarget.material.SetInteger("_RouteCount", curRouteCount);
        }
    }

    public void StartTransition(in float timeDuration, in Texture beforeTexture, in Texture afterTexture, in AudioSource bgmAudioSources = null, in AudioSource sfxAudioSources = null)
    {
        m_timeDuration = timeDuration;
        m_timer = 0.0f;

        m_bgmAudioSource = bgmAudioSources;
        m_sfxAudioSource = sfxAudioSources;

        m_curSettingData = SettingDataBuffer.Instance.SettingData;

        m_image_RenderTarget.material = m_ScreenBrushTransitionMat;
        m_image_RenderTarget.material.SetTexture("_BeforeTex", beforeTexture);
        m_image_RenderTarget.material.SetTexture("_AfterTex", afterTexture);

        m_bisStart = true;
    }

    internal void ClearSettings()
    {
        m_image_RenderTarget.material = null;

        m_timer = 0.0f;
        m_timeDuration = -1.0f;

        m_curSettingData = null;

        m_image_RenderTarget.material.SetTexture("_BeforeTex", null);
        m_image_RenderTarget.material.SetTexture("_AfterTex", null);

        for(int index = 0; index < m_routes.Length; index++)
        {
            m_routes[index] = Vector4.zero;
        }
        m_image_RenderTarget.material.SetVectorArray("_Routes", m_routes);
        m_image_RenderTarget.material.SetInteger("_RouteCount", 0);

        m_bisStart = false;
    }
}