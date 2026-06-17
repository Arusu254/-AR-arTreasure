using UnityEngine;

public class ChestAudioPlayer : MonoBehaviour
{
    public AudioClip OpenClip;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // 对外提供播放方法，给TreasureManager调用
    public void PlayOpenSound()
    {
        if (OpenClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(OpenClip);
        }
    }
}