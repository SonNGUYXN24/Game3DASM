using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject panelTutorial1; // Panel Tutorial 1
    public GameObject panelTutorial2; // Panel Tutorial 2
    public GameObject panelTutorial3; // Panel Tutorial 3

    public GameObject escController;

    private int currentPanelIndex = 0; // Panel hiện tại (0 = chưa mở, 1 = panel1, 2 = panel2, 3 = panel3)
    private GameObject[] panels; // Mảng chứa các panel

    void Start()
    {
        // Gán tất cả panel vào mảng để dễ quản lý
        panels = new GameObject[] { panelTutorial1, panelTutorial2, panelTutorial3 };

        // Đảm bảo tất cả panel đều tắt khi bắt đầu
        CloseAllPanels();
    }

    void Update()
    {
        // Nhấn Esc để thoát tutorial
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseAllPanels();
            escController.SetActive(true);
        }
    }

    public void OpenTutorial()
    {
        // Mở panel đầu tiên
        currentPanelIndex = 1; // Panel đầu tiên là 1
        UpdatePanels();
        escController.SetActive(false);
    }

    public void SlideToNextTutorial()
    {
        // Tăng chỉ số panel và xoay vòng
        currentPanelIndex = (currentPanelIndex % 3) + 1;
        UpdatePanels();
    }

    private void UpdatePanels()
    {
        // Cập nhật trạng thái hiển thị của các panel
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == currentPanelIndex - 1); // Bật panel tương ứng, tắt các panel khác
        }
    }

    private void CloseAllPanels()
    {
        // Đóng tất cả các panel và reset trạng thái
        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }
        currentPanelIndex = 0;
    }
}
