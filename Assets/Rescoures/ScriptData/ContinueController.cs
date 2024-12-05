using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ContinueController : MonoBehaviour
{
    public Button continueButton;
    public Button newGameButton;

    private void Start()
    {
        // Kiểm tra dữ liệu đã tồn tại chưa
        if (PlayerPrefs.GetInt("HasData", 0) == 1)
        {
            continueButton.gameObject.SetActive(true);
        }
        else
        {
            continueButton.gameObject.SetActive(false);
        }
    }

    public void OnContinue()
    {
        var data = AutoSave.Instance.LoadPlayerData();
        if (!string.IsNullOrEmpty(data.Item2))
        {
            SceneManager.LoadScene(data.Item2);
            StartCoroutine(WaitForSceneLoadAndPlacePlayer(data.Item1));
        }
        else
        {
            Debug.LogWarning("No saved data found!");
        }
    }

    private IEnumerator WaitForSceneLoadAndPlacePlayer(Vector3 playerPosition)
    {
        yield return new WaitForEndOfFrame(); // Đợi scene load xong
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = playerPosition;
        }
    }

    public void OnNewGame()
    {
        AutoSave.Instance.ResetPlayerData();
        SceneManager.LoadScene("SampleScene");
        Debug.Log("Starting a new game!");
    }
}
