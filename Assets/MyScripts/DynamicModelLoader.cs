using UnityEngine;
using System.IO;

public class DynamicModelLoader : MonoBehaviour
{
    private TreasureManager treasureManager;

    void Start()
    {
        treasureManager = TreasureManager.Instance;
        Debug.LogWarning("临时关闭GL模型动态加载，请在场景中手动放置chest、key模型");
    }

    // 保留层级打印调试工具
    void PrintHierarchy(Transform parent, string indent = "")
    {
        Debug.Log(indent + parent.name);
        foreach (Transform child in parent)
            PrintHierarchy(child, indent + "  ");
    }
}