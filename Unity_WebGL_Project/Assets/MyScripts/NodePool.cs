using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodePool
{
    public readonly Stack<GameObject> pool = new Stack<GameObject>();
    public readonly List<GameObject> usedArray = new List<GameObject>();

    private GameObject mItemPrefab;
    private Transform ItemParent;
    private int nMaxCapicity = -1;
    
    public void Init(GameObject mItemPrefab, int nInitCount = 0)
    {
        this.mItemPrefab = mItemPrefab;
        this.mItemPrefab.SetActive(false);
        this.ItemParent = this.mItemPrefab.transform.parent;

        this.preLoadObj(nInitCount);
    }

    private GameObject InnerCreateItem()
    {
        GameObject go = UnityEngine.Object.Instantiate(this.mItemPrefab) as GameObject;
        PrintTool.Assert(go != null, "go is Null");
        go.transform.SetParent(this.ItemParent, false);
        go.SetActive(false);
        return go;
    }
    
    public void recycleObj(GameObject obj)
    {
        obj.transform.SetParent(this.ItemParent, false);

        int nIndex = this.usedArray.IndexOf(obj);
        PrintTool.Assert(nIndex >= 0, "recyleObj 000 Error: ", nIndex);
        this.usedArray.RemoveAt(nIndex);
        nIndex = this.usedArray.IndexOf(obj);
        PrintTool.Assert(nIndex == -1, "recyleObj 111 Error");
        obj.gameObject.SetActive(false);
        this.pool.Push(obj);
    }

    public GameObject popObj()
    {
        GameObject mItem = null;
        if (this.pool.Count > 0)
        {
            mItem = this.pool.Pop();
        }
        else
        {
            mItem = this.InnerCreateItem();
        }

        mItem.gameObject.SetActive(true);
        this.usedArray.Add(mItem);

        if (this.nMaxCapicity > 0 && this.usedArray.Count + this.pool.Count > this.nMaxCapicity)
        {
            PrintTool.LogError("超出最大容量限制： ", this.nMaxCapicity);
        }
        return mItem;
    }

    public void SetMaxCapacity(int nMaxCapacity)
    {
        this.nMaxCapicity = nMaxCapacity;
    }

    public int GetSumCount()
    {
        return this.usedArray.Count + this.pool.Count;
    }

    public void SetItemParent(Transform ItemParent)
    {
        this.ItemParent = ItemParent;
    }

    public void preLoadObj(int nMaxCount)
    {
        int nNowCount = this.GetSumCount();
        for (int j = nNowCount; j < nMaxCount; j++)
        {
            this.pool.Push(this.InnerCreateItem());
        }
    }

    public IEnumerator preLoadObj_Co(int nFrameCount, int nCount, Action finishFunc = null)
    {
        Action mFinishFunc = finishFunc;
        int nCreateCountSingle = Mathf.CeilToInt(nCount / nFrameCount);

        while(this.GetSumCount() < nCount)
        {
            for (int j = 0; j < nCreateCountSingle; j++)
            {
                if (this.GetSumCount() >= nCount)
                {
                    if (mFinishFunc != null)
                    {
                        mFinishFunc();
                        mFinishFunc = null;
                    }
                    break;
                }
                this.pool.Push(this.InnerCreateItem());
            }
            yield return null;
        }
    }

    public void preLoadObj(int nFrameCount, int nCount, Action finishFunc = null)
    {
        Action mFinishFunc = finishFunc;
        int nCreateCountSingle = Mathf.CeilToInt(nCount / nFrameCount);

        Action preLoadInnerFunc = () =>
        {
            for (int j = 0; j < nCreateCountSingle; j++)
            {
                if (this.GetSumCount() >= nCount)
                {
                    if (mFinishFunc != null)
                    {
                        mFinishFunc();
                        mFinishFunc = null;
                    }
                    break;
                }
                this.pool.Push(this.InnerCreateItem());
            }
        };

        Timer mTimer = Timer.New(preLoadInnerFunc, 1 / 60f, nFrameCount);
        mTimer.Start();
    }
}
