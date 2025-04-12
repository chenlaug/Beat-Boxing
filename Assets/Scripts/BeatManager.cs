using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _bpm;
    [SerializeField] private Interval[] _intervals;

    private void Update()
    {
        foreach (Interval interval in _intervals)
        {
            float sampleTime = (_audioSource.timeSamples /
                                (_audioSource.clip.frequency * interval.GetIntervalLength(_bpm)));
            interval.CheckForNewinterval(sampleTime);

        }
    }
}

[System.Serializable]
public class Interval
{
    [SerializeField] private float _steps;
    [SerializeField] private UnityEvent _Trigger;
    private int _lastInterval;

    public float GetIntervalLength(float bmp)
    {
        return 60f / (bmp * _steps);
    }

    public void CheckForNewinterval (float interval)
    {
        if (Mathf.FloorToInt(interval) != _lastInterval)
        {
            _lastInterval = Mathf.FloorToInt(interval);
            _Trigger.Invoke();
        }
    }
}
