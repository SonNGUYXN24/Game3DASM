using UnityEngine;
using TMPro;

public class ItemController : MonoBehaviour
{
    [Header("Item Settings")]
    public string itemTag; // Tag của vật phẩm
    public GameObject bloodPrefab; // Prefab bình máu

    [Header("UI Elements")]
    public TextMeshProUGUI itemCountText; // Text để hiển thị số lượng vật phẩm
    public GameObject bloodButton; // Nút bình máu

    [Header("Player Settings")]
    public Character playerCharacter; // Đối tượng nhân vật (tham chiếu đến class Character)

    private int itemCount = 0; // Số lượng vật phẩm thu thập được

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(itemTag))
        {
            itemCount++; // Tăng số lượng vật phẩm
            UpdateItemCountText(); // Cập nhật text hiển thị
            Destroy(other.gameObject); // Xóa vật phẩm
        }
    }

    public void UseBloodItem()
    {
        if (itemCount > 0)
        {
            itemCount--; // Giảm số lượng vật phẩm
            playerCharacter.currentHP += 20; // Tăng máu cho nhân vật
            UpdateItemCountText(); // Cập nhật text hiển thị
        }
    }

    public void Update(){
        UpdateBloodButtonState();
    }

    private void UpdateItemCountText()
    {
        itemCountText.text = $"{itemCount}";
    }

    private void UpdateBloodButtonState()
    {
        if(itemCount > 0){
            bloodButton.SetActive(true);
        }else{
            bloodButton.SetActive(false);
        }
        
    }
}
