using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameProgressManager : MonoBehaviour
{
    public List<SceneDescriptor> NormalScenes;
    public SceneDescriptor ShopScene;
    public int ShopFrequency = 4;
    public int CurrentLevel;
    public static GameProgressManager Instance { get; set; }

    private void Awake()
    {
        if (Instance)
            Debug.LogError("GameProgressManager is already initialized");
        
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentLevel = 0;
    }

    public SceneDescriptor GetNextScene()
    {
        if ((CurrentLevel + 1) % ShopFrequency == 0)
        {
            return ShopScene;
        }

        var nextScenes = NormalScenes.Where(s => s.Scene != SceneManager.Instance.CurrentScene.Scene).ToList();
        
        return nextScenes[Random.Range(0, nextScenes.Count)];
    }
}
