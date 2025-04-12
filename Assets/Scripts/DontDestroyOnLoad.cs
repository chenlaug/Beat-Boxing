using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    [Header("Dont Destroy On Load")]
    [SerializeField] private GameObject[] gameObjects;

    private void Awake()
    {
        foreach (GameObject element in gameObjects) 
        {
            if (element.transform.parent != null)
            {
                element.transform.SetParent(null);
            }
            DontDestroyOnLoad(element);
        }
    }
}
