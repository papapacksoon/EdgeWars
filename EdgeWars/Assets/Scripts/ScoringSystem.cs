using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ScoringSystem : MonoBehaviour
{

    public Text playerOneScore;

    public void PlayerOneHit(int score)
    {
        playerOneScore.text = "Score : " + score;
    }
}


