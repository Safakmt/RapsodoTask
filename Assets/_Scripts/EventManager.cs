using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager 
{
    public static event Action OnGameStart;
    public static void GameStart() => OnGameStart?.Invoke();
}
