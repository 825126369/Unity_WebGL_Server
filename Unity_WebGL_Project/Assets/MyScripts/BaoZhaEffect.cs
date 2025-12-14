using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaoZhaEffect : MonoBehaviour
{
    public Image mImage;
    
    public void SetImage(GameMgr mMgr, string Key)
    {
        string prefix = "majiang_pai_";
        mImage.sprite = mMgr.mResMgr.FindSprite(Key.Substring(prefix.Length));
    }
}
