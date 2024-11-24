using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerQuests : MonoBehaviour
{
    public List<QuestItem> questItems = new List<QuestItem>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Nhận nhiệm vụ
    public void TakeQuest(QuestItem questItem)
    {
        //Kiểm tra có nhiệm vụ đó chưa
        var check = questItems.FirstOrDefault(x => x.questItemName == questItem.questItemName);
        questItems.Add(questItem);
    }

}
