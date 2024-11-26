using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestItem : MonoBehaviour
{
    public string questItemName;  //Tên của nhiệm vụ
    public int questTargetAmount; //Số lượng cần tìm
    public int currentAmount;  //
    public string targetItemTag; //Tag của các Item cần tìm
    public bool isComplete ;
}
