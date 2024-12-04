using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FireSound : MonoBehaviour
{
    // Biến tham chiếu đến người chơi
    public GameObject player;

    // Khoảng cách tối đa để nghe thấy âm thanh
    public float maxDistance = 50f;

    // Biến AudioSource
    private AudioSource fireAudio;

    // Thời gian chờ giữa các lần trừ máu
    public float damageInterval = 0.5f;

    // Lượng máu trừ mỗi lần
    public int damageAmount = 5;

    // Biến kiểm tra trạng thái gây sát thương
    private bool isPlayerInFire = false;

    // Tham chiếu đến script Character để trừ máu
    private Character characterScript;

    void Start()
    {
        // Lấy AudioSource từ GameObject này
        fireAudio = GetComponent<AudioSource>();

        // Kiểm tra xem player đã được gán hay chưa
        if (player == null)
        {
            Debug.LogError("Player chưa được gán! Hãy gán Player trong Inspector.");
        }
        else
        {
            // Lấy script Character từ Player
            characterScript = player.GetComponent<Character>();
            if (characterScript == null)
            {
                Debug.LogError("Player không có script Character! Hãy thêm script này.");
            }
        }
    }

    void Update()
    {
        // Nếu Player chưa được gán thì không làm gì
        if (player == null) return;

        // Tính khoảng cách giữa Player và GameObject
        float distance = Vector3.Distance(player.transform.position, transform.position);

        // Nếu trong phạm vi nghe được
        if (distance <= maxDistance)
        {
            // Tính toán âm lượng dựa trên khoảng cách (càng gần càng to)
            fireAudio.volume = Mathf.Lerp(0f, 1f, 1f - (distance / maxDistance));
        }
        else
        {
            // Nếu vượt quá khoảng cách tối đa, âm lượng về 0
            fireAudio.volume = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra nếu Player chạm vào collider có bật Trigger
        if (other.CompareTag("Player") && characterScript != null)
        {
            Debug.Log("Player đã vào vùng lửa!");
            isPlayerInFire = true;
            StartCoroutine(DamagePlayer());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Kiểm tra nếu Player rời khỏi vùng lửa
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player đã rời khỏi vùng lửa!");
            isPlayerInFire = false;
        }
    }

    private IEnumerator DamagePlayer()
    {
        // Gây sát thương liên tục khi Player còn trong vùng lửa
        while (isPlayerInFire)
        {
            if (characterScript != null)
            {
                characterScript.currentHP -= damageAmount;
                Debug.Log($"Player bị trừ {damageAmount} máu. Máu hiện tại: {characterScript.currentHP}");
            }
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
