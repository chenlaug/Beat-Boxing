using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct NoteData
{
    public string NoteType;
    public float TriggerTime;
    public float Duration;
    public float Frequency;
    public float Amplitude;

    public NoteData(string noteType, float triggerTime, float duration = 0f, float frequency = 0f, float amplitude = 0f)
    {
        NoteType = noteType;
        TriggerTime = triggerTime;
        Duration = duration;
        Frequency = frequency;
        Amplitude = amplitude;
    }
}

[RequireComponent(typeof(AudioSource))]
public class AudioAnalyzer : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource _m_AudioSource;

    [Header("Audio Settings")]
    [SerializeField] private float songBpm = 132f;
    [SerializeField] private float pitch = 1f;
    [SerializeField] private float firstBeatOffset = 0.2f;
    [SerializeField] private float preStartDelay = 0.3f;
    [SerializeField] private float noteTravelTime = 1f; // Temps de déplacement des notes

    [Header("Frequency Settings")]
    [SerializeField] private float _redFreqThreshold = 0.5f;
    [SerializeField] private float _yellowMinFreq = 0.15f;
    [SerializeField] private float _yellowMaxFreq = 0.35f;
    [SerializeField] private float minNoteSpacing = 0.2f;
    [SerializeField] private float yellowMergeThreshold = 0.15f; // Seuil pour fusionner les notes jaunes


    [Header("Note Spawner")]
    [SerializeField] private NoteSpawner _noteSpawner;

    private float _secondsPerBeat;
    private float _songPosition;
    private double _dspSongStartTime;
    private List<NoteData> _noteChart;
    private int _noteCount = 0;

    private void Awake()
    {
        _secondsPerBeat = 60f / songBpm;
        _dspSongStartTime = AudioSettings.dspTime + preStartDelay;
        _noteChart = new List<NoteData>();
    }

    private async void Start()
    {
        _m_AudioSource.pitch = pitch;
        _noteSpawner.LoadNotePrefabs();
        await PreAnalyzeTrack();
        _m_AudioSource.PlayScheduled(_dspSongStartTime);
    }

    private void Update()
    {
        double currentDspTime = AudioSettings.dspTime;

        if (currentDspTime < _dspSongStartTime)
            return;

        _songPosition = (float)((currentDspTime - _dspSongStartTime) * pitch) - firstBeatOffset;

        while (_noteCount < _noteChart.Count &&
               _noteChart[_noteCount].TriggerTime - _songPosition <= noteTravelTime)
        {
            TriggerNote(_noteChart[_noteCount]);
            _noteCount++;
        }
    }

    private void TriggerNote(NoteData note)
    {
        _noteSpawner.SpawnNoteAtTime(note.NoteType, note.Duration, noteTravelTime);
    }

    private async Task PreAnalyzeTrack()
    {
        AudioClip clip = _m_AudioSource.clip;
        int sampleRate = clip.frequency;
        int channels = clip.channels;
        float clipLength = clip.length;

        float detectionInterval = _secondsPerBeat / 4f;
        int samplesPerInterval = Mathf.CeilToInt(sampleRate * detectionInterval);
        float[] samples = new float[samplesPerInterval * channels];

        float currentTime = 0f;
        string lastNote = "";
        float yellowNoteStart = 0f;
        float lastNoteTime = -999f;

        Debug.Log("Analyse des amplitudes en cours...");

        while (currentTime < clipLength)
        {
            int samplePosition = Mathf.FloorToInt(currentTime * sampleRate);
            int samplesToRead = Mathf.Min(samplesPerInterval, clip.samples - samplePosition);

            System.Array.Clear(samples, 0, samples.Length);
            clip.GetData(samples, samplePosition);

            float maxAmplitude = 0f;
            float avgAmplitude = 0f;

            for (int i = 0; i < samplesToRead * channels; i += channels)
            {
                float sum = 0f;
                for (int c = 0; c < channels; c++)
                    sum += Mathf.Abs(samples[i + c]);

                float amplitude = sum / channels;
                avgAmplitude += amplitude;
                if (amplitude > maxAmplitude)
                    maxAmplitude = amplitude;
            }

            avgAmplitude /= samplesToRead;
            float dominantFreq = GetDominantFrequency(samples, sampleRate, channels);
            string currentNote = "";

            if (maxAmplitude > _redFreqThreshold)
            {
                currentNote = "RedNote";

                bool overlapsYellow = _noteChart.Exists(n =>
                    n.NoteType == "YellowNote" &&
                    currentTime >= n.TriggerTime &&
                    currentTime <= n.TriggerTime + n.Duration);

                bool tooCloseToLast = currentTime - lastNoteTime < minNoteSpacing;

                if (!overlapsYellow && !tooCloseToLast)
                {
                    _noteChart.Add(new NoteData("RedNote", currentTime, 0f, dominantFreq, maxAmplitude));
                    lastNoteTime = currentTime;
                }
            }
            else if (avgAmplitude > _yellowMinFreq && avgAmplitude < _yellowMaxFreq)
            {
                currentNote = "YellowNote";
                if (lastNote != "YellowNote")
                    yellowNoteStart = currentTime;
            }

            if (lastNote == "YellowNote" && currentNote != "YellowNote")
            {
                float duration = currentTime - yellowNoteStart;
                float endTime = yellowNoteStart + duration;
                bool tooCloseToLast = yellowNoteStart - lastNoteTime < minNoteSpacing;

                // Récupérer la dernière note jaune
                int lastIndex = _noteChart.Count - 1;
                if (_noteChart.Count > 0 && _noteChart[lastIndex].NoteType == "YellowNote")
                {
                    NoteData lastYellow = _noteChart[lastIndex];

                    // Vérifie si la nouvelle note jaune est proche de la dernière
                    float lastEnd = lastYellow.TriggerTime + lastYellow.Duration;
                    if (yellowNoteStart - lastEnd < yellowMergeThreshold)
                    {
                        float newStart = lastYellow.TriggerTime;
                        float newDuration = Mathf.Max(endTime, lastEnd) - newStart;

                        _noteChart[lastIndex] = new NoteData("YellowNote", newStart, newDuration);
                    }
                    else if (!tooCloseToLast)
                    {
                        _noteChart.Add(new NoteData("YellowNote", yellowNoteStart, duration));
                        lastNoteTime = yellowNoteStart;
                    }
                }
                else if (!tooCloseToLast)
                {
                    _noteChart.Add(new NoteData("YellowNote", yellowNoteStart, duration));
                    lastNoteTime = yellowNoteStart;
                }

                yellowNoteStart = 0f;
            }


            lastNote = currentNote;
            currentTime += detectionInterval;
        }

        if (lastNote == "YellowNote" && yellowNoteStart > 0f)
        {
            float duration = clipLength - yellowNoteStart;
            float endTime = yellowNoteStart + duration;

            int lastIndex = _noteChart.Count - 1;
            if (_noteChart.Count > 0 && _noteChart[lastIndex].NoteType == "YellowNote")
            {
                NoteData lastYellow = _noteChart[lastIndex];
                float lastEnd = lastYellow.TriggerTime + lastYellow.Duration;

                if (yellowNoteStart - lastEnd < yellowMergeThreshold)
                {
                    float newStart = lastYellow.TriggerTime;
                    float newDuration = Mathf.Max(endTime, lastEnd) - newStart;
                    _noteChart[lastIndex] = new NoteData("YellowNote", newStart, newDuration);
                }
                else
                {
                    _noteChart.Add(new NoteData("YellowNote", yellowNoteStart, duration));
                }
            }
            else
            {
                _noteChart.Add(new NoteData("YellowNote", yellowNoteStart, duration));
            }
        }

        Debug.Log($"Analyse terminée. Total notes : {_noteChart.Count}");
        await Task.Yield();
    }


    private float GetDominantFrequency(float[] samples, int sampleRate, int channels)
    {
        int n = samples.Length;
        float max = 0f;
        int maxIndex = 0;

        for (int i = 0; i < n; i++)
        {
            float val = Mathf.Abs(samples[i]);
            if (val > max)
            {
                max = val;
                maxIndex = i;
            }
        }

        float freq = (maxIndex / (float)n) * (sampleRate / 2f);
        return freq;
    }
}