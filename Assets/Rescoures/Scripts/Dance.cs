using UnityEngine;

public class Dance : MonoBehaviour
{
    // AudioSource để phát nhạc
    public AudioSource musicDance;

    // Đối tượng cần tính khoảng cách (thường là Player)
    public Transform listener;

    // Khoảng cách tối đa và tối thiểu
    public float maxDistance = 200f;
    public float minDistance = 20f;

    private void Update()
    {
        if (musicDance == null || listener == null)
        {
            Debug.LogWarning("Hãy gắn AudioSource và Listener vào script!");
            return;
        }

        // Tính khoảng cách giữa nguồn phát và người nghe
        float distance = Vector3.Distance(transform.position, listener.position);

        // Điều chỉnh âm lượng dựa trên khoảng cách
        if (distance > maxDistance)
        {
            musicDance.volume = 0f; // Ngoài phạm vi tối đa không nghe thấy gì
        }
        else if (distance <= minDistance)
        {
            musicDance.volume = 1f; // Trong phạm vi tối thiểu âm lượng tối đa
        }
        else
        {
            // Tính toán âm lượng theo tỷ lệ khoảng cách
            float normalizedDistance = (distance - minDistance) / (maxDistance - minDistance);
            musicDance.volume = Mathf.Lerp(1f, 0f, normalizedDistance);
        }
    }
}
