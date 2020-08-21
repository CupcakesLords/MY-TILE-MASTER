using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;
    //
    public List<Sprite> characters = new List<Sprite>(); //add from editor
    public GameObject tile;                              //add from editor
    //
    private GameLevel currentLevel;
    //
    private GameObject[] gameTiles;
    private int count;
    //
    public StackInBoard bar;
    //
    public int xBar, yBar;
    //
    private List<Record> histoire;

    void Start()
    {
        instance = GetComponent<BoardManager>();
        currentLevel = Utils.ReadDefaultGameLevelFromAsset(GlobalStatic.PlayerChoice); //SET LEVEL

        xBar = -3; yBar = -3;
        bar = new StackInBoard();

        histoire = new List<Record>();
 
        CreateBoard(); 
    }

    private void CreateBoard()
    {
        int total = currentLevel.tiles.Count; count = total;
        gameTiles = new GameObject[total];

        //
        int[] TileRation = new int[characters.Count];
        for(int i = 0; i < TileRation.Length; i++)
        {
            TileRation[i] = 3;
        }
        //

        for(int i = 0; i < total; i++)
        {
            GameObject newTile = Instantiate(tile, new Vector3(currentLevel.tiles[i].x, currentLevel.tiles[i].y, 0), tile.transform.rotation);
            gameTiles[i] = newTile;
            
            newTile.transform.parent = transform;
            newTile.GetComponent<SpriteRenderer>().sortingOrder = (int)currentLevel.tiles[i].z; 

            int rand = Random.Range(0, characters.Count);
            //
            while(true)
            {
                if (TileRation[rand] == 0)
                {
                    rand = Random.Range(0, characters.Count); continue;
                }
                else
                    break;
            }
            TileRation[rand] -= 1;
            //
            Sprite newSprite = characters[rand];
            newTile.GetComponent<SpriteRenderer>().sprite = newSprite;
        }
    }

    public void OnTileDestroy()
    {
        count = count - 1;
    }

    public bool CheckIfLost()
    {
        if(bar.IsFull())
        {
            GameEventSystem.current.PopUpWinUI(0);
        }
        return bar.IsFull();
    }

    public bool CheckIfFull()
    {
        return bar.IsFull();
    }

    public bool CheckIfWon()
    {
        if(count == 0)
        {
            GameEventSystem.current.PopUpWinUI(1);
        }
        return count == 0;
    }

    public void PrintListRecord()
    {
        Debug.Log("Record " + Random.Range(900, 1000));
        int iter = 0;
        foreach(Record i in histoire)
        {
            Debug.Log("ITEM " + iter + ": " + i.tile.GetComponent<SpriteRenderer>().sprite);
            Debug.Log(i.prevX);
            Debug.Log(i.prevY);
            iter++;
        }
    }

    public void AddToRecord(GameObject Tile, float x, float y)
    {
        histoire.Add(new Record(x, y, Tile));
    }

    public void RecordClearMatch(GameObject Tile)
    {
        int count = 0;
        Sprite temp = Tile.GetComponent<SpriteRenderer>().sprite;
        List<Record> needRemoving = new List<Record>();
        foreach(Record i in histoire)
        {
            if (i.tile.GetComponent<SpriteRenderer>().sprite == temp)
            {
                needRemoving.Add(i);
                count++;
            }
        }
        if (count != 3)
            return;
        else
        {
            foreach(Record i in needRemoving)
            {
                histoire.Remove(i);
            }
        }
    }

    public void UndoRecord()
    {
        if (histoire.Count == 0)
            return;
        Record r = histoire[histoire.Count - 1];
        histoire.RemoveAt(histoire.Count - 1);

        GameObject tile = r.tile;
        bar.UponUndo(tile);

        GameEventSystem.current.Undo(r);
    }
}
