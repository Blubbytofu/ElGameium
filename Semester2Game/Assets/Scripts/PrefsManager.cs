using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;
using TMPro;
using UnityEngine.UI;

public class PrefsManager : MonoBehaviour
{
    [SerializeField] private GameObject settings;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private PlayerCamera playerCamera;
    public bool settingsOpen { get; private set; }

    [SerializeField] private Slider xSensSlider;
    [SerializeField] private TextMeshProUGUI xSensText;

    [SerializeField] private Slider ySensSlider;
    [SerializeField] private TextMeshProUGUI ySensText;

    [SerializeField] private Slider FOVSlider;
    [SerializeField] private TextMeshProUGUI FOVText;

    [SerializeField] private Slider WFOVSlider;
    [SerializeField] private TextMeshProUGUI WFOVText;

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
        if (playerInventory.isDead || playerInventory.wonLevel)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.P))
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
