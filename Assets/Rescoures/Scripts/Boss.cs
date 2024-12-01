using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public float maxHP = 100f;
    private float currentHP;
    public Slider healthBar; // Tham chiếu đến thanh máu
    public Transform player; // Tham chiếu đến người chơi
    public float detectRange = 10f; // Khoảng cách phát hiện

    void Start()
    {
        currentHP = maxHP;
        healthBar.maxValue = maxHP;
        healthBar.value = currentHP;
        healthBar.gameObject.SetActive(false); // Ẩn thanh máu khi bắt đầu
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= detectRange)
        {
            healthBar.gameObject.SetActive(true); // Hiển thị thanh máu khi đến gần
        }
        else
        {
            healthBar.gameObject.SetActive(false); // Ẩn thanh máu khi đi xa
        }
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        healthBar.value = currentHP;
        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Xử lý khi boss chết
        Debug.Log("Boss died");
    }
}
