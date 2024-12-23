using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTerrain : MonoBehaviour
{
    public enum TerrainType
    {
        FallAfterStepped, // Rơi xuống sau khi dẫm lên
        OscillateY,       // Dao động lên xuống theo trục Y
        RotateY,          // Xoay quanh trục Y
        RotateZ           // Xoay quanh trục Z
    }

    [Header("Terrain Settings")]
    public TerrainType terrainType; // Loại địa hình
    public float fallDelay = 2f; // Độ trễ trước khi rơi (áp dụng cho FallAfterStepped)
    public float oscillateHeight = 2f; // Độ cao dao động (áp dụng cho OscillateY)
    public float oscillateDuration = 1f; // Thời gian hoàn thành một chu kỳ dao động (áp dụng cho OscillateY)
    public float rotationSpeed = 50f; // Tốc độ quay (áp dụng cho RotateY và RotateZ)

    private Rigidbody _rigidbody;
    private Vector3 _initialPosition;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        // Lưu vị trí ban đầu (dùng cho OscillateY)
        _initialPosition = transform.position;

        // Nếu là dao động, bắt đầu animation bằng DoTween
        if (terrainType == TerrainType.OscillateY)
        {
            StartOscillating();
        }
    }

    private void Update()
    {
        // Xử lý chuyển động quay quanh trục Y và Z
        if (terrainType == TerrainType.RotateY)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
        else if (terrainType == TerrainType.RotateZ)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Nếu Player chạm vào địa hình loại rơi
        if (terrainType == TerrainType.FallAfterStepped && collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(FallAfterDelay());
        }
    }

    /// <summary>
    /// Xử lý rơi xuống sau một khoảng thời gian.
    /// </summary>
    private IEnumerator FallAfterDelay()
    {
        yield return new WaitForSeconds(fallDelay);

        if (_rigidbody != null)
        {
            _rigidbody.isKinematic = false; // Bật vật lý để rơi tự do
        }
    }

    /// <summary>
    /// Bắt đầu dao động lên xuống bằng DoTween.
    /// </summary>
    private void StartOscillating()
    {
        // Animation dao động lên xuống
        transform.DOMoveY(_initialPosition.y + oscillateHeight, oscillateDuration)
            .SetEase(Ease.InOutSine) // Làm mượt chuyển động
            .SetLoops(-1, LoopType.Yoyo); // Lặp vô hạn theo kiểu Yoyo
    }
}
