using UnityEngine;

public class ParticleFollowKey : MonoBehaviour
{
    [Tooltip("钥匙物体")]
    public Transform targetKey;
    [Tooltip("本地Z额外偏移量")]
    public float localZAdd = 0.18f;

    private Quaternion lockRotation;

    void Start()
    {
        // 锁定粒子初始旋转，永远不变
        lockRotation = transform.localRotation;
    }

    void LateUpdate()
    {
        if (targetKey == null) return;

        // 完全复制钥匙的本地XY、本地Z+0.18偏移
        transform.localPosition = new Vector3(
            targetKey.localPosition.x,
            targetKey.localPosition.y,
            targetKey.localPosition.z + localZAdd
        );
        // 强制保持初始旋转，不受钥匙旋转影响
        transform.localRotation = lockRotation;
    }
}