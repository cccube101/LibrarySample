using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }
    // ---------------------------- SerializeField
    [SerializeField] private Player _player;

    // ---------------------------- Field
    public Player Player => _player;
    public GameData GameData { get; private set; } = new();

    // ---------------------------- UnityMessage
    private void Awake()
    {
        Inst = this;
    }

    // ---------------------------- PublicMethod





    // ---------------------------- PrivateMethod



}
