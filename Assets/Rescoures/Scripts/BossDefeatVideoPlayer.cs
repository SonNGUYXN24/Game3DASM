using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class BossDefeatVideoPlayer : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RawImage videoRawImage; // RawImage chứa VideoPlayer
    [SerializeField] private Canvas videoCanvas; // Canvas chứa RawImage

    [Header("GameObjects")]
    [SerializeField] private GameObject boss; // GameObject đại diện cho boss

    [Header("Video Player")]
    [SerializeField] private VideoPlayer videoPlayer; // VideoPlayer để phát video

    private List<GameObject> hiddenObjects = new List<GameObject>(); // Danh sách các GameObject bị ẩn
    private bool isVideoPlaying = false; // Trạng thái kiểm tra video đang phát
    private bool isSkippingVideo = false; // Trạng thái cho phép bỏ qua video

    private void Start()
    {
        // Đảm bảo Canvas ẩn khi bắt đầu
        if (videoCanvas != null)
        {
            videoCanvas.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Kiểm tra nếu boss đã bị phá hủy
        if (boss == null && !isVideoPlaying)
        {
            StartCoroutine(PlayVideoOnBossDefeat());
        }

        // Kiểm tra phím Space để quay lại MainMenu
        if (isVideoPlaying && Input.GetKeyDown(KeyCode.Space))
        {
            isSkippingVideo = true;
        }
    }

    private IEnumerator PlayVideoOnBossDefeat()
    {
        isVideoPlaying = true;

        // Chờ 3 giây trước khi chạy video
        yield return new WaitForSeconds(3f);

        // Hiển thị Canvas và phát video
        if (videoCanvas != null)
        {
            videoCanvas.gameObject.SetActive(true);
        }

        if (videoPlayer != null)
        {
            videoPlayer.Play();
            Debug.Log("Playing video...");
        }
        else
        {
            Debug.LogError("VideoPlayer không được gắn!");
            yield break;
        }

        // Ẩn tất cả các GameObject khác
        HideAllOtherGameObjects();

        // Chờ cho đến khi video phát xong hoặc người chơi nhấn Space
        while ((videoPlayer.isPlaying || videoPlayer.time < videoPlayer.length) && !isSkippingVideo)
        {
            yield return null;
        }

        // Nếu video phát xong hoặc người chơi nhấn Space
        videoPlayer.Stop();
        Debug.Log("Video ended or skipped. Loading MainMenu...");
        SceneManager.LoadScene("MainMenu");
    }

    private void HideAllOtherGameObjects()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // Không ẩn các GameObject cần thiết
            if (obj == gameObject || obj == videoPlayer.gameObject || obj.CompareTag("MainCamera") ||
                obj.name == "EventSystem" || obj.name == "Directional Light" ||
                obj.name == "AutoSave" || obj == videoCanvas.gameObject)
            {
                continue;
            }

            // Ẩn các GameObject khác
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
