using UnityEditor;
using UnityEngine;
using System.Collections;

public class EditorSample : EditorWindow
{
    //-------------------------------------------------------------------------
    [MenuItem("GameCloud.UCenter/导出GameCloud.Unity.UCenter.unitypackage")]
    static void exportGameCloudUCenterPackage()
    {
        string[] arr_assetpathname = new string[1];
        arr_assetpathname[0] = "Assets/GameCloud.Unity.UCenter";
        AssetDatabase.ExportPackage(arr_assetpathname, "GameCloud.Unity.UCenter.unitypackage", ExportPackageOptions.Recurse);

        Debug.Log("Export GameCloud.Unity.UCenter.unitypackage Finished!");
    }
}
