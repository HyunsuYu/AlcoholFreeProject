using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


public abstract class AUIPage : MonoBehaviour, IDisposable
{
    public abstract void Dispose();

    internal abstract void Init(in int pageIndex);

    internal abstract void OpenThisPage(in PageSynchronizer.PageOpenType pageOpenType);
    internal abstract void CloseThisPage(in PageSynchronizer.PageOpenType pageOpenType);
}