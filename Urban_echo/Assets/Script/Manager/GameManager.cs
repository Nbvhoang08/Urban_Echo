using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public Transform player; // Player
    public Transform goal; // Goal

    private float _initialDistance; // Khoảng cách ban đầu giữa player và goal theo trục Z
    public float maxProgress; // Tiến trình xa nhất đã đạt được (0 -> 1)
    public GameObject celebrationEffectPrefab; // Prefab cho hiệu ứng ăn mừng
    public bool goalReached; // Cờ để kiểm tra xem đã vượt qua goal hay chưa
    public Camera mainCamera; // Camera chính
    private void Start()
    {
        // Tính khoảng cách ban đầu theo trục Z
        _initialDistance = Mathf.Abs(player.position.z - goal.position.z);
        mainCamera = Camera.main;
        // Đảm bảo progress bar bắt đầu từ 0
        maxProgress = 0f;
        goalReached = false;
      
    }
    private void Update()
    {
        // Nếu đã vượt qua Goal, không cập nhật nữa
        if (goalReached)
            return;

        // Tính khoảng cách hiện tại theo trục Z
        float currentDistance = Mathf.Abs(player.position.z - goal.position.z);

        // Tỷ lệ quãng đường đã đi được
        float progress = Mathf.Clamp01(1 - (currentDistance / _initialDistance));

        // Cập nhật tiến trình xa nhất đã đạt được
        if (progress > maxProgress)
        {
            maxProgress = progress;
        }

        // Kiểm tra nếu Player đã vượt qua Goal (tiến trình đạt 1)
        if (progress >= 0.999f)
        {
            goalReached = true; // Đặt cờ để dừng cập nhật
            CelebrateAndChangeScene(); // Gọi chức năng ăn mừng và chuyển scene
        }
    }

    private void CelebrateAndChangeScene()
    {
        SoundManager.Instance.PlayVFXSound(3);
        // Tạo hiệu ứng ăn mừng
        for (int i = 0; i < 5; i++)
        {
            // Lấy vị trí ngẫu nhiên trong tầm nhìn của camera
            Vector3 randomViewportPosition = new Vector3(
                Random.Range(0.1f, 0.9f), // Vị trí X trong viewport
                Random.Range(0.1f, 0.9f), // Vị trí Y trong viewport
                Random.Range(5f, 15f)    // Khoảng cách Z trong không gian camera
            );

            // Chuyển vị trí từ viewport sang world space
            Vector3 worldPosition = mainCamera.ViewportToWorldPoint(randomViewportPosition);

            // Tạo hiệu ứng tại vị trí tính toán được
            Instantiate(celebrationEffectPrefab, worldPosition, Quaternion.identity);
        }

        // Chuyển sang scene tiếp theo sau 3 giây
        StartCoroutine(NextSceneAfterDelay(3f));
    }

    private IEnumerator NextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0; // Nếu không có scene kế tiếp, load scene đầu tiên
            UIManager.Instance.CloseUI<GamePlayCanvas>(0.2f);
            UIManager.Instance.OpenUI<StartCanvas>();
        }
        SceneManager.LoadScene(nextSceneIndex);
    }


}
