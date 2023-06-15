using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    //Переменные для работы
    public static GameManager instance;

    public Board board;
    public GameObject[] gameplayUIElements, gameOverUIElements;
    public GameObject additionUI, shuffleUI, nextRoundUI;
    [HideInInspector] public int score, totalScore,  scoreRoundGoal;
    [HideInInspector] public float timer;
    [HideInInspector] public bool isSelected;
    private int selectedXCor_1, selectedYCor_1, selectedXCor_2, selectedYCor_2;
    private bool isSwitching = false;
    private bool gameover = false;
    private const int ROUND_GOAL_INCREASE = 35;
    private const int RESET_TIMER = 120; //секунды

    void Awake() {
        instance = this;
        Setup();
    }

    void Update()
    {
        Timer();
    }

    //Начальные условия для игры
    void Setup() 
    {
        timer = RESET_TIMER;
        isSelected = false;
    }

    //Работа с таймером, если таймер равен 0, то игра закончилась.
    void Timer()
    {
        if (timer <= 0 && !gameover)
        {
            GameOver();
        } 
        else 
        {
            timer -= Time.deltaTime;
        }
    }

    //Перемещение
    public void CheckIfPossibleMove() 
    {
        if (PerpendicularMove() && !isSwitching) 
        {
            if (board.CheckIfSwitchIsPossible(selectedXCor_1, selectedYCor_1, selectedXCor_2, selectedYCor_2, true)) 
            {
                AudioManager.instance.Play("Swap");
                board.MakeSpriteSwitch(selectedXCor_1, selectedYCor_1, selectedXCor_2, selectedYCor_2);
                isSwitching = true;
                ResetCoordinates();
                isSelected = false;
            }
        }
    }

    //Исключение в которых нельзя делать по диагонали
    bool PerpendicularMove()
    {
        if ((selectedXCor_1 + 1 == selectedXCor_2 && selectedYCor_1 == selectedYCor_2) ||           //право
                   (selectedXCor_1 - 1 == selectedXCor_2 && selectedYCor_1 == selectedYCor_2) ||    //лево
                   (selectedXCor_1 == selectedXCor_2 && selectedYCor_1 + 1 == selectedYCor_2) ||    //верх
                   (selectedXCor_1 == selectedXCor_2 && selectedYCor_1 - 1 == selectedYCor_2)) //вниз
        { 
            return true;
        } 
        else
        {
            return false;
        }
    }

    //Посчет баллов
    public void AddScore(int sequenceCount, int combo = 1)
    {
        score += (sequenceCount * combo);
        totalScore += (sequenceCount * combo);
        UI.instance.AdditionPopUp(additionUI, 1.0f, (sequenceCount * combo));
    }

    public void SetIsSelected(bool status) 
    {
        AudioManager.instance.Play("Select");
        isSelected = status;
    }

    public void SetIsSwitching(bool status)
    {
        isSwitching = status;
    }

    public void SetSelectedCoordinates(bool firstSelection, int x, int y)
    {
        if (firstSelection) 
        {
            selectedXCor_1 = x;
            selectedYCor_1 = y;
        }
        else 
        {
            selectedXCor_2 = x;
            selectedYCor_2 = y;
        }
    }

    void ResetCoordinates() 
    {
        selectedXCor_1 = 0;
        selectedYCor_1 = 0;
        selectedXCor_2 = 0;
        selectedYCor_2 = 0;
    }

    void GameOver() 
    {
        gameover = true;
        board.DeactivateBoard();
        UI.instance.DisplayGameOverText();
        foreach (var item in gameplayUIElements) item.SetActive(false);
        foreach (var item in gameOverUIElements) item.SetActive(true);
    }

    public void ButtonRestartGame() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ButtonReturnMenu() 
    {
        SceneManager.LoadScene(0);
    }
}

