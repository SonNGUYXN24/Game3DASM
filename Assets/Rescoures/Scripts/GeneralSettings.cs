using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneralSettings : MonoBehaviour
{
    [Header("Keybinding Input Fields")]
    [SerializeField] public TMP_InputField moveForwardInputField;
    [SerializeField] public TMP_InputField moveBackwardInputField;
    [SerializeField] public TMP_InputField moveLeftInputField;
    [SerializeField] public TMP_InputField moveRightInputField;

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
        moveForwardInputField.onEndEdit.AddListener((value) => OnKeyBindingChanged("MoveForward", value));
        moveBackwardInputField.onEndEdit.AddListener((value) => OnKeyBindingChanged("MoveBackward", value));
        moveLeftInputField.onEndEdit.AddListener((value) => OnKeyBindingChanged("MoveLeft", value));
        moveRightInputField.onEndEdit.AddListener((value) => OnKeyBindingChanged("MoveRight", value));

        applyButton.onClick.AddListener(ApplySettings);
    }

    private void OnKeyBindingChanged(string action, string newKey)
    {
        // Trim and validate input
        newKey = newKey.Trim().ToUpper();

        if (newKey.Length == 1 && Enum.TryParse(newKey, out KeyCode newKeyCode))
        {
            keyBindings[action] = newKeyCode;

            // Update InputField text to reflect the new key binding
            UpdateInputField(action, newKeyCode);

            settingsChanged = true;
            applyButton.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Invalid key: {newKey}");
        }
    }

    private void UpdateInputField(string action, KeyCode newKeyCode)
    {
        switch (action)
        {
            case "MoveForward":
                moveForwardInputField.text = newKeyCode.ToString();
                break;
            case "MoveBackward":
                moveBackwardInputField.text = newKeyCode.ToString();
                break;
            case "MoveLeft":
                moveLeftInputField.text = newKeyCode.ToString();
                break;
            case "MoveRight":
                moveRightInputField.text = newKeyCode.ToString();
                break;
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
