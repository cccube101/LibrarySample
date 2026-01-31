using Cysharp.Threading.Tasks;
using Helper;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // ---------------------------- SerializeField
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _groundRayLength;
    [SerializeField] private float _wallRayLength;

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _jumpForce;

    // ---------------------------- Field
    private readonly ReactiveProperty<float> _hp = new(200);
    public ReadOnlyReactiveProperty<float> Hp => _hp;

    private Transform _tr;
    private Rigidbody2D _rb;

    private float _dir = 0f;
    private bool _groundRay = false;
    private bool _wallRay = false;

    // ---------------------------- UnityMessage
    private void OnGUI()
    {
        var pos = C3Logger.LogParam.pos;
        var style = C3Logger.LogParam.style;
        GUI.TextField(pos[0], $"dir:{_dir} ground:{_groundRay} wall:{_wallRay}", style);
    }

    private void Awake()
    {
        _tr = transform;
        _rb = _tr.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        InputObserve();
    }

    private void Update()
    {
        RayUpdate();
    }

    private void FixedUpdate()
    {
        MoveUpdate();
    }

    private void OnEnable()
    {
        Input.SetEnable();
    }

    private void OnDisable()
    {
        Input.SetDisable();
    }



    // ---------------------------- PublicMethod
    public void SetPosition(Vector2 pos) => _tr.position = pos;

    public void AddHP(float add)
    {
        _hp.Value += add;
    }

    public void SubHP(float sub)
    {
        _hp.Value -= sub;
    }



    // ---------------------------- PrivateMethod
    private void InputObserve()
    {
        Input.MoveDir.Subscribe(dir =>
        {
            _dir = dir.x;
        })
        .AddTo(this);

        Input.OnJump.Subscribe(phase =>
        {
            if (phase == InputActionPhase.Performed)
            {
                _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            }
        })
        .AddTo(this);

        Input.OnFire.Subscribe(phase =>
        {
            if (phase == InputActionPhase.Performed)
            {
                // Fire action
            }
        })
        .AddTo(this);
    }

    private void RayUpdate()
    {
        var pos = _tr.position;

        _groundRay = CastRay(Vector2.down, _groundRayLength);

        var hitLeft = CastWallRay(Vector2.left, _dir < 0);
        var hitRight = CastWallRay(Vector2.right, _dir > 0);
        _wallRay = hitLeft || hitRight;

        bool CastRay(Vector2 dir, float length)
        {
            var hit = Physics2D.Raycast(pos, dir, length, _groundMask);
            Debug.DrawRay(pos, dir * length, Color.red);
            return hit;
        }

        bool CastWallRay(Vector2 dir, bool shouldStop)
        {
            var hit = CastRay(dir, _wallRayLength);
            if (!hit) return false;
            if (shouldStop && !_groundRay) _dir = 0f;
            return true;
        }
    }

    private void MoveUpdate()
    {
        _rb.linearVelocityX = _dir * _moveSpeed;
    }
}
