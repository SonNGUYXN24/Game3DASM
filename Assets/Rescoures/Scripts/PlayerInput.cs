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
    public bool fireSwordRotationInput; // Biến kiểm tra phím "X" cho kỹ năng quay kiếm

    public Camera mainCamera;
    public float movementSpeed = 5f;
    public float rotationSpeed = 10f;
    public GameObject swordFirePrefab; // Prefab của kiếm lửa
    public Transform[] swordSpawnPoints; // Mảng các điểm sinh ra kiếm lửa
    public Transform[] swordRotationSpawn; // Dùng riêng cho FireSwordRotation
    public ParticleSystem[] swordTrails; // Mảng các ParticleSystem cho Sword Trail
    public ParticleSystem fireSwordRotationEffect; // Hiệu ứng riêng cho kỹ năng FireSwordRotation
    public Vector3 shootDirection = Vector3.forward;

    public MP mp;
    public TextMeshProUGUI insufficientMPText;
    public float mpCost = 20f; // MP tiêu hao cho FireSword
    public float fireSwordRotationCost = 100f; // MP tiêu hao cho FireSwordRotation

    private CharacterController characterController;
    private Vector3 moveDirection;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Tắt tất cả hiệu ứng khi bắt đầu
        foreach (ParticleSystem trail in swordTrails)
        {
            trail.Stop();
        }

        if (fireSwordRotationEffect != null)
        {
            fireSwordRotationEffect.Stop();
        }

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

        if (!fireSwordInput && Time.timeScale > 0)
        {
            fireSwordInput = Input.GetKeyDown(KeyCode.C);
        }

        if (!fireSwordRotationInput && Time.timeScale > 0)
        {
            fireSwordRotationInput = Input.GetKeyDown(KeyCode.X);
        }

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

    private void FixedUpdate()
    {
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        moveDirection = forward * verticalInput + right * horizontalInput;
        moveDirection.Normalize();
        moveDirection *= movementSpeed;

        if (moveDirection != Vector3.zero && verticalInput > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

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
        if (fireSwordRotationEffect != null)
        {
            fireSwordRotationEffect.Play(); // Bật hiệu ứng
        }

        GameObject[] swords = new GameObject[swordRotationSpawn.Length];

        // Tạo kiếm tại các vị trí ban đầu
        for (int i = 0; i < swordRotationSpawn.Length; i++)
        {
            swords[i] = Instantiate(swordFirePrefab, swordRotationSpawn[i].position, Quaternion.identity);
        }

        float rotationTime = 15f;
        float elapsedTime = 0f;

        while (elapsedTime < rotationTime)
        {
            elapsedTime += Time.deltaTime;

            float angleStep = 360f / swordRotationSpawn.Length;

            for (int i = 0; i < swordRotationSpawn.Length; i++)
            {
                float angle = elapsedTime * 90f + i * angleStep; // Xoay theo thời gian
                float radians = angle * Mathf.Deg2Rad;

                Vector3 newPosition = new Vector3(
                    transform.position.x + Mathf.Cos(radians) * 3f, // Bán kính 3f
                    transform.position.y,
                    transform.position.z + Mathf.Sin(radians) * 3f
                );

                swords[i].transform.position = newPosition;
            }

            yield return null;
        }

        // Xóa các kiếm sau khi hết thời gian
        foreach (GameObject sword in swords)
        {
            Destroy(sword);
        }

        if (fireSwordRotationEffect != null)
        {
            fireSwordRotationEffect.Stop(); // Dừng hiệu ứng
        }
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
