using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private float _pushForse;

    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Movement_Controller player = collision.GetComponent<Movement_Controller>();
        if (player != null)
        {
            player.Push();
            Vector2 pushDir = GetComponent<Rigidbody2D>().velocity.normalized;
            pushDir.y = 0.3f;
            player.PlayerRb.AddForce(pushDir * _pushForse, ForceMode2D.Impulse);
        }

        _animator.SetBool("Hit", true);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    private void OnHit()
    {
        Destroy(gameObject);
    }
}
