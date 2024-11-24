using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float sensitivityX = 1f;  // Giảm độ nhạy trục X để di chuyển mượt mà hơn
    public float sensitivityY = 1f;  // Giảm độ nhạy trục Y để di chuyển mượt mà hơn
    public float distanceFromPlayer = 5f;
    public float heightOffset = 2f;
    public float rotationSmoothTime = 0.5f;  // Tăng thời gian để xoay mượt mà hơn

    public PlayerInput playerInput;
    public Image crosshair;  // Tham chiếu tới Image Crosshair

    private Vector3 initialRotation;
    private Vector3 currentRotation;
    private Vector3 rotationSmoothVelocity;
    private bool isRightMouseDown = false;

    private void Start()
    {
        // Kiểm tra xem crosshair đã được tham chiếu hay chưa
        if (crosshair == null)
        {
            Debug.LogWarning("Crosshair Image chưa được thiết lập.");
        }

        // Lưu lại rotation ban đầu của camera
        initialRotation = transform.eulerAngles;
        currentRotation = initialRotation;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isRightMouseDown = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isRightMouseDown = false;
        }

        if (isRightMouseDown)
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivityX;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivityY;

            currentRotation.x -= mouseY;
            currentRotation.y += mouseX;
            currentRotation.x = Mathf.Clamp(currentRotation.x, -35f, 60f);
        }
        else
        {
            // Trở về rotation ban đầu khi thả chuột phải
            currentRotation = Vector3.Slerp(currentRotation, initialRotation, rotationSmoothTime * Time.deltaTime);
        }

        // Chỉ di chuyển camera theo hướng của player khi nhấn "W" và di chuyển về phía trước
        if (playerInput.verticalInput > 0 && !isRightMouseDown)
        {
            currentRotation = player.eulerAngles;
        }

        Vector3 targetRotation = Vector3.SmoothDamp(transform.eulerAngles, currentRotation, ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = targetRotation;

        Vector3 targetPosition = player.position - transform.forward * distanceFromPlayer + Vector3.up * heightOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, rotationSmoothTime);

        // Bật/tắt crosshair dựa trên điều kiện
        if (crosshair != null)
        {
            crosshair.enabled = !isRightMouseDown;
        }
    }
}
