using UnityEngine;
using UnityEngine.EventSystems;
public class ChestClickHandler : MonoBehaviour, IPointerClickHandler
{
    public GearLockManager gearLock;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("点击箱子触发了");
        if (TreasureManager.Instance == null)
        {
            Debug.LogError("TreasureManager为空");
            return;
        }
        if (TreasureManager.Instance.IsOpened)
        {
            Debug.Log("箱子已经开了，不弹面板");
            return;
        }
        gearLock.OnChestClick();
    }
}