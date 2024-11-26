using System.Collections;
using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameObject npcPanel; //tham chiếu đến panel
    public string[] content; //nội dung của npc
    public TextMeshProUGUI npcTextContent; //Tham chiếu đến text content

    private Coroutine coroutine;

    // Nhiệm vụ NPC
    public QuestItem questItem;

    private PlayerQuest playerQuests;

    private void Start()
    {
        npcPanel.SetActive(false);
        npcTextContent.text = "";
    }

    public IEnumerator ReadContent()
    {
        npcTextContent.text = "";
        foreach (var line in content)
        {
            foreach (char character in line)
            {
                npcTextContent.text += character;
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(1f); // Thêm độ trễ giữa các dòng để người chơi có thể đọc dễ dàng hơn
        }
    }

    public void SkipContent()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            npcTextContent.text = string.Join("\n", content); // Hiển thị toàn bộ nội dung ngay lập tức
        }
        // Hiện nút qua nhiệm vụ
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerQuests = other.GetComponent<PlayerQuest>();
            npcPanel.SetActive(true);
            coroutine = StartCoroutine(ReadContent());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            npcPanel.SetActive(false);
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }

    public void TakeQuest()
    {
        if (playerQuests != null && questItem != null)
        {
            playerQuests.TakeQuest(questItem);
            Debug.Log("Nhiệm vụ đã được giao cho người chơi.");
        }
    }
}
