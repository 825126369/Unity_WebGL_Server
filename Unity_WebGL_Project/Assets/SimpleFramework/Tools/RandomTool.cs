using System;
using System.Collections.Generic;

public static class RandomTool
{
    private static readonly System.Random random = null;
    static RandomTool()
    {
        int nSeed = (int)(DateTime.Now.Ticks % int.MaxValue);
        random = new System.Random(nSeed);
    }
    
    public static int RandomInt(int min, int max) 
    {
        return random.Next(min, max + 1);
    }

    public static int RandomArrayIndex(int min, int max)
    {
        return random.Next(min, max);
    }

    public static int Random(int number) 
    {
        return RandomInt(0, number);
    }

    public static int GetIndexByRate(List<int> rateArray)
    {
        int nSumRate = 0;
        foreach(int v in rateArray)
        {
            nSumRate = nSumRate + v;
        }
        
        int nTempTargetRate = nSumRate + 1;
        if (nSumRate >= 1)
        {
            nTempTargetRate = RandomInt(1, nSumRate + 1);
        }
        
        int nTempRate = 0;
        int nTargetIndex = -1;
        for(int i = 0; i < rateArray.Count; i++)
        {
            nTempRate = nTempRate + rateArray[i];
            if(nTempRate >= nTempTargetRate)
            {
                nTargetIndex = i;
                break;
            }
        }

        return nTargetIndex;
    }
}


