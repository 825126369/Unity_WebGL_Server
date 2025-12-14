using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EditorTools
{
    public static void WaitSecond(int waitSeconds)
    {
        var lastTime = System.DateTime.Now;
        while ((System.DateTime.Now - lastTime).TotalSeconds < waitSeconds) { }
    }
}
