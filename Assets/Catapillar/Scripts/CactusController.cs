using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusController : EnemyBase
{
    [SerializeField] private Collider2D _atackCollider;
    [SerializeField] private Transform _centrePoint;
    [SerializeField] private float _angerRange;
    [SerializeField] private LayerMask _player;

    private bool _isAtacking;
    private bool _playerDamaged;

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

        if(_isAtacking)
        {
            if(!_playerDamaged)
            {
                _playerDamaged = true;
            }

            Push(_player, _playerDamaged);
            return;
        }
        else if(_player.CentrePoint.y > _centrePoint.position.y)
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
        _enemyAnimator.SetBool("Atack", false);
        _isAtacking = false;
        _playerDamaged = false;
        _atackCollider.enabled = false;
        CheckPlayerInRange();
    }

    private void OnAtack()
    {
        _atackCollider.enabled = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _angerRange);
    }

    private bool CheckPlayerInRange()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, _angerRange, _player);

        if (collider != null)
        {
            Movement_Controller player = collider.GetComponent<Movement_Controller>();
            _isAtacking = true;
            _enemyAnimator.SetBool("Atack", true);
            return true;
        }
        else
        {
            StartCoroutine(ScanForPlayer());
        }
            
        return false;
    }
}
