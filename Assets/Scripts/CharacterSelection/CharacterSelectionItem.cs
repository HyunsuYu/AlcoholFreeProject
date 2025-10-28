using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public sealed class CharacterSelectionItem : MonoBehaviour
{
    [Header("Core")]
    [SerializeField] private CharacterResourceDesc.CharacterListType m_characterListType;

    [Header("Render")]
    [SerializeField] private SpriteRenderer[] m_spriteRenderers;

    [SerializeField] private float m_itemFloatingHeight = 10.0f;
    [SerializeField] private float m_itemFloatingTime = 1.0f;

    [SerializeField] private Color m_nonSelectedColorMask;

    private float m_floatingTimer = 0.0f;
    private Vector3 m_initialPosition;

    private Vector3 m_initialScale = Vector3.one;

    private bool m_bisFloating = false;


    public void Awake()
    {
        m_initialPosition = m_spriteRenderers[0].transform.position;

        m_initialScale = m_spriteRenderers[0].transform.localScale;
    }
    public void Update()
    {
        if (m_bisFloating)
        {
            m_floatingTimer += Time.fixedDeltaTime;
            if (m_floatingTimer > m_itemFloatingTime)
            {
                m_floatingTimer = m_itemFloatingTime;
            }

            m_spriteRenderers[0].enabled = false;
            m_spriteRenderers[1].enabled = true;
            foreach (var item in m_spriteRenderers)
            {
                item.color = Color.white;
            }
        }
        else
        {
            m_floatingTimer -= Time.fixedDeltaTime;
            if (m_floatingTimer < 0.0f)
            {
                m_floatingTimer = 0.0f;
            }

            m_spriteRenderers[0].enabled = true;
            m_spriteRenderers[1].enabled = false;
            if (ChiefCharacterSelectionManager.CurSelectedCharacterType != CharacterResourceDesc.CharacterListType.MainCharacter)
            {
                foreach (var item in m_spriteRenderers)
                {
                    item.color = m_nonSelectedColorMask;
                }
            }
            else
            {
                foreach (var item in m_spriteRenderers)
                {
                    item.color = Color.white;
                }
            }
        }

        for (int index = 0; index < m_spriteRenderers.Length; index++)
        {
            m_spriteRenderers[index].transform.position = Vector3.Lerp(
                m_initialPosition,
                m_initialPosition + Vector3.up * m_itemFloatingHeight,
                m_floatingTimer / m_itemFloatingTime
            );
            m_spriteRenderers[index].transform.localScale = Vector3.Lerp(
                m_initialScale,
                m_initialScale * 1.1f,
                m_floatingTimer / m_itemFloatingTime
            );
        }
    }

    internal bool BIsFloating
    {
        set
        {
            m_bisFloating = value;
        }
    }

    internal CharacterResourceDesc.CharacterListType CharacterListType
    {
        get
        {
            return m_characterListType;
        }
    }
}