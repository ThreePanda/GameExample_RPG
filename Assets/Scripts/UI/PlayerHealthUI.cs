using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    private TextMeshProUGUI _levelText;
    private Image _healthSlider;
    private Image _expSlider;

    private void Awake()
    {
        //todo:不易扩展，后期Obeject层级变更后需要修改此处代码
        _levelText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        _healthSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        _expSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
    }

    private void Update()
    {
        //ToString(format)可以转换表示的形式
        _levelText.text = "Level  " + GameManager.Instance.playerStats.characterData.currentLevel.ToString("00");
        UpdateHealth();
        UpdateExp();
    }

    private void UpdateHealth()
    {
        float sliderPercent =
            (float)GameManager.Instance.playerStats.CurrentHealth 
            / GameManager.Instance.playerStats.MaxHealth;
        _healthSlider.fillAmount = sliderPercent;
    }

    private void UpdateExp()
    {
        float sliderPercent =
            (float)GameManager.Instance.playerStats.characterData.currentExp 
            / GameManager.Instance.playerStats.characterData.baseExp;
        _expSlider.fillAmount = sliderPercent;
    }
}
