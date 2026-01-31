using UnityEngine;
using Helper;

public class AttributeDemo : MonoBehaviour
{
    [HorizontalLine("Line1")]
    [Button("Push Button")]
    private void Btn()
    {
        C3Logger.LogN("Button Pressed");
    }

    [HorizontalLine("Line2", "#ff1100")]
    [SerializeField] private float _aValue;
    [HorizontalLine("Line3", "#ffd900", 20f, 50f)]
    [SerializeField] private float _bValue;
    [Disable]
    [SerializeField] private float _cValue;
    [Space()]
    [SerializeField] private float _dValue;
    [Space(20f)]
    [SerializeField] private float _eValue;
}