﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    //Переменные для обработки
    public GameObject gem;
    public Sprite[] gemsSprites;
    private int[,] indexGrid;
    private int comboCount = 1;
    private bool reset = true;
    private bool combo = false;
    private Vector3 initialPos;
    private GameObject[,] gems;
    private List<Dictionary<string, int>> matchCoordinates;

    private const int GRID_WIDTH = 9;
    private const int GRID_HEIGHT = 9;
    private const float TILE_SIZE = 9;
    private const float WAIT_TIME_MOVE = 0.5f;

    void Awake()
    {
        Setup();
    }

    void Start()
    {
        SetupBoard(false);
    }

    //Начальные условия
    void Setup() 
    {
        initialPos = gameObject.transform.position;
        indexGrid = new int[GRID_WIDTH, GRID_HEIGHT];
        gems = new GameObject[GRID_WIDTH, GRID_HEIGHT];
        matchCoordinates = new List<Dictionary<string, int>>();
    }

    //При том случае когда хода нет
    void SetupBoard(bool shuffle) 
    {
        while (reset)
        {
            for (int x = 0; x < indexGrid.GetLength(0); x++) 
            {
                for (int y = 0; y < indexGrid.GetLength(1); y++) 
                {
                    indexGrid[x, y] = Random.Range(0, gemsSprites.GetLength(0));
                }
            }
            if (!CheckMatches(false) && CheckIfThereArePossibleMoves()) 
            {
                reset = false;
            }
        }
        for (int x = 0; x < indexGrid.GetLength(0); x++) 
        {
            for (int y = 0; y < indexGrid.GetLength(1); y++) 
            {
                if (!shuffle) 
                {
                    GameObject tempGemObj = Gem.Start(gem, initialPos, x, y, TILE_SIZE, gameObject.transform);
                    gems[x, y] = tempGemObj;
                }
                gems[x, y].GetComponent<SpriteRenderer>().sprite = gemsSprites[indexGrid[x, y]];
                Gem tempGem = gems[x, y].GetComponent<Gem>();
                tempGem.SetCoordinates(new Dictionary<string, int>() { { "x", x }, { "y", y } });
            }
        }
    }

    //Одигаквые коминации, чтоб не было их
    bool CheckMatches(bool isPlayerMove) 
    {
        bool ifMatch = false;
        //По горизонтали
        for (int x = 0; x < indexGrid.GetLength(0) - 2; x++) 
        {
            for (int y = 0; y < indexGrid.GetLength(1); y++) 
            {
                int current = indexGrid[x, y];
                if (current == indexGrid[x + 1, y] && current == indexGrid[x + 2, y]) 
                {
                    if (isPlayerMove) 
                    {
                        SaveMatchCoordinates(x, y, true);
                    }
                    ifMatch = true;
                }
            }
        }
        //По вертикали
        for (int x = 0; x < indexGrid.GetLength(0); x++) 
        {
            for (int y = 0; y < indexGrid.GetLength(1) - 2; y++) 
            {
                int current = indexGrid[x, y];
                if (current == indexGrid[x, y + 1] && current == indexGrid[x, y + 2]) 
                {
                    if (isPlayerMove) 
                    {
                        SaveMatchCoordinates(x, y, false);
                    }
                    ifMatch = true;
                }
            }
        }
        return ifMatch;
    }

    //Изменения
    public void MakeSpriteSwitch(int x_1, int y_1, int x_2, int y_2) 
    {
        Sprite spriteTemp_1 = gems[x_1, y_1].GetComponent<SpriteRenderer>().sprite;
        Sprite spriteTemp_2 = gems[x_2, y_2].GetComponent<SpriteRenderer>().sprite;
        gems[x_1, y_1].GetComponent<SpriteRenderer>().sprite = spriteTemp_2;
        gems[x_2, y_2].GetComponent<SpriteRenderer>().sprite = spriteTemp_1;
        gems[x_1, y_1].GetComponent<Gem>().ResetSelection();
        StartCoroutine(TriggerMatch());
    }

    //Проверка
    public bool CheckIfSwitchIsPossible(int x_1, int y_1, int x_2, int y_2, bool isPlayerMove)
    {
        int temp_1 = indexGrid[x_1, y_1];
        int temp_2 = indexGrid[x_2, y_2];
        indexGrid[x_1, y_1] = temp_2;
        indexGrid[x_2, y_2] = temp_1;
        if (CheckMatches(isPlayerMove))
        {
            if (isPlayerMove)
            {
                return true;
            } 
            else 
            {
                indexGrid[x_1, y_1] = temp_1;
                indexGrid[x_2, y_2] = temp_2;
                return true;
            }
        } 
        else
        {
            indexGrid[x_1, y_1] = temp_1;
            indexGrid[x_2, y_2] = temp_2;
        }
        return false;
    }

    //Сохранение совпадений
    void SaveMatchCoordinates(int x, int y, bool isHorizontal) 
    {
        matchCoordinates.Add(new Dictionary<string, int>() { { "x", x }, { "y", y } });
        if (isHorizontal) 
        {
            for (int i = 1; i < (GRID_WIDTH - x); i++) 
            {
                if (indexGrid[x, y] == indexGrid[x + i, y]) 
                {
                    matchCoordinates.Add(new Dictionary<string, int>() { { "x", x + i }, { "y", y } });
                } 
                else
                {
                    break;
                }
            }
        } 
        else 
        {
            for (int i = 1; i < (GRID_HEIGHT - y); i++) 
            {
                if (indexGrid[x, y] == indexGrid[x, y + i]) 
                {
                    matchCoordinates.Add(new Dictionary<string, int>() { { "x", x }, { "y", y + i } });
                } 
                else 
                {
                    break;
                }
            }
        }
    }

    //Обмена, переммещение и добавление новых объектов сверху
    IEnumerator TriggerMatch() 
    {
        foreach (Dictionary<string, int> unit in matchCoordinates) 
        {
            gems[unit["x"], unit["y"]].GetComponent<SpriteRenderer>().sprite = null;
        }
        GameManager.instance.AddScore(matchCoordinates.Count, comboCount);
        matchCoordinates.Clear();
        yield return new WaitForSeconds(WAIT_TIME_MOVE);
        AudioManager.instance.Play("Clear");
        MoveGemsDownwards();
        yield return new WaitForSeconds(WAIT_TIME_MOVE);
        combo = CheckMatches(true);
        if (combo)
        {
            comboCount += 1;
            //Recalls the method due to combo
            StartCoroutine(TriggerMatch());
        } 
        else 
        {
            CheckIfThereArePossibleMoves();
            GameManager.instance.SetIsSwitching(false);
            combo = false;
            comboCount = 1;
        }
    }

    //Заполение
    void MoveGemsDownwards() 
    {
        bool hasNull = true;
        while (hasNull) 
        {
            hasNull = false;
            for (int y = 0; y < GRID_HEIGHT; y++) 
            {
                for (int x = 0; x < GRID_WIDTH; x++) 
                {
                    SpriteRenderer currentGemSprite = gems[x, y].GetComponent<SpriteRenderer>();
                    if (currentGemSprite.sprite == null) 
                    {
                        hasNull = true;
                        if (y != GRID_HEIGHT - 1) 
                        {
                            SpriteRenderer aboveGemSprite = gems[x, y + 1].GetComponent<SpriteRenderer>();
                            indexGrid[x, y] = indexGrid[x, y + 1];
                            indexGrid[x, y + 1] = 7;
                            currentGemSprite.sprite = aboveGemSprite.sprite;
                            aboveGemSprite.sprite = null;

                        } 
                        else 
                        {
                            indexGrid[x, y] = Random.Range(0, gemsSprites.GetLength(0));
                            currentGemSprite.sprite = gemsSprites[indexGrid[x, y]];
                        }
                    }
                }
            }
        }
    }

    //Проверка, решения и соблюдении игры
    bool CheckIfThereArePossibleMoves()
    {
        bool ifPossibleMove = false;
        for (int x = 0; x < GRID_WIDTH; x++) 
        {
            for (int y = 0; y < GRID_HEIGHT; y++) 
            {
                try 
                {
                    ifPossibleMove = CheckIfSwitchIsPossible(x, y, x, y + 1, false); //UP
                    if (ifPossibleMove) 
                    {
                        Debug.Log("Hint  x: " + x + " y: " + y);
                        break;
                    }
                }
                catch (System.IndexOutOfRangeException) { }
                try
                {
                    ifPossibleMove = CheckIfSwitchIsPossible(x, y, x, y - 1, false); //DOWN
                    if (ifPossibleMove)
                    {
                        Debug.Log("Hint  x: " + x + " y: " + y);
                        break;
                    }
                } 
                catch (System.IndexOutOfRangeException) { }
                try 
                {
                    ifPossibleMove = CheckIfSwitchIsPossible(x, y, x + 1, y, false); //RIGHT
                    if (ifPossibleMove)
                    {
                        Debug.Log("Hint  x: " + x + " y: " + y);
                        break;
                    }
                } 
                catch (System.IndexOutOfRangeException) { }
                try
                {
                    ifPossibleMove = CheckIfSwitchIsPossible(x, y, x - 1, y, false); //LEFT
                    if (ifPossibleMove)
                    {
                        Debug.Log("Hint  x: " + x + " y: " + y);
                        break;
                    }
                } 
                catch (System.IndexOutOfRangeException) { }
            }
            if (ifPossibleMove) 
            {
                break;
            }
        }
        if (!ifPossibleMove) 
        {
            reset = true;
            SetupBoard(true);
            StartCoroutine(UI.instance.PopUpFadeAway(GameManager.instance.shuffleUI, 3.0f)); //Shuffle Pop-Up
        }
        return ifPossibleMove;
    }

    public void DeactivateBoard() 
    {
        gameObject.SetActive(false);
    }
}
