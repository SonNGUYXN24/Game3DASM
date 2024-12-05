using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    public Camera freeCamera; // Camera dùng để di chuyển tự do
    public float baseSpeed = 5f; // Tốc độ mặc định
    public float boostSpeed = 20f; // Tốc độ tăng cường khi nhấn Shift
    public Canvas[] canvasesToHide; // Các canvas cần ẩn khi kích hoạt chế độ camera
    public PlayerInput playerInputScript; // Script PlayerInput cần tắt khi ở chế độ camera tự do

    private bool isFreeCameraActive = false; // Trạng thái chế độ camera
    private Vector3 currentVelocity = Vector3.zero;

    void Update()
    {
        // Bật/tắt chế độ camera bằng phím F9
        if (Input.GetKeyDown(KeyCode.F9))
        {
            ToggleFreeCameraMode();
        }

        // Nếu camera tự do đang hoạt động, xử lý di chuyển
        if (isFreeCameraActive)
        {
            HandleCameraMovement();
        }
    }

    private void ToggleFreeCameraMode()
    {
        isFreeCameraActive = !isFreeCameraActive;
        freeCamera.gameObject.SetActive(isFreeCameraActive);

        // Ẩn hoặc hiện các canvas
        foreach (var canvas in canvasesToHide)
        {
            canvas.gameObject.SetActive(!isFreeCameraActive);
        }

        // Bật hoặc tắt PlayerInput
        if (playerInputScript != null)
        {
            playerInputScript.enabled = !isFreeCameraActive;
        }
    }

    private void HandleCameraMovement()
    {
        // Lấy tốc độ di chuyển (Shift để tăng tốc)
        float speed = Input.GetKey(KeyCode.LeftShift) ? boostSpeed : baseSpeed;

        // Xử lý di chuyển bằng A, W, S, D
        Vector3 direction = new Vector3(
            Input.GetAxis("Horizontal"), // Trục X (A/D)
            0,
            Input.GetAxis("Vertical") // Trục Z (W/S)
        );

        // Xử lý di chuyển lên/xuống bằng chuột
        if (Input.GetMouseButton(1)) // Giữ chuột phải để xoay camera
        {
            float mouseY = Input.GetAxis("Mouse Y");
            float mouseX = Input.GetAxis("Mouse X");

            // Xoay camera theo chuột
            freeCamera.transform.Rotate(-mouseY, mouseX, 0);
        }

        if (Input.GetKey(KeyCode.E)) // Bay lên
        {
            direction.y += 1;
        }
        if (Input.GetKey(KeyCode.Q)) // Bay xuống
        {
            direction.y -= 1;
        }

        // Di chuyển camera theo hướng
        Vector3 move = direction.normalized * speed * Time.deltaTime;
        freeCamera.transform.Translate(move, Space.Self);
    }
}
