using Helper;
using R3;
using System;
using System.Collections.Generic;

public enum GameState
{
    Title = 0,
    StageSelect = 1,
    GamePlay = 2,
    End = 3,
}

public enum PlayState
{
    Play,
    Pause,
}

public static class Data
{
    // ---------------------------- Property
    public static SceneName[] SceneNames { get; } = (SceneName[])Enum.GetValues(typeof(SceneName));
    public static SceneName CurrentScene { get; private set; } = SceneName.Stage01;
    public static bool IsInit { get; private set; } = false;
    public static Dictionary<EnemyType, EnemySO.Param> EnemyDict { get; private set; }

    private static readonly ReactiveProperty<GameState> _gameState = new(global::GameState.Title);
    public static ReadOnlyReactiveProperty<GameState> GameState => _gameState;

    private static readonly ReactiveProperty<PlayState> _playState = new(global::PlayState.Play);
    public static ReadOnlyReactiveProperty<PlayState> PlayState => _playState;

    // ---------------------------- Field



    // ---------------------------- PublicMethod
    public static void Init(EnemySO enemy)
    {
        EnemyDict = enemy.GetDict();
        IsInit = true;
    }

    public static void SetCurrentScene(SceneName scene)
    {
        CurrentScene = scene;
    }

    public static void SetGameState(GameState state)
    {
        _gameState.Value = state;
    }

    public static void SetPlayState(PlayState playState)
    {
        _playState.Value = playState;
    }


    // ---------------------------- PrivateMethod



}