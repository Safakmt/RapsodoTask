using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public TextMeshProUGUI scoreUi;
    public int score;

    public void UpdatePoint(int point)
    {
        score = point;
        scoreUi.text = point.ToString();
    }

    
}
