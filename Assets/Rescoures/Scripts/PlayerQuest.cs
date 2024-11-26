using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerQuest : MonoBehaviour
{
    public QuestItem currentQuest; // Nhiệm vụ hiện tại
    public TextMeshProUGUI questDescriptionText; // Hiển thị nhiệm vụ
    public TextMeshProUGUI progressText; // Hiển thị tiến độ
    public TextMeshProUGUI timerText; // Hiển thị thời gian còn lại
    public TextMeshProUGUI notificationText; // Hiển thị thông báo nhận nhiệm vụ
    public GameObject healthPotionPrefab; // Prefab bình máu
    public Transform rewardSpawnPoint; // Vị trí sinh phần thưởng
    public string rewardTag; // Tag phần thưởng
    public TextMeshProUGUI potionCountText; // TextMeshPro để hiển thị số lượng bình máu
    private int potionCount = 0; // Số lượng bình máu hiện tại

    private float questDuration = 600f; // Thời gian nhiệm vụ (10 phút)
    private float remainingTime;

    private bool isQuestActive = false;

    void Update()
    {
        if (isQuestActive)
        {
            remainingTime -= Time.deltaTime;

            // Cập nhật hiển thị thời gian
            UpdateTimerUI();

            // Kiểm tra nếu hết thời gian
            if (remainingTime <= 0)
            {
                EndQuest(false); // Kết thúc nhiệm vụ thất bại
            }

            // Cập nhật tiến độ
            UpdateProgressUI();
        }
    }

    public void TakeQuest(QuestItem quest)
    {
        if (isQuestActive)
        {
            Debug.Log("Bạn đang thực hiện một nhiệm vụ!");
            return;
        }

        currentQuest = quest;
        remainingTime = questDuration;
        isQuestActive = true;

        // Hiển thị nhiệm vụ
        UpdateQuestUI();

        // Hiển thị thông báo nhận nhiệm vụ
        StartCoroutine(ShowNotification("Đã nhận nhiệm vụ!", 2f));
    }

    private void UpdateQuestUI()
    {
        if (currentQuest != null)
        {
            questDescriptionText.text = $"Nhiệm vụ: {currentQuest.questItemName}";
            UpdateProgressUI(); // Hiển thị tiến độ
            UpdateTimerUI(); // Hiển thị thời gian
        }
    }


    private void UpdateProgressUI()
    {
        if (currentQuest != null && progressText != null)
        {
            progressText.text = $"{currentQuest.currentAmount}/{currentQuest.questTargetAmount}";
        }
    }


    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = $"Thời gian còn lại: {minutes:00}:{seconds:00}";
    }

    public void CollectItem(string itemTag)
    {
        if (isQuestActive && currentQuest != null && currentQuest.targetItemTag == itemTag)
        {
            currentQuest.currentAmount++;
            UpdateProgressUI(); // Cập nhật giao diện tiến độ

            if (currentQuest.currentAmount >= currentQuest.questTargetAmount)
            {
                EndQuest(true); // Kết thúc nhiệm vụ thành công
            }
        }
    }


    private void EndQuest(bool success)
    {
        isQuestActive = false;

        if (success)
        {
            Debug.Log("Nhiệm vụ hoàn thành!");
            questDescriptionText.text = "Nhiệm vụ hoàn thành!";
            // Tạo phần thưởng
            GameObject reward = Instantiate(healthPotionPrefab, rewardSpawnPoint.position, Quaternion.identity);
            reward.tag = rewardTag; // Gán tag cho phần thưởng
        }
        else
        {
            Debug.Log("Nhiệm vụ thất bại!");
            questDescriptionText.text = "Nhiệm vụ thất bại!";
        }

        // Reset nhiệm vụ
        currentQuest = null;
        progressText.text = "";
        timerText.text = "";
    }

    private IEnumerator ShowNotification(string message, float duration)
    {
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        notificationText.gameObject.SetActive(false);
    }

    

    
}
