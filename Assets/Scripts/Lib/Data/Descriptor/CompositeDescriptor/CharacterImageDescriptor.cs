using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


public class CharacterImageDescriptor : IDescriptor
{
    [SerializeField] private Image[] m_image_Characters;

    private RectTransform[] m_rectTranform_Characters;
    [SerializeField] private HorizontalLayoutGroup m_horizontalLayoutGroup_Character;


    public void Awake()
    {
        m_rectTranform_Characters = new RectTransform[m_image_Characters.Length];
        for(int index = 0; index < m_image_Characters.Length; index++)
        {
            m_rectTranform_Characters[index] = m_image_Characters[index].GetComponent<RectTransform>();
        }
    }

    public override void Clear()
    {
        throw new System.NotImplementedException();
    }

    public override void Dispose()
    {
        throw new System.NotImplementedException();
    }

    public override object Read()
    {
        throw new System.NotImplementedException();
    }

    public override void Write(in object value)
    {
        FrameData frameData = (FrameData)value;

        if (frameData.CurFrameType == FrameData.FrameType.CutScene)
        {
            return;
        }

        if(frameData.CurBasicData.AppearCharacters.Count > 0)
        {
            int targetCharacterConfigIndex = frameData.CurBasicData.AppearCharacters.Count - 1;
            m_horizontalLayoutGroup_Character.padding = new RectOffset()
            {
                left = (int)ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[targetCharacterConfigIndex].Padding.x,
                right = (int)ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[targetCharacterConfigIndex].Padding.y,
                top = (int)ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[targetCharacterConfigIndex].Padding.z,
                bottom = (int)ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[targetCharacterConfigIndex].Padding.w
            };
            m_horizontalLayoutGroup_Character.spacing = ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[targetCharacterConfigIndex].HorizontalSpacing;
        }

        for (int index = 0; index < m_image_Characters.Length; index++)
        {
            m_image_Characters[index].gameObject.SetActive(false);
            m_image_Characters[index].sprite = null;

            Debug.Log(frameData.CurBasicData.AppearCharacters.Count);
            if (frameData.CurBasicData.AppearCharacters.Count > 0)
            {
                int targetCharacterConfigIndex = frameData.CurBasicData.AppearCharacters.Count - 1;
                Debug.Log(ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[targetCharacterConfigIndex].ImageSize);
                m_rectTranform_Characters[index].sizeDelta = new Vector2()
                {
                    x = ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[targetCharacterConfigIndex].ImageSize.x,
                    y = ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[targetCharacterConfigIndex].ImageSize.y
                };
                m_rectTranform_Characters[index].pivot = new Vector2()
                {
                    x = 0.5f,
                    y = ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[targetCharacterConfigIndex].PerCharacterImagePositionPivot[index]
                };
            }

            if(index < frameData.CurBasicData.AppearCharacters.Count)
            {
                m_image_Characters[index].sprite = ResourceDescBuffer.Instance.GetCharacterResourceDesc(
                    frameData.CurBasicData.AppearCharacters[index].Item1).CharacterSprites[
                    (int)frameData.CurBasicData.AppearCharacters[index].Item2];

                m_image_Characters[index].gameObject.SetActive(true);
            }
        }
    }
}