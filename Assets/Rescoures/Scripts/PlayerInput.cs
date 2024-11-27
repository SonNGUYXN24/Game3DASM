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

    public Camera mainCamera; // KhaiBaoCamera
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

    [SerializeField]private CharacterController characterController;
    private Vector3 moveDirection;

    public bool isFlying = false; // Trạng thái bay
    public GameObject swordFly; // Kiếm dưới chân khi bay
    public float flySpeed = 5f;
    public AudioSource flySound; // Âm thanh khi bay
    public ParticleSystem flyEffect; // Hiệu ứng khi bay
    private Vector3 velocity; // Tốc độ rơi

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
    }

    private void Update()
    {
        // Lấy input từ bàn phím
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // Chuyển đổi trạng thái bay
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleFlyMode();
        }

        // Điều khiển skill luôn khả dụng dù bay hay không bay
        if (Time.timeScale > 0)
        {
            if (!attackInput)
            {
                attackInput = Input.GetMouseButtonDown(0);
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
            SwordFlyMove();
        }
        else
        {
            NormalMove();
        }
    }

    private void FixedUpdate()
    {
        if (!isFlying)
        {
            ApplyGravity(); // Áp dụng trọng lực khi không bay
            NormalMove();   // Di chuyển bình thường
        }
    }

    private void ApplyGravity()
    {
        velocity.y += Physics.gravity.y * Time.deltaTime; // Áp dụng trọng lực chỉ khi không bay
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

    private void SwordFlyMove()
    {
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;

        forward.Normalize();
        right.Normalize();

        // Xử lý di chuyển XZ
        moveDirection = forward * verticalInput + right * horizontalInput;
        moveDirection.Normalize();
        moveDirection *= flySpeed;

        // Bay lên hoặc xuống dựa trên phím nhấn
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

        // Xoay nhân vật nếu đang di chuyển
        if (moveDirection.x != 0 || moveDirection.z != 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void ToggleFlyMode()
    {
        isFlying = !isFlying; // Chuyển đổi trạng thái bay

        if (isFlying)
        {
            velocity = Vector3.zero; // Đặt lại vận tốc
            flySound?.Play();
            flyEffect?.Play();

            // Kích hoạt GameObject SwordFly
            if (swordFly != null)
            {
                swordFly.SetActive(true);
            }
        }
        else
        {
            flySound?.Stop();
            flyEffect?.Stop();

            // Ẩn GameObject SwordFly
            if (swordFly != null)
            {
                swordFly.SetActive(false);
            }
        }
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

        foreach (GameObject sword in swords)
        {
            Destroy(sword);
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
