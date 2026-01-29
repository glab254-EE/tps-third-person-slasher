using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Set-up")]
    [SerializeField] private EnemyHealth enemyHealth;
    [Header("Settings")]
    [field: SerializeField] private AttackSettings attackSetting;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float updateInterval = 0.3f;
    [SerializeField] private float rotationSpeed = 8f;

    private NavMeshAgent _agent;
    private Transform _player;
    private Animator _animator;
    private bool _isActive = false;
    private bool _isAttacking = false;
    private bool _isPlayerInTrigger = false;
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        StartCoroutine(MainCoroutine());
    }
    public void Activate(Transform target)
    {
        _player = target;
        _isActive = true;
    }
    IEnumerator MainCoroutine()
    {
        while (true)
        {
            if (enemyHealth.Health <= 0)
            {
                Destroy(gameObject);
                yield break;
            }
            if (_isActive && _player != null && !_isPlayerInTrigger && !_isAttacking && _agent.isActiveAndEnabled)
            {
                _animator.SetBool("EnemyWalk", true);
                _agent.SetDestination(_player.position);
            }
            yield return new WaitForSeconds(updateInterval);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _isActive)
        {
            _isPlayerInTrigger = true;

            if (!_isAttacking)
            {
                _animator.SetBool("EnemyWalk", false);
                _animator.SetTrigger("StayAnimForEnemy");
                StartCoroutine(AttackSequence());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInTrigger = false;
        }
    }

    IEnumerator AttackSequence()
    {
        _isAttacking = true;
        _agent.isStopped = true;

        bool originalUpdateRotation = _agent.updateRotation;

        if (enemyHealth.Health <= 0) yield break;

        _agent.updateRotation = false;

        _animator.SetTrigger("EnemyAtack");
        yield return new WaitForSeconds(attackSetting.AttackWindupTime);

        Vector3 hitboxOrigin = transform.position;

        hitboxOrigin += transform.forward * attackSetting.HitboxOffset.z;
        hitboxOrigin += transform.right * attackSetting.HitboxOffset.x;
        hitboxOrigin += transform.up * attackSetting.HitboxOffset.y;

        bool haveHitPlayer = StatcHitboxCreator.TryHitWithBoxHitbox(hitboxOrigin, attackSetting.HitboxSize, playerMask, attackSetting.Damage, true, transform.rotation);

        if (haveHitPlayer)
        {
            Debug.Log("Hit");
        }

        yield return new WaitForSeconds(attackSetting.Duration);
        _animator.SetTrigger("StayAnimForEnemy");

        if (_player != null)
        {
            Vector3 direction = (_player.position - transform.position).normalized;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                while (Quaternion.Angle(transform.rotation, targetRotation) > 5f)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    yield return null;
                }
                transform.rotation = targetRotation;
            }
        }

        if (_isPlayerInTrigger && _player != null)
        {
            yield return new WaitForSeconds(attackSetting.Cooldown);
            _isAttacking = false;
            StartCoroutine(AttackSequence());
        }
        else
        {
            _isAttacking = false;
            _agent.updateRotation = true;
            _agent.isStopped = false;
        }
    }
}