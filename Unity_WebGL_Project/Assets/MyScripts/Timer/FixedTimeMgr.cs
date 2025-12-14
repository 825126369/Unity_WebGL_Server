using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedTimeMgr : SingleTonMonoBehaviour<FixedTimeMgr>
{
    readonly List<Action> mapUpdateFunc = new List<Action>();
    
    public void FixedUpdate()
    {
        int nUpdateCount = mapUpdateFunc.Count;
        for(int i = nUpdateCount - 1; i >= 0; i--)
        {
            mapUpdateFunc[i]();
        }
    }

    public void AddListener(Action func)
    {
        if (mapUpdateFunc.IndexOf(func) == -1)
        {
            mapUpdateFunc.Add(func);
        }
    }

    public void RemoveListener(Action func)
    {
        this.mapUpdateFunc.Remove(func);
    }
}
