using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    public GameObject tabPanel; // Canvas chính
    public GameObject panelInfo; // Panel Info
    public GameObject panelInventory; // Panel Inventory
    public GameObject panelQuest; // Panel Quest
    public AudioSource clickSoundEffect; // Hiệu ứng âm thanh khi nhấn nút

    private void Start()
    {
        // Ban đầu ẩn canvas và hiện panel Info
        tabPanel.SetActive(false);
        ShowInfo();
        clickSoundEffect.Stop();
    }

    private void Update()
    {
        // Hiện/Ẩn canvas khi nhấn phím "Tab"
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool isActive = tabPanel.activeSelf;
            tabPanel.SetActive(!isActive);

            if (!isActive)
            {
                ShowInfo(); // Hiện panel Info mặc định khi canvas được bật
            }
        }
    }

    public void ShowInfo()
    {
        clickSoundEffect.Play(); // Phát âm thanh click
        panelInfo.SetActive(true);
        panelInventory.SetActive(false);
        panelQuest.SetActive(false);

    }

    public void ShowInventory()
    {
        clickSoundEffect.Play(); // Phát âm thanh click
        panelInfo.SetActive(false);
        panelInventory.SetActive(true);
        panelQuest.SetActive(false);

    }

    public void ShowQuest()
    {
        clickSoundEffect.Play(); // Phát âm thanh click
        panelInfo.SetActive(false);
        panelInventory.SetActive(false);
        panelQuest.SetActive(true);

    }

    public void ShowQuit()
    {
        clickSoundEffect.Play(); // Phát âm thanh click
        panelInfo.SetActive(false);
        panelInventory.SetActive(false);
        panelQuest.SetActive(false);
        tabPanel.SetActive(false);
    }
}
