using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Skeleton : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public Transform target; // muc tiêu

    public float radius = 10f; // bán kính tìm kiem muc tiêu
    public Vector3 originalePosition; // vi trí ban dau
    public float maxDistance = 50f; // khoang cách 

    public float maxHP;
    public float currentHp;


    public Animator animator; // khai báo component

    // state machine
    public enum CharacterState
    {
        Normal,
        Attack,
        Die
    }
    public CharacterState currentState; // trang thái hien tai


    void Start()
    {
        originalePosition = transform.position;
        navMeshAgent.SetDestination(target.position);
        currentHp = maxHP;
    }

    void Update()
    {
        if (currentState == CharacterState.Die)
        {
            return;
        }
        // kho?ng cách tu vu trí hien tai den vi trí ban dau
        var distanceToOriginal = Vector3.Distance(originalePosition, transform.position);
        // khoang cách tu vi trí hien tai den muc tiêu
        var distance = Vector3.Distance(target.position, transform.position);
        if (distance <= radius && distanceToOriginal <= maxDistance)
        {
            // di chuyen den muc tiêu
            navMeshAgent.SetDestination(target.position);
            animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);

            distance = Vector3.Distance(target.position, transform.position);
            if (distance < 2f)
            {
                // tan công
                ChangeState(CharacterState.Attack);
            }
        }

        if (distance > radius || distanceToOriginal > maxDistance)
        {
            // quay ve vi trí ban dau
            navMeshAgent.SetDestination(originalePosition);
            animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);

            // chuyen sang trang thái dung yên
            distance = Vector3.Distance(originalePosition, transform.position);
            if (distance < 1f)
            {
                animator.SetFloat("Speed", 0);
            }

            // bình thuong
            ChangeState(CharacterState.Normal);
        }
    }

    // chuyen  trang thái
    private void ChangeState(CharacterState newState)
    {
        // exit current state
        switch (currentState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attack:
                break;
        }

        // enter new state
        switch (newState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attack:
                animator.SetTrigger("Attack");
                break;

            case CharacterState.Die:
                animator.SetTrigger("Attack");
                Destroy(gameObject, 5f);
                break;
        }

        // update current state
        currentState = newState;


    }
    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Max(0, currentHp);
        if (currentHp <= 0)
        {
            ChangeState(CharacterState.Die);
        }
    }
}
