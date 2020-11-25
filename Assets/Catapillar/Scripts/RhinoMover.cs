using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RhinoMover : EnemyBase
{
    protected Rigidbody2D _enemyRb;
    private Collider2D _collider;

    [Header("Movement")]
    [SerializeField] private float _speed;
    [SerializeField] private float _range;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _whatIsGround;
    protected Vector2 _startPoint;

    private bool _canMove = true;
    
    protected override void Start()
    {
        base.Start();
        _startPoint = transform.position;
        _enemyRb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (transform.position.x - _startPoint.x > _range && _faceRight)
            Flip();
        else if (_startPoint.x - transform.position.x > _range && !_faceRight)
            Flip();
    }

    private void FixedUpdate()
    {
        if (IsGroundEnding())
            Flip();

        Move();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(_range * 2, 0.5f, 0));
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        Movement_Controller _player = collision.collider.GetComponent<Movement_Controller>();
        if (_player == null)
            return;

        if (_player.CentrePoint.y > transform.position.y)
        {
            DisableEnemy();
            Push(_player, false);
        }
        else
        {
            Push(_player, true);
        }
    }

    protected override void Hurt()
    {
        base.Hurt();
        _canMove = false;
        _enemyRb.velocity = Vector3.zero;

    }

    protected override void EndHurt()
    {
        base.EndHurt();
        _canMove = true;
    }

    protected override void DisableEnemy()
    {
        base.DisableEnemy();
        _enemyRb.velocity = Vector2.zero;
        _enemyRb.bodyType = RigidbodyType2D.Static;
        _collider.enabled = false;
    }

    private void Move()
    {
        if(_canMove)
            _enemyRb.velocity = transform.right * new Vector2(_speed, _enemyRb.velocity.y);
    }

    private bool IsGroundEnding()
    {
        return !Physics2D.OverlapPoint(_groundCheck.position, _whatIsGround);
    }
}
