using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    public BaoZhaEffect mBaoZhaEffect;
    public GameObject BadOpEffect;
    public GameObject goClickOk;
    public Loading mLoadingView;
    public CommonResSerialization mResMgr;
    public Item mItemPrefab;
    public Transform ItemParent;
    private readonly Vector2 cellSize = new Vector2(128, 157);
    const int nMaxX = 20;
    const int nMaxY = 40;

    List<Sprite> mSpriteList = new List<Sprite>(); 
    readonly List<Item> mItemPool = new List<Item>();
    readonly List<List<Item>> mItemList = new List<List<Item>>();
    readonly List<Item> mSelectItemList = new List<Item>();

    readonly NodeComponentPool<BaoZhaEffect> mBaoZhaEffectPool = new NodeComponentPool<BaoZhaEffect>();
    int nXiaoChuCount = 0;

    void Start()
    {
        mBaoZhaEffect.gameObject.SetActive(false);
        BadOpEffect.gameObject.SetActive(false);
        mBaoZhaEffectPool.Init(mBaoZhaEffect.gameObject, 2);
        goClickOk.SetActive(false);
        mLoadingView.gameObject.SetActive(true);
        InitGame();
    }

    void InitGame()
    {
        mSpriteList = mResMgr.FindSpriteList("majiang_pai_");
        
        for (int i = 0; i < nMaxX; i++)
        {
            var mItemList2 = new List<Item>();
            mItemList.Add(mItemList2);
            for (int j = 0; j < nMaxY; j++)
            {
                var mItem = PopItem();
                int nRandomIndex = RandomTool.RandomArrayIndex(0, mSpriteList.Count);   
                mItem.Refresh(this, mSpriteList[nRandomIndex]);
                mItem.transform.localPosition = GetPos(i, j);
                mItemList2.Add(mItem);
            }
        }

        var mPos = (GetPos(0, 0) + GetPos(nMaxX - 1, nMaxY - 1)) / 2f;
        this.ItemParent.transform.localPosition = -mPos;
    }

    private Vector3 GetPos(int nX, int nY)
    {
        return new Vector3(cellSize.x * nX, cellSize.y * nY, 0);
    }

    public void OnClickCell(Item mSelect)
    {
        if (!mSelectItemList.Contains(mSelect))
        {
            mSelectItemList.Add(mSelect);
        }

        foreach (var v in mItemList)
        {
            foreach (var v2 in v)
            {
                v2.OnSelect(mSelectItemList);
            }
        }

        if(mSelectItemList.Count == 2)
        {
            if (mSelectItemList[0].Key == mSelectItemList[1].Key)
            {
                nXiaoChuCount++;
                DoMove();
            }
            else
            {
                goClickOk.SetActive(false);
                foreach (var v in mSelectItemList)
                {
                    var mItem = v;
                    LeanTween.delayedCall(0.5f, () =>
                    {
                        mItem.mDiBan.sprite = mResMgr.FindSprite("pai_diban_error");
                        LeanTween.alpha(mItem.gameObject, 0, 0.3f).setOnComplete(() =>
                        {
                            mItem.mDiBan.sprite = mResMgr.FindSprite("pai_diban_white");
                        });

                    });
                }

                LeanTween.delayedCall(0.5f, () =>
                {
                    BadOpEffect.SetActive(true);

                });
            }

            mSelectItemList.Clear();
        }
    }

    private void DoMove()
    {
        List<int> mNeedMoveList = new List<int>();
        for (int i = 0; i < nMaxX; i++)
        {
            var mItemList2 = mItemList[i];
            for (int j = mItemList2.Count - 1; j >= 0; j--)
            {
                var mItem = (Item)mItemList2[j];
                if (mSelectItemList.Contains((Item)mItem))
                {
                    mItemList2.Remove(mItem);
                    RecycleItem(mItem);

                    if (!mNeedMoveList.Contains(i))
                    {
                        mNeedMoveList.Add(i);
                    }
                }
            }
        }

        goClickOk.SetActive(true);
        foreach (var v in mSelectItemList)
        {
            var mItem = v;
            LeanTween.delayedCall(0.5f, () =>
            {
                var mEffect = mBaoZhaEffectPool.popObj();
                mEffect.SetImage(this, mItem.Key);
                mEffect.transform.SetParent(this.transform, false);
                mEffect.transform.position = mItem.transform.position;
                mEffect.gameObject.SetActive(true);

                LeanTween.delayedCall(1.5f, () =>
                {
                    mBaoZhaEffectPool.recycleObj(mEffect);
                });
            });
        }

        mSelectItemList.Clear();
        LeanTween.delayedCall(1.0f, () =>
        {
            foreach (var v in mNeedMoveList)
            {
                int i = v;
                var mItemList2 = mItemList[i];
                for (int j = 0; j < mItemList2.Count; j++)
                {
                    var mItem = (Item)mItemList2[j];
                    LeanTween.moveLocalY(mItem.gameObject, GetPos(i, j).y, 0.5f);
                }
            }

            if (nXiaoChuCount >= 15)
            {
                //跳转到商店界面
                Debug.Log("跳转");
            }
        });

    }

    private Item PopItem()
    {
        Item mItem = null;
        if (mItemPool.Count > 0)
        {
            mItem = mItemPool[mItemPool.Count - 1];
        }
        else
        {
            var go = Instantiate<GameObject>(mItemPrefab.gameObject);
            mItem = go.GetComponent<Item>();
            mItem.transform.SetParent(this.ItemParent, false);
            mItem.transform.localScale = Vector3.one;
        }

        mItem.gameObject.SetActive(true);
        return mItem;
    }

    private void RecycleItem(Item mItem)
    {
        mItem.gameObject.SetActive(false);
        mItemPool.Add(mItem);
    }


}
