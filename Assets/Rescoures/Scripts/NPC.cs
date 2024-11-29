using System.Collections;
using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameObject npcPanel; // Tham chiếu đến panel
    public string[] content; // Nội dung của NPC
    public TextMeshProUGUI npcTextContent; // Tham chiếu đến Text Content
    public TextMeshProUGUI interactionText; // TextMeshPro để hiển thị hướng dẫn
    public AudioSource audioSource; // Âm thanh phát khi trò chuyện
    public AudioClip talkingClip; // Âm thanh khi nói chuyện
    public GameObject playerCanvas; // Canvas của người chơi
    public GameObject buttonAcceptQuest; // Nút nhận nhiệm vụ

    public QuestItem questItem;

    private Coroutine coroutine;
    private bool isTalking = false; // Trạng thái đang nói chuyện
    private PlayerQuest playerQuests;

    private void Start()
    {
        npcPanel.SetActive(false);
        npcTextContent.text = "";
        interactionText.text = "";
        buttonAcceptQuest.SetActive(false); // Nút nhận nhiệm vụ ẩn ban đầu
    }

    public IEnumerator ReadContent()
    {
        npcTextContent.text = "";
        audioSource.clip = talkingClip;
        audioSource.Play(); // Phát âm thanh khi bắt đầu nói chuyện

        foreach (var line in content)
        {
            foreach (char character in line)
            {
                npcTextContent.text += character;
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(1f); // Thời gian giữa các dòng
        }

        audioSource.Stop(); // Dừng âm thanh khi kết thúc nội dung
        buttonAcceptQuest.SetActive(true); // Hiện nút nhận nhiệm vụ khi kết thúc
    }

    public void SkipContent()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            npcTextContent.text = string.Join("\n", content); // Hiển thị toàn bộ nội dung ngay lập tức
            audioSource.Stop(); // Dừng âm thanh
            buttonAcceptQuest.SetActive(true); // Hiện nút nhận nhiệm vụ
        }
    }

    private void Update()
    {
        if (isTalking && Input.GetKeyDown(KeyCode.Space))
        {
            // Nhấn Space để kết thúc ngay lập tức
            SkipContent();
        }

        if (Input.GetKeyDown(KeyCode.E) && interactionText.text != "")
        {
            if (!isTalking)
            {
                StartConversation();
            }
            else
            {
                EndConversation();
            }
        }
    }

    private void StartConversation()
    {
        isTalking = true;
        npcPanel.SetActive(true);
        playerCanvas.SetActive(false); // Ẩn player canvas khi trò chuyện
        interactionText.text = "Use press \"E\" to stop talking!";
        coroutine = StartCoroutine(ReadContent());
    }

    private void EndConversation()
    {
        isTalking = false;
        npcPanel.SetActive(false);
        playerCanvas.SetActive(true); // Hiển thị lại player canvas khi kết thúc
        interactionText.text = "Use press \"E\" to talking!";
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        audioSource.Stop(); // Dừng âm thanh khi kết thúc trò chuyện
        buttonAcceptQuest.SetActive(true); // Hiện nút nhận nhiệm vụ
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerQuests = other.GetComponent<PlayerQuest>();
            interactionText.text = "Use press \"E\" to talking!";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactionText.text = ""; // Xóa hướng dẫn khi rời vùng
            if (isTalking)
            {
                EndConversation(); // Thoát cuộc trò chuyện nếu người chơi rời vùng
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
