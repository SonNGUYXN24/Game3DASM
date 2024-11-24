using UnityEngine;
using TMPro;
using System.Collections;
using System.Runtime.CompilerServices;
using System;
using Newtonsoft.Json;
using UnityEngine.Networking;

public class RegisterController : MonoBehaviour
{
    public GameObject mainGroup;
    public GameObject loginGroup;
    public GameObject registerGroup;

    public TMP_InputField emailInput;
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField nameInput;
    public TMP_InputField avatarInput; // Thêm InputField cho Avatar URL
    public TMP_Text txtMessage;

    private string baseUrl = "http://localhost:5195/api/gameuser"; // URL đến API


    [System.Serializable]
    public class RegisterRequest{
        public string Email;
        public string Username;
        public string Password;
        public string Name;
        public string AvatarUrl;
    }

    void Start()
    {
        ShowMainGroup();
    }
    public void OnRegisterPressed(){
        string email = emailInput.text;
        string username = usernameInput.text;
        string password = passwordInput.text;
        string name = nameInput.text;
        string avatarUrl = avatarInput.text;

        StartCoroutine(Register(email, username, password, name, avatarUrl));
    }

    public IEnumerator Register(string email, string username, string password, string name, string avatarUrl){
        var registerRequest = new RegisterRequest{
            Email = email,
            Username = username,
            Password = password,
            Name = name,
            AvatarUrl = avatarUrl
        };
        
        string jsonData = JsonConvert.SerializeObject(registerRequest);
        Debug.Log("Register JSON: " +jsonData);

        UnityWebRequest request = new UnityWebRequest($"{baseUrl}/register", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json"); // Sửa lỗi chính tả từ Context-Type thành Content-Type

        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.Success){
            txtMessage.text = "Đăng ký thành công!";
            ShowMainGroup();
        }
        else{
            string errorResponse = request.downloadHandler.text;
            Debug.LogError("Register Failed. Error: " +request.error);
            Debug.LogError("Server Response: " +errorResponse);
            txtMessage.text = "Đăng ký thất bại: " +errorResponse;
        }
    }

    public void ShowMainGroup()
    {
        mainGroup.SetActive(true);
        loginGroup.SetActive(false);
        registerGroup.SetActive(false);
    }

    public void ShowRegisterGroup()
    {
        mainGroup.SetActive(false);
        loginGroup.SetActive(false);
        registerGroup.SetActive(true);
    }
}
