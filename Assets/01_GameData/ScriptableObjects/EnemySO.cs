using System;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    WalkEnemy = 1,
}

[CreateAssetMenu(menuName = "SO/EnemySO")]
public class EnemySO : ScriptableObject
{
    [SerializeField] private Item[] _items;

    [Serializable]
    public class Item
    {
        public EnemyType key;
        public Param param;
    }

    [Serializable]
    public class Param
    {
        public float attackDamage;
    }

    public Dictionary<EnemyType, Param> GetDict()
    {
        var result = new Dictionary<EnemyType, Param>();
        var keyCheck = new HashSet<EnemyType>();

        foreach (var item in _items)
        {
            if (!keyCheck.Add(item.key))
            {
                throw new InvalidOperationException(
                    $"EnemySO Ç…èdï°ÇµÇΩÉLÅ[Ç™ë∂ç›ÇµÇ‹Ç∑: {item.key}");
            }

            result[item.key] = item.param;
        }

        return result;
    }
}
