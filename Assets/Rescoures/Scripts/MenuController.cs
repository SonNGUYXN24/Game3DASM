using UnityEngine;

public class MenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingPanel;
    public GameObject generalPanel;
    public GameObject graphicsPanel;
    public GameObject audioPanel;
    public GameObject exitGamePanel;

    private void Start()
    {
        // Đảm bảo tất cả panel ẩn trừ settingPanel và generalPanel khi khởi động
        settingPanel.SetActive(false);
        generalPanel.SetActive(false);
        graphicsPanel.SetActive(false);
        audioPanel.SetActive(false);
        exitGamePanel.SetActive(false);
    }

    private void Update()
    {
        // Nhấn ESC để đóng settingPanel và các panel con
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSettingPanel();
        }
    }

    public void ShowSetting()
    {
        settingPanel.SetActive(true);
        generalPanel.SetActive(true);
        graphicsPanel.SetActive(false);
        audioPanel.SetActive(false);
    }

    public void ShowGraphicsSetting()
    {
        generalPanel.SetActive(false);
        graphicsPanel.SetActive(true);
        audioPanel.SetActive(false);
    }

    public void ShowAudioSetting()
    {
        generalPanel.SetActive(false);
        graphicsPanel.SetActive(false);
        audioPanel.SetActive(true);
    }

    public void ShowGeneralSetting()
    {
        generalPanel.SetActive(true);
        graphicsPanel.SetActive(false);
        audioPanel.SetActive(false);
    }

    public void CloseSettingPanel()
    {
        settingPanel.SetActive(false);
        generalPanel.SetActive(false);
        graphicsPanel.SetActive(false);
        audioPanel.SetActive(false);
    }

    public void ShowExitGame()
    {
        exitGamePanel.SetActive(true);
    }

    public void ApplyExitGame()
    {
        // Thoát khỏi ứng dụng
        Application.Quit();
        Debug.Log("Game is exiting...");
    }

    public void DontExitGame()
    {
        exitGamePanel.SetActive(false);
    }
}
