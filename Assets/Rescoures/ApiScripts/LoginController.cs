using UnityEngine;
using TMPro;

public class LoginController : MonoBehaviour
{
    public ApiService apiService;
    public GameObject mainGroup;
    public GameObject loginGroup;
    public GameObject registerGroup;
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_Text txtMessage;

    void Start()
    {
        ShowMainGroup();
    }

    public void OnLoginButtonPressed()
    {
        Debug.Log("Login button pressed");
        string username = usernameInput.text;
        string password = passwordInput.text;

        StartCoroutine(apiService.Login(username, password, OnLoginSuccess, OnLoginFailure));
    }

    public void OnLoginSuccess(ApiService.LoginResponse response)
    {
        if (response.isSuccess)
        {
            // Đăng nhập thành công, lưu token và hiển thị thông báo
            txtMessage.text = response.notification + " Xin chào, " + response.data.user.Username;
            Debug.Log("Token: " + response.data.token);
            ShowMainGroup();
        }
        else
        {
            txtMessage.text = "Đăng nhập thất bại: " + response.notification;
        }
    }

    private void OnLoginFailure(string error)
    {
        txtMessage.text = "Đăng nhập thất bại: " + error;
    }

    public void ShowMainGroup()
    {
        mainGroup.SetActive(true);
        loginGroup.SetActive(false);
        registerGroup.SetActive(false);
    }

    public void ShowLoginGroup()
    {
        mainGroup.SetActive(false);
        loginGroup.SetActive(true);
        registerGroup.SetActive(false);
    }

    public void ShowRegisterGroup()
    {
        mainGroup.SetActive(false);
        loginGroup.SetActive(false);
        registerGroup.SetActive(true);
    }
}
