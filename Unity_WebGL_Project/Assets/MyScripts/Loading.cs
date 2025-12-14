using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public Slider mSlider;

    void Start()
    {
        LeanTween.value(0f, 1f, 1.0f).setOnUpdate((float fValue) =>
        {
            mSlider.value = fValue;
        }).setOnComplete(()=>
        {
            this.gameObject.SetActive(false);
        });
    }
    
    void Update()
    {
        
    }
}
