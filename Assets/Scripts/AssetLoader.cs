using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

public class AssetLoader : MonoBehaviour
{
    public static AssetLoader Instance { get; private set; }

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

    // Charge un asset de n'importe quel type (ex: GameObject, Texture2D, AudioClip, etc.)
    public void LoadAsset<T>(string address, Action<T> onSuccess = null, Action onFailure = null) where T : UnityEngine.Object
    {
        Addressables.LoadAssetAsync<T>(address).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                onSuccess?.Invoke(handle.Result);
            }
            else
            {
                Debug.LogError($"Echec du chargement de l'asset à l'adresse: {address}");
                onFailure?.Invoke();
            }
        };
    }
}