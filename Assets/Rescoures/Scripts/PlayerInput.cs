using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerInput : MonoBehaviour
{
    public float horizontalInput;
    public float verticalInput;
    public bool attackInput;
    public bool fireSwordInput; // Biến kiểm tra phím tấn công kiếm lửa

    public Camera mainCamera;
    public float movementSpeed = 5f;
    public float rotationSpeed = 10f;
    public GameObject swordFirePrefab; // Prefab của kiếm lửa
    public Transform[] swordSpawnPoints; // Mảng các điểm sinh ra kiếm lửa
    public ParticleSystem[] swordTrails; // Mảng các ParticleSystem cho Sword Trail
    public Vector3 shootDirection = Vector3.forward; // Hướng bắn mặc định

    public MP mp; // Đối tượng MP của nhân vật
    public TextMeshProUGUI insufficientMPText; // Text thông báo khi MP không đủ
    public float mpCost = 20f; // MP tiêu hao khi sử dụng kỹ năng

    private CharacterController characterController;
    private Vector3 moveDirection;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Dừng tất cả Sword Trail khi bắt đầu
        foreach (ParticleSystem trail in swordTrails)
        {
            trail.Stop();
        }

        // Đảm bảo Text thông báo "MP không đủ" bị ẩn khi bắt đầu
        if (insufficientMPText != null)
        {
            insufficientMPText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        if (!attackInput && Time.timeScale > 0)
        {
            attackInput = Input.GetMouseButtonDown(0);
        }

        // Kiểm tra phím "C" để sử dụng kỹ năng kiếm lửa
        if (!fireSwordInput && Time.timeScale > 0)
        {
            fireSwordInput = Input.GetKeyDown(KeyCode.C);
        }

        // Kích hoạt kiếm lửa nếu đủ MP
        if (fireSwordInput)
        {
            TryFireSword();
            fireSwordInput = false; // Đặt lại biến để tránh kích hoạt liên tục
        }
    }

    private void FixedUpdate()
    {
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // Tính toán hướng di chuyển
        moveDirection = forward * verticalInput + right * horizontalInput;
        moveDirection.Normalize();
        moveDirection *= movementSpeed;

        // Xoay hướng nhân vật theo hướng di chuyển
        if (moveDirection != Vector3.zero && verticalInput > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Di chuyển nhân vật
        characterController.Move(moveDirection * Time.fixedDeltaTime);
    }

    private void OnDisable()
    {
        horizontalInput = 0;
        verticalInput = 0;
        attackInput = false;
    }

    private void TryFireSword()
    {
        if (mp.UseMP(20)) // Trừ 20 MP
        {
            FireSword(); // Kích hoạt kỹ năng
        }
        else
        {
            ShowInsufficientMPMessage(); // Hiển thị thông báo không đủ MP
        }

    }

    private void FireSword()
    {
        // Chọn ngẫu nhiên một vị trí để sinh ra kiếm lửa
        int randomIndex = Random.Range(0, swordSpawnPoints.Length);
        Transform spawnPoint = swordSpawnPoints[randomIndex];

        // Tạo kiếm lửa tại vị trí đã chọn
        GameObject swordFireObject = Instantiate(swordFirePrefab, spawnPoint.position, Quaternion.identity);

        // Truyền hướng bắn cho kiếm lửa
        SwordFire swordFire = swordFireObject.GetComponent<SwordFire>();
        swordFire.shootDirection = transform.forward;

        // Chạy hiệu ứng Sword Trail
        swordTrails[randomIndex].Play();
    }

    private void ShowInsufficientMPMessage()
    {
        if (insufficientMPText != null)
        {
            StartCoroutine(DisplayInsufficientMPMessage());
        }
    }

    private IEnumerator DisplayInsufficientMPMessage()
    {
        insufficientMPText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f); // Hiển thị thông báo trong 2 giây
        insufficientMPText.gameObject.SetActive(false);
    }
}
