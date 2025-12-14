using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
    public Text lb_FPS = null;
    public Text lb_Memory = null;
    private int nFameCount;
    private float _timer = 0.0f;

    void Start() 
    {
        this.nFameCount = 0;
        this._timer = 0;
    }   

    void Update() 
    {
        float deltaTime = Time.deltaTime;
        this.nFameCount++;
        this._timer += deltaTime;
        if(this._timer >= 0.5f)
        {
            this.lb_FPS.text = "FPS: " + (this.nFameCount / 0.5f);
            this.lb_Memory.text = "Memory: " + (System.GC.GetTotalMemory(false) / 1024f / 1024f);
            this._timer = 0;
            this.nFameCount = 0;
        }
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }    
}