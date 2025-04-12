using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : MonoBehaviour
{
    [SerializeField] private float _pulseSize = 1.15f;
    [SerializeField] private float _returnSpeed = 5f;
    private Vector3 _startingScale;

    private void Start()
    {
        _startingScale = transform.localScale;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _startingScale, Time.deltaTime * _returnSpeed);
    }

    public void PulseOnBeat()
    {
        transform.localScale = _startingScale * _pulseSize;
    }
}