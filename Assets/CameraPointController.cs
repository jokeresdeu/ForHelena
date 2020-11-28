using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPointController : MonoBehaviour
{
    [SerializeField] private Transform _player;
    private Vector2 _destenation;
    private bool _crawlingMove;
    private bool _needToMove;
    private float _speed;
    
    void Start()
    {
        transform.position = _player.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (_crawlingMove)
        {
            if (_needToMove)
                transform.position += new Vector3(_speed, 0, 0);
            else return;
            if (Vector2.Distance(transform.position, _destenation) <= _speed)
                _needToMove = false;
        }
        else if(!_needToMove)
            transform.position = _player.position;
    }

    public void CrawlMove(Vector2 destination, float time)
    {
        _destenation = destination;
        float distance = destination.x - transform.position.x;
        _speed = distance / time * Time.deltaTime * 0.8f;
        _crawlingMove = true;
        _needToMove = true;
    }

    public void EndCrawlMove()
    {
        _crawlingMove = false;
    }
}
