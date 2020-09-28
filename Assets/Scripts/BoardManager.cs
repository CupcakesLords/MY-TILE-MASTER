using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AFramework.UI;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;                                    //CONSTANT
  
    private List<Sprite> characters = new List<Sprite>();
    private List<Sprite> all_characters = new List<Sprite>();
    public GameObject tile;                                                 //add from editor
    public GameObject Bar;                                                  //add from editor
    public Image BG;                                                        //add from editor

    private GameLevelObject currentLevel;
    private GameObject[] gameTiles;
    private int count;
    public StackInBoard bar;

    [HideInInspector]
    public float xBar, yBar;                                                //CONSTANT

    private List<Record> histoire;

    public int bar_layer = 30000;                                           //CONSTANT AND FINAL
    public float speed = 0.375f;                                            //CONSTANT AND FINAL
    public float destruction_speed = 0.375f;                                //CONSTANT AND FINAL
  
    [HideInInspector]
    public int coin = 0;
    [HideInInspector]
    public int star = 3;

    private bool AlreadyLost = false;
    private bool AlreadyWon = false;
    private bool AlreadyFirstMatch = false;

    private int CurrentLevel = 1;

    //USED IN UI ONLY
    public int world = 0;
    public int level = 0;

    //RESOURCES
    private int totalSprites = 20;
    private int totalThemes = 5;
    private int totalBG = 20;
    private int totalLevel = 1000;

    void Start()
    {
        for(int i = 1; i <= totalSprites; i++)
        {
            all_characters.Add(Resources.Load<Sprite>("Sprites/temp/tiles/tile01/" + i));
        }

        instance = GetComponent<BoardManager>();
        xBar = -(Bar.GetComponent<SpriteRenderer>().bounds.size.x / 2) + (tile.GetComponent<SpriteRenderer>().bounds.size.x / 1.5f);  yBar = Bar.transform.position.y  + (Bar.GetComponent<SpriteRenderer>().bounds.size.y / 7.75f);
    }

    public bool IsPlaying()
    {
        return gameTiles != null;
    }

    public bool AlreadyFirstMatchDestroyed()
    {
        return AlreadyFirstMatch;
    }

    public void ResetBG(int n)
    {
        if (n <= 0 || n > totalBG)
            return;

        BG.sprite = Resources.Load<Sprite>("Bundle/Game/BG/" + n);
    }

    public void ResetCharacter(int n) 
    {
        if (n <= 0 || n > totalThemes)
            return;

        all_characters.Clear();
        for (int i = 1; i <= totalSprites; i++)
        {
            all_characters.Add(Resources.Load<Sprite>("Sprites/temp/tiles/tile0" + n + "/" + i));
        }

        if (gameTiles != null) //playing
        {
            characters.Clear();
            characters = all_characters.GetRange(0, SpriteInUse(CurrentLevel));
            
            for (int i = 0; i < gameTiles.Length; i++)
            {
                if (!gameTiles[i]) //already destroyed                                                                                    
                    continue;
                else
                {
                    string name = gameTiles[i].GetComponent<SpriteRenderer>().sprite.name;
                    int id = int.Parse(name); id = id - 1;
                    gameTiles[i].GetComponent<SpriteRenderer>().sprite = characters[id];
                }
            }
        }
    }

    public void RewindLost()
    {
        AlreadyLost = false;
    }

    public void RefreshMemory()
    {
        for (int i = 0; i < gameTiles.Length; i++)
        {
            if (!gameTiles[i]) //already destroyed                                                                                    
                continue;
            else
                Destroy(gameTiles[i]);
        }
        Bar.SetActive(false);
        currentLevel = null;
        gameTiles = null;
        bar = null;
        histoire = null;
        coin = 0; star = 3;
        AlreadyLost = false;
        AlreadyWon = false;
        AlreadyFirstMatch = false;
        CurrentLevel = 1;
        //world = 0; level = 0; //UI variables
    }

    public void StartNewGame(int Level)
    {
        if (Level <= 0 || Level >= totalLevel)
            return;

        Bar.SetActive(true); CurrentLevel = Level;
        currentLevel = Utility.ReadGameLevelFromAsset(Level);

        // level time and number of tiles
        GameEventSystem.current.SetTimeBar(300f, 60f, 150f, 240f);

        GameEventSystem.current.TimeControl(1);

        characters.Clear();
        characters = all_characters.GetRange(0, SpriteInUse(Level));
        //

        bar = new StackInBoard();
        histoire = new List<Record>();
        CreateBoard();
    }

    private int SpriteInUse(int lv)
    {
        int result = (lv / 50) + 8;
        if (result <= totalSprites)
            return result;
        else
            return totalSprites;
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

        RefreshAnimation();
    }

    public void Delete(LinkedList<GameObject> chosenTile, List<GameObject> NeedRemoving)
    {
        if(AlreadyFirstMatch == false)
        {
            GameEventSystem.current.TimeControl(2);
            AlreadyFirstMatch = true;
        }
        else
        {
            GameEventSystem.current.TimeControl(4);
        }
        StartCoroutine(DeleteCallback(chosenTile, NeedRemoving));
    }

    IEnumerator DeleteCallback(LinkedList<GameObject> chosenTile, List<GameObject> NeedRemoving)
    {
        for (int i = 0; i < 3; i++)                                                                               //destroy those tiles
            GameEventSystem.current.Match_Destroy(NeedRemoving[i]);
        yield return new WaitForSeconds(destruction_speed);
        int iterator3 = 0;
        foreach (GameObject i in chosenTile)                                                                      //rearrange the bar
        {
            GameEventSystem.current.RearrangeBar(i, iterator3);
            iterator3++;
        }
        CheckIfWon();
    }

    public void OnTileDestroy()
    {
        count = count - 1;
    }

    public bool CheckIfLost()
    {
        if (AlreadyLost)
            return true;
        if(bar.IsFull())
        {
            StartCoroutine(YouFuckingLost());
        }
        return bar.IsFull();
    }

    IEnumerator YouFuckingLost()
    {
        AlreadyLost = true; GameEventSystem.current.TimeControl(1);
        CanvasManager.Push(GlobalInfor.EmptyMenu, null);

        float t = 0f;
        float start = 1f;
        float v = 0.5f;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            bar.Darken((float)(start - v * t / 0.5), (float)(start - v * t / 0.5), (float)(start - v * t / 0.5));
            yield return null;
        }
        bar.Darken(v, v, v);

        CanvasManager.Pop(GlobalInfor.EmptyMenu);

        object[] param = new object[1];
        param[0] = CurrentLevel;
        CanvasManager.Push(GlobalInfor.LoseMenu, param); 
    }

    public bool CheckIfFull()
    {
        return bar.IsFull();
    }

    public bool CheckIfWon()
    {
        if (AlreadyWon)
            return true;
        if (count == 0)
        {
            object[] param = new object[1];
            param[0] = CurrentLevel;
            CanvasManager.Push(GlobalInfor.WinMenu, param); AlreadyWon = true; GameEventSystem.current.TimeControl(1);
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

    public bool UndoRecord()
    {
        if (histoire.Count == 0)
            return false;

        Record r = histoire[histoire.Count - 1];
        histoire.RemoveAt(histoire.Count - 1);

        GameObject tile = r.tile;
        bar.UponUndo(tile);

        GameEventSystem.current.Undo(r);
        return true;
    }

    public void Refresh()
    {
        int[] TileRemain = new int[characters.Count];
        for (int i = 0; i < TileRemain.Length; i++)
            TileRemain[i] = 0;
        
        for (int i = 0; i < gameTiles.Length; i++)
        {
            if (!gameTiles[i])                                                                                                   //tile destroyed
                continue;
            else if (gameTiles[i].transform.position.y == yBar || gameTiles[i].tag == "Moving")                                  //tile selected or is moving
                continue;
            else
            {
                Sprite temp = gameTiles[i].GetComponent<SpriteRenderer>().sprite;
                for (int j = 0; j < characters.Count; j++)
                {
                    if (characters[j] == temp)
                    {
                        TileRemain[j]++;
                        break;
                    }
                }
            }
        }

        for (int i = 0; i < gameTiles.Length; i++)
        {
            if (!gameTiles[i])                                                                                                   //destroyed
                continue;
            else if (gameTiles[i].transform.position.y == yBar || gameTiles[i].tag == "Moving")                                  //selected or moving
                continue;
            else
            {
                int rand = Random.Range(0, characters.Count);

                while (true)
                {
                    if (TileRemain[rand] == 0)
                    {
                        rand = Random.Range(0, characters.Count); continue;
                    }
                    else
                        break;
                }
                TileRemain[rand] -= 1;

                Sprite newSprite = characters[rand];
                gameTiles[i].GetComponent<SpriteRenderer>().sprite = newSprite;
            }
        }

        RefreshAnimation();
    }

    private void RefreshAnimation()
    {
        List<int> layers = new List<int>();
        List<float> parent = currentLevel.pos.p;
        foreach (L i in currentLevel.pos.l)
        {
            layers.Add(Utility.CalculateBaseLayer((int)parent[2], (int)i.p[2]));
        }

        int direction = 0;
        foreach (int i in layers)
        {
            GameEventSystem.current.Refresh(i, direction);
            direction++;
            if (direction > 2)
                direction = 0;
        }
        StartCoroutine(RefreshQueue(layers));
    }
    
    IEnumerator RefreshQueue(List<int> layers)
    {
        CanvasManager.Push(GlobalInfor.EmptyMenu, null);
        WaitForSeconds waitTime = new WaitForSeconds(0.075f);
        int direction = 0;
        foreach (int i in layers)
        {
            GameEventSystem.current.BackFromRefresh(i, direction);
            direction++;
            if (direction > 2)
                direction = 0;
            yield return waitTime;
        }
        yield return new WaitForSeconds(0.3f);
        CanvasManager.Pop(GlobalInfor.EmptyMenu);
    }
    
    public void Hint()
    {
        int[] TileTaken = new int[characters.Count];
        for (int i = 0; i < TileTaken.Length; i++)
            TileTaken[i] = 0;

        List<GameObject> tempo = new List<GameObject>();

        for (int i = 0; i < gameTiles.Length; i++)
        {
            if (!gameTiles[i])                                                                                                   //tile destroyed
            {
                continue;
            }
            else if (gameTiles[i].transform.position.y == yBar || gameTiles[i].tag == "Moving")                                  //tile selected or is moving
            {
                Sprite temp = gameTiles[i].GetComponent<SpriteRenderer>().sprite;
                for (int j = 0; j < characters.Count; j++)
                {
                    if (characters[j] == temp)
                    {
                        TileTaken[j]++;
                        break;
                    }
                }
            }
            else
            {
                //if(CheckIfClickable(gameTiles[i]))
                    tempo.Add(gameTiles[i]);
            }
        }

        int available = bar.Available();
        
        for (int i = 0; i < TileTaken.Length; i++) //characters[i] is sprite, TileTaken[i] is number of said sprite selected
        {
            int need = 3 - TileTaken[i];
            if (need > available)
                continue;
            List<GameObject> tip = new List<GameObject>();
            
            foreach (GameObject j in tempo)
            {
                if(j.GetComponent<SpriteRenderer>().sprite == characters[i])
                {
                    tip.Add(j);
                }
            }

            if (tip.Count == 0)
                continue;
            if (tip.Count < need) //dig deeper cause there is not enough tile to form a match
            {
                
            }
            if (tip.Count >= need)  //this sprite has enough tile available to form a match
            {
                foreach(GameObject j in tip)
                {
                    if (need == 0)
                        break;
                    GameEventSystem.current.Hint(j); 
                    need = need - 1;
                }
                break;
            }
        }
    }

    private bool CheckIfClickable(GameObject t)
    {
        for (int i = 0; i < gameTiles.Length; i++)
        {
            if (!gameTiles[i]) //tile destroyed
                continue;
            
            if (t.transform.position.x - 0.5 <= gameTiles[i].transform.position.x && gameTiles[i].transform.position.x <= t.transform.position.x + 0.5)
            {
                if (t.transform.position.y - 0.5 <= gameTiles[i].transform.position.y && gameTiles[i].transform.position.y <= t.transform.position.y + 0.5)
                {
                    if (t.GetComponent<SpriteRenderer>().sortingOrder < gameTiles[i].GetComponent<SpriteRenderer>().sortingOrder)
                        return false;
                }
            }
        }
        return true;
    }
}
