using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using TMPro;


public class TextDescriptor : IDescriptor
{
    [SerializeField] private TMP_Text m_text_Resource;


    public override void Clear()
    {
        m_text_Resource.text = string.Empty;
    }

    public override void Dispose()
    {
        throw new System.NotImplementedException();
    }

    public override object Read()
    {
        return m_text_Resource.text;
    }

    public override void Write(in object value)
    {
        switch (m_descriptorType)
        {
            case DescriptorType.Text_Name:
                {
                    FrameData frameData = (FrameData)value;
                    m_text_Resource.text = ResourceDescBuffer.Instance.GetCharacterResourceDesc(frameData.CurBasicData.AppearCharacters[frameData.CurBasicData.SpeakerCharacterIndex].Item1).CharacterName;
                }
                m_text_Resource.text = value as string;
                break;

            case DescriptorType.Text_Conversation:
                {
                    FrameData frameData = (FrameData)value;
                    m_text_Resource.text = frameData.CurBasicData.CurConversationData.SpeakText;
                }
                break;

            default:
                throw new System.Exception($"TextDescriptor.Write() : Invalid DescriptorType {m_descriptorType}");
        }
    }
}