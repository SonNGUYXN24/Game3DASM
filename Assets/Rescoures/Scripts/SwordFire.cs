using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordFire : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f; // Thời gian tồn tại nếu không va chạm
    public ParticleSystem fireEffect;
    public ParticleSystem explosionEffect;
    public Vector3 shootDirection = Vector3.forward; // Hướng bắn mặc định
    public List<string> enemyTags; // Danh sách tag của Enemy
    public float maxDistance = 20f; // Khoảng cách tối đa để kiếm bay theo Enemy
    public float damageAmount = 50f; // Lượng sát thương gây ra

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
            Vector3 direction = shootDirection; // Hướng bắn mặc định là shootDirection

            GameObject closestEnemy = null;
            float closestDistance = Mathf.Infinity;

            // Kiểm tra tất cả các tag trong danh sách enemyTags
            foreach (string enemyTag in enemyTags)
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
                foreach (GameObject enemy in enemies)
                {
                    float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                    if (distanceToEnemy < closestDistance && distanceToEnemy <= maxDistance)
                    {
                        closestDistance = distanceToEnemy;
                        closestEnemy = enemy;
                    }
                }
            }

            if (closestEnemy != null)
            {
                // Nếu có Enemy trong khoảng cách quy định, kiếm sẽ bay theo hướng của Enemy gần nhất
                direction = (closestEnemy.transform.position - transform.position).normalized;

                // Tính toán góc quay cho trục Z để kiếm luôn chỉ hướng bay
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }

            // Di chuyển kiếm về phía trước theo hướng đã tính
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Kiểm tra nếu tag của đối tượng va chạm thuộc danh sách enemyTags
        if (enemyTags.Contains(other.tag))
        {
            // Khi va chạm với Enemy, dừng di chuyển và bắt đầu hiệu ứng nổ
            hasExploded = true;
            fireEffect.Stop();
            explosionEffect.Play();
            audioSource.PlayOneShot(explosionSoundEX);

            // Gây sát thương
            var health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
            }

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
