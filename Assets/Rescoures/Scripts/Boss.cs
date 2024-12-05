using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Health
{
    public NavMeshAgent navMeshAgent;
    public Transform target; // Mục tiêu (Player)
    public float detectionRadius = 20f; // Bán kính phát hiện mục tiêu
    public float attackRange = 5f; // Khoảng cách tấn công thông thường
    public float fireAttackRange = 10f; // Khoảng cách tấn công bằng lửa
    public float retreatRange = 2f; // Khoảng cách tối thiểu để giữ khoảng cách khi tấn công
    public float maxDistance = 50f; // Khoảng cách tối đa từ vị trí ban đầu
    public Animator animator;
    public float attackCooldown = 2f; // Thời gian hồi đòn tấn công thông thường
    public float fireAttackCooldown = 5f; // Thời gian hồi đòn tấn công bằng lửa
    public float damage = 20f; // Lượng sát thương thông thường
    public float fireDamage = 40f; // Sát thương đòn lửa
    private float lastAttackTime = 0f; // Lần tấn công cuối cùng
    private float lastFireAttackTime = 0f; // Lần tấn công lửa cuối cùng
    public DamageZone damageZone;

    private Vector3 originalPosition; // Vị trí ban đầu
    private BossState currentState = BossState.Normal; // Trạng thái hiện tại

    public ParticleSystem fireAttackParticles; // Hiệu ứng đòn lửa
    public AudioClip runSoundEffect;
    public AudioClip attackSoundEffect;
    public AudioClip fireAttackSoundEffect;
    public AudioClip dieSoundEffect;
    private AudioSource audioSource;

    public enum BossState
    {
        Normal,
        NormalAttack,
        FireAttack,
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

        // Kiểm tra nếu Boss đang trên NavMesh
        if (!navMeshAgent.isOnNavMesh)
        {
            Debug.LogError("Boss không nằm trên NavMesh.");
            enabled = false; // Tắt script để tránh lỗi
        }
    }

    private void Update()
    {
        if (currentState == BossState.Die || navMeshAgent == null || !navMeshAgent.isActiveAndEnabled)
        {
            return;
        }

        float distanceToTarget = Vector3.Distance(target.position, transform.position);
        float distanceToOriginal = Vector3.Distance(originalPosition, transform.position);

        if (distanceToTarget <= detectionRadius && distanceToOriginal <= maxDistance)
        {
            HandleMovementAndAttack(distanceToTarget);
        }
        else
        {
            if (distanceToOriginal > maxDistance)
            {
                navMeshAgent.SetDestination(originalPosition);
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
        Vector3 randomDirection = Random.insideUnitSphere * 10f;
        randomDirection += originalPosition;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            navMeshAgent.SetDestination(hit.position);
        }
    }

    private void HandleMovementAndAttack(float distanceToTarget)
    {
        if (distanceToTarget > fireAttackRange)
        {
            // Di chuyển đến gần mục tiêu
            navMeshAgent.SetDestination(target.position);
            animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
            ChangeState(BossState.Normal);
        }
        else if (distanceToTarget <= fireAttackRange && Time.time > lastFireAttackTime + fireAttackCooldown)
        {
            // Tấn công bằng lửa
            navMeshAgent.SetDestination(transform.position); // Dừng di chuyển
            ChangeState(BossState.FireAttack);
            lastFireAttackTime = Time.time;
        }
        else if (distanceToTarget <= attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            // Tấn công thông thường
            navMeshAgent.SetDestination(transform.position);
            ChangeState(BossState.NormalAttack);
            lastAttackTime = Time.time;
        }
        else if (distanceToTarget <= retreatRange)
        {
            // Rút lui nếu quá sát
            Vector3 retreatDirection = (transform.position - target.position).normalized;
            Vector3 retreatPosition = transform.position + retreatDirection * 2f;
            navMeshAgent.SetDestination(retreatPosition);
            animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
        }
    }

    private void ChangeState(BossState newState)
    {
        if (currentState == newState) return;

        switch (currentState)
        {
            case BossState.Normal:
                damageZone.EndAttack();
                break;
            case BossState.NormalAttack:
                break;
            case BossState.FireAttack:
                fireAttackParticles.Stop();
                break;
        }

        switch (newState)
        {
            case BossState.Normal:
                animator.SetFloat("Speed", 0f);
                break;
            case BossState.NormalAttack:
                animator.SetTrigger("NormalAttack");
                break;
            case BossState.FireAttack:
                animator.SetTrigger("FireAttack");
                fireAttackParticles.Play();
                PlaySound(fireAttackSoundEffect);
                break;
            case BossState.Die:
                navMeshAgent.enabled = false;
                animator.SetTrigger("Die");
                PlaySound(dieSoundEffect);
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
            ChangeState(BossState.Die);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Animation Events
    public void BossBeginAttack()
    {
        damageZone.BeginAttack();
        PlaySound(attackSoundEffect);
    }

    public void BossEndAttack()
    {
        damageZone.EndAttack();
    }

    public void BossOnAttackEnd()
    {
        ChangeState(BossState.Normal);
    }
}
