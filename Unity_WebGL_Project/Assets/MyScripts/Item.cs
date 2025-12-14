using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public Image mDiBan;
    public Image mImage;
    public Button clickBtn;
    GameMgr mMgr;

    private void Start()
    {
        clickBtn.onClick.AddListener(() =>
        {
            mMgr.OnClickCell(this);
        });
    }

    public string Key => this.mImage.sprite.name;

    public void OnSelect(List<Item> mSelectItemList)
    {
        if(mSelectItemList.Contains(this))
        {
            mDiBan.sprite = mMgr.mResMgr.FindSprite("pai_diban_select");
        }
        else
        {
            mDiBan.sprite = mMgr.mResMgr.FindSprite("pai_diban_white");
        }
    }

    public void Refresh(GameMgr mMgr, Sprite Key)
    {
        this.mMgr = mMgr;
        this.mImage.sprite = Key;
    }
}
