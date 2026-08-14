using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    
    public event Action<int> OnScoreChanged;
    
    public int Score  { get; private set; }

    private void Awake() => Instance = this;

    public void AddScore(int amount)
    {
        Score += amount;
        OnScoreChanged?.Invoke(Score);
    }
}