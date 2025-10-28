using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CommonUtilLib.ThreadSafe;


public sealed class UIThemeControl : SingleTonForGameObject<UIThemeControl>
{
    public void Awake()
    {
        SetInstance(this);
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}