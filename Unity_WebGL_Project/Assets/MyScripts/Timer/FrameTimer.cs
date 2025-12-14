using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameTimer
{
	Action func;
	int loop;
	int nNowFrame = 0;
	int nSumFrame = 0;
	bool running = false;

	public static FrameTimer New(Action func, int nFrameCount, int loop)
	{
		var o = new FrameTimer();
		o.Init(func, nFrameCount, loop);
		return o;
	}

	public void Init(Action func, int nFrameCount, int loop = 1)
	{
		this.func = func;
		this.nSumFrame = nFrameCount;
		this.loop = loop;
		this.nNowFrame = nFrameCount;
		this.running = false;
	}

	public void Reset(Action func, int nFrameCount, int loop)
	{
		this.func = func;
		this.nSumFrame = nFrameCount;
		this.loop = loop;
		this.nNowFrame = nFrameCount;
	}

	public void Start()
	{
		TimeMgr.Instance.AddListener(this.Update);
		this.running = true;
	}

	public void Stop()
	{
		this.running = false;
		TimeMgr.Instance.RemoveListener(this.Update);
	}

	public void Update()
	{
        if (!this.running)
		{
			return;
		}

        float deltaTime = Time.time;
        this.nNowFrame = this.nNowFrame - 1;
		if (this.nNowFrame <= 0)
		{
			this.func();

			if (this.loop > 0)
			{
				this.loop = this.loop - 1;
			}

			if (this.loop == 0)
			{
				this.Stop();
			}
			else
			{
				this.nNowFrame = this.nSumFrame;
			}
		}

	}

}