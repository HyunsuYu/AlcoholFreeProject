using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


public sealed class ImageDescriptor : IDescriptor
{
    [SerializeField] private Image m_image_Resource;

    public override void Clear()
    {
        m_image_Resource.sprite = null;
    }

    public override void Dispose()
    {
        throw new System.NotImplementedException();
    }

    public override object Read()
    {
        return m_image_Resource.sprite;
    }

    public override void Write(in object value)
    {
        switch(m_descriptorType)
        {
            case DescriptorType.Basic:
                m_image_Resource.sprite = value as Sprite;
                break;

            case DescriptorType.Background:
                FrameData frameData = (FrameData)value;
                m_image_Resource.sprite = ResourceDescBuffer.Instance.GetBackgroundSprite(frameData.CurBasicData.BackgroundType);
                break;

            default:
                throw new System.Exception($"ImageDescriptor.Write() : Invalid DescriptorType {m_descriptorType}");
        }
    }
}