using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class SymbolDefinitionEditor
{
    const string SymbolDefinition = "APP_DEBUG";
    [MenuItem("预定义符号/Debug")]
    public static void DoDefineSymbol1()
    {
        DoDefineSymbol(false);
    }

    [MenuItem("预定义符号/Release")]
    public static void DoDefineSymbol2()
    {
        DoDefineSymbol(true);
    }

    public static void DoDefineSymbol(bool bReleaseApp)
    {
        string define = SymbolDefinition;
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
        if (bReleaseApp)
        {
            if (defines.Contains(define))
            {
                int nRemoveBeginIndex = defines.IndexOf(define);
                int nRemoveCount = define.Length;
                defines = defines.Remove(nRemoveBeginIndex, nRemoveCount);
            }
        }
        else
        {
            if (!defines.Contains(define))
            {
                if (defines.EndsWith(";"))
                {
                    defines += define;
                }
                else
                {
                    defines += ";" + define;
                }

                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defines);
            }
        }
        
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defines);
    }
}