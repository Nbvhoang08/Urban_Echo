using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPCamera : MonoBehaviour
{

    public Transform target; // Nhân vật hoặc đối tượng cần theo dõi
    public float distance = 5.0f; // Khoảng cách từ Camera tới nhân vật
    public float height = 2.0f; // Độ cao của Camera
    public float horizontalOffset = 2.0f; // Độ lệch ngang để tạo góc chéo
    public float rotationSpeed = 5.0f; // Tốc độ xoay Camera
    public float followSpeed = 5.0f; // Tốc độ Camera di chuyển theo nhân vật

    private float currentRotationAngle; // Góc xoay hiện tại của Camera
    private float desiredRotationAngle; // Góc xoay mong muốn của Camera
    private Vector3 offset; // Offset tổng hợp (khoảng cách + lệch ngang)

    void LateUpdate()
    {
        if (target != null)
        {
            // Tính góc xoay mong muốn dựa trên hướng của nhân vật
            desiredRotationAngle = target.eulerAngles.y;
            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, desiredRotationAngle, rotationSpeed * Time.deltaTime);

            // Xử lý góc xoay (rotation)
            Quaternion rotation = Quaternion.Euler(0, currentRotationAngle, 0);

            // Tính toán offset với khoảng cách (distance) và độ lệch ngang (horizontalOffset)
            Vector3 distanceOffset = -Vector3.forward * distance; // Khoảng cách lùi về phía sau
            Vector3 horizontalOffsetVector = target.right * horizontalOffset; // Lệch ngang
            offset = rotation * (distanceOffset + horizontalOffsetVector); // Tích hợp offset với góc xoay

            // Áp dụng chiều cao
            Vector3 targetPosition = target.position + offset;
            targetPosition.y = target.position.y + height;

            // Di chuyển Camera tới vị trí mới (mượt mà)
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

            // Camera luôn nhìn vào nhân vật
            transform.LookAt(target);
        }
    }
}
