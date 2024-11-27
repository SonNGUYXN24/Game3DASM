using System.Collections;
using System.Collections.Generic; // Thêm using cho List
using UnityEngine;

public class SwordFireRotation : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f; // Thời gian tồn tại nếu không va chạm với "Albino"
    public ParticleSystem explosionEffect;
    public List<string> enemyTags; // Danh sách tag của Enemy
    public float maxDistance = 20f; // Khoảng cách tối đa để kiếm bay theo Enemy

    private bool hasExploded = false;
    private bool isChasingEnemy = false; // Kiểm tra xem đã bắt đầu bay tới Enemy chưa
    private GameObject target; // Thêm biến để lưu trữ mục tiêu
    public AudioSource audioSource;
    public AudioClip explosionSoundEX;
    public AudioClip rotationSwordEF;

    void Start()
    {
        // Bắt đầu hiệu ứng lửa khi phóng kiếm
        explosionEffect.Stop();

        // Hủy đối tượng sau khi hết thời gian tồn tại nếu không va chạm
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Nếu chưa nổ và chưa bắt đầu bay tới Enemy, kiểm tra xung quanh
        if (!hasExploded && !isChasingEnemy)
        {
            GameObject closestEnemy = FindClosestEnemy();
            if (closestEnemy != null)
            {
                target = closestEnemy;
                isChasingEnemy = true;
            }
        }

        // Nếu đang bay tới Enemy, tiếp tục di chuyển
        if (!hasExploded && isChasingEnemy)
        {
            if (target != null)
            {
                Vector3 direction = (target.transform.position - transform.position).normalized;
                transform.Translate(direction * speed * Time.deltaTime, Space.World);
            }
        }
    }

    private GameObject FindClosestEnemy()
    {
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

        return closestEnemy;
    }

    void OnTriggerEnter(Collider other)
    {
        // Kiểm tra nếu đối tượng va chạm có tag nằm trong danh sách enemyTags
        if (enemyTags.Contains(other.tag))
        {
            // Khi va chạm với đối tượng thuộc danh sách enemyTags, dừng di chuyển và bắt đầu hiệu ứng nổ
            hasExploded = true;
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
