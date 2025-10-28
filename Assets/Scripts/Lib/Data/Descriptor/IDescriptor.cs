using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public abstract class IDescriptor : MonoBehaviour, IDisposable
{
    public enum DescriptorType
    {
        Basic,

        Text_Name,
        Text_Conversation,

        Background,

        Branch,
        Character
    }


    [SerializeField] protected DescriptorType m_descriptorType;


    public DescriptorType Type
    {
        get { return m_descriptorType; }
    }

    public abstract object Read();
    public abstract void Write(in object value);

    public abstract void Clear();

    public abstract void Dispose();
}