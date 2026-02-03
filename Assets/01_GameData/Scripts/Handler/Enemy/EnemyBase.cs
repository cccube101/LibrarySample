using Helper;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [HorizontalLine("Base")]
    [SerializeField] protected EnemyType _type;
    // ---------------------------- Field
    protected EnemySO.Param _param;
    protected GameObject _obj;
    protected Transform _tr;

    // ---------------------------- UnityMessage
    protected virtual void Awake()
    {
        _obj = gameObject;
        _tr = transform;
    }

    private void Start()
    {
        _param = Data.EnemyDict[_type];
    }

    // ---------------------------- PublicMethod


    // ---------------------------- PrivateMethod

}