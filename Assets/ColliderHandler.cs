using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderHandler : MonoBehaviour
{
    MushroomController _mushroom;
    private void Start()
    {
        _mushroom = GetComponentInParent<MushroomController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(_mushroom.Player == null)
            _mushroom.Player = collision.collider.GetComponent<Movement_Controller>();
    }

    private void OnCollisionExit(Collision collision)
    {
        _mushroom.Player = null;
    }
}
