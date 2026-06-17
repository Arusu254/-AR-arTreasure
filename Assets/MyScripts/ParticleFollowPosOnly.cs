using UnityEngine;
public class ParticleFollowPosOnly : MonoBehaviour
{
    public Transform targetKey;

    void Update()
    {
        if (targetKey == null) return;
        Vector3 newPos = targetKey.position;
        newPos.z += 0.1f;
        transform.position = newPos;
    }
}