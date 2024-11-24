using System.Collections;
using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameObject npcPanel; //tham chiếu đến panel
    public string[] content; //nội dung của npc
    public TextMeshProUGUI npcTextContent; //Tham chiếu đến text content

    public Coroutine coroutine;


    //Nhiệm vụ NPC
    public QuestItem questItem;

    public PlayerQuests playerQuests;

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
            for(int i = 0; i < line.Length; i++)
            {
                npcTextContent.text += line[i];
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    public void SkipContent()
    {
        StopCoroutine(coroutine);
        //hiện nút qua nhiệm vụ
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerQuests = other.gameObject.GetComponent<PlayerQuests>();
            npcPanel.SetActive(true);
            coroutine = StartCoroutine(ReadContent());
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            npcPanel.SetActive(false);
            StopCoroutine(coroutine);
        }
    }

    public void TakeQuest()
    {
        if(playerQuests != null)
        {
            playerQuests.TakeQuest(questItem);
        }
    }

    
}

