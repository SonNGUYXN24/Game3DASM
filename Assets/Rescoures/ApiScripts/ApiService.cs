using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.SceneManagement;

public class ApiService : MonoBehaviour
{
    private string baseUrl = "http://localhost:5195/api/gameuser";

    [System.Serializable]
    public class LoginRequest
    {
        public string Username;
        public string Password;
    }

    [System.Serializable]
    public class LoginResponse
    {
        public bool isSuccess;
        public string notification;
        public LoginData data;

        [System.Serializable]
        public class LoginData
        {
            public string token;
            public GameUser user;
        }
    }

    [System.Serializable]
    public class GameUser
    {
        public int Id;
        public string Username;
        public string Email;
        public string AvatarUrl;
        public string Role;
    }

    public IEnumerator Login(string username, string password, System.Action<LoginResponse> onSuccess, System.Action<string> onFailure)
    {
        LoginRequest loginRequest = new LoginRequest { Username = username, Password = password };
        string jsonData = JsonUtility.ToJson(loginRequest);

        UnityWebRequest request = new UnityWebRequest($"{baseUrl}/login", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("Response JSON: " + jsonResponse);

            try
            {
                if (!string.IsNullOrEmpty(jsonResponse))
                {
                    LoginResponse response = JsonUtility.FromJson<LoginResponse>(jsonResponse);
                    if (response != null && response.data != null && !string.IsNullOrEmpty(response.data.token))
                    {
                        PlayerPrefs.SetString("AuthToken", response.data.token);
                        PlayerPrefs.Save();

                        SceneManager.LoadScene("SampleScene");

                        onSuccess?.Invoke(response);
                    }
                    else
                    {
                        Debug.LogError("Token is null or missing in response data");
                        onFailure?.Invoke("Token is null or missing in response data");
                    }
                }
                else
                {
                    Debug.LogError("Empty or null response from API");
                    onFailure?.Invoke("Empty or null response from API");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("JSON Parse Error: " + ex.Message);
                onFailure?.Invoke("JSON Parse Error: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("Request Failed: " + request.error);
            onFailure?.Invoke(request.error);
        }
    }
}
