using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : Health
{
    public NavMeshAgent navMeshAgent;
    public Transform target; // Mục tiêu (Player)
    public float detectionRadius = 10f; // Bán kính phát hiện mục tiêu
    public float attackRange = 2.5f; // Khoảng cách tấn công
    public float retreatRange = 1.5f; // Khoảng cách tối thiểu để giữ khoảng cách khi tấn công
    public float maxDistance = 50f; // Khoảng cách tối đa từ vị trí ban đầu
    public Animator animator;
    public float attackCooldown = 2f; // Thời gian hồi đòn tấn công
    public float damage = 10f; // Lượng sát thương
    private float lastAttackTime = 0f; // Lần tấn công cuối cùng
    public DamageZone damageZone;

    private Vector3 originalPosition; // Vị trí ban đầu
    private CharacterState currentState = CharacterState.Normal; // Trạng thái hiện tại

    // Di chuyển ngẫu nhiên
    public float wanderRadius = 10f; // Bán kính lang thang
    public float wanderDelay = 5f; // Thời gian chờ trước khi chọn vị trí tiếp theo
    private float lastWanderTime = 0f;

    // Biến âm thanh
    public AudioClip runSoundEffect;
    public AudioClip attackSoundEffect;
    private AudioSource audioSource;
    public float maxSoundDistance = 50f; // Khoảng cách tối đa để nghe rõ âm thanh

    // Biến để sinh vật phẩm
    public GameObject itemPrefab;

    public enum CharacterState
    {
        Normal,
        Attack,
        Die
    }

    private void Start()
    {
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent chưa được gắn.");
            return;
        }

        if (target == null)
        {
            Debug.LogError("Target chưa được gắn.");
            return;
        }

        originalPosition = transform.position;
        currentHP = maxHP;

        // Thiết lập AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;

        // Kiểm tra nếu Enemy đang trên NavMesh
        if (!navMeshAgent.isOnNavMesh)
        {
            Debug.LogError("Enemy không nằm trên NavMesh.");
            enabled = false; // Tắt script để tránh lỗi
        }
    }

    private void Update()
    {
        if (currentState == CharacterState.Die || navMeshAgent == null || !navMeshAgent.isActiveAndEnabled)
        {
            return;
        }

        float distanceToTarget = Vector3.Distance(target.position, transform.position);
        float distanceToOriginal = Vector3.Distance(originalPosition, transform.position);

        AdjustSoundVolume(distanceToTarget);

        // Enemy ở trong phạm vi phát hiện
        if (distanceToTarget <= detectionRadius && distanceToOriginal <= maxDistance)
        {
            HandleMovementAndAttack(distanceToTarget);
        }
        else
        {
            // Nếu không phát hiện mục tiêu, lang thang ngẫu nhiên
            if (distanceToOriginal > maxDistance)
            {
                navMeshAgent.SetDestination(originalPosition); // Trở về vị trí ban đầu
            }
            else
            {
                Wander();
            }
            animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
        }
    }

    private void Wander()
    {
        if (Time.time > lastWanderTime + wanderDelay)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += originalPosition;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            {
                navMeshAgent.SetDestination(hit.position);
                PlayRunSound();
            }

            lastWanderTime = Time.time;
        }
    }

    private void HandleMovementAndAttack(float distanceToTarget)
    {
        if (distanceToTarget > attackRange)
        {
            // Di chuyển đến gần mục tiêu
            navMeshAgent.SetDestination(target.position);
            animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
            PlayRunSound();
            ChangeState(CharacterState.Normal);
        }
        else if (distanceToTarget <= attackRange && distanceToTarget > retreatRange)
        {
            // Tấn công nếu đủ gần nhưng không quá sát
            if (Time.time > lastAttackTime + attackCooldown)
            {
                navMeshAgent.SetDestination(transform.position); // Dừng di chuyển
                StopRunSound();
                ChangeState(CharacterState.Attack);
                lastAttackTime = Time.time;
            }
        }
        else if (distanceToTarget <= retreatRange)
        {
            // Rút lui nếu quá sát
            Vector3 retreatDirection = (transform.position - target.position).normalized;
            Vector3 retreatPosition = transform.position + retreatDirection * 2f; // Di chuyển ra xa 2 đơn vị
            navMeshAgent.SetDestination(retreatPosition);
            animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
            PlayRunSound();
        }
    }

    private void PlayRunSound()
    {
        if (!audioSource.isPlaying || audioSource.clip != runSoundEffect)
        {
            audioSource.clip = runSoundEffect;
            audioSource.Play();
        }
    }

    private void PlayAttackSound()
    {
        if (attackSoundEffect != null)
        {
            audioSource.PlayOneShot(attackSoundEffect);
        }
    }

    private void StopRunSound()
    {
        if (audioSource.isPlaying && audioSource.clip == runSoundEffect)
        {
            audioSource.Stop();
        }
    }

    private void AdjustSoundVolume(float distanceToTarget)
    {
        if (distanceToTarget <= maxSoundDistance)
        {
            audioSource.volume = 1f - (distanceToTarget / maxSoundDistance);
        }
        else
        {
            audioSource.volume = 0f;
        }
    }

    private void ChangeState(CharacterState newState)
    {
        if (currentState == newState) return;

        switch (currentState)
        {
            case CharacterState.Normal:
                damageZone.EndAttack();
                break;
            case CharacterState.Attack:
                break;
        }

        switch (newState)
        {
            case CharacterState.Normal:
                animator.SetFloat("Speed", 0f);
                break;
            case CharacterState.Attack:
                animator.SetTrigger("Attack");
                break;
            case CharacterState.Die:
                navMeshAgent.enabled = false;
                animator.SetTrigger("Die");
                SpawnItem();
                Destroy(gameObject, 5f);
                break;
        }

        currentState = newState;
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (currentHP <= 0)
        {
            ChangeState(CharacterState.Die);
        }
    }

    private void SpawnItem()
    {
        if (itemPrefab != null)
        {
            Instantiate(itemPrefab, transform.position, Quaternion.identity);
        }
    }

    // Phương thức sự kiện bắt đầu vùng sát thương khi animation tấn công bắt đầu
    public void DrBeginAttack()
    {
        damageZone.BeginAttack();
        PlayAttackSound(); // Phát âm thanh tấn công tại thời điểm này
    }

    // Phương thức sự kiện kết thúc vùng sát thương khi animation tấn công kết thúc
    public void DrEndAttack()
    {
        damageZone.EndAttack();
    }

    // Phương thức được gọi khi animation tấn công kết thúc
    public void OnAttackEnd()
    {
        ChangeState(CharacterState.Normal);
    }
}
