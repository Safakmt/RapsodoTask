using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public TextMeshProUGUI pointUi;
    public int point;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePoint(int point)
    {
        this.point = point;
        pointUi.text = point.ToString();
    }
}
