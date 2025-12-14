using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeComponentPool<T> where T : Component
{
    public readonly Stack<T> pool = new Stack<T>();
    public readonly List<T> usedArray = new List<T>();

    private GameObject mItemPrefab;
    private Transform ItemParent;
    private int nMaxCapicity = -1;

    public void Init(GameObject mItemPrefab, int nInitCount = 0)
    {
        this.mItemPrefab = mItemPrefab;
        this.mItemPrefab.SetActive(false);
        this.ItemParent = this.mItemPrefab.transform.parent;
        preLoadObj(nInitCount);
    }

    private T InnerCreateItem()
    {
        GameObject go = UnityEngine.Object.Instantiate(this.mItemPrefab);
        T mItem = go.GetComponent<T>();

        PrintTool.Assert(mItem != null, "mItem is Null");
        PrintTool.Assert(mItem.gameObject != null, "mItem.node is Null");

        mItem.transform.SetParent(this.ItemParent, false);
        mItem.gameObject.SetActive(false);
        return mItem;
    }

    public void recycleObj(T obj)
    {
        int nIndex = this.usedArray.IndexOf(obj);
        PrintTool.Assert(nIndex >= 0, "recyleObj 000 Error: ", nIndex);
        this.usedArray.RemoveAt(nIndex);
        nIndex = this.usedArray.IndexOf(obj);
        PrintTool.Assert(nIndex == -1, "recyleObj 111 Error");
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(ItemParent, false);
        this.pool.Push(obj);
    }

    public T popObj()
    {
        T mItem = null;
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
