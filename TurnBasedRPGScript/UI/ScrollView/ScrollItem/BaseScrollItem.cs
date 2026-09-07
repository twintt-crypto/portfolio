using Gpm.Ui;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScrollItem<T> : InfiniteScrollItem where T : InfiniteScrollData
{        
    public override void UpdateData(InfiniteScrollData scrollData)
    {        
        UpdateData(scrollData as T);
    }

    protected virtual void UpdateData(T scrollData)
    {
        
    }

    protected T GetData()
    {
        return scrollData as T;
    }    
}
