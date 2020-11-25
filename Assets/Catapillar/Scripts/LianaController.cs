using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LianaController : EnemyBase
{

    [SerializeField] private Transform _centrePoint;
    [SerializeField] private float _angerRange;
    [SerializeField] private LayerMask _player;
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _attackRadius;
    
    private bool _isAtacking;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        StartCoroutine(ScanForPlayer());
    }
    protected virtual void OnTriggerEnter2D(Collider2D info)
    {
        Movement_Controller _player = info.GetComponent<Movement_Controller>();
        if (_player == null)
            return;

        Push(_player, false);

        DisableEnemy();
    }

    IEnumerator ScanForPlayer()
    {
        yield return new WaitForSeconds(1);
        CheckPlayerInRange();
    }

    private void OnEndAtack()
    {
        _enemyAnimator.SetBool("Attack", false);
        _isAtacking = false;
        StartCoroutine(ScanForPlayer());
    }

    private void OnAtack()
    {
        Collider2D coll = Physics2D.OverlapCircle(_attackPoint.position, _attackRadius, _player);
        if(coll!=null)
        {
            Movement_Controller player = coll.GetComponent<Movement_Controller>();
            if(player!=null)
            {
                Push(player, true);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_centrePoint.position, _angerRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_attackPoint.position, _attackRadius);
    }

    private void CheckPlayerInRange()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, _angerRange, _player);

        if (collider != null)
        {
            Movement_Controller player = collider.GetComponent<Movement_Controller>();
            if (player != null)
            {
                if (player.transform.position.x > transform.position.x && !_faceRight || player.transform.position.x < transform.position.x && _faceRight)
                    Flip();
                _isAtacking = true;
                _enemyAnimator.SetBool("Attack", true);
                return;
            }
        }
        else
        {
            StartCoroutine(ScanForPlayer());
        }
    }
}
