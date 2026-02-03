using Cysharp.Threading.Tasks;
using Helper;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // ---------------------------- SerializeField
    [HorizontalLine("Param")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private int _jumpLimit;
    [SerializeField] private float _jumpForce;

    [HorizontalLine("Ray")]
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _groundRayLength;
    [SerializeField] private float _wallRayLength;

    // ---------------------------- Field
    private readonly ReactiveProperty<float> _hp = new(200);
    public ReadOnlyReactiveProperty<float> Hp => _hp;

    private Transform _tr;
    private Rigidbody2D _rb;

    private float _dir = 0f;
    private bool _groundRay = false;
    private bool _wallRay = false;
    private int _jumpCount = 0;



    // ---------------------------- UnityMessage
    private void OnGUI()
    {
        var pos = C3Logger.LogParam.pos;
        var style = C3Logger.LogParam.style;
        GUI.TextField(pos[0], $"dir:{_dir} ground:{_groundRay} wall:{_wallRay} jump:{_jumpCount}", style);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var obj = collision.gameObject;
        if (obj.TryGetComponent<IEnemy>(out var enemy))
        {
            SubHP(enemy.GetAttackDamage());
        }
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
            if (dir.x != 0)
            {
                var angle = dir.x > 0 ? Vector2.zero : new Vector2(0, 180);
                _tr.eulerAngles = angle;
            }
        })
        .AddTo(this);

        Input.OnJump.SubscribeAwait(async (phase, ct) =>
        {
            if (phase == InputActionPhase.Performed && _jumpCount < _jumpLimit)
            {
                _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
                await UniTask.WaitUntil(() => !_groundRay, cancellationToken: ct);
                _jumpCount++;
            }

        }, AwaitOperation.Drop)
        .RegisterTo(destroyCancellationToken);

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
        _groundRay = CastRay(-_tr.up, _groundRayLength);
        _wallRay = CastRay(_tr.right, _wallRayLength);
        bool CastRay(Vector2 dir, float length)
        {
            var hit = Physics2D.Raycast(pos, dir, length, _groundMask);
            Debug.DrawRay(pos, dir * length, Color.red);
            return hit;
        }

        if (_wallRay && !_groundRay)
        {
            _dir = 0;
        }
        if (_groundRay)
        {
            _jumpCount = 0;
        }
    }

    private void MoveUpdate()
    {
        _rb.linearVelocityX = _dir * _moveSpeed;
    }
}
