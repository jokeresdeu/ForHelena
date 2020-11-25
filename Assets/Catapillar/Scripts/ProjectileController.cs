using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private int damage;

    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyBase enemy = collision.GetComponent<EnemyBase>();
        if (enemy != null)
            enemy.TakeDamage(damage);

        _animator.SetBool("Hit", true);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    private void OnHit()
    {
        Destroy(gameObject);
    }
}
