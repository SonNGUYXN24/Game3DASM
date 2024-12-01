using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public Slider healthBar; // Tham chiếu đến thanh máu trong UI
    public Transform boss; // Tham chiếu đến boss
    public Transform player; // Tham chiếu đến người chơi
    public float detectRange = 10f; // Khoảng cách phát hiện

    private void Update()
    {
        float distance = Vector3.Distance(player.position, boss.position);
        if (distance <= detectRange)
        {
            healthBar.gameObject.SetActive(true); // Hiển thị thanh máu khi đến gần
            Vector3 screenPos = Camera.main.WorldToScreenPoint(boss.position);
            healthBar.transform.position = screenPos + new Vector3(0, 50, 0); // Điều chỉnh vị trí trên màn hình
        }
        else
        {
            healthBar.gameObject.SetActive(false); // Ẩn thanh máu khi đi xa
        }
    }

    public void UpdateHealthBar(float currentHP, float maxHP)
    {
        healthBar.maxValue = maxHP;
        healthBar.value = currentHP;
    }
}
