using System.Collections;
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
    public GameObject stunEffectPrefab; // Tham chiếu đến prefab hiệu ứng stun
    private GameObject _currentStunEffect; // Lưu hiệu ứng stun hiện tại
    private Collider _collider;
    private Vector2 _startTouchPosition;
    private Vector2 _endTouchPosition;
    public bool stun;
    public bool isJumping;
    public bool Die;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider>();
        stun = false;
        Die = false;
    }
    private void Start()
    {
        
        if(thirdP.isTransitioning)
        {
            _animator.SetTrigger("idle");
            moveSpeed = 0;
        }
    }

    private void Update()
    {
        if (Die) return;
        // Di chuyển liên tục về phía trước
        _animator.SetBool("isGround", _isGrounded);
        
        _animator.SetBool("idle",thirdP.HasIntro);
        if (!thirdP.HasIntro && thirdP.isTransitioning)
        {
            moveSpeed = 2;
        }
        else if (!thirdP.HasIntro && !thirdP.isTransitioning && !stun && !isJumping)
        {
            moveSpeed = 5;
        }
        // Xử lý thao tác vuốt
        HandleSwipe();
        if (transform.position.y <= -4)
        {
            Die = true;
            StartCoroutine(DelayOpenUI());
            _animator.SetTrigger("die");
        }
    }
    private void FixedUpdate()
    {
        if (Die) return;

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
        }   
    }
    public void verRotateCollider()
    {
        if (_collider != null && _collider is CapsuleCollider capsule)
        {
            capsule.direction = 1; // 1 là trục Y
            Vector3 center = capsule.center;
            center.y = 0.03f; // Gán giá trị mới
            capsule.center = center;
        }
       
    }
    public void XRotateCollider()
    {
        if (_collider != null && _collider is CapsuleCollider capsule)
        {
            capsule.direction = 0; // 0 là trục X
        }
       
    }

    private void HighJump()
    {
        if (_isGrounded)
        {
            _isGrounded = false;
            _canDoubleJump = true; // Sau lần nhảy đầu tiên, cho phép double jump
            moveSpeed = 0;
            _animator.SetTrigger("jump"); // Chuyển sang animation nhảy cao
            isJumping = true;
        }
        
    }
    private void Jump()
    {
        _rb.AddForce(new Vector3(0, 1.2f, 0.6f) * jumpForceHigh, ForceMode.Impulse);
    }
    private void DoubleJump()
    {
        _canDoubleJump = false; // Không thể double jump thêm lần nữa
        _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z); // Reset vận tốc Y để lần nhảy thứ 2 mượt hơn
        _rb.AddForce(new Vector3(0, 1.2f, 1) * jumpForceFlip, ForceMode.Impulse);
        _animator.SetTrigger("santo"); // Chuyển sang animation Santo
        moveSpeed = 0;
        isJumping = true;
    }

    private void FlipJump()
    {
        if (_isGrounded)
        {
            _rb.AddForce(new Vector3(0, 1, 1) * jumpForceFlip, ForceMode.Impulse);
            _animator.SetTrigger("santo"); // Chuyển sang animation Santo
            _isGrounded = false;
        }
    }

    private void WallJump()
    {
        if (_isGrounded)
        {
            _rb.AddForce(new Vector3(0, 1.6f, 1) * jumpForceWall, ForceMode.Impulse);
            _animator.SetTrigger("overWall"); // Chuyển sang animation vượt tường
            _isGrounded = false;
            isJumping = true;
            moveSpeed = 0;

        }
    }

    public void GameOver()
    {
        StartCoroutine(DelayOpenUI());
    }
    IEnumerator DelayOpenUI()
    {
        yield return new WaitForSeconds(3);
        UIManager.Instance.OpenUI<Lose>();
        Time.timeScale = 0;
    }

    private void Dash()
    {
        if (isJumping) return;
        if (stun) return;
        _rb.AddForce(Vector3.forward * dashSpeed, ForceMode.Impulse);
        _animator.SetTrigger("dash"); // Chuyển sang animation Dash
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
            _canDoubleJump = false; // Reset khả năng double jump khi chạm đất
            isJumping = false;

        }

        // Nếu va chạm với Obstacle
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Vector3 contactPoint = collision.contacts[0].point; // Lấy vị trí va chạm
            Vector3 playerPosition = transform.position;

            moveSpeed = 0;
            stun = true;
    

            // Xác định vị trí Player so với Obstacle
            bool isPlayerAboveObstacle = playerPosition.y > contactPoint.y -0.1f; // Kiểm tra nếu Player ở trên Obstacle (thêm 0.1f để tránh va chạm nhỏ)
            if (isPlayerAboveObstacle)
            {
         
                moveSpeed = 5;
                stun = false;
                _isGrounded = true;
                isJumping = false;
                return; // Không làm gì nếu Player ở trên Obstacle
            }

            // Xác định vị trí Obstacle so với Player
            bool isAbovePlayer = contactPoint.y > playerPosition.y + _collider.bounds.extents.y; // Obstacle ở trên Player

            if (isAbovePlayer)
            {
                // Nếu Obstacle ở trên, Player sẽ bị rơi xuống đất
                _rb.velocity = new Vector3(0, -jumpForceHigh, 0); // Tạo vận tốc rơi xuống
                _animator.SetTrigger("stun"); // Chuyển sang animation stun
                SoundManager.Instance.PlayVFXSound(1);
                SpawnStunEffect();
                GameOver();
            }
            else
            {
                // Nếu Obstacle ở ngang hoặc dưới, Player bị knockback
                Vector3 knockbackDirection = (playerPosition - contactPoint).normalized; // Hướng knockback
                _rb.AddForce(knockbackDirection * 8f + Vector3.back * 2f, ForceMode.Impulse); // Knockback + chút lực nhảy lên
                _animator.SetTrigger("stun"); // Chuyển sang animation stun
                SoundManager.Instance.PlayVFXSound(1);
                SpawnStunEffect();
                GameOver();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            CoinManager.Instance.AddCoins(10);
            Destroy(other.gameObject);
            SoundManager.Instance.PlayVFXSound(0);

        }
    }
    private void SpawnStunEffect()
    {
        // Nếu đã có hiệu ứng stun, không tạo thêm
        if (_currentStunEffect != null) return;
        // Xác định vị trí spawn hiệu ứng stun (trên đầu Player)
        Vector3 stunPosition = transform.position + new Vector3(0,1,1*(_collider.bounds.extents.z -1f)) ;

        // Spawn hiệu ứng stun
        _currentStunEffect = Instantiate(stunEffectPrefab, stunPosition, Quaternion.Euler(90, 0, 0));
        
        // Gắn hiệu ứng vào Player để nó di chuyển theo
        _currentStunEffect.transform.SetParent(transform);

        // Tự động hủy hiệu ứng stun sau một khoảng thời gian (nếu cần)
        Destroy(_currentStunEffect, 5f);
    }
}
