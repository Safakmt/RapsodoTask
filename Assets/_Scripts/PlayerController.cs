using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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
    [SerializeField]
    private Slider _slider;
    [SerializeField]
    private TextMeshProUGUI _timeText;
    [SerializeField]
    private TextMeshProUGUI _scoreText;

    public float maxTime = 30f; // Oyun süresi (saniye)
    private string LIFT = "Lift";
    private string IDLE = "Idle";
    private string RUN = "Run";
    private int _currentRound = 0;
    private float remainingTime;
    private int totalScore = 0;
    private Transform currentTarget = null;
    private List<Ball> _balls = new List<Ball>();
    private PlayerState _currentState;
    enum PlayerState
    {
        Idle,
        Collecting,
        Returning,
        Finished,
        Lifting
    }
    void Start()
    {
        remainingTime = maxTime;
        _timeText.text = maxTime.ToString();
        _currentState = PlayerState.Collecting;
    }

    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            CalculateTime();
        }
        else if( _currentState != PlayerState.Idle) {
            _currentState = PlayerState.Idle;    
        }


        if (_currentState == PlayerState.Idle) { 
            _animator.SetTrigger(IDLE);
            _agent.isStopped = true;
        }

        if (_currentState == PlayerState.Collecting && currentTarget == null) {
            currentTarget = FindBestBall();
            if (currentTarget != null)
            {
                _agent.SetDestination(currentTarget.position);
                _animator.SetTrigger(RUN);
            }
            else
            {
                _currentState = PlayerState.Idle;
            }
        }

        if (_currentState == PlayerState.Returning && currentTarget == null) { 
            _agent.SetDestination(_golfCart.position);
            currentTarget = _golfCart;
            _animator.SetTrigger(RUN);
        }
        if (_currentState == PlayerState.Lifting) return;
        





    }

    void CalculateTime()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        _slider.value = remainingTime / maxTime;
        if(minutes<0) minutes = 0;
        if(seconds<0) seconds = 0;
        _timeText.text = minutes + ":" + seconds;
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

            if (value > bestValue)
            {
                bestValue = value;
                bestBall = ball.transform;
            }
        }

        return bestBall;
    }

    bool CanFinish()
    {
        float distance = Vector3.Distance(transform.position, _golfCart.position);
        float timeToReach = distance / _agent.speed;
        return timeToReach <= remainingTime - 1;
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
                _currentState = PlayerState.Lifting;
                StartCoroutine(AnimationWait(ball,other));

            }
        }

        if (other.CompareTag("GolfCart") && currentTarget == _golfCart)
        {
            _scoreText.text = totalScore.ToString();
            _currentRound = 0;
            _currentState = PlayerState.Collecting;
            currentTarget = null;
        }
    }

    public void Getballs(List<Ball> ballList)
    {
        _balls = ballList;
    }
    
    private IEnumerator AnimationWait(Ball ball,Collider other)
    {
        _agent.isStopped = true;
        _animator.SetTrigger(LIFT);
        yield return new WaitForSeconds(1.7f);
        totalScore += ball.score;
        _balls.Remove(ball);
        Destroy(other.gameObject);
        _currentRound++;

        if (_currentRound >= maxRound || !CanFinish())
        {
            _currentState = PlayerState.Returning;
            _agent.isStopped = false;

        }
        else
        {
            _currentState = PlayerState.Collecting;
            _agent.isStopped = false;
        }

    }
}
