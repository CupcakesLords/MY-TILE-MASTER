using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackInBoard
{
    LinkedList<GameObject> chosenTile = new LinkedList<GameObject>();

    public bool IsFull()
    {
        return chosenTile.Count == 7;
    }

    public int StackUp(GameObject Tile)                                                   //this function is called when player clicks on a tile
    {
        int position = CalculatePosititonOfNewTile(Tile);                                 //insert new tile to the list, calculate its position in the list

        //GameEventSystem.current.SelectedTileMove(position);                             //adjust the bar so the new chosen tile can fit in
        int iterator = 0;
        foreach(GameObject i in chosenTile)
        {
            GameEventSystem.current.SelectedTileMove(i, iterator);
            iterator++;
        }

        return position;                                                                  //return position of the new tile so move it
    }

    public bool CheckForMatch()                                                           //check if there is A match in the bar
    {
        if (chosenTile.Count < 3)
            return false;
        bool result = false;                                                              //this bool is the result
        int MATCH = 0;
        GameObject before = chosenTile.First.Value;
        int iterator = 0;
        foreach (GameObject i in chosenTile)                                              //this loop is used to check if there is a match in the list
        {
            if (before.GetComponent<SpriteRenderer>().sprite == i.GetComponent<SpriteRenderer>().sprite)
                MATCH++;
            else
            {
                before = i;
                MATCH = 1;
            }

            if (MATCH == 3)
            {
                result = true;
                break;
            }
            iterator++;
        }
        if (result == false) return false;                                                //IF: if there is no match, return
        int iterator2 = 0;
        List<GameObject> NeedRemoving = new List<GameObject>();                           //A list to hold tiles that are in a match 
        foreach(GameObject i in chosenTile)                                               //find those tiles in a match and put it in the list
        {
            if(iterator2 == iterator)
            {
                NeedRemoving.Add(i);
                break;
            }
            if(iterator2 == iterator - 1 || iterator2 == iterator - 2)
            {
                NeedRemoving.Add(i);
            }
            iterator2++;
        }
        for(int i = 0; i < 3; i++)                                                        //remove those tiles from the list(they still exist)
            chosenTile.Remove(NeedRemoving[i]);
        for (int i = 0; i < 3; i++)                                                       //destroy those tiles
            GameEventSystem.current.Match_Destroy(NeedRemoving[i]);
        int iterator3 = 0;
        foreach(GameObject i in chosenTile)                                               //rearrange the bar
        {
            GameEventSystem.current.RearrangeBar(i, iterator3);
            iterator3++;
        }
        return result;
    }

    private int CalculatePosititonOfNewTile(GameObject Tile)                              //insert new tile to the list, calculate its position in the list
    {
        if (chosenTile.Count == 0)                                                        //IF: list is empty (no tile chosen), so add first
        {
            chosenTile.AddFirst(Tile);
            return 0;
        }
        else                                                                              //ELSE: list consists of tile(s)
        {
            int iterator = 0; 
            int match = 0;
            Sprite a = Tile.GetComponent<SpriteRenderer>().sprite;                        //FIRST LOOP
            foreach (GameObject i in chosenTile)                                          //iterate the whole list from start to end, find the position of the last tile with the same type as the new tile
            {                                                                             //new tile: a
                Sprite b = i.GetComponent<SpriteRenderer>().sprite;                       //list: a a b b c d e
                if (a == b)                                                               //result: 1 (the second a)
                {
                    match = iterator;                                                     
                }
                iterator++;
            }

            bool match_Position_Zero = false;                                             //check if the result from the loop above is the first one in the list
            if (a == chosenTile.First.Value.GetComponent<SpriteRenderer>().sprite)
                match_Position_Zero = true;

            if (match == 0 && !match_Position_Zero)                                       //IF: tile type doesnt exist in the list, add to last 
            {
                int position = chosenTile.Count;
                chosenTile.AddLast(Tile);
                return position;                                                          //return last position
            }
            else                                                                          //ELSE: tile type existed and match is the last position of the matching tile
            {
                int iterate = 0;
                GameObject j = null;
                foreach (GameObject i in chosenTile)
                {
                    if (iterate == match)
                    {
                        j = i;
                        break;
                    }
                    iterate++;
                }
                chosenTile.AddAfter(chosenTile.Find(j), Tile);
                
                return match + 1;
            }
        }
    }
}
