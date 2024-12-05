using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    public GameObject virtualCamera;    // Gắn VirtualCamera vào đây
    public GameObject freeLookCamera;  // Gắn FreeLookCamera vào đây

    private bool isUsingVirtualCamera = true; // Mặc định là VirtualCamera

    void Start()
    {
        // Bật VirtualCamera và tắt FreeLookCamera khi game bắt đầu
        if (virtualCamera != null) virtualCamera.SetActive(true);
        if (freeLookCamera != null) freeLookCamera.SetActive(false);
    }

    void Update()
    {
        // Kiểm tra xem người chơi có nhấn phím F5 hay không
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SwitchActiveCamera();
        }
    }

    private void SwitchActiveCamera()
    {
        // Đảo trạng thái giữa hai camera
        isUsingVirtualCamera = !isUsingVirtualCamera;

        if (virtualCamera != null) virtualCamera.SetActive(isUsingVirtualCamera);
        if (freeLookCamera != null) freeLookCamera.SetActive(!isUsingVirtualCamera);
    }
}
