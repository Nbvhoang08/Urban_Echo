using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPCamera : MonoBehaviour
{    
    public Transform target; // Nhân vật hoặc đối tượng cần theo dõi
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public bool HasIntro;
    public bool isTransitioning = false; // Để kiểm soát trạng thái chuyển góc quay
    private Quaternion initialRotation;
    private Quaternion targetRotation;
    [SerializeField] private GameManager gameManager;
    public PlayerMove player;
    private void Start()
    {
        // Đặt góc nhìn ban đầu của camera (quay về phía trước của player)
        if(HasIntro)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            isTransitioning = true;
        }else
        {
            Vector3 endPosition = target.position + offset;
            transform.rotation = Quaternion.Euler(0, -90, 0);
            // Lerp vị trí
            transform.position =  endPosition;
        }
        
    }

    private void FixedUpdate()
    {
        if (!gameManager.goalReached && !player.Die)
        {
            if (!isTransitioning)
            {
                // Camera di chuyển theo target khi không còn trong trạng thái chuyển góc quay
                Vector3 desiredPosition = target.position + offset;
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                transform.position = smoothedPosition;
            }
        }
        else
        {
            Camera.main.transform.LookAt(target);
        }
       
    }

    public void StartTransition()
    {
        // Bắt đầu chuyển góc quay
        initialRotation = transform.rotation;
        targetRotation = Quaternion.Euler(0, -90, 0); // Góc Y = -90
        StartCoroutine(TransitionToTargetRotation());
    }

    private System.Collections.IEnumerator TransitionToTargetRotation()
    {
        isTransitioning = true;
        float duration = 2f; // Thời gian chuyển đổi góc quay (giây)
        float elapsedTime = 0f;
        HasIntro = false;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = target.position + offset;

        while (elapsedTime < duration)
        {
            // Lerp góc quay
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / duration);
            // Lerp vị trí
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null; // Đợi frame tiếp theo
        }

        // Đảm bảo camera đặt đúng vị trí cuối cùng sau khi hoàn tất
        transform.rotation = targetRotation;
        transform.position = endPosition;
        isTransitioning = false;
    }
}
