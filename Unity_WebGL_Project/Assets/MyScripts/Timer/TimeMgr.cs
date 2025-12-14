using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeOutGenerator
{
    float fLastUpdateTime = 0;
    float fInternalTime = 0;
    public static TimeOutGenerator New(float fInternalTime)
    {
        var temp = new TimeOutGenerator();
        temp.Init(fInternalTime);
        return temp;
    }

    public void Init(float fInternalTime = 1.0f)
    {
        this.fInternalTime = fInternalTime;
        this.Reset();
    }

    public void Reset()
    {
        this.fLastUpdateTime = Time.time;
    }

    public bool orTimeOut()
    {
        if ((Time.time - fLastUpdateTime) > fInternalTime)
        {
            this.Reset();
            return true;
        }

        return false;
    }

    public bool orTimeOutWithSpecialTime(float fInternalTime)
    {
        if (Time.time - fLastUpdateTime > fInternalTime)
        {
            this.Reset();
            return true;
        }

        return false;
    }
}

//-------------------------------------------------------------------------------------

public class TimeMgr:SingleTonMonoBehaviour<TimeMgr>
{
    readonly List<Action> mapUpdateFunc = new List<Action>();
    
    public void Update()
    {
        int nUpdateCount = mapUpdateFunc.Count;
        for(int i = 0; i < nUpdateCount; i++)
        {
            if(i < mapUpdateFunc.Count)
            {
                mapUpdateFunc[i]();
            }
            else
            {
                break;
            }
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
