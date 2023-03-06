using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;
using TMPro;
using UnityEngine.UI;

public class PrefsManager : MonoBehaviour
{
    public GameObject settings;
    public PlayerInventory playerInventory;
    public PlayerCamera playerCamera;
    public bool settingsOpen;

    public Slider xSensSlider;
    public TextMeshProUGUI xSensText;

    public Slider ySensSlider;
    public TextMeshProUGUI ySensText;

    public Slider FOVSlider;
    public TextMeshProUGUI FOVText;

    public Slider WFOVSlider;
    public TextMeshProUGUI WFOVText;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("xSens"))
        {
            PlayerPrefs.SetFloat("xSens", 2);
        }

        if (!PlayerPrefs.HasKey("ySens"))
        {
            PlayerPrefs.SetFloat("ySens", 2);
        }

        if (!PlayerPrefs.HasKey("FOV"))
        {
            PlayerPrefs.SetFloat("FOV", 90);
        }

        if (!PlayerPrefs.HasKey("WFOV"))
        {
            PlayerPrefs.SetFloat("WFOV", 90);
        }

        LoadSettings();

        UpdateSliders();
        SliderLengths();
        settings.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !playerInventory.isDead)
        {
            settingsOpen = !settingsOpen;
            if (settingsOpen)
            {
                settings.SetActive(true);
                SliderLengths();
            }
            else
            {
                settings.SetActive(false);
            }
        }

        if (!settingsOpen && Time.timeScale != 1)
        {
            Time.timeScale = 1;
        }
        else if (settingsOpen && Time.timeScale != 0)
        {
            Time.timeScale = 0;
        }
    }

    public void HideSettings()
    {
        settingsOpen = false;
        settings.SetActive(false);
    }

    private void SliderLengths()
    {
        xSensSlider.value = PlayerPrefs.GetFloat("xSens");
        ySensSlider.value = PlayerPrefs.GetFloat("ySens");
        FOVSlider.value = PlayerPrefs.GetFloat("FOV");
        WFOVSlider.value = PlayerPrefs.GetFloat("WFOV");
    }

    private void UpdateSliders()
    {
        XSensSlider();
        YSensSlider();
        SliderFOV();
        SliderWFOV();
    }

    public void XSensSlider()
    {
        xSensText.text = xSensSlider.value.ToString("0.0");
    }

    public void YSensSlider()
    {
        ySensText.text = ySensSlider.value.ToString("0.0");
    }

    public void SliderFOV()
    {
        FOVText.text = FOVSlider.value.ToString("0");
    }

    public void SliderWFOV()
    {
        WFOVText.text = WFOVSlider.value.ToString("0");
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("xSens", xSensSlider.value);
        PlayerPrefs.SetFloat("ySens", ySensSlider.value);
        PlayerPrefs.SetFloat("FOV", FOVSlider.value);
        PlayerPrefs.SetFloat("WFOV", WFOVSlider.value);
        PlayerPrefs.Save();
        LoadSettings();
    }

    private void LoadSettings()
    {
        xSensSlider.value = PlayerPrefs.GetFloat("xSens");
        ySensSlider.value = PlayerPrefs.GetFloat("ySens");
        FOVSlider.value = PlayerPrefs.GetFloat("FOV");
        WFOVSlider.value = PlayerPrefs.GetFloat("WFOV");

        playerCamera.SetXSens(PlayerPrefs.GetFloat("xSens"));
        playerCamera.SetYSens(PlayerPrefs.GetFloat("ySens"));
        playerCamera.SetFOV(PlayerPrefs.GetFloat("FOV"));
        playerCamera.SetSecondaryFOV(PlayerPrefs.GetFloat("WFOV"));
    }
}
