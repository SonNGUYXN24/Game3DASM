using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class ItemController : MonoBehaviour
{
    [Header("Item Settings")]
    public List<string> itemTags; // Danh sách các tag của vật phẩm
    public GameObject bloodPrefab; // Prefab bình máu
    public GameObject manaPrefab; // Prefab bình mana

    [Header("UI Elements")]
    public TextMeshProUGUI bloodCountText; // Text để hiển thị số lượng bình máu
    public TextMeshProUGUI manaCountText;  // Text để hiển thị số lượng bình mana
    public TextMeshProUGUI insufficientItemText; // Text thông báo khi vật phẩm không đủ

    [Header("Player Settings")]
    public Character playerCharacter; // Đối tượng nhân vật (tham chiếu đến class Character)
    public MP mP;

    private int bloodItemCount = 0; // Số lượng bình máu
    private int manaItemCount = 0;  // Số lượng bình mana

    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra tag của vật phẩm
        if (itemTags.Contains(other.tag))
        {
            if (other.CompareTag("Blood"))
            {
                bloodItemCount++; // Tăng số lượng bình máu
            }
            else if (other.CompareTag("Mana"))
            {
                manaItemCount++; // Tăng số lượng bình mana
            }

            UpdateItemCountText(); // Cập nhật text hiển thị
            Destroy(other.gameObject); // Xóa vật phẩm
        }
    }

    public void UseBloodItem()
    {
        if (bloodItemCount > 0)
        {
            bloodItemCount--; // Giảm số lượng bình máu
            playerCharacter.currentHP += 20; // Tăng máu cho nhân vật
            UpdateItemCountText(); // Cập nhật text hiển thị
        }
        else
        {
            ShowInsufficientItemMessage();
        }
    }

    public void UseManaItem()
    {
        if (manaItemCount > 0)
        {
            manaItemCount--; // Giảm số lượng bình mana
            mP.currentMP += 100; // Tăng mana cho nhân vật
            UpdateItemCountText(); // Cập nhật text hiển thị
        }
        else
        {
            ShowInsufficientItemMessage();
        }
    }

    private void UpdateItemCountText()
    {
        // Cập nhật riêng text số lượng máu và mana
        bloodCountText.text = $"{bloodItemCount}";
        manaCountText.text = $"{manaItemCount}";
    }

    private void ShowInsufficientItemMessage()
    {
        if (insufficientItemText != null)
        {
            StartCoroutine(DisplayInsufficientItemMessage());
        }
    }

    public IEnumerator DisplayInsufficientItemMessage()
    {
        insufficientItemText.gameObject.SetActive(true);
        insufficientItemText.text = "Số lượng vật phẩm không đủ để sử dụng!";
        yield return new WaitForSeconds(2f);
        insufficientItemText.gameObject.SetActive(false);
    }
}
