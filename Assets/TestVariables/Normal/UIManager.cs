using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Singleton

    public static UIManager instance { get; private set; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    #endregion

    public Slider healthSlider;

    private void Start()
    {
        healthSlider.value = 1;
    }

    public void SetHealthSliderValue(float value)
    {
        healthSlider.value = value;
    }
}
