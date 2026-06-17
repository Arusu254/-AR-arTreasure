using UnityEngine;

public class ARModelActive : MonoBehaviour
{
    public Transform lid;
    public Transform lidLock;
    public Transform key;
    public Material chestMat;

    void Start()
    {
        if (TreasureManager.Instance != null)
        {
            TreasureManager.Instance.SetLid(lid);
            TreasureManager.Instance.SetLidLock(lidLock);
            TreasureManager.Instance.SetKey(key);
            TreasureManager.Instance.SetChestMat(chestMat);
        }
    }
}