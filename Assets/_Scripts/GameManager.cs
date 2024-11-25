using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private RandomBallGeneration _randomBallGeneration;
    [SerializeField]
    private GameObject _player;
    [SerializeField]
    private PlayerController _playerController;
    void Start()
    {
        _randomBallGeneration.SpawnBalls();
        _randomBallGeneration.AssignPointsToBalls();
        List<Ball> ballList = _randomBallGeneration.GetBalls();
        _playerController.Getballs(ballList);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
