using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Không phá hủy Player khi chuyển Scene
        }
        else
        {
            Destroy(gameObject); // Đảm bảo chỉ tồn tại một Player
        }
    }
}
