using UnityEngine;

public class UIGearClick : MonoBehaviour
{
    public GearLockManager lockMgr;
    public int GearIndex;

    // 注释掉旧的3D物体鼠标点击，现在由预览图射线接管
    /*
    private void OnMouseDown()
    {
        Debug.Log($"点击UI齿轮{GearIndex}");
        lockMgr.gearNum[GearIndex] = (lockMgr.gearNum[GearIndex] + 1) % 10;
        float targetAngle = lockMgr.gearNum[GearIndex] * 90f;
        transform.localEulerAngles = new Vector3(0, targetAngle, 0);
        lockMgr.gearObjs[GearIndex].transform.localEulerAngles = new Vector3(0, targetAngle, 0);
    }
    */
}