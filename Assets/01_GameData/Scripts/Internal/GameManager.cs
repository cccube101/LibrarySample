using Helper;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }
    // ---------------------------- SerializeField
    [SerializeField] private EnemySO _enemySO;
    [SerializeField] private Player _player;

    // ---------------------------- Field
    public Player Player => _player;
    public GameData GameData { get; private set; } = new();



    // ---------------------------- UnityMessage
    private async void Awake()
    {
        Inst = this;
        Data.Init(_enemySO);
        await SceneManager.LoadSceneAsync((int)Data.CurrentScene, LoadSceneMode.Additive);
    }

    // ---------------------------- PublicMethod



    // ---------------------------- PrivateMethod



}
