using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    public int gridWidth = 9;
    public int gridHeight = 9;

    public int minesAmount = 0;
    TMP_Text minesAmountTMP;

    public TileController[,] grid;
    public List<TileController> tilesToCheck;

    bool gameOverFlag = false;
    bool deadOnMine = false;

    GameObject gameOverImg;
    TMP_Text gameOverStatus;

    void Start()
    {
        gameOverImg = GameObject.Find("ImgGameOver");
        gameOverStatus = GameObject.Find("GameOverStatus").GetComponent<TMP_Text>();

        RunNewGame();
    }

    void RunNewGame()
    {
        gameOverImg.SetActive(false);

        grid = new TileController[gridWidth, gridHeight];
        
        tilesToCheck = new List<TileController>();

        int multiplier = UnityEngine.Random.Range(1, gridWidth - 1);
        int minesCount = UnityEngine.Random.Range(1, (gridHeight - 1) * multiplier);

        minesAmount = minesCount;
        minesAmountTMP = GameObject.Find("MinesCount").GetComponent<TMP_Text>();
        minesAmountTMP.text = minesAmount.ToString();

        for (int i = 0; i < minesCount; i++)
        {
            PlaceMines();
        }

        PlaceClues();

        PlaceBlanks();
    }

    void Update()
    {
        CheckInputMouseButtonLeft();
        CheckInputMouseButtonRight();

        CheckIfWinOrLose();
    }

    void CheckInputMouseButtonLeft()
    {
        if (gameOverFlag == false && Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            int x = Mathf.RoundToInt(mousePosition.x);
            int y = Mathf.RoundToInt(mousePosition.y);

            TileController tile = grid[x, y];

            if (tile.tileState == TileController.TileState.Normal && tile.isCovered)
            {
                if (tile.tileKind == TileController.TileKind.Mine)
                {
                    RunGameOver(tile);
                }
                else
                {
                    tile.SetIsCovered(false);
                }
                
                if (tile.tileKind == TileController.TileKind.Blank)
                {
                    RevealAdjacentTilesForCurrentPos(x, y);
                }
            }
        }
    }

    void CheckInputMouseButtonRight()
    {
        if (gameOverFlag == false && Input.GetMouseButtonDown(1))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            int x = Mathf.RoundToInt(mousePosition.x);
            int y = Mathf.RoundToInt(mousePosition.y);

            TileController tile = grid[x, y];

            if (tile.tileState == TileController.TileState.Normal && tile.isCovered && minesAmount > 0)
            {
                tile.tileState = TileController.TileState.Flagged;
                tile.SetFlagMarked();

                minesAmount--;
                minesAmountTMP.text = minesAmount.ToString();
            }
            else if (tile.tileState == TileController.TileState.Flagged && tile.isCovered)
            {
                tile.tileState = TileController.TileState.Normal;
                tile.SetCoverNoFlag();

                minesAmount++;
                minesAmountTMP.text = minesAmount.ToString();
            }
        }
    }

    void CheckIfWinOrLose()
    {
        if (gameOverFlag == true && deadOnMine == true)
        {
            Debug.Log("You Loose!");

            gameOverImg.SetActive(true);
            gameOverStatus.text = "Unfortunately You Lose.";

            gameOverFlag = false;
            deadOnMine = false;
        }
    }

    void RunGameOver(TileController tile)
    {
        GameObject[] allMines = GameObject.FindGameObjectsWithTag("Mine");

        foreach (GameObject mine in allMines)
        {
            TileController eachTile = mine.GetComponent<TileController>();
            eachTile.SetIsCovered(false);
        }
        
        tile.SetMineClicked();
        deadOnMine = true;

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                TileController curTile = grid[x, y];

                if (curTile.tileState == TileController.TileState.Flagged)
                {
                    if (curTile.tileKind == TileController.TileKind.Mine)
                    {
                        curTile.SetMineDestroyed();
                    }
                    else
                    {
                        curTile.SetFlagWasIncorrect();
                    }
                }
            }
        }

        gameOverFlag = true;
    }

    void PlaceMines()
    {
        int x = UnityEngine.Random.Range(0, gridWidth);
        int y = UnityEngine.Random.Range(0, gridHeight);

        if (grid[x, y] == null)
        {
            TileController mineTile = Instantiate(Resources.Load("Prefabs/mine", typeof(TileController)), new Vector3(x, y, 0), Quaternion.identity) as TileController;

            grid[x, y] = mineTile;

            // Debug.Log("(" + x + ", " + y + ")");
        }
        else
        {
            PlaceMines();
        }
    }

    void PlaceClues()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y] == null)
                {
                    // Nothing is here, can't be a mine
                    int numMines = 0;

                    // Checking North
                    if (y + 1 < gridHeight)
                    {
                        if (grid[x, y + 1] != null && grid[x, y + 1].tileKind == TileController.TileKind.Mine) numMines++;
                    }

                    // Checking East
                    if (x + 1 < gridWidth)
                    {
                        if (grid[x + 1, y] != null && grid[x + 1, y].tileKind == TileController.TileKind.Mine) numMines++;
                    }

                    // Checking South
                    if (y - 1 >= 0)
                    {
                        if (grid[x, y - 1] != null && grid[x, y - 1].tileKind == TileController.TileKind.Mine) numMines++;
                    }

                    // Checking West
                    if (x - 1 >= 0)
                    {
                        if (grid[x - 1, y] != null && grid[x - 1, y].tileKind == TileController.TileKind.Mine) numMines++;
                    }

                    // Checking NorthEast
                    if (x + 1 < gridWidth && y + 1 < gridHeight)
                    {
                        if (grid[x + 1, y + 1] != null && grid[x + 1, y + 1].tileKind == TileController.TileKind.Mine) numMines++;
                    }

                    // Checking NorthWest
                    if (x - 1 >= 0 && y + 1 < gridHeight)
                    {
                        if (grid[x - 1, y + 1] != null && grid[x - 1, y + 1].tileKind == TileController.TileKind.Mine) numMines++;
                    }

                    // Checking SouthEast
                    if (x + 1 < gridWidth && y - 1 >= 0)
                    {
                        if (grid[x + 1, y - 1] != null && grid[x + 1, y - 1].tileKind == TileController.TileKind.Mine) numMines++;
                    }

                    // Checking SouthWest
                    if (x - 1 >= 0 && y - 1 >= 0)
                    {
                        if (grid[x - 1, y - 1] != null && grid[x - 1, y - 1].tileKind == TileController.TileKind.Mine) numMines++;
                    }

                    if (numMines > 0)
                    {
                        TileController clueTile = Instantiate(Resources.Load("Prefabs/" + numMines, typeof(TileController)), new Vector3(x, y, 0), Quaternion.identity) as TileController;

                        grid[x, y] = clueTile;
                    }
                }
            }
        }
    }

    void PlaceBlanks()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y] == null)
                {
                    TileController blankTile = Instantiate(Resources.Load("Prefabs/blank", typeof(TileController)), new Vector3(x, y, 0), Quaternion.identity) as TileController;

                    grid[x, y] = blankTile;
                }
            }
        }
    }

    void RevealAdjacentTilesForCurrentPos(int x, int y)
    {
        // Checking North
        if (y + 1 < gridHeight) CheckTileAtPos(x, y + 1);

        // Checking East
        if (x + 1 < gridWidth) CheckTileAtPos(x + 1, y);

        // Checking South
        if (y - 1 >= 0) CheckTileAtPos(x, y - 1);

        // Checking West
        if (x - 1 >= 0) CheckTileAtPos(x - 1, y);

        // Checking NorthEast
        if (x + 1 < gridWidth && y + 1 < gridHeight) CheckTileAtPos(x + 1, y + 1);

        // Checking NorthWest
        if (x - 1 >= 0 && y + 1 < gridHeight) CheckTileAtPos(x - 1, y + 1);

        // Checking SouthEast
        if (x + 1 < gridWidth && y - 1 >= 0) CheckTileAtPos(x + 1, y - 1);

        // Checking SouthWest
        if (x - 1 >= 0 && y - 1 >= 0) CheckTileAtPos(x - 1, y - 1);

        for (int i = tilesToCheck.Count - 1; i >= 0; i--)
        {
            if (tilesToCheck[i].didCheck)
            {
                tilesToCheck.RemoveAt(i);
            }
        }

        if (tilesToCheck.Count > 0)
        {
            RevealAdjacentTilesForTiles();
        }
    }

    void RevealAdjacentTilesForTiles()
    {
        for (int i = 0; i < tilesToCheck.Count; i++)
        {
            TileController tile = tilesToCheck[i];

            int x = (int)tile.gameObject.transform.localPosition.x;
            int y = (int)tile.gameObject.transform.localPosition.y;

            tile.didCheck = true;

            if (tile.tileState != TileController.TileState.Flagged)
            {
                tile.SetIsCovered(false);
            }
            
            RevealAdjacentTilesForCurrentPos(x, y);
        }
    }

    void CheckTileAtPos(int x, int y)
    {
        TileController tile = grid[x, y];

        if (tile.tileKind == TileController.TileKind.Blank)
        {
            tilesToCheck.Add(tile);
            // Debug.Log("Tile @ (" + x + ", " + y + ") is a Blank");
        }
        else if (tile.tileKind == TileController.TileKind.Clue && tile.tileState != TileController.TileState.Flagged)
        {
            tile.SetIsCovered(false);
            // Debug.Log("Tile @ (" + x + ", " + y + ") is a Clue");
        }
        else if (tile.tileKind == TileController.TileKind.Mine)
        {
            // Debug.Log("Tile @ (" + x + ", " + y + ") is a Mine");
        }
    }
}
