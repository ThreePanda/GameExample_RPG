using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : Singleton<GameManager>
{
    private CharacterStats _playerStats;
    private List<IEndGameObserver> _endGameObservers = new();

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
        if (_playerStats.CurrentHealth == 0)
        {
            NotifyObserver();
        }
    }

    public void RegisterPlayer(CharacterStats player)
    {
        _playerStats = player;
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
