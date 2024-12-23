using DG.Tweening;
using UnityEngine;

public class ButtonAnimController : MonoBehaviour
{
    [SerializeField] private Vector3 _originalPosition; // Vị trí gốc của button
    [SerializeField] private Tween _yoyoTween; // Tween cho animation Yoyo
    [SerializeField] private DOTweenAnimation _moveAnimation; // DOTweenAnimation cho Move

    private void Awake()
    {
        _originalPosition = transform.position;

        // Tìm Move Animation trong component DOTweenAnimation
        _moveAnimation = GetComponents<DOTweenAnimation>()[1];

        // Tạo Tween Yoyo (thay vì dùng DOTweenAnimation trực tiếp)
        _yoyoTween = transform.DOScale(new Vector3(1.1f, 1.1f, 1), 1f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetAutoKill(false)
            .Pause(); // Không tự động chạy

        StartYoyo();
    }

    private void StartYoyo()
    {
        _yoyoTween?.Restart();
    }

    private void StopYoyo()
    {
        _yoyoTween?.Pause();
    }

    public void OnButtonClick()
    {
        StopYoyo(); // Dừng hiệu ứng Yoyo

        // Khởi động Move Animation
        _moveAnimation.DORestartById("Move");
        
    }

    private void OnMoveComplete()
    {
        gameObject.SetActive(false); // Ẩn object sau khi Move
    }

    private void OnEnable()
    {
        transform.position = _originalPosition; // Reset vị trí
        StartYoyo(); // Restart Yoyo
    }
}

