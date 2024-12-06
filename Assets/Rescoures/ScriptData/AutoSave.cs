using UnityEngine;

public class AutoSave : MonoBehaviour
{
    public static AutoSave Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Đảm bảo script không bị xóa khi chuyển scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Kiểm tra nếu nhấn phím F1
        if (Input.GetKeyDown(KeyCode.F1))
        {
            // Giả sử bạn có một Player trong game và cần lấy thông tin
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Vector3 playerPosition = player.transform.position;
                string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                int collectedItems = 0; // Thay thế bằng logic lấy số lượng item đã thu thập

                // Gọi hàm SavePlayerData để lưu dữ liệu
                SavePlayerData(playerPosition, currentScene, collectedItems);
            }
            else
            {
                Debug.LogWarning("Không tìm thấy Player để lưu dữ liệu!");
            }
        }
    }

    public void SavePlayerData(Vector3 playerPosition, string currentScene, int collectedItems)
    {
        PlayerPrefs.SetString("LastScene", currentScene);
        PlayerPrefs.SetFloat("PlayerPosX", playerPosition.x);
        PlayerPrefs.SetFloat("PlayerPosY", playerPosition.y);
        PlayerPrefs.SetFloat("PlayerPosZ", playerPosition.z);
        PlayerPrefs.SetInt("CollectedItems", collectedItems);
        PlayerPrefs.SetInt("HasData", 1); // Đánh dấu là có dữ liệu
        PlayerPrefs.Save();
        Debug.Log($"Game data saved at position ({playerPosition.x}, {playerPosition.y}, {playerPosition.z}) in scene '{currentScene}'.");
    }


    public (Vector3, string, int) LoadPlayerData()
    {
        if (PlayerPrefs.GetInt("HasData", 0) == 1)
        {
            Vector3 playerPosition = new Vector3(
                PlayerPrefs.GetFloat("PlayerPosX"),
                PlayerPrefs.GetFloat("PlayerPosY"),
                PlayerPrefs.GetFloat("PlayerPosZ")
            );
            string currentScene = PlayerPrefs.GetString("LastScene");
            int collectedItems = PlayerPrefs.GetInt("CollectedItems");
            return (playerPosition, currentScene, collectedItems);
        }
        return (Vector3.zero, null, 0);
    }

    public void ResetPlayerData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("Game data reset!");
    }
}
