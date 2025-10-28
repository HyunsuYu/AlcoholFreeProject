using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CommonUtilLib.ThreadSafe;


public class DescriptorBuffer : SingleTonForGameObject<DescriptorBuffer>
{
    [SerializeField] private List<IDescriptor> m_descriptorList;
    

    public void Awake()
    {
        SetInstance(this);
    }

    internal List<IDescriptor> Descriptors
    {
        get
        {
            return m_descriptorList;
        }
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}