using System.Collections;
using UnityEngine;

public class MP : MonoBehaviour
{
    public int maxMP = 100; // Lượng MP tối đa
    public int currentMP = 100; // Lượng MP hiện tại bắt đầu đầy
    public float recoverRate = 1f; // Tốc độ phục hồi MP (1 MP mỗi giây)
    public Character character; // Tham chiếu đến nhân vật để cập nhật UI

    private Coroutine recoverCoroutine; // Để kiểm soát coroutine phục hồi MP

    void Start()
    {
        // Đảm bảo MP luôn khởi tạo ở mức hợp lệ
        currentMP = Mathf.Clamp(currentMP, 0, maxMP);
        character.UpdateMP(); // Cập nhật giao diện người dùng lúc bắt đầu

        // Bắt đầu phục hồi MP tự động
        recoverCoroutine = StartCoroutine(RecoverMP());
    }

    /// <summary>
    /// Phương thức sử dụng MP
    /// </summary>
    /// <param name="amount">Lượng MP cần sử dụng</param>
    /// <returns>Trả về true nếu đủ MP, false nếu không đủ</returns>
    public bool UseMP(int amount)
    {
        if (currentMP >= amount)
        {
            currentMP -= amount; // Trừ MP
            character.UpdateMP(); // Cập nhật giao diện người dùng

            // Kiểm tra nếu đang phục hồi MP và MP chưa đầy, thì đảm bảo coroutine vẫn hoạt động
            if (recoverCoroutine == null)
            {
                recoverCoroutine = StartCoroutine(RecoverMP());
            }
            return true; // Đủ MP
        }

        return false; // Không đủ MP
    }

    /// <summary>
    /// Coroutine phục hồi MP theo thời gian
    /// </summary>
    /// <returns></returns>
    private IEnumerator RecoverMP()
    {
        while (currentMP < maxMP)
        {
            yield return new WaitForSeconds(0.1f / recoverRate); // Tăng MP mỗi khoảng thời gian
            currentMP += 1; // Phục hồi 1 MP
            currentMP = Mathf.Clamp(currentMP, 0, maxMP); // Đảm bảo MP không vượt quá giới hạn
            character.UpdateMP(); // Cập nhật giao diện
        }

        // Khi MP đã đầy, dừng coroutine
        recoverCoroutine = null;
    }
}
