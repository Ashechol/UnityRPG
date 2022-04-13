using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject heathBar;
    public Transform healthBarPoint;
    public bool alwaysVisible;
    public float visibleTime;
    private float timeLeft;
    private Image healthSlider;
    private Transform UIbar;
    private Transform cam;
    private EnemyStats stats;

    void Awake()
    {
        stats = GetComponent<EnemyStats>();
        stats.OnHealthBarUpdate += UpdateHealthBar;
    }

    void OnEnable()
    {
        cam = Camera.main.transform;

        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.name == "HealthBar Canvas")
            {
                UIbar = Instantiate(heathBar, canvas.transform).transform;
                healthSlider = UIbar.GetChild(0).GetComponent<Image>();
                UIbar.gameObject.SetActive(alwaysVisible);
            }
        }
    }

    // 角色移动是在update，为了防止血条闪烁，所以血条在LateUpdate中更新位置
    void LateUpdate()
    {
        if (UIbar != null)
        {
            UIbar.position = healthBarPoint.position;
            UIbar.forward = -cam.forward;
            if (timeLeft <= 0 && !alwaysVisible)
                UIbar.gameObject.SetActive(false);
            else if (timeLeft >= 0)
                timeLeft -= Time.deltaTime;
        }

    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        timeLeft = visibleTime;
        if (currentHealth == 0)
            Destroy(UIbar.gameObject);

        UIbar.gameObject.SetActive(true);
        float sliderPercent = (float)currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;
    }
}
