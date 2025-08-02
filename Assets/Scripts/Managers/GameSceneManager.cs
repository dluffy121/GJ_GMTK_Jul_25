using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public bool IsSceneReLoading = false;

    private void OnEnable()
    {
        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (scene.name == "MainMenu")
        {
            GameManager.OnMainMenuSceneUnloaded?.Invoke();
        }
        if (scene.name == "Gameplay")
        {
            GameManager.OnGameplaySceneUnloaded?.Invoke();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        IsSceneReLoading = false;
        if (scene.name == "Gameplay")
        {
            GameManager.OnGameplaySceneLoaded?.Invoke();
        }
        if (scene.name == "MainMenu")
        {
            GameManager.OnMainMenuSceneLoaded?.Invoke();
        }
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadNextScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ReloadScene()
    {
        IsSceneReLoading = true;
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}
