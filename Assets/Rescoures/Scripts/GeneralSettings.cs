using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneralSettings : MonoBehaviour
{
    [Header("Keybinding Input Fields")]
    [SerializeField] public TMP_InputField moveForwardInputField; // InputField cho phím "Tiến"
    [SerializeField] public TMP_InputField moveBackwardInputField; // InputField cho phím "Lùi"
    [SerializeField] public TMP_InputField moveLeftInputField; // InputField cho phím "Trái"
    [SerializeField] public TMP_InputField moveRightInputField; // InputField cho phím "Phải"

    [Header("Apply Settings Button")]
    [SerializeField] public Button applyButton;

    [Header("Sound Settings")]
    [SerializeField] public AudioSource clickSound;

    private Dictionary<string, KeyCode> keyBindings = new Dictionary<string, KeyCode>
    {
        { "MoveForward", KeyCode.W },
        { "MoveBackward", KeyCode.S },
        { "MoveLeft", KeyCode.A },
        { "MoveRight", KeyCode.D }
    };

    private bool settingsChanged = false;

    private void Start()
    {
        // Initialize InputFields with default keybindings
        moveForwardInputField.text = keyBindings["MoveForward"].ToString();
        moveBackwardInputField.text = keyBindings["MoveBackward"].ToString();
        moveLeftInputField.text = keyBindings["MoveLeft"].ToString();
        moveRightInputField.text = keyBindings["MoveRight"].ToString();

        // Hide Apply button initially
        applyButton.gameObject.SetActive(false);

        // Add listeners
        moveForwardInputField.onValueChanged.AddListener((value) => OnKeyBindingChanged("MoveForward", value));
        moveBackwardInputField.onValueChanged.AddListener((value) => OnKeyBindingChanged("MoveBackward", value));
        moveLeftInputField.onValueChanged.AddListener((value) => OnKeyBindingChanged("MoveLeft", value));
        moveRightInputField.onValueChanged.AddListener((value) => OnKeyBindingChanged("MoveRight", value));

        applyButton.onClick.AddListener(ApplySettings);
    }

    private void OnKeyBindingChanged(string action, string newKey)
    {
        // Validate new key
        if (newKey.Length == 1 && Enum.TryParse(newKey.ToUpper(), out KeyCode newKeyCode))
        {
            keyBindings[action] = newKeyCode;
            settingsChanged = true;
            applyButton.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Invalid key: {newKey}");
        }
    }

    public void ApplySettings()
    {
        if (clickSound != null)
        {
            clickSound.Play();
        }

        // Apply new key bindings
        foreach (var keyBinding in keyBindings)
        {
            Debug.Log($"Action: {keyBinding.Key}, Key: {keyBinding.Value}");
        }

        // Reset settingsChanged flag and hide Apply button
        settingsChanged = false;
        applyButton.gameObject.SetActive(false);

        Debug.Log("Keybindings Applied");
    }

    public KeyCode GetKeyForAction(string action)
    {
        return keyBindings.ContainsKey(action) ? keyBindings[action] : KeyCode.None;
    }
}
