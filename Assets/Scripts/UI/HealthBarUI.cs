using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPrefab;
    public Transform barPoint;
    public bool alwaysVisible;
    public float visibleTime;
    
    private Image _healthSlider;
    private Transform _UIBar;
    private Transform _cam;
    private CharacterStats _characterStats;
    private float _timeLeft;

    private void OnEnable()
    {
        //引用形式
        _cam = Camera.main.transform;
        //背包、任务等UI
        foreach (Canvas canvas in FindObjectsByType<Canvas>(FindObjectsSortMode.None))
        {
            _UIBar = Instantiate(healthUIPrefab, canvas.transform).transform;
            _healthSlider = _UIBar.GetChild(0).GetComponent<Image>();
            _UIBar.gameObject.SetActive(alwaysVisible);
        }
    }

    private void Awake()
    {
        _characterStats = GetComponent<CharacterStats>();
        _characterStats.UpdataHealthOnAttack += UpdateHealthBar;
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0)
            Destroy(_UIBar.gameObject);
        
        _UIBar.gameObject.SetActive(true);
        
        _timeLeft = visibleTime; 
        float sliderPercent = (float)currentHealth / maxHealth;
        _healthSlider.fillAmount = sliderPercent;
    }

    private void LateUpdate()
    {
        if (_UIBar == null)return;
        _UIBar.position = barPoint.position;
        _UIBar.forward = -_cam.forward;
        
        if (alwaysVisible) return;
        if (_timeLeft <= 0)
            _UIBar.gameObject.SetActive(false);
        else
            _timeLeft -= Time.deltaTime;
    }
}
