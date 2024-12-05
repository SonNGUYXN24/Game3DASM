using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettings : MonoBehaviour
{
    [Header("Resolution Settings")]
    [SerializeField] public TMP_Dropdown resolutionDropdown; // Thay private bằng public để kéo thả trong Inspector
    private readonly Resolution[] resolutions = new Resolution[]
    {
        new Resolution(1920, 1080),
        new Resolution(1600, 900),
        new Resolution(1366, 768),
        new Resolution(1280, 720),
        new Resolution(1024, 576)
    };

    [Header("Frame Rate Settings")]
    [SerializeField] public TMP_InputField fpsInputField; // Tương tự cho các InputField
    private int currentFPS = 60;

    [Header("Screen Mode Settings")]
    [SerializeField] public TMP_Dropdown screenModeDropdown;
    private readonly FullScreenMode[] screenModes = new FullScreenMode[]
    {
        FullScreenMode.FullScreenWindow, // Fullscreen
        FullScreenMode.Windowed,        // Windowed
        FullScreenMode.MaximizedWindow  // Borderless
    };

    [Header("Graphics Quality Settings")]
    [SerializeField] public TMP_Dropdown graphicsDropdown;

    [Header("Apply Settings Button")]
    [SerializeField] public Button applyButton;

    [Header("Sound Settings")]
    [SerializeField] public AudioSource clickSound;

    private bool settingsChanged = false;

    private void Start()
    {
        // Initialize resolution dropdown
        resolutionDropdown.ClearOptions();
        List<string> resolutionOptions = new List<string>();
        foreach (var res in resolutions)
        {
            resolutionOptions.Add($"{res.width}x{res.height}");
        }
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = 0;
        resolutionDropdown.RefreshShownValue();

        // Initialize frame rate input field
        fpsInputField.text = currentFPS.ToString();

        // Initialize screen mode dropdown
        screenModeDropdown.ClearOptions();
        screenModeDropdown.AddOptions(new List<string> { "Fullscreen", "Windowed", "Borderless" });
        screenModeDropdown.value = 0;
        screenModeDropdown.RefreshShownValue();

        // Initialize graphics quality dropdown
        graphicsDropdown.ClearOptions();
        graphicsDropdown.AddOptions(new List<string> { "Cao", "Trung bình", "Thấp" });
        graphicsDropdown.value = 0;
        graphicsDropdown.RefreshShownValue();

        // Hide apply button initially
        applyButton.gameObject.SetActive(false);

        // Add listeners
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        fpsInputField.onValueChanged.AddListener(OnFPSChanged);
        screenModeDropdown.onValueChanged.AddListener(OnScreenModeChanged);
        graphicsDropdown.onValueChanged.AddListener(OnGraphicsQualityChanged);

        applyButton.onClick.AddListener(ApplySettings);
    }

    public void OnResolutionChanged(int index)
    {
        settingsChanged = true;
        applyButton.gameObject.SetActive(true);
    }

    public void OnFPSChanged(string fpsValue)
    {
        settingsChanged = true;
        applyButton.gameObject.SetActive(true);
    }

    public void OnScreenModeChanged(int index)
    {
        settingsChanged = true;
        applyButton.gameObject.SetActive(true);
    }

    public void OnGraphicsQualityChanged(int index)
    {
        settingsChanged = true;
        applyButton.gameObject.SetActive(true);
    }

    public void ApplySettings()
    {
        // Apply resolution
        Resolution selectedResolution = resolutions[resolutionDropdown.value];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreenMode);

        // Apply FPS
        if (int.TryParse(fpsInputField.text, out int fps))
        {
            currentFPS = Mathf.Clamp(fps, 30, 1000); // Limit FPS range
            Application.targetFrameRate = currentFPS;
        }

        // Apply screen mode
        FullScreenMode selectedMode = screenModes[screenModeDropdown.value];
        Screen.fullScreenMode = selectedMode;

        // Apply graphics quality
        QualitySettings.SetQualityLevel(graphicsDropdown.value);

        // Reset settingsChanged flag and hide apply button
        settingsChanged = false;
        applyButton.gameObject.SetActive(false);

        Debug.Log("Settings Applied");
    }
}

[System.Serializable]
public struct Resolution
{
    public int width;
    public int height;

    public Resolution(int width, int height)
    {
        this.width = width;
        this.height = height;
    }
}
