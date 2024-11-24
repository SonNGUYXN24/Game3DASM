using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public Collider damageCollider;
    public float damageAmount = 10f;
    public string targetTag; // Tag của Enemy

    // Danh sách các Collider của enemy
    public List<Collider> colliderTargets = new List<Collider>();

    void Start()
    {
        damageCollider.enabled = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(targetTag) && !colliderTargets.Contains(other))
        {
            colliderTargets.Add(other);
            var health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
            }
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(targetTag) && !colliderTargets.Contains(other))
        {
            colliderTargets.Add(other);
            var health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
            }
        }
    }

    public void BeginAttack()
    {
        colliderTargets.Clear();
        damageCollider.enabled = true;
    }

    public void EndAttack()
    {
        colliderTargets.Clear();
        damageCollider.enabled = false;
    }
}
