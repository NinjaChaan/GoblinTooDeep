using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }
    
    public SceneDescriptor MainMenuScene;

    [SceneName]
    public string GameplayEssentialsScene;
    
    public SceneDescriptor FirstRoom;
    public List<SceneDescriptor> RegularRooms;
    public List<SceneDescriptor> ShopRooms;

    public SceneDescriptor CurrentScene => _currentScene;

    private SceneDescriptor _currentScene;
    private bool _isLoadingScene = false;
    private bool _isGameplaySceneLoaded = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance)
        {
            Debug.LogError("SceneManager is already initialized");
            return;
        }
        Instance = this;
        LoadScene(MainMenuScene);
    }

    public static void LoadScene(SceneDescriptor sceneDescriptor)
    {
        Instance.StartCoroutine(Instance.LoadSceneCoroutine(sceneDescriptor));
    }
    
    IEnumerator LoadSceneCoroutine(SceneDescriptor sceneDescriptor)
    {
        if (sceneDescriptor == null)
        {
            Debug.LogError("SceneDescriptor is null");
            yield break;
        }
        if (string.IsNullOrEmpty(sceneDescriptor.Scene))
        {
            Debug.LogError("Scene name is null or empty");
            yield break;
        }
        while (_isLoadingScene)
        {
            Debug.LogWarning("Waiting for previous scene to load");
            yield return new WaitForSecondsRealtime(0.5f);
        }
        
        _isLoadingScene = true;
        if (_currentScene != null)
        {
            yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(_currentScene.Scene);
            _currentScene = null;
        }

        if (sceneDescriptor == MainMenuScene && _isGameplaySceneLoaded)
        {
            yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(GameplayEssentialsScene);
            _isGameplaySceneLoaded = false;
        } else if (sceneDescriptor != MainMenuScene && !_isGameplaySceneLoaded)
        {
            yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(GameplayEssentialsScene, LoadSceneMode.Additive);
            _isGameplaySceneLoaded = true;
        }
        
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneDescriptor.Scene, LoadSceneMode.Additive);
        UnityEngine.SceneManagement.SceneManager.SetActiveScene(
            UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneDescriptor.Scene));

        _currentScene = sceneDescriptor;
        
        _isLoadingScene = false;
    }
}
