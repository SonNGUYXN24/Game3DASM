using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public Collider damageCollider; // Collider gây sát thương
    public float damageAmount = 10f; // Lượng sát thương
    public List<string> targetTags = new List<string>(); // Danh sách các tag của đối tượng mục tiêu

    // Danh sách các đối tượng đã nhận sát thương
    private HashSet<Collider> colliderTargets = new HashSet<Collider>();

    void Start()
    {
        // Vô hiệu hóa vùng sát thương khi bắt đầu
        damageCollider.enabled = false;
    }

    // Kiểm tra xem đối tượng có thuộc danh sách tag mục tiêu không
    private bool IsTarget(Collider other)
    {
        return targetTags.Contains(other.gameObject.tag);
    }

    // Khi một đối tượng đi vào vùng sát thương
    private void OnTriggerEnter(Collider other)
    {
        if (IsTarget(other) && colliderTargets.Add(other))
        {
            // Gây sát thương cho đối tượng
            var health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
            }
        }
    }

    // Khi một đối tượng vẫn ở trong vùng sát thương
    private void OnTriggerStay(Collider other)
    {
        if (IsTarget(other) && colliderTargets.Add(other))
        {
            var health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
            }
        }
    }

    // Khi một đối tượng rời khỏi vùng sát thương
    private void OnTriggerExit(Collider other)
    {
        if (colliderTargets.Contains(other))
        {
            colliderTargets.Remove(other);
        }
    }

    // Bắt đầu tấn công
    public void BeginAttack()
    {
        colliderTargets.Clear(); // Xóa danh sách các đối tượng cũ
        damageCollider.enabled = true; // Bật vùng sát thương
    }

    // Kết thúc tấn công
    public void EndAttack()
    {
        colliderTargets.Clear(); // Xóa danh sách các đối tượng đã bị sát thương
        damageCollider.enabled = false; // Tắt vùng sát thương
    }
}
