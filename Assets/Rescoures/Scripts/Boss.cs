using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public enum BossState { Normal, Move, NormalAttack, FireAttack, Die }
    public BossState currentState = BossState.Normal;

    public float maxHP = 100000f;
    private float currentHP;
    public Slider healthBar;
    public Transform player;
    public float detectRange = 50f; // Phạm vi phát hiện Player
    public float attackRange = 20f; // Khoảng cách để tấn công Player
    public float moveSpeed = 5f; // Tốc độ di chuyển
    public float fireAttackCooldown = 5f;
    private float fireAttackTimer = 0f;

    public Animator animator;
    public AudioClip normalAttackSound;
    public AudioClip fireAttackSound;
    public AudioClip moveSound;
    public AudioClip dieSound;

    public ParticleSystem fireAttackParticles;
    public ParticleSystem normalAttackParticles; // Hiệu ứng tấn công thường
    public Collider normalAttackCollider;
    public Collider fireAttackCollider;
    public DamageZone damageZone;

    public NavMeshAgent navMeshAgent;
    public float wanderRadius = 10f;
    public float wanderDelay = 5f;
    private float lastWanderTime = 0f;
    private bool isPlayerDetected = false;

    private void Start()
    {
        currentHP = maxHP;
        healthBar.maxValue = maxHP;
        healthBar.value = currentHP;
        healthBar.gameObject.SetActive(false);

        fireAttackParticles.Stop();
        normalAttackParticles.Stop();
        normalAttackCollider.enabled = false;
        fireAttackCollider.enabled = false;

        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent chưa được gắn.");
            return;
        }
    }

    private void Update()
    {
        if (currentState == BossState.Die) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        isPlayerDetected = distanceToPlayer <= detectRange;

        if (isPlayerDetected)
        {
            healthBar.gameObject.SetActive(true);
            if (distanceToPlayer <= attackRange)
            {
                // Ưu tiên trạng thái FireAttack nếu cooldown đã sẵn sàng
                if (fireAttackTimer >= fireAttackCooldown && currentState != BossState.FireAttack)
                {
                    TransitionToState(BossState.FireAttack);
                }
                else if (currentState != BossState.FireAttack) // Chỉ chuyển sang NormalAttack nếu không phải FireAttack
                {
                    TransitionToState(BossState.NormalAttack);
                }
            }
            else
            {
                TransitionToState(BossState.Move);
            }
        }
        else
        {
            TransitionToState(BossState.Move);
        }

        fireAttackTimer += Time.deltaTime;

        switch (currentState)
        {
            case BossState.Normal:
                HandleNormalState();
                break;
            case BossState.Move:
                HandleMoveState();
                break;
            case BossState.NormalAttack:
                HandleNormalAttackState();
                break;
            case BossState.FireAttack:
                HandleFireAttackState();
                break;
            case BossState.Die:
                HandleDieState();
                break;
        }
    }

    private void HandleNormalState()
    {
        animator.SetFloat("Speed", 0);
        fireAttackParticles.Stop();
        normalAttackParticles.Stop();
        normalAttackCollider.enabled = false;
        fireAttackCollider.enabled = false;
    }

    private void HandleMoveState()
    {
        animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
        fireAttackParticles.Stop();
        normalAttackParticles.Stop();
        normalAttackCollider.enabled = false;
        fireAttackCollider.enabled = false;

        if (isPlayerDetected)
        {
            navMeshAgent.SetDestination(player.position);
        }
        else
        {
            Wander();
        }
    }

    private void Wander()
    {
        if (Time.time > lastWanderTime + wanderDelay)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            {
                navMeshAgent.SetDestination(hit.position);
            }

            lastWanderTime = Time.time;
        }
    }

    private void HandleNormalAttackState()
    {
        animator.SetTrigger("NormalAttack");
        normalAttackCollider.enabled = true;
        fireAttackCollider.enabled = false;

        // Bật hiệu ứng tấn công thường
        normalAttackParticles.Play();

        // Phát âm thanh tấn công thường
        PlaySound(normalAttackSound);

        // Tắt hiệu ứng sau 1 giây
        Invoke(nameof(StopNormalAttackEffect), 1f);
    }

    private void HandleFireAttackState()
    {
        animator.SetTrigger("FireAttack");
        normalAttackCollider.enabled = false;
        fireAttackCollider.enabled = true;

        Vector3 direction = (player.position - transform.position).normalized;
        fireAttackParticles.transform.forward = direction;
        fireAttackCollider.transform.forward = direction;

        // Bật hiệu ứng tấn công lửa
        fireAttackParticles.Play();

        // Phát âm thanh tấn công lửa
        PlaySound(fireAttackSound);

        fireAttackTimer = 0f;

        // Tắt hiệu ứng sau 3 giây
        Invoke(nameof(StopFireAttackEffect), 3f);

        // Đảm bảo trạng thái không bị gián đoạn
        Invoke(nameof(ResetToNormalState), 3.1f);
    }

    private void HandleDieState()
    {
        animator.SetTrigger("Die");
        fireAttackParticles.Stop();
        normalAttackParticles.Stop();
        normalAttackCollider.enabled = false;
        fireAttackCollider.enabled = false;
    }

    private void TransitionToState(BossState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
    }

    public void TakeDamage(float damage)
    {
        if (currentState == BossState.Die) return;

        currentHP -= damage;
        healthBar.value = currentHP;

        if (currentHP <= 0)
        {
            TransitionToState(BossState.Die);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, transform.position);
    }

    private void StopNormalAttackEffect()
    {
        normalAttackParticles.Stop();
        normalAttackCollider.enabled = false;
    }

    private void StopFireAttackEffect()
    {
        fireAttackParticles.Stop();
        fireAttackCollider.enabled = false;
    }

    private void ResetToNormalState()
    {
        TransitionToState(BossState.Normal);
    }

    // Animation Events
    public void BossBeginAttack()
    {
        if (damageZone != null)
        {
            damageZone.BeginAttack();
        }
    }

    public void BossEndAttack()
    {
        if (damageZone != null)
        {
            damageZone.EndAttack();
        }
    }

    public void BossOnAttackEnd()
    {
        TransitionToState(BossState.Normal);
    }
}
