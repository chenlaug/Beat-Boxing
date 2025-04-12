using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ScUI : MonoBehaviour
{

    //Necessite Score.CS
    [SerializeField] private TextMeshProUGUI scoreText;
    void Start()
    {
        UpdateScoreText();
    }


    void Update()
    {
        UpdateScoreText();
    }
    /// <summary>
    /// Update du Text UI
    /// </summary>
    private void UpdateScoreText()
    {
        if (ScoreManager.Instance != null)
        {
            scoreText.text = "Score: " + ScoreManager.Instance.Score.ToString();
        }
    }
}
