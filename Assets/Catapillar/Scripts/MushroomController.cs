using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomController : MonoBehaviour
{
    private Animator _animator;
    public Movement_Controller Player { get; set; }
    [SerializeField] private Transform[] _movePoints;
    [SerializeField] private Transform _centrePoint;
    private Collider2D _collider;
    private Vector3 _playerPos;
    float delta;
    private int _сurrentPoint;
    [SerializeField] private float _radius;
    [SerializeField] private float _pushPower;
    private bool _isPushing;
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _collider = _centrePoint.GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (_animator.GetBool("Push") || _isPushing || Player == null)
            return;
        if(IsPlayerInRange())
        {
            _isPushing = true;
            Player.CanMove = false;
            Invoke("StartPushing", 1f);
        }
    }

    private void StartPushing()
    {
        _playerPos = Player.transform.position;
        delta = _playerPos.y - _movePoints[0].position.y;
        Player.PlayerRb.bodyType = RigidbodyType2D.Static;
        _animator.SetBool("Push", true);
        _collider.enabled = false;
        _сurrentPoint = 0;
    }

    private bool IsPlayerInRange()
    {
        if (Player == null)
            return false;

        if (Vector2.Distance(Player.CentrePoint, _centrePoint.position) < _radius && Player.transform.position.y > _centrePoint.position.y)
            return true;

        return false;
    }

    private void Push()
    {
        _сurrentPoint = _movePoints.Length - 1;
      
        _animator.SetBool("Back", true);
        _animator.SetBool("Push", false);

        if (Player == null)
            return;
        Player.PlayerRb.bodyType = RigidbodyType2D.Dynamic;
        Player.PlayerRb.AddForce(Vector2.up * _pushPower);
        Player.CanMove = true;
    }

    private void PushUp()
    {
        if (_сurrentPoint < _movePoints.Length)
        {
            Player.transform.position = new Vector3(Player.transform.position.x, _movePoints[_сurrentPoint].position.y + delta, Player.transform.position.z); 
            _сurrentPoint++;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_centrePoint.position, _radius);
    }

    private void EndPush()
    {
        _collider.enabled = true;
        _isPushing = false;
    }
}
