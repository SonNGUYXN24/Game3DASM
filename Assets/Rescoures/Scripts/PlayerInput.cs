using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerInput : MonoBehaviour
{
    public float horizontalInput;
    public float verticalInput;
    public bool attackInput;
    public bool fireSwordInput;
    public bool fireSwordRotationInput;

    public Camera mainCamera; // Khai báo Camera
    public float movementSpeed = 5f;
    public float rotationSpeed = 10f;
    public GameObject swordFirePrefab;
    public GameObject swordFireRotationPrefab;
    public Transform[] swordSpawnPoints;
    public Transform[] swordRotationSpawn;
    public ParticleSystem[] swordTrails;
    public ParticleSystem fireSwordRotationEffect;
    public Vector3 shootDirection = Vector3.forward;

    public MP mp;
    public TextMeshProUGUI insufficientMPText;
    public float mpCost = 20f;
    public float fireSwordRotationCost = 100f;

    [SerializeField] private CharacterController characterController;
    private Vector3 moveDirection;

    public bool isFlying = false; // Trạng thái bay
    public float flySpeed = 5f;
    public AudioSource flySound; // Âm thanh khi bay
    public ParticleSystem flyEffect; // Hiệu ứng khi bay
    private Vector3 velocity; // Tốc độ rơi

    public AudioClip magicSound; // Âm thanh khi bật chế độ bay

    public float flySkillDuration = 90f; // Thời gian sử dụng skill (1 phút 30 giây)
    private float currentFlyTime = 0f; // Thời gian còn lại
    public TextMeshProUGUI flySkillTimerText; // TextMeshPro hiển thị thời gian skill
    private Coroutine flySkillCoroutine; // Để quản lý coroutine skill bay

    private void Start()
    {
        foreach (ParticleSystem trail in swordTrails)
        {
            trail.Stop();
        }

        fireSwordRotationEffect?.Stop();
        insufficientMPText?.gameObject.SetActive(false);

        if (flyEffect != null)
        {
            flyEffect.Stop();  
        }
        flySound.Stop();
    }

    private void Update()
    {
        // Lấy input từ bàn phím
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // Chuyển đổi trạng thái bay
         if (Input.GetKeyDown(KeyCode.H))
        {
            if (!isFlying)
            {
                TryToggleFlyMode();
            }
            else
            {
                ToggleFlyMode(false); // Tắt bay thủ công nếu đang bay
            }
        }

        // Điều khiển skill luôn khả dụng dù bay hay không bay
        if (Time.timeScale > 0)
        {
            if (!attackInput)
            {
                attackInput = Input.GetKeyDown(KeyCode.F);
            }
            if (!fireSwordInput)
            {
                fireSwordInput = Input.GetKeyDown(KeyCode.C);
            }
            if (!fireSwordRotationInput)
            {
                fireSwordRotationInput = Input.GetKeyDown(KeyCode.X);
            }

            // Kích hoạt skill
            if (fireSwordInput)
            {
                TryFireSword();
                fireSwordInput = false;
            }
            if (fireSwordRotationInput)
            {
                TryFireSwordRotation();
                fireSwordRotationInput = false;
            }
        }

        // Di chuyển khi bay hoặc đi bộ
        if (isFlying)
        {
            FlyModeMove();
        }
        else
        {
            NormalMove();
            flySound.Stop();
        }
    }

     private void TryToggleFlyMode()
    {
        if (mp.UseMP(150)) // Kiểm tra đủ MP
        {
            ToggleFlyMode(true); // Bật chế độ bay
        }
        else
        {
            ShowInsufficientMPMessage(); // Hiển thị cảnh báo thiếu MP
        }
    }

    private void FixedUpdate()
    {
        if (!isFlying)
        {
            ApplyGravity(); // Áp dụng trọng lực khi không bay
            NormalMove();   // Di chuyển bình thường
        }
        else
        {
            FlyModeMove(); // Điều khiển bay khi đang ở chế độ bay
        }
    }

    private void ApplyGravity()
    {
        velocity.y += Physics.gravity.y * Time.deltaTime; // Áp dụng trọng lực
        characterController.Move(velocity * Time.deltaTime);
    }

    private void NormalMove()
    {
        // Di chuyển theo camera
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        moveDirection = forward * verticalInput + right * horizontalInput;
        moveDirection.Normalize();
        moveDirection *= movementSpeed;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        characterController.Move(moveDirection * Time.fixedDeltaTime);
    }

    private void FlyModeMove()
    {
        // Vector điều khiển di chuyển
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;

        forward.Normalize();
        right.Normalize();

        // Di chuyển XZ
        moveDirection = forward * verticalInput + right * horizontalInput;
        moveDirection.Normalize();
        moveDirection *= flySpeed;

        // Điều khiển độ cao
        if (Input.GetKey(KeyCode.Space))
        {
            moveDirection.y = flySpeed; // Bay lên
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            moveDirection.y = -flySpeed; // Hạ xuống
        }
        else
        {
            moveDirection.y = 0; // Giữ nguyên độ cao
        }

        // Di chuyển nhân vật
        characterController.Move(moveDirection * Time.deltaTime);

        // Xoay nhân vật theo hướng di chuyển
        if (moveDirection.x != 0 || moveDirection.z != 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void ToggleFlyMode(bool enable)
    {
        if (enable)
        {
            isFlying = true; // Kích hoạt trạng thái bay
            velocity = Vector3.zero; // Đặt lại vận tốc
            flySound?.Play();
            flyEffect?.Play();

            if (magicSound != null)
            {
                AudioSource.PlayClipAtPoint(magicSound, transform.position); // Âm thanh khi bật chế độ bay
            }

            if (flySkillCoroutine != null)
            {
                StopCoroutine(flySkillCoroutine); // Dừng coroutine cũ nếu có
            }

            currentFlyTime = flySkillDuration; // Đặt thời gian bắt đầu
            flySkillCoroutine = StartCoroutine(FlySkillTimer()); // Bắt đầu đếm ngược
        }
        else
        {
            isFlying = false; // Tắt trạng thái bay
            flySound?.Stop();
            flyEffect?.Stop();
            currentFlyTime = 0f; // Đặt thời gian còn lại về 0
            flySkillTimerText?.gameObject.SetActive(false); // Ẩn text hiển thị
        }
    }

    private IEnumerator FlySkillTimer()
    {
        flySkillTimerText?.gameObject.SetActive(true); // Hiển thị text
        while (currentFlyTime > 0)
        {
            currentFlyTime -= Time.deltaTime;
            UpdateFlySkillTimerText();
            yield return null;
        }

        ToggleFlyMode(false); // Tắt bay khi hết thời gian
    }

    private void UpdateFlySkillTimerText()
    {
        int minutes = Mathf.FloorToInt(currentFlyTime / 60);
        int seconds = Mathf.FloorToInt(currentFlyTime % 60);
        flySkillTimerText.text = $"Thời gian cường hoá: {minutes:00}:{seconds:00}";
    }



    private void OnDisable()
    {
        horizontalInput = 0;
        verticalInput = 0;
        attackInput = false;
    }

    private void TryFireSword()
    {
        if (mp.UseMP(20))
        {
            FireSword();
        }
        else
        {
            ShowInsufficientMPMessage();
        }
    }

    private void TryFireSwordRotation()
    {
        if (mp.UseMP(100))
        {
            StartCoroutine(FireSwordRotation());
        }
        else
        {
            ShowInsufficientMPMessage();
        }
    }

    private void FireSword()
    {
        int randomIndex = Random.Range(0, swordSpawnPoints.Length);
        Transform spawnPoint = swordSpawnPoints[randomIndex];

        GameObject swordFireObject = Instantiate(swordFirePrefab, spawnPoint.position, Quaternion.identity);

        SwordFire swordFire = swordFireObject.GetComponent<SwordFire>();
        swordFire.shootDirection = transform.forward;

        swordTrails[randomIndex].Play();
    }

    private IEnumerator FireSwordRotation()
{
    fireSwordRotationEffect?.Play();

    GameObject[] swords = new GameObject[swordRotationSpawn.Length];

    // Spawn kiếm ban đầu
    for (int i = 0; i < swordRotationSpawn.Length; i++)
    {
        swords[i] = Instantiate(swordFireRotationPrefab, swordRotationSpawn[i].position, Quaternion.identity);
    }

    float rotationTime = 10f;
    float elapsedTime = 0f;

    while (elapsedTime < rotationTime)
    {
        elapsedTime += Time.deltaTime;

        float angleStep = 360f / swordRotationSpawn.Length;

        for (int i = 0; i < swordRotationSpawn.Length; i++)
        {
            if (swords[i] == null) // Nếu kiếm bị phá hủy, spawn lại kiếm mới
            {
                swords[i] = Instantiate(swordFireRotationPrefab, swordRotationSpawn[i].position, Quaternion.identity);
            }

            float angle = elapsedTime * 90f + i * angleStep;
            float radians = angle * Mathf.Deg2Rad;

            Vector3 newPosition = new Vector3(
                transform.position.x + Mathf.Cos(radians) * 3f,
                transform.position.y,
                transform.position.z + Mathf.Sin(radians) * 3f
            );

            swords[i].transform.position = newPosition;
        }

        yield return null;
    }

    // Dừng hiệu ứng và phá hủy tất cả kiếm
    foreach (GameObject sword in swords)
    {
        if (sword != null)
        {
            Destroy(sword);
        }
    }

    fireSwordRotationEffect?.Stop();
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
        yield return new WaitForSeconds(2f);
        insufficientMPText.gameObject.SetActive(false);
    }
}
