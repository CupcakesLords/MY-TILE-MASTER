using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;
  
    public List<Sprite> characters = new List<Sprite>(); //add from editor
    public GameObject tile;                              //add from editor
    public GameObject Bar;                               //add from editor
    
    private GameLevelObject currentLevel;
    private GameObject[] gameTiles;
    private int count;
    public StackInBoard bar;
    public float xBar, yBar;
    private List<Record> histoire;

    void Start()
    {
        instance = GetComponent<BoardManager>();
        
        currentLevel = Utility.ReadGameLevelFromAsset(GlobalStatic.PlayerChoice);

        xBar = -(Bar.GetComponent<SpriteRenderer>().bounds.size.x / 2) + (tile.GetComponent<SpriteRenderer>().bounds.size.x / 1.5f);  yBar = Bar.transform.position.y  + (Bar.GetComponent<SpriteRenderer>().bounds.size.y / 7.75f);
        
        bar = new StackInBoard();

        histoire = new List<Record>();
 
        CreateBoard(); 
    }

    private void CreateBoard()
    {
        int total = currentLevel.tiles.Count; count = total;
        gameTiles = new GameObject[total];

        //
        int rationTimes = (total / 3);
        int[] TileRation = new int[characters.Count];
        for (int i = 0; i < TileRation.Length; i++)
        {
            TileRation[i] = 0;
        }
        for (int i = 0; i < rationTimes; i++)
        {
            int rand = Random.Range(0, characters.Count);
            TileRation[rand] += 3;
        }
        //

        for(int i = 0; i < total; i++)
        {
            GameObject newTile = Instantiate(tile, new Vector3(currentLevel.tiles[i].x, currentLevel.tiles[i].y, 0), tile.transform.rotation);
            //gameTiles[i] = newTile;
            
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

            gameTiles[i] = newTile;
        }
    }

    public void Delete(LinkedList<GameObject> chosenTile, List<GameObject> NeedRemoving)
    {
        StartCoroutine(DeleteCallback(chosenTile, NeedRemoving));
    }

    IEnumerator DeleteCallback(LinkedList<GameObject> chosenTile, List<GameObject> NeedRemoving)
    {
        for (int i = 0; i < 3; i++)                                                                               //destroy those tiles
            GameEventSystem.current.Match_Destroy(NeedRemoving[i]);
        yield return new WaitForSeconds(0.3f);
        int iterator3 = 0;
        foreach (GameObject i in chosenTile)                                                                      //rearrange the bar
        {
            GameEventSystem.current.RearrangeBar(i, iterator3);
            iterator3++;
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

    public void Refresh()
    {
        int TileRemain = 0;
        int[] TileTaken = new int[characters.Count];
        for (int i = 0; i < TileTaken.Length; i++)
        {
            TileTaken[i] = 0;
        }

        for (int i = 0; i < gameTiles.Length; i++)
        {
            if (!gameTiles[i])                                                                                                   //tile destroyed
            {
                continue;
            }
            else if (gameTiles[i].transform.position.y == yBar || gameTiles[i].tag == "Moving")                                  //tile selected or is moving
            {
                TileRemain++;
                Sprite temp = gameTiles[i].GetComponent<SpriteRenderer>().sprite; 
                for(int j = 0; j < characters.Count; j++)
                {
                    if(characters[j] == temp)
                    {
                        TileTaken[j]++;
                        break;
                    }
                }
            }
            else
            {
                TileRemain++;
            }
        }

        if (TileRemain == 0)
            return;

        int rationTimes = (TileRemain / 3);
        int[] TileRation = new int[characters.Count];
        for (int i = 0; i < TileRation.Length; i++)
        {
            TileRation[i] = 0;
        }
        for (int i = 0; i < rationTimes; i++)
        {
            int rand = Random.Range(0, characters.Count);
            TileRation[rand] += 3;
        }
        for (int i = 0; i < TileRation.Length; i++)
        {
            TileRation[i] = TileRation[i] - TileTaken[i];
        }

        for (int i = 0; i < gameTiles.Length; i++)
        {
            if (!gameTiles[i])                                                                                                   //destroyed
            {
                continue;
            }
            else if (gameTiles[i].transform.position.y == yBar || gameTiles[i].tag == "Moving")                                  //selected or moving
            {
                continue;
            }
            else
            {
                int rand = Random.Range(0, characters.Count);
                
                while (true)
                {
                    if (TileRation[rand] == 0)
                    {
                        rand = Random.Range(0, characters.Count); continue;
                    }
                    else
                        break;
                }
                TileRation[rand] -= 1;
             
                Sprite newSprite = characters[rand];
                gameTiles[i].GetComponent<SpriteRenderer>().sprite = newSprite;
            }
        }
        
        List<int> layers = new List<int>();
        List<float> parent = currentLevel.pos.p;
        foreach(L i in currentLevel.pos.l)
        {
            layers.Add((int)parent[2] + (int)i.p[2]);
        }
        int direction = 0;
        foreach(int i in layers)
        {
            GameEventSystem.current.Refresh(i, direction);
            direction++;
            if (direction > 2)
                direction = 0;
        }
    }
}
