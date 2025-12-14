using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ClearCacheEditor : MonoBehaviour
{
    [MenuItem("清理Cache工具/Clear All Cache")]
    private static void ClearAllCache()
    {
        PlayerPrefs.DeleteAll();
        Caching.ClearCache();
		Directory.Delete(Application.persistentDataPath, true);

		AssetDatabase.Refresh();
		AssetDatabase.SaveAssets();
    }

    [MenuItem("清理Cache工具/Clear 数据库")]
	private static void NewMenuOption1()
	{
		PlayerPrefs.DeleteAll();
	}

	[MenuItem("清理Cache工具/Clear WWW Cache")]
	private static void NewMenuOption2()
	{
		Caching.ClearCache();
    }

	[MenuItem("清理Cache工具/Open persistentDataPath")]
	private static void OpenPersistentDataPath()
	{
		Process.Start(Application.persistentDataPath);
	}
	
}