using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] protected int _maxHp;
    protected int _currentHp;

    [SerializeField] private float _speed;
    private bool _canAttack = true;
    private bool _kill;
    protected Rigidbody2D _enemyRb;
    protected Animator _enemyAnimator;
    protected bool _needToMove;
    protected Vector2 _startPoint;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        _currentHp = _maxHp;
        _enemyAnimator = GetComponent<Animator>();
        _enemyRb = GetComponent<Rigidbody2D>();
        _startPoint = transform.position;
        StartCoroutine(AttackPlayer());
    }

    public void StartToMoveDown()
    {
        _enemyRb.velocity = Vector2.down * _speed * 2;
        _enemyAnimator.SetBool("Move", true);
    }

    public void StartToMoveUp()
    {
        if(_enemyAnimator.GetBool("Kill"))
        {
            _speed /= 2;
        }
        _enemyRb.velocity = Vector2.up * _speed;
    }

    public void EndMove()
    {
        _enemyRb.velocity = Vector2.zero;
        if (_enemyAnimator.GetBool("Kill"))
            return;
        _enemyAnimator.SetBool("Move", false);
        _canAttack = true;
        StartCoroutine(AttackPlayer());
    }

    private void Update()
    {
        if (_enemyRb.velocity.y > 0 && Vector2.Distance(_startPoint, transform.position) < 0.2f)
            EndMove();
    }

    IEnumerator AttackPlayer()
    {
        yield return new WaitForSeconds(Random.Range(0, 5));
        StartToMoveDown();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_canAttack)
            return;
        Movement_Controller player = collision.GetComponent<Movement_Controller>();
        _canAttack = false;
        _enemyAnimator.SetBool("Attack", true);
        _enemyRb.velocity = Vector2.zero;
        if(player!=null)
        {
            player.DeathFromSpider();
            _kill = true;
            _enemyAnimator.SetBool("Kill", true);
        }
    }       

    private void OnEndAttack()
    {
        _enemyAnimator.SetBool("Attack", false);
        StartToMoveUp();
    }






}
