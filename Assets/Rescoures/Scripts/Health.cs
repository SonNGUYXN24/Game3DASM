using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHP;
    public float currentHP;


    public virtual void TakeDamage(float damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);

    }

}
