/*using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("���������")]
    [SerializeField] private float updateInterval = 0.3f;
    [SerializeField] private float atackInterval = 3f;

    private NavMeshAgent _agent;
    private Transform _player;
    private bool _isActive = false;
    private bool _isAttacking = false;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        StartCoroutine(FollowRoutine());
    }

    public void Activate(Transform target)
    {
        _player = target;
        _isActive = true;
    }

    public void DeActivate(Transform target)
    {
        _isActive = false;
    }

    IEnumerator FollowRoutine()
    {
        while (true)
        {
            if (_isActive && _player != null && _agent.isActiveAndEnabled)
            {
                _agent.SetDestination(_player.position);
            }
            yield return new WaitForSeconds(updateInterval);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isAttacking)
        {
            _isAttacking = true;
            _agent.isStopped = true;
            Attack();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CancelInvoke("Attack");
            _isAttacking = false;
            _agent.isStopped = false;
        }
    }

    void Attack()
    {
        if (!_isAttacking) return;

        Debug.Log("��� ������!");

        Invoke("Attack", atackInterval);
    }
}*/

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
    [SerializeField] private float updateInterval = 0.3f;
    [SerializeField] private float attackDuration = 1.5f;
    [SerializeField] private float attackCooldown = 0.5f;
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
    void Update()
    {
        HandleAnimations();
    }
    public void Activate(Transform target)
    {
        _player = target;
        _isActive = true;
    }

    /*public void DeActivate(Transform target)
    {
        _isActive = false;
    }*/

    IEnumerator MainCoroutine()
    {
        while (true)
        {
            if (_isActive && _player != null && !_isPlayerInTrigger && !_isAttacking && _agent.isActiveAndEnabled)
            {
                _animator.SetBool("EnemyWalk", true);
                _agent.SetDestination(_player.position);
            }
            yield return new WaitForSeconds(updateInterval);
        }
    }
    void HandleAnimations()
    {
        if (_agent.velocity.magnitude > 0)
        {
            _animator.SetBool("Walking",true);
        } else
        {
            _animator.SetBool("Walking",false);
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
            //_animator.SetBool("EnemyWalk", true);
            _isPlayerInTrigger = false;
        }
    }

    IEnumerator AttackSequence()
    {
        _isAttacking = true;
        _agent.isStopped = true;

        bool originalUpdateRotation = _agent.updateRotation;
        _agent.updateRotation = false;

        _animator.SetTrigger("EnemyAtack");
        yield return new WaitForSeconds(attackDuration);
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

        yield return new WaitForSeconds(attackCooldown);

        if (_isPlayerInTrigger && _player != null)
        {
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