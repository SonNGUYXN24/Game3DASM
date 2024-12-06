using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class ContinueController : MonoBehaviour
{
    public Button continueButton;
    public Button newGameButton;
    public Canvas videoCanvas; // Canvas chứa Raw Image và VideoPlayer
    public VideoPlayer videoPlayer; // VideoPlayer để chạy video

    private List<GameObject> hiddenObjects = new List<GameObject>(); // Danh sách các GameObject bị ẩn
    private bool isSkippingVideo = false; // Trạng thái cho phép bỏ qua video
    private bool isVideoFinished = false; // Trạng thái video đã chạy xong hoặc bị bỏ qua

    private void Start()
    {
        // Kiểm tra dữ liệu đã tồn tại chưa
        if (PlayerPrefs.GetInt("HasData", 0) == 1)
        {
            continueButton.gameObject.SetActive(true);
            continueButton.onClick.AddListener(OnContinue); // Gắn sự kiện OnContinue cho nút Continue
        }
        else
        {
            continueButton.gameObject.SetActive(false);
        }

        // Ẩn Canvas chứa video khi bắt đầu
        if (videoCanvas != null)
        {
            videoCanvas.gameObject.SetActive(false);
        }

        // Gắn sự kiện cho VideoPlayer
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted += OnVideoPrepared;
        }
    }

    public void OnContinue()
    {
        var data = AutoSave.Instance.LoadPlayerData();
        if (!string.IsNullOrEmpty(data.Item2))
        {
            Debug.Log($"Loading Scene: {data.Item2}");
            Debug.Log($"Restoring Player position to: {data.Item1}");

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
        // Đợi Scene load xong
        yield return new WaitUntil(() => SceneManager.GetActiveScene().isLoaded);

        // Tìm đối tượng Player và đợi nếu chưa tìm thấy
        GameObject player = null;
        while (player == null)
        {
            player = GameObject.FindWithTag("Player");
            yield return null; // Đợi 1 frame nếu Player chưa tồn tại
        }

        // Đặt vị trí của Player
        player.transform.position = playerPosition;
        Debug.Log($"Player position set to: {player.transform.position}");

        // Kiểm tra lại xem vị trí Player đã được gán đúng
        yield return new WaitForEndOfFrame();
        Debug.Log($"Final Player position: {player.transform.position}");
    }



    public void OnNewGame()
    {
        AutoSave.Instance.ResetPlayerData();

        if (videoPlayer != null && videoCanvas != null)
        {
            StartCoroutine(PlayIntroVideoAndLoadScene());
        }
        else
        {
            SceneManager.LoadScene("SampleScene");
            Debug.Log("Starting a new game!");
        }
    }

    private IEnumerator PlayIntroVideoAndLoadScene()
    {
        isSkippingVideo = false;
        isVideoFinished = false;

        // Hiện Canvas chứa VideoPlayer
        if (videoCanvas != null)
        {
            videoCanvas.gameObject.SetActive(true);
        }

        // Ẩn tất cả các GameObject trừ VideoPlayer, MainCamera, DirectionalLight, và đối tượng chứa script
        HideAllOtherGameObjects();

        // Chuẩn bị VideoPlayer trước khi phát
        videoPlayer.Prepare();
        Debug.Log("Preparing video...");

        // Đợi VideoPlayer sẵn sàng
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        // Bắt đầu phát video sau khi chuẩn bị xong
        videoPlayer.Play();
        Debug.Log("Playing intro video...");

        // Đợi video hoàn thành hoặc bị bỏ qua
        while (videoPlayer.isPlaying && !isSkippingVideo)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isSkippingVideo = true;
                videoPlayer.Stop(); // Dừng video nếu người chơi nhấn Space
            }
            yield return null;
        }

        // Đảm bảo trạng thái video đã hoàn tất hoặc bị bỏ qua
        isVideoFinished = true;

        // Tắt VideoPlayer và Canvas chứa nó
        videoPlayer.Stop();
        if (videoCanvas != null)
        {
            videoCanvas.gameObject.SetActive(false);
        }

        // Hiện lại các GameObject bị ẩn
        ShowAllHiddenGameObjects();

        // Chuyển sang SampleScene
        SceneManager.LoadScene("SampleScene");
        Debug.Log("Video ended or skipped, loading new game scene.");
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        Debug.Log("Video is ready to play!");
    }

    private void HideAllOtherGameObjects()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj == gameObject || obj == videoPlayer.gameObject || obj.CompareTag("MainCamera") || obj.name == "Directional Light" || obj == videoCanvas.gameObject || obj.name == "EventSystem" || obj.name == "AutoSave")
                continue;

            if (obj.activeSelf)
            {
                obj.SetActive(false);
                hiddenObjects.Add(obj);
            }
        }
    }

    private void ShowAllHiddenGameObjects()
    {
        foreach (GameObject obj in hiddenObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        hiddenObjects.Clear();
    }
}
