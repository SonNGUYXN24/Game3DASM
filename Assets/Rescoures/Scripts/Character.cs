using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public CharacterController characterController; // Khai báo Component
    public float speed = 2f; // Vận tốc chuyển động
    public Vector3 movementVelocity;  // Vector vận tốc chuyển động
    public PlayerInput playerInput; // Khai báo component

    public Animator animator;

    //state machine
    public enum CharacterState
    {
        Normal, 
        Attack
    }
    public CharacterState currentState;


    // Update is called once per frame
    void FixedUpdate()
    {
        switch (currentState)
        {
            case CharacterState.Normal:
                CalculateMovement(); // Call CalculateMovement in FixedUpdate
                break;
            case CharacterState.Attack:
                break;
        }
        
        characterController.Move(movementVelocity * Time.fixedDeltaTime); // Apply movement
    }

    void CalculateMovement()
    {
        if (playerInput.attackInput)
        {
            ChangeState(CharacterState.Attack);
            animator.SetFloat("Speed", 0);
            return;
        }

        movementVelocity.Set(playerInput.horizontalInput, 0, playerInput.verticalInput);
        movementVelocity.Normalize();
        movementVelocity = Quaternion.Euler(0, -45, 0) * movementVelocity;
        movementVelocity *= speed;
        animator.SetFloat("Speed", movementVelocity.magnitude);
        if(movementVelocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(movementVelocity);
        }
    }

    //chuyển đổi trạng thái
    private void ChangeState(CharacterState newState)
    {
        //xoá cache
        playerInput.attackInput = false;

        //exit currentSate
        switch (currentState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attack:
                break;
        }

        //enter new state
        switch (newState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attack:
                animator.SetTrigger("Attack");
                break;
        }
        //update current state
        currentState = newState;
    }
    public void OnAttackEnd()
    {
        ChangeState(CharacterState.Normal);
    }
}
