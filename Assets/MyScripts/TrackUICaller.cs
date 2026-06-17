using easyar;
using UnityEngine;

public class TrackUICaller : MonoBehaviour
{
    public GameObject chestRoot;
    private ActiveController activeCtrl;
    private bool isTracked = false;

    void Start()
    {
        activeCtrl = GetComponent<ActiveController>();
    }

    void Update()
    {
        // 判定宝箱刚被激活（识别成功瞬间）
        if (chestRoot.activeSelf && !isTracked)
        {
            isTracked = true;
            GearLockManager lockMgr = chestRoot.GetComponent<GearLockManager>();
            if (lockMgr != null)
            {
                lockMgr.ShowClickTip();
                Debug.Log("识别成功，弹出点击提示UI");
            }
        }
        // 判定宝箱刚隐藏（丢失识别）
        if (!chestRoot.activeSelf && isTracked)
        {
            isTracked = false;
            GearLockManager lockMgr = chestRoot.GetComponent<GearLockManager>();
            if (lockMgr != null)
            {
                lockMgr.ResetAllGear();
                Debug.Log("丢失识别，重置齿轮与锁");
            }
            TreasureManager.Instance.IsOpened = false;
        }
    }
}