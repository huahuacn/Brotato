using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState 
{
    Playing,
    Paused,
    GameOver,
}

public class GameManager : PersistenSingleton<GameManager>
{
    public static GameState GameState {get => Instance.gameState; set => Instance.gameState = value; }
    [SerializeField] GameState gameState = GameState.Playing;

    public static bool Playing() => GameState == GameState.Playing;
    public static bool Paused() => GameState == GameState.Paused;
    public static bool GameOver() => GameState == GameState.GameOver;
}
