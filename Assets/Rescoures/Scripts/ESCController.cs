using UnityEngine;
using UnityEngine.SceneManagement;

public class ESCController : MonoBehaviour
{
    // Tham chiếu tới canvasQuit
    public GameObject canvasQuit;

    // Biến theo dõi trạng thái của canvasQuit
    private bool isCanvasActive = false;

    void Start()
    {
        // Đảm bảo canvasQuit bắt đầu ở trạng thái ẩn
        if (canvasQuit != null)
        {
            canvasQuit.SetActive(false);
        }
        else
        {
            Debug.LogWarning("CanvasQuit chưa được gán trong Inspector!");
        }
    }

    void Update()
    {
        // Kiểm tra phím ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCanvas();
        }
    }

    // Hàm bật/tắt canvasQuit
    public void ToggleCanvas()
    {
        if (canvasQuit != null)
        {
            isCanvasActive = !isCanvasActive;
            canvasQuit.SetActive(isCanvasActive);
        }
    }

    // Hàm thoát game
    public void QuitGame()
    {
        Debug.Log("Thoát game!"); // Dòng này chỉ để kiểm tra trong Editor
        Application.Quit();
    }

    // Hàm ẩn canvasQuit
    public void ResumeGame()
    {
        if (canvasQuit != null)
        {
            isCanvasActive = false;
            canvasQuit.SetActive(false);
        }
    }

    // Hàm trở về scene MainMenu
    public void ExitToMainMenu()
    {
        Debug.Log("Trở về MainMenu!"); // Dòng này chỉ để kiểm tra trong Editor
        SceneManager.LoadScene("MainMenu");
    }
}
