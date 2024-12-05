using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BossMusicTrigger : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource bossMusic; // AudioSource phát nhạc boss

    private void Start()
    {
        // Đảm bảo bossMusic được gán và tắt nhạc khi bắt đầu
        if (bossMusic == null)
        {
            bossMusic = GetComponent<AudioSource>();
        }

        if (bossMusic != null)
        {
            bossMusic.Stop(); // Tắt nhạc nếu đang phát
        }
        else
        {
            Debug.LogWarning("AudioSource for BossMusicTrigger is missing!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra nếu "Player" đi vào vùng trigger
        if (other.CompareTag("Player") && bossMusic != null)
        {
            bossMusic.Play();
            Debug.Log("Player entered boss area. Boss music playing...");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Kiểm tra nếu "Player" rời khỏi vùng trigger
        if (other.CompareTag("Player") && bossMusic != null)
        {
            bossMusic.Stop();
            Debug.Log("Player left boss area. Boss music stopped.");
        }
    }
}
