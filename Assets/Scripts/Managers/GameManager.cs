using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector] public CharacterStats playerStats;
    private List<IEndGameObserver> _endGameObservers = new();
    private CinemachineFreeLook _followCamera;
    protected override void Awake()
    {
        base.Awake();
        //避免类在新场景加载中被销毁，导致其后的流程无法进行
        DontDestroyOnLoad(this);
    }

    // private void OnEnable()
    // {
    //     Debug.Log("Game Manager OnEnable");
    // }
    // private void OnDisable()
    // {
    //     Debug.Log("Game Manager OnDisable");
    // }
    // void OnDestroy()
    // {
    //     Debug.Log("Game Manager OnDestroy");
    // }

    private void Update()
    {
        if (playerStats.CurrentHealth == 0)
        {
            NotifyObserver();
        }
    }

    public void RegisterPlayer(CharacterStats player)
    {
        playerStats = player;
        _followCamera = FindFirstObjectByType<CinemachineFreeLook>();
        if (_followCamera != null)
        {
            _followCamera.Follow = playerStats.transform.GetChild(2);
            _followCamera.LookAt = playerStats.transform.GetChild(2);
        }
    }

    public void AddObserver(IEndGameObserver observer)
    {
        _endGameObservers.Add(observer);
    }

    public void RemoveObserver(IEndGameObserver observer)
    {
        _endGameObservers.Remove(observer);
    }

    public void NotifyObserver()
    {
        foreach (var observer in _endGameObservers)
        {   
             observer.EndNotify();
        }
    }
}
