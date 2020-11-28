using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Movement_Controller : MonoBehaviour
{
    [SerializeField] private CameraPointController _cameraPoint;

    [Header("HorizontalMove")]
    [SerializeField] private Transform _movePoint;
    [SerializeField] private Transform _locatorPoint;
    [SerializeField] private Vector2 _locatorArea;
    [SerializeField] private AnimationClip _crawlAnim;
    [SerializeField] private float _speed;
    [SerializeField] private bool _crawl;
    private bool _faceRight = true;
    private float _move;
    private bool _canMove;

    [Header("Jump")]
    [SerializeField] private float _jumpForse;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private LayerMask _whatIsGround;
    private bool _jump;
    private bool _grounded;

    [Header("Bored")]
    [SerializeField] private float _boredTimeOut;
    [SerializeField] private bool _randomBoredTime;

    [Header("Climb")]
    [SerializeField] private Transform _cellCheck;
    [SerializeField] private Transform _climbPoint;
    [SerializeField] private float _cellCheckRadius;
    [SerializeField] private Transform _aboveChecker;
    [SerializeField] private LayerMask _onWhatCanClimb;
    private bool _celled;
    private Vector3 _fallPoint;
    private bool _canClimb;
    private bool _upSideDowm;
    private bool _pushedInReverse;

    [Header("Attack")]
    [SerializeField] private GameObject _projectile;
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private Transform _shootPoint;

    [Header("Hp")]
    [SerializeField] private int _maxHp;
    private int _currentHp;

    private float _lastActionTime;
    private bool _pushed;
    private float _pushTime;

    private Animator _playerAnimator;
    private Rigidbody2D _playerRB;
    public Rigidbody2D PlayerRb => _playerRB;

    public Vector3 CentrePoint => _groundCheck.position;
    public bool CanMove { 
        set
        {
            _canMove = value;
        }
    }

    private ActionType _currentAction = ActionType.Awake;
    
    private void Start()
    {
        _playerRB = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
        _lastActionTime = Time.time;
        _currentHp = _maxHp;

        if (_randomBoredTime)
            _boredTimeOut = Random.Range(3f, 15f);
        CanMove = true;
    }

    private void Update()
    {
        if (!_canMove || _currentAction == ActionType.Awake)
            return;

        _move = Input.GetAxisRaw("Horizontal");

        if (_move != 0)
        {
            Debug.Log(_grounded);
            if(_crawl && _grounded)
                TryToAction(ActionType.Move);
            else
            {
                if ((_move > 0 && !_faceRight) || (_move < 0 && _faceRight))
                {
                    TryToAction(ActionType.Flip);
                }
                else if (_currentAction != ActionType.Walk)
                    TryToAction(ActionType.Walk);
            }

            TryToAction(ActionType.Move);
        }

        if (_currentAction == ActionType.Walk && _move == 0)
            EndAction();

        if (Input.GetKeyUp (KeyCode.Space))
            _jump = true;

        if (Time.time - _lastActionTime > _boredTimeOut && _currentAction == ActionType.None)
            TryToAction(ActionType.Bored);

        if (Input.GetKeyUp(KeyCode.E))
            CheckIfCanClimb();


        if (Input.GetButtonUp("Fire1"))
            TryToAction(ActionType.Shoot);
    }

    private void FixedUpdate()
    {
        _grounded = Physics2D.OverlapBox(_groundCheck.position, new Vector2(_groundCheckRadius, 0.2f), 0, _whatIsGround);
        if (_pushed && _grounded && Time.time - _pushTime > 1f)
        {
            EndAction();
            _playerRB.velocity = Vector2.zero;
        }

        if(_upSideDowm)
            _celled = Physics2D.OverlapBox(_groundCheck.position, new Vector2(_groundCheckRadius, 0.2f), _onWhatCanClimb);

        if (_upSideDowm && !_celled)
            StartFall();

        if (!_canMove)
            return;

        if (_jump && _grounded && _currentAction != ActionType.Walk)
        {
            if (_upSideDowm)
                TryToAction(ActionType.Fall);
            else
                TryToAction(ActionType.Jump);
        }
        _jump = false;

        if (_currentAction == ActionType.Walk && _grounded && _crawl)
            EndAction();
        else if(_currentAction == ActionType.Walk && _grounded && !_crawl)
            _playerRB.velocity = new Vector2(_move * _speed * Time.fixedDeltaTime, _playerRB.velocity.y);
        else if(_currentAction == ActionType.Walk)
        {
            _playerRB.velocity = new Vector2(_move * _speed * Time.fixedDeltaTime * 0.6f, _playerRB.velocity.y);
        }
            
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_upSideDowm || _currentAction != ActionType.Fall)
            return;
        if (collision.collider.tag != "Ground")
            _pushedInReverse = true;
        EndAction();
    }

    public void Push()
    {
        TryToAction(ActionType.Hurt);
    }

    private void CheckIfCanClimb()
    {
        bool _canClimb = Physics2D.OverlapCircle(_cellCheck.position, _cellCheckRadius, _onWhatCanClimb);
        if (_canClimb)
        {
          TryToAction(ActionType.TryClimb);
        }
        else 
        TryToAction(ActionType.Refuse);
    }

    private void TakeDamage()
    {
        _currentHp -= 1;
        if (_currentHp <= 0)
        {
            _currentAction = ActionType.None;
            ResetAnimations();
            OnDeath();
        }
            
    }

    public void OnDeath()
    {
        TryToAction(ActionType.Death);
    }

    public void TryToAction(ActionType action)
    {
        if (!CanTransition(action))
          return;

        if (_playerAnimator.GetBool(action.ToString()))
            return;

        if (action != ActionType.Bored)
        {
            _playerAnimator.SetBool(ActionType.Bored.ToString(), false);
        }

        if (action != ActionType.None)
           _lastActionTime = Time.time;

        switch (action)
        {
            case ActionType.Move:
                if ((_move > 0 && !_faceRight) || (_move < 0 && _faceRight))
                {
                    TryToAction(ActionType.Flip);
                    return;
                }
                if(!CanCrawl())
                {
                    TryToAction(ActionType.Refuse);
                    return;
                }   
                _cameraPoint.CrawlMove(_movePoint.position, _crawlAnim.length);
                _canMove = false;
                if (!_grounded)
                    return;
                break;
            case ActionType.Bored:
                if (_randomBoredTime)
                    _boredTimeOut = Random.Range(3f, 15f);
                break;
            case ActionType.Death:
                _canMove = false;
                GetComponent<Collider2D>().enabled = false;
                _playerRB.bodyType = RigidbodyType2D.Static;
                break;
            case ActionType.Hurt:
                _pushed = true;
                _canMove = false;
                _pushTime = Time.time;
                break;
            case ActionType.Flip:
            case ActionType.Jump:
            case ActionType.Shoot:
            case ActionType.Climb:
                break;
            case ActionType.Fall:
                StartFall();
                break;
        }
        _currentAction = action;
        _playerAnimator.SetBool(_currentAction.ToString(), true);
    }

    private bool CanTransition(ActionType action)
    {
        if (_currentAction == ActionType.Death)
            return false;

        switch(action)
        {
            case ActionType.Death:
            case ActionType.Hurt:
            case ActionType.None:
                return true;
            case ActionType.Bored:
                if (_currentAction == ActionType.None)
                    return true;
                break;
            case ActionType.TryClimb:
                if (_currentAction == ActionType.Move || _currentAction == ActionType.None || _currentAction == ActionType.Bored || _currentAction == ActionType.Walk || _currentAction == ActionType.Jump && !_upSideDowm)
                    return true;
                break;
            case ActionType.Shoot:
            case ActionType.Jump:
            case ActionType.Flip:
            case ActionType.Refuse:
                if (_currentAction == ActionType.Move || _currentAction == ActionType.None || _currentAction == ActionType.Bored || _currentAction == ActionType.Jump)
                    return true;
                break;                
            case ActionType.Move:
            case ActionType.Walk:
                if (_currentAction == ActionType.None || _currentAction == ActionType.Bored)
                    return true;
                break;
            case ActionType.Climb:
            case ActionType.ClimbFailed:
                if (_currentAction == ActionType.TryClimb)
                    return true;
                break;
            case ActionType.Fall:
                if (_upSideDowm)
                    return true;
                break;
        }
        return false;
    }

    private void EndAction()
    {
        switch (_currentAction)
        {
            case ActionType.Move:
                Move();
                _canMove = true;
                _cameraPoint.EndCrawlMove();
                break;
            case ActionType.Flip:
                Flip();
                break;
            case ActionType.TryClimb:
                TryToClimb();
                return;
            case ActionType.Climb:
                Climb();
                break;
            case ActionType.Death:
                Destroy(gameObject);
                break;
            case ActionType.Hurt:
                _canMove = true;
                _pushed = false;
                TakeDamage();
                break;
            case ActionType.Fall:
                EndFall();
                break;
        }
        if (_currentAction == ActionType.Death)
            return;
        _currentAction = ActionType.None;
        ResetAnimations();
    }

    private void ResetAnimations()
    {
        for(int i = 1; i< Enum.GetValues(typeof(ActionType)).Length; i++)
        {
            ActionType type = (ActionType)i;
            _playerAnimator.SetBool(type.ToString(), false);
        }
    }

    private void DoAction()
    {
        switch (_currentAction)
        {
            case ActionType.Move:
            case ActionType.Flip:
            case ActionType.Bored:
                break;
            case ActionType.Jump:
                Jump();
                break;
            case ActionType.Shoot:
                Shoot();
                break;
            case ActionType.Fall:
                break;
        }
    }

    private void Flip()
    {
        _faceRight = !_faceRight;
        transform.Rotate(0, 180, 0);
        Move();
    }

    public void DeathFromSpider()
    {
        Destroy(gameObject);
    }

    public void DisablePlayer()
    {
        gameObject.SetActive(false);
    }

    private bool CanCrawl()
    {
        return Physics2D.OverlapBox(_locatorPoint.position, _locatorArea, 0) == null;
    }
            

    private void Move()
    {
        Vector2 newPosition = _movePoint.position;
        newPosition.y = transform.position.y;
        transform.position = _movePoint.position;
    }

    private void Jump()
    {
        _playerRB.AddForce(new Vector2(_move * 0.4f, 1) * _jumpForse);
    }

    private void TryToClimb()
    {
        _canClimb = Physics2D.OverlapCircle(_cellCheck.position, _cellCheckRadius, _onWhatCanClimb);
        if (_canClimb)
            TryToAction(ActionType.Climb);
        else TryToAction(ActionType.ClimbFailed);
    }

    private void Climb()
    {
         _playerRB.gravityScale = _playerRB.gravityScale * -1;
        transform.position = _climbPoint.position;
        FlipUpDown();
        _upSideDowm = true;
        
    }

    private void StartFall()
    {
        _fallPoint = transform.position;
        _playerRB.gravityScale = _playerRB.gravityScale * -1;
        _upSideDowm = false;
    }

    private void EndFall()
    {
        if (_pushedInReverse)
            _fallPoint.y += (_fallPoint.y - transform.position.y);
        FlipUpDown();
        transform.position = _fallPoint;
    }

    private void FlipUpDown()
    {
        transform.Rotate(180, 0, 0);
    }

    private void Shoot()
    {
        GameObject projectile = Instantiate(_projectile, _shootPoint.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody2D>().velocity = transform.right * _projectileSpeed;
        projectile.GetComponent<SpriteRenderer>().flipX = !_faceRight;
        Destroy(projectile, 5f);
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(_groundCheck.position, new Vector2(_groundCheckRadius, 0.2f));
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_cellCheck.position, _cellCheckRadius);
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(_locatorPoint.position, _locatorArea);
    }
}

public enum ActionType
{
    None,
    Bored, 
    Move, 
    Flip, 
    Jump, 
    Shoot, 
    TryClimb, 
    Climb, 
    ClimbFailed,
    Walk,
    Refuse,
    Fail,
    Death,
    Hurt,
    Awake,
    Fall,
}
