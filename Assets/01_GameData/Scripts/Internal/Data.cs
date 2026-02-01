using Helper;
using R3;
using System;

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

    private static readonly ReactiveProperty<GameState> _gameState = new(global::GameState.Title);
    public static ReadOnlyReactiveProperty<GameState> GameState => _gameState;

    private static readonly ReactiveProperty<PlayState> _playState = new(global::PlayState.Play);
    public static ReadOnlyReactiveProperty<PlayState> PlayState => _playState;

    // ---------------------------- Field



    // ---------------------------- PublicMethod
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