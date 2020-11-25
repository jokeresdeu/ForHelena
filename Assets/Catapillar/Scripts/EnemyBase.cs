using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] protected int _maxHp;
    protected int _currentHp;

    [SerializeField] protected float _pushForse;

    protected Animator _enemyAnimator;
    protected bool _faceRight = true;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        _currentHp = _maxHp;
        _enemyAnimator = GetComponent<Animator>();
    }

    public virtual void TakeDamage(int damage)
    {
        Debug.Log(_currentHp);
        _currentHp -= damage;
        Debug.Log(_currentHp);
        if (_currentHp <= 0)
        {
            DisableEnemy();
        }
        else Hurt();
    }

    protected virtual void Hurt()
    {
        _enemyAnimator.SetBool("Hurt", true);
    }

    protected virtual void EndHurt()
    {
        _enemyAnimator.SetBool("Hurt", false);
    }

    public virtual void OnDeath()
    {
        Destroy(gameObject);
    }

    protected virtual void DisableEnemy()
    {
        _enemyAnimator.SetBool("Death", true);
        GetComponent<Collider2D>().enabled = false;
    }

    protected void Flip()
    {
        _faceRight = !_faceRight;
        transform.Rotate(0, 180, 0);
    }

    protected virtual void Push(Movement_Controller player, bool playerDamaged)
    {
        player.PlayerRb.velocity = Vector2.zero;
        Vector2 pushDirection = (player.transform.position - transform.position).normalized;
        pushDirection.y = 0.5f;
        if (playerDamaged)
        {
            player.Push();
            //pushDirection.x *= -1;
        }   
            
        player.PlayerRb.AddForce(pushDirection * _pushForse, ForceMode2D.Impulse);
    }
}
