using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BadOpEffect : MonoBehaviour
{
    public CanvasGroup mCanvasGroup;

    void OnEnable()
    {
        mCanvasGroup.alpha = 0f;
        LeanTween.value(0f, 0.5f, 0.3f).setOnUpdate((float fValue)=>
        {
            mCanvasGroup.alpha = fValue;
        }).setLoopPingPong(1).setOnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
