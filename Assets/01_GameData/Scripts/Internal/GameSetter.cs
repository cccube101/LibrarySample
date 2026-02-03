using Helper;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSetter : MonoBehaviour
{
    // ---------------------------- UnityMessage
    private void Awake()
    {
        if (!Data.IsInit)
        {
            var name = SceneManager.GetActiveScene().name;
            if (Enum.TryParse<SceneName>(name, out var scene))
            {
                Data.SetCurrentScene(scene);
            }
            else
            {
                C3Logger.LogE("ステージ読み込み不可");
            }
            Data.SetGameState(GameState.GamePlay);
            SceneManager.LoadScene((int)SceneName.BaseScene);
            return;
        }
        GameManager.Inst.Player.SetPosition(transform.position);
        Destroy(gameObject);
    }
}