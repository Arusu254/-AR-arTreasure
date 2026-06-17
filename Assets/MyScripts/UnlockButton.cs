using UnityEngine;

public class UnlockButton : MonoBehaviour
{
    public GearLockManager lockMgr;

    public void OnClickUnlock()
    {
        if (lockMgr != null)
        {
            lockMgr.CheckPassword();
        }
        else
        {
            Debug.LogError("Î´°ó¶¨GearLockManager");
        }
    }
}