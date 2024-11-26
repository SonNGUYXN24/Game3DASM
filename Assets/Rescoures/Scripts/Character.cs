using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Character : Health
{
    public CharacterController characterController;
    public float speed = 2f;
    public float rotationSpeed = 10f; // Tốc độ xoay hướng của nhân vật
    public Vector3 movementVelocity;
    public PlayerInput playerInput;
    public DamageZone damageZone;
    public Animator animator;

    public GameObject sword;
    public GameObject shield;

    public Camera mainCamera; // Thêm camera chính để làm tham chiếu cho hướng xoay
    public Slider healthBar;
    public Slider MPBar;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI mpText;
    public MP mp;
    public ParticleSystem attackHitFX;
    public AudioSource audioSource;
    public AudioClip attackSound;
    

    public float healthRegenRate = 2f; // Lượng máu hồi mỗi giây
    public enum CharacterState
    {
        Normal,
        Attack,
        Die
    }
    public CharacterState currentState;

    void Start()
    {
        currentHP = maxHP;
        UpdateHealthUI();
        UpdateMP();
        attackHitFX.Stop(); // Dừng Particle System khi bắt đầu

        // Bắt đầu quá trình hồi máu
        StartCoroutine(HealthRegenRoutine());
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case CharacterState.Normal:
                CalculateMovement();
                break;
            case CharacterState.Attack:
                break;
            case CharacterState.Die:
                return;
        }

        characterController.Move(movementVelocity * Time.fixedDeltaTime);
    }

    void CalculateMovement()
    {
        if (playerInput.attackInput)
        {
            ChangeState(CharacterState.Attack);
            animator.SetFloat("Speed", 0);
            BeginAttack();
            return;
        }

        // Tính toán vận tốc di chuyển
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        movementVelocity = forward * playerInput.verticalInput + right * playerInput.horizontalInput;
        movementVelocity.Normalize();
        movementVelocity *= speed;

        animator.SetFloat("Speed", movementVelocity.magnitude);

        // Xoay hướng nhân vật theo hướng camera khi di chuyển về phía trước
        if (movementVelocity != Vector3.zero && playerInput.verticalInput > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void ChangeState(CharacterState newState)
    {
        playerInput.attackInput = false;

        switch (currentState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attack:
                EndAttack();
                break;
            case CharacterState.Die:
                return;
        }

        switch (newState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attack:
                animator.SetTrigger("Attack");
                attackHitFX.Play(); // Bắt đầu phát Particle System khi tấn công
                audioSource.PlayOneShot(attackSound);
                break;
            case CharacterState.Die:
                sword.transform.SetParent(null);
                shield.transform.SetParent(null);
                animator.SetTrigger("Die");
                movementVelocity = Vector3.zero;
                characterController.enabled = false;
                StopCoroutine(HealthRegenRoutine()); // Dừng hồi máu khi nhân vật chết
                StartCoroutine(DestroyAfterDelay(5f));
                break;
        }

        currentState = newState;
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        UpdateHealthUI();
        if (currentHP <= 0)
        {
            ChangeState(CharacterState.Die);
        }
    }

    public void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHP / maxHP;
        }

        if (healthText != null)
        {
            healthText.text = $"{currentHP}/{maxHP}";
        }
    }

    public void UpdateMP()
    {
        if (MPBar != null)
        {
            MPBar.value = (float)mp.currentMP / mp.maxMP; // Chuyển đổi sang kiểu float để có tỷ lệ chính xác
        }
        if (mpText != null)
        {
            mpText.text = $"{mp.currentMP}/{mp.maxMP}";
        }
    }

    public void OnAttackEnd()
    {
        ChangeState(CharacterState.Normal);
        attackHitFX.Stop(); // Dừng Particle System khi kết thúc tấn công
    }

    public void BeginAttack()
    {
        damageZone.BeginAttack();
        attackHitFX.Play(); // Bắt đầu phát Particle System khi tấn công
        audioSource.PlayOneShot(attackSound);
    }

    public void EndAttack()
    {
        damageZone.EndAttack();
        attackHitFX.Stop(); // Dừng Particle System khi kết thúc tấn công
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    // Coroutine để hồi máu theo thời gian
    private IEnumerator HealthRegenRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f); // Chờ 1 giây
            if (currentHP < maxHP && currentState != CharacterState.Die)
            {
                currentHP += healthRegenRate;
                currentHP = Mathf.Clamp(currentHP, 0, maxHP); // Đảm bảo máu không vượt quá maxHP
                UpdateHealthUI();
            }
        }
    }


    public void OnTriggerEnter(Collider other){
        if(gameObject.CompareTag("Blood")){

        }
    }
}
