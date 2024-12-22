using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float jumpForceHigh = 12f; // Lực nhảy cao khi vuốt lên
    public float jumpForceFlip = 8f; // Lực nhảy lật khi double jump
    public float jumpForceWall = 10f; // Lực nhảy vượt tường khi vuốt từ phải sang trái
    public float dashSpeed = 20f; // Lực dash khi vuốt trái hoặc phải
    public ThirdPCamera thirdP;
    private Rigidbody _rb;
    [SerializeField] private Animator _animator; // Animator để thay đổi trigger animation
    private bool _isGrounded = true; // Kiểm tra trạng thái trên mặt đất
    private bool _canDoubleJump = false; // Kiểm tra xem có thể thực hiện double jump không
    private Collider _collider;
    private Vector2 _startTouchPosition;
    private Vector2 _endTouchPosition;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider>();
    }
    private void Start()
    {
        
        if(thirdP.isTransitioning)
        {
            _animator.SetTrigger("idle");
        }
    }

    private void Update()
    {
        // Di chuyển liên tục về phía trước

        _animator.SetBool("isGround", _isGrounded);
        
        _animator.SetBool("idle",thirdP.HasIntro);
        // Xử lý thao tác vuốt
        HandleSwipe();
    }
    private void FixedUpdate()
    {
       
        transform.Translate(Vector3.forward * moveSpeed * Time.fixedDeltaTime);
   

        
    }
    private void HandleSwipe()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _startTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                _endTouchPosition = touch.position;

                Vector2 swipeDelta = _endTouchPosition - _startTouchPosition;

                if (swipeDelta.magnitude > 50) // Đảm bảo vuốt đủ dài
                {
                    // Vuốt lên - Nhảy
                    if (swipeDelta.y > Mathf.Abs(swipeDelta.x) && swipeDelta.y > 0)
                    {
                        if (_isGrounded)
                        {
                            HighJump(); // Nhảy cao lần đầu
                        }
                        else if (_canDoubleJump)
                        {
                            DoubleJump(); // Double jump
                        }
                    }
                    // Vuốt xuống - Santo
                    else if (-swipeDelta.y > Mathf.Abs(swipeDelta.x) && swipeDelta.y < 0)
                    {
                        FlipJump();
                    }
                    // Vuốt từ phải sang trái - Nhảy vượt tường
                    else if (-swipeDelta.x > Mathf.Abs(swipeDelta.y) && swipeDelta.x < 0)
                    {
                        WallJump();
                    }
                    // Vuốt từ trái sang phải - Dash
                    else if (swipeDelta.x > Mathf.Abs(swipeDelta.y) && swipeDelta.x > 0)
                    {
                        Dash();
                    }
                }
            }
        }
    }

    public void activeCollider()
    {
         _collider.enabled = true;
    }
    public void unActiveCollider()
    {
        _collider.enabled = false;
    }
    public void horRotateCollider()
    {
        if (_collider != null && _collider is CapsuleCollider capsule)
        {
            capsule.direction = 2; // 1 là trục Z
            Vector3 center = capsule.center;
            center.y = 0.012f; // Gán giá trị mới
            capsule.center = center;
            Debug.Log("CapsuleCollider direction set to Y-axis.");
        }
        else
        {
            Debug.LogWarning("Collider is either null or not a CapsuleCollider.");
        }
    }
    public void verRotateCollider()
    {
        if (_collider != null && _collider is CapsuleCollider capsule)
        {
            capsule.direction = 1; // 1 là trục Y
            Debug.Log("CapsuleCollider direction set to Y-axis.");
            Vector3 center = capsule.center;
            center.y = 0.03f; // Gán giá trị mới
            capsule.center = center;
        }
        else
        {
            Debug.LogWarning("Collider is either null or not a CapsuleCollider.");
        }
    }
    public void XRotateCollider()
    {
        if (_collider != null && _collider is CapsuleCollider capsule)
        {
            capsule.direction = 0; // 0 là trục X
            Debug.Log("CapsuleCollider direction set to X-axis.");
        }
       
    }



    private void HighJump()
    {
        if (_isGrounded)
        {
            _isGrounded = false;
            _canDoubleJump = true; // Sau lần nhảy đầu tiên, cho phép double jump
           
            _animator.SetTrigger("jump"); // Chuyển sang animation nhảy cao
        }
        
    }
    private void Jump()
    {
        _rb.AddForce(new Vector3(0, 1, 1) * jumpForceHigh, ForceMode.Impulse);

    }
    private void DoubleJump()
    {
        _canDoubleJump = false; // Không thể double jump thêm lần nữa
        _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z); // Reset vận tốc Y để lần nhảy thứ 2 mượt hơn
        _rb.AddForce(new Vector3(0, 1, 1) * jumpForceFlip, ForceMode.Impulse);
        _animator.SetTrigger("santo"); // Chuyển sang animation Santo
    }

    private void FlipJump()
    {
        if (_isGrounded)
        {
            _rb.AddForce(new Vector3(0, 1, 1) * jumpForceFlip, ForceMode.Impulse);
            _animator.SetTrigger("santo"); // Chuyển sang animation Santo
            _isGrounded = false;
   /*         XRotateCollider();*/
        }
    }

    private void WallJump()
    {
        if (_isGrounded)
        {
            _rb.AddForce(new Vector3(0, 1, 1) * jumpForceWall, ForceMode.Impulse);
            _animator.SetTrigger("overWall"); // Chuyển sang animation vượt tường
            _isGrounded = false;
         
        }
    }

    private void Dash()
    {
        _rb.AddForce(Vector3.forward * dashSpeed, ForceMode.Impulse);
        _animator.SetTrigger("dash"); // Chuyển sang animation Dash
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
            _canDoubleJump = false; // Reset khả năng double jump khi chạm đất
        }
    }
   
}
