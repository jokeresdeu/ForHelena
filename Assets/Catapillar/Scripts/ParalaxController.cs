using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxController: MonoBehaviour
{
    private Transform _cameraTransfrom;
    private Vector3 _lastCameraPos;
    [SerializeField] private float _xMovementParam;
    [SerializeField] private float _yMovementParam;
    // Start is called before the first frame update
    void Start()
    {
        _cameraTransfrom = Camera.main.transform;
        _lastCameraPos = _cameraTransfrom.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 deltaMovement = _cameraTransfrom.position - _lastCameraPos;
        transform.position = transform.position + new Vector3(_xMovementParam * deltaMovement.x, _yMovementParam * deltaMovement.y, 0);
        _lastCameraPos = _cameraTransfrom.position;
    }
}
