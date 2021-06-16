using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public enum TileKind
    {
        Blank
       ,Mine
       ,Clue
    }

    public enum TileState
    {
        Normal
       ,Flagged
    }

    public TileKind tileKind = TileKind.Blank;

    public TileState tileState = TileState.Normal;

    public bool isCovered = true;
    public bool didCheck = false;

    public Sprite coverSprite;
    public Sprite flagSprite;
    public Sprite mineClickedSprite;
    public Sprite flagIsMine;
    public Sprite flagIsNotMine;

    private Sprite defaultSprite;

    private void Start()
    {
        defaultSprite = GetComponent<SpriteRenderer>().sprite;

        GetComponent<SpriteRenderer>().sprite = coverSprite;
    }

    public void SetIsCovered(bool covered)
    {
        isCovered = covered;
        GetComponent<SpriteRenderer>().sprite = defaultSprite;
    }

    // Tile showing that on top is - cover flagged.png
    public void SetFlagMarked()
    {
        GetComponent<SpriteRenderer>().sprite = flagSprite;
    }

    // Tile showing that on top is - cover.png
    public void SetCoverNoFlag()
    {
        GetComponent<SpriteRenderer>().sprite = coverSprite;
    }

    // Tile showing that player died on the Mine - mine clicked.png
    public void SetMineClicked()
    {
        GetComponent<SpriteRenderer>().sprite = mineClickedSprite;
    }

    // Tile showing that Mine was correctly marked with Flag and destroyed at GameOver - mine flagged.png
    public void SetMineDestroyed()
    {
        GetComponent<SpriteRenderer>().sprite = flagIsMine;
    }

    // Tile showing what is behind a Flag at GameOver - depending of the source it can be one of the following:
    // 1 flagged.png | 2 flagged.png | 3 flagged.png | 4 flagged.png | 5 flagged.png | 6 flagged.png | 7 flagged.png | 8 flagged.png | blank flagged.png
    public void SetFlagWasIncorrect()
    {
        GetComponent<SpriteRenderer>().sprite = flagIsNotMine;
    }
}
