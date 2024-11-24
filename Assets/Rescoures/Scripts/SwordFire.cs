using System.Collections;
using UnityEngine;

public class SwordFire : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f; // Thời gian tồn tại nếu không va chạm với "Albino"
    public ParticleSystem fireEffect;
    public ParticleSystem explosionEffect;
    public Vector3 shootDirection = Vector3.forward; // Biến hướng bắn có thể chỉnh

    private bool hasExploded = false;
    private bool isRotationComplete = false;
    public AudioSource audioSource;
    public AudioClip explosionSoundEX;
    public AudioClip rotationSwordEF;

    void Start()
    {
        // Bắt đầu hiệu ứng lửa khi phóng kiếm
        fireEffect.Play();
        explosionEffect.Stop();

        // Hủy đối tượng sau khi hết thời gian tồn tại nếu không va chạm
        Destroy(gameObject, lifetime);

        // Bắt đầu việc xoay trục trước khi lao về phía trước
        StartCoroutine(RotateAndMove());
    }

    private IEnumerator RotateAndMove()
    {
        float duration = 1f; // Thời gian xoay trục là 1 giây
        float elapsed = 0f;
        Quaternion initialRotation = transform.rotation;
        Quaternion targetRotation = initialRotation * Quaternion.Euler(90, 0, 0);

        // Phát âm thanh RotationSwordEF một lần ở đầu xoay
        audioSource.PlayOneShot(rotationSwordEF);

        // Thực hiện xoay trục trong 1 giây
        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Đảm bảo rotation cuối cùng là targetRotation
        transform.rotation = targetRotation;

        // Đánh dấu hoàn thành xoay trục
        isRotationComplete = true;
    }

    void Update()
    {
        // Chỉ di chuyển nếu chưa nổ và đã thực hiện xoay trục
        if (!hasExploded && isRotationComplete)
        {
            transform.Translate(shootDirection * speed * Time.deltaTime, Space.World);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Albino"))
        {
            // Khi va chạm với Albino, dừng di chuyển và bắt đầu hiệu ứng nổ
            hasExploded = true;
            fireEffect.Stop();
            explosionEffect.Play();
            audioSource.PlayOneShot(explosionSoundEX);
            // Dừng lại và xóa kiếm sau 2 giây
            StartCoroutine(ExplodeAndDestroy());
        }
    }

    private IEnumerator ExplodeAndDestroy()
    {
        yield return new WaitForSeconds(2f); // Thời gian cho hiệu ứng nổ
        Destroy(gameObject);
    }
}
