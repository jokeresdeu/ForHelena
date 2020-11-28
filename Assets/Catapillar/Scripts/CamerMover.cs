using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerMover : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset;
    [Range(0, 1)]
    [SerializeField] private float _smoothnes;

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = new Vector3(_target.position.x + _offset.x, _target.position.y + _offset.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, newPos, _smoothnes);
    }

}