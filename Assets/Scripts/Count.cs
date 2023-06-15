using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Count : MonoBehaviour
{
    [SerializeField]
    private Text text;
    private int RecordScore;
    // Start is called before the first frame update
    //void Start()
    //{
    //    RecordScore = GameManager.instance.recordScore.ToString();
    //    //text.text = GameManager.instance.score.ToString();
    //}

    // Update is called once per frame
    void Update()
    {
        //VariableText();
        text.text = PlayerPrefs.GetInt("score").ToString();
    }

    //void VariableText()
    //{
    //    //RecordScore = (int)GameManager.instance.score;
    //    //if(PlayerPrefs.GetInt("score")<= RecordScore)
    //    //{
    //    //    PlayerPrefs.SetInt("score", RecordScore);
    //    //}
    //    //text.text = PlayerPrefs.GetInt("score").ToString();
    //    text.text = GameManager.instance.score.ToString();

    //}
}
