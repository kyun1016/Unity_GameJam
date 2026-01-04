using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _walkSpeed = 3f;
    [SerializeField] private float _runSpeed = 6f;

    [Header("References")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Animator _animator;

    private Vector2 _moveInput;
    private bool _isRunning;
    
    // Animator Parameter Hashes (성능 최적화)
    private readonly int _animSpeed = Animator.StringToHash("Speed");
    private readonly int _animDirectionX = Animator.StringToHash("DirectionX");
    private readonly int _animDirectionY = Animator.StringToHash("DirectionY");

    private void Awake()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();
        if (_animator == null) _animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        UpdateAnimation();
    }

    // Input System의 "Move" 액션과 연결 (Send Messages 방식 또는 Unity Events)
    public void OnMove(InputValue value)
    {
        DevDebug.Log($"[PlayerController] Move Input: {value.Get<Vector2>()}");
        _moveInput = value.Get<Vector2>();
    }

    // Input System의 "Sprint" 액션과 연결 (Shift 키 등)
    public void OnSprint(InputValue value)
    {
        _isRunning = value.isPressed;
    }

    private void Move()
    {
        float currentSpeed = _isRunning ? _runSpeed : _walkSpeed;
        // _rb.velocity = new Vector3(_moveInput.x, 0, _moveInput.y) * currentSpeed;
        Vector3 movement = new Vector3(_moveInput.x, 0, _moveInput.y).normalized * currentSpeed;
        _rb.MovePosition(_rb.position + movement * Time.fixedDeltaTime);
    }

    private void UpdateAnimation()
    {
        if (_moveInput != Vector2.zero)
        {
            // 이동 중일 때만 방향 업데이트 (멈췄을 때 마지막 방향 유지)
            _animator.SetFloat(_animDirectionX, _moveInput.x);
            _animator.SetFloat(_animDirectionY, _moveInput.y);
            
            // Speed 값 설정 (0: Idle, 0.5: Walk, 1: Run)
            // Blend Tree 설정에 따라 값 조정 필요. 여기서는 단순화하여 전달.
            // 예: Walk=1, Run=2 라면 currentSpeed 자체를 넘겨도 됨.
            // 질문에서 'Speed' 입력에 따른 제어라고 했으므로, 0~1 정규화된 값이 아닌 실제 속도나 상태값을 넘기는 것이 일반적.
            // 여기서는 걷기=0.5, 달리기=1.0 으로 가정하고 세팅.
            float speedValue = _isRunning ? 1.0f : 0.5f;
            _animator.SetFloat(_animSpeed, speedValue);
        }
        else
        {
            // 정지 상태
            _animator.SetFloat(_animSpeed, 0f);
        }
    }
}
