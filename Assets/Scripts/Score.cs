using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    public static ScoreManager Instance { get; private set; }
    private int score = 0;
    public int Score
    {
        get { return score; }
    }

    private void Awake()
    {
      if (Instance == null)
      {
            Instance = this;
            DontDestroyOnLoad(gameObject);
      }
      else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Ajoute X quantité au score
    /// </summary>
    /// <param name="amount">Quantité à ajouter au score. Doit etre positive.</param>
    public void AddScore(int amount)
    {
        if (amount > 0) //Verifie que le score soit pas negatif
        {
            score += amount;
            Debug.Log("Score: " + score);
        }
    }


    //action requis pour ajouter un score
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            AddScore(10);
        }
    }
}
