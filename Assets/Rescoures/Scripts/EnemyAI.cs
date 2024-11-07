using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public Transform target;
    public float radius = 100f; //bán kính tìm kiếm mục tiêu
    public Vector3 originalPosition; //Vij tris ban ddaauf
    public float maxDistance = 50f;


    private void Start()
    {
        originalPosition = transform.position;
    }


    void Update()
    {
        //khoảng cách từ vị trí hiện tại đến vị trí ban đầu
        var distanceToOriginal = Vector3.Distance(originalPosition, transform.position);
        //khoảng cách từ vị trí hiện tại đến mục tiêu
        var distance = Vector3.Distance(target.position, transform.position);
        if (distance <= radius && distanceToOriginal <= maxDistance)
        {
            //di chuyển đến mục tiêu
            navMeshAgent.SetDestination(target.position);
        }
        if (distance > radius || distanceToOriginal > maxDistance)
        {
            //di chuyển về vị trí ban đầu
            navMeshAgent.SetDestination(originalPosition);
        }
    }
}
