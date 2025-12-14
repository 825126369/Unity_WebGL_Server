using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerWithGo
{
    bool unscaled = false;
    int loop = 0;
    float duration = 0f;
    float time = 0f;
    bool running = false;
    Action func;
    public GameObject go;

    public TimerWithGo New(GameObject go, Action func, float duration, int loop = 1, bool unscaled = false)
    {
        var o = new TimerWithGo();
        o.go = go;
        o.func = func;
        o.duration = duration;
        o.time = duration;
        o.loop = loop;
        o.unscaled = unscaled;
        o.running = false;
        return o;
    }

    public void Start()
    {
        this.running = true;
        TimeMgr.Instance.AddListener(this.Update);
    }

    public void Stop()
    {
        this.running = false;
        TimeMgr.Instance.RemoveListener(this.Update);
    }

    private void Update()
    {
        if (!this.running)
        {
            return;
        }

        if(go == null)
        {
            this.Stop();
            return;
        }

        float delta = this.unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
        this.time = this.time - delta;

        if (this.time <= 0)
        {
            this.func();

            if (this.loop > 0)
            {
                this.loop = this.loop - 1;
                this.time = this.time + this.duration;
            }

            if (this.loop == 0)
            {
                this.Stop();
            }
            else if (this.loop < 0)
            {
                this.time = this.time + this.duration;
            }
        }
    }
}
