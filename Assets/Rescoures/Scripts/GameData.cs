using UnityEngine;

public class GameData : MonoBehaviour
{
    private const string ScoreKey = "Score"; // Khóa lưu trữ điểm
    public int score; // Điểm của người chơi

    void Start()
    {
        // Tải dữ liệu khi trò chơi bắt đầu
        LoadData();
    }

    /// <summary>
    /// Lưu điểm của người chơi vào PlayerPrefs.
    /// </summary>
    public void SaveData()
    {
        PlayerPrefs.SetInt(ScoreKey, score);
        PlayerPrefs.Save(); // Lưu lại dữ liệu để đảm bảo an toàn
        Debug.Log($"Đã lưu điểm: {score}");
    }

    /// <summary>
    /// Tải điểm của người chơi từ PlayerPrefs.
    /// </summary>
    public void LoadData()
    {
        if (PlayerPrefs.HasKey(ScoreKey)) // Kiểm tra nếu đã có dữ liệu được lưu trữ
        {
            score = PlayerPrefs.GetInt(ScoreKey);
            Debug.Log($"Đã tải điểm: {score}");
        }
        else
        {
            score = 0; // Giá trị mặc định nếu không tìm thấy dữ liệu
            Debug.Log("Chưa có dữ liệu điểm, đặt mặc định là 0.");
        }
    }
}
