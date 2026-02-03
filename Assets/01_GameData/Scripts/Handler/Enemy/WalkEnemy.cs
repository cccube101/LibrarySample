using Helper;
using UnityEngine;

public class WalkEnemy : EnemyBase, IEnemy
{
    // ---------------------------- SerializeField
    [HorizontalLine("Param")]
    [SerializeField] private float _moveSpeed;

    [HorizontalLine("Ray")]
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _wallRayLength;

    // ---------------------------- Field
    private Rigidbody2D _rb;
    private float _dir = 1;
    private bool _wallRay;

    // ---------------------------- UnityMessage
    protected override void Awake()
    {
        base.Awake();
        _rb = _tr.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        RayUpdate();
    }

    private void FixedUpdate()
    {
        MoveUpdate();
    }

    // ---------------------------- PublicMethod
    public float GetAttackDamage()
    {
        return _param.attackDamage;
    }


    // ---------------------------- PrivateMethod
    private void RayUpdate()
    {
        var pos = _tr.position;
        _wallRay = CastRay(_tr.right, _wallRayLength);
        bool CastRay(Vector2 dir, float length)
        {
            var hit = Physics2D.Raycast(pos, dir, length, _groundMask);
            Debug.DrawRay(pos, dir * length, Color.red);
            return hit;
        }
        if (_wallRay)
        {
            _dir *= -1;
            var angle = _dir > 0 ? Vector2.zero : new Vector2(0, 180);
            _tr.eulerAngles = angle;
        }
    }

    private void MoveUpdate()
    {
        _rb.linearVelocityX = _dir * _moveSpeed;
    }
}
