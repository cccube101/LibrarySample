using Helper;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSetter : MonoBehaviour
{
    // ---------------------------- UnityMessage
    private async void Start()
    {
        Data.SetGameState(GameState.GamePlay);
        await SceneManager.LoadSceneAsync((int)SceneName.BaseScene, LoadSceneMode.Additive);
        GameManager.Inst.Player.SetPosition(transform.position);
        Destroy(gameObject);
    }
}