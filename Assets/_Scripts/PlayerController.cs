using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent _agent;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Transform _targetTransform;
    [SerializeField]
    private int maxRound = 5;
    [SerializeField]
    private Transform _golfCart;

    public float maxTime = 30f; // Oyun süresi (saniye)
    private string LIFT = "Lift";
    private string IDLE = "Idle";
    private string RUN = "Run";
    private int _currentRound = 0;
    private float remainingTime;
    private int totalScore = 0;
    private Transform currentTarget = null;
    private List<Ball> _balls = new List<Ball>();
    void Start()
    {
        remainingTime = maxTime;

    }

    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;

            if (currentTarget == null || HasReachedTarget())
            {
                currentTarget = FindBestBall();
                if (currentTarget != null)
                {
                    _agent.SetDestination(currentTarget.position);
                    _animator.SetTrigger(RUN);
                }
            }
        }

    }


    Transform FindBestBall()
    {
        Transform bestBall = null;
        float bestValue = -1;

        foreach (Ball ball in _balls)
        {
            if (ball == null) continue; 

            float distance = Vector3.Distance(transform.position, ball.transform.position);
            int score = ball.GetComponent<Ball>().score;

            //float groupAdvantage = CalculateGroupAdvantage(ball.transform);

            float value = (score / distance);

            if (value > bestValue && CanReach(ball.transform))
            {
                bestValue = value;
                bestBall = ball.transform;
            }
        }

        return bestBall;
    }

    bool CanReach(Transform ball)
    {
        float distance = Vector3.Distance(transform.position, ball.position);
        float timeToReach = distance / _agent.speed;
        return timeToReach <= remainingTime;
    }


    //float CalculateGroupAdvantage(Transform ball)
    //{
    //    float advantage = 0f;
    //    float groupRadius = 5f; 

    //    foreach (Ball otherBall in _balls)
    //    {
    //        if (otherBall == null || otherBall == ball) continue;

    //        float distanceToOther = Vector3.Distance(ball.position, otherBall.transform.position);
    //        if (distanceToOther <= groupRadius)
    //        {
    //            int otherScore = otherBall.GetComponent<Ball>().score;
    //            advantage += otherScore / distanceToOther; 
    //        }
    //    }

    //    return advantage;
    //}

    // Hedefe ulaþýlýp ulaþýlmadýðýný kontrol et
    bool HasReachedTarget()
    {
        if (currentTarget == null) return true;
        float distance = Vector3.Distance(transform.position, currentTarget.position);
        return distance < _agent.stoppingDistance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Ball>(out Ball ball))
        {
            if (ball.transform == currentTarget)
            {
                totalScore += ball.score;
                _balls.Remove(ball);
                Destroy(other.gameObject);
                _currentRound++;
                if (_currentRound >= maxRound)
                {
                    currentTarget = _golfCart;
                }
            }
        }

        if (other.CompareTag("GolfCart") && currentTarget == _golfCart)
        {
            _currentRound = 0;
        }
    }

    public void Getballs(List<Ball> ballList)
    {
        _balls = ballList;
    }
}
