using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour 
{

    public static UI instance;

    public Slider slider;
    public Text[] roundText, scoreText, timerText, totalScoreText, finalRoundText;

    public static float score;

    int highscore;
    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        VariableText();
    }

    //Изменение текста пользовательского интерфейса во время игры
    void VariableText() 
    {
        //Результат и вывод
        highscore = GameManager.instance.score;
        scoreText[0].text = highscore.ToString();
        scoreText[1].text = highscore.ToString();
        //Подсчет рекорда и вывод его
        if (PlayerPrefs.GetInt("score") <= highscore)
            PlayerPrefs.SetInt("score", highscore);
        roundText[0].text = "Record:" + PlayerPrefs.GetInt("score").ToString();
        roundText[1].text = "Record:" + PlayerPrefs.GetInt("score").ToString();

        timerText[0].text = Mathf.Round(GameManager.instance.timer) + "";
        slider.value = GameManager.instance.timer;
    }

    public IEnumerator PopUpFadeAway(GameObject obj, float time)
    {
        obj.SetActive(true);
        Color originalcolor = obj.GetComponent<Text>().color;
        for (float t = 1.0f; t >= 0.0f; t -= Time.deltaTime / time) 
        {
            Color newColor = new Color(originalcolor.r, originalcolor.g, originalcolor.b, t);
            obj.GetComponent<Text>().color = newColor;
            yield return null;
        }
        obj.SetActive(false);
    }

    public void AdditionPopUp(GameObject obj, float time, int scoreAdd)
    {
        if (scoreAdd > 0)
        {
            obj.GetComponent<Text>().text = "+" + scoreAdd + "pts.";
        }
        StartCoroutine(PopUpFadeAway(obj, time));
    }


    public void DisplayGameOverText()
    {

        totalScoreText[0].text = GameManager.instance.totalScore + "";
        totalScoreText[1].text = GameManager.instance.totalScore + "";

        finalRoundText[0].text = PlayerPrefs.GetInt("score").ToString();//*GameManager.instance.round + "";*/
        finalRoundText[1].text = PlayerPrefs.GetInt("score").ToString();/*/ GameManager.instance.round + "";*/
    }

}
