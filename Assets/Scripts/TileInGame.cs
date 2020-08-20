using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInGame : MonoBehaviour
{
    private float speed = 0.5f;
    //
    private int collisionCount = 0;
    private bool selected = false;
    //
    private bool isMoving = false;
    private IEnumerator mover;

    public IEnumerator MoveToCoroutine(Transform targ, Vector3 pos, float dur)
    {
        isMoving = true;

        float t = 0f;
        Vector3 start = targ.position;
        Vector3 v = pos - start;
        while (t < dur)
        {
            t += Time.deltaTime;
            targ.position = start + v * t / dur;
            yield return null;
        }

        targ.position = pos;

        isMoving = false;
        doneMoving = true;
    }

    void Start()
    {
        if (GetComponent<SpriteRenderer>().sortingLayerName != "0")
            gameObject.layer = 2; 
    }

    private int UponOtherTileSelected(GameObject id, int pivot)
    {
        if(id == gameObject && selected)
        {
            if (isMoving)
            {
                StopCoroutine(mover);
                isMoving = false;
            }
            mover = MoveToCoroutine(transform, new Vector3(BoardManager.instance.xBar + pivot, BoardManager.instance.yBar, 0), speed);
            StartCoroutine(mover);
        }
        return 0;
    }

    private int UponMatchDestruction(GameObject identity)
    {
        if (identity == gameObject && selected)
        {
            BoardManager.instance.OnTileDestroy();
            Destroy(gameObject);
        }
        return 0;
    }

    private int UponMatchRearrange(GameObject identity, int pos)
    {
        if(selected && identity == gameObject)
        {
            if (isMoving)
            {
                StopCoroutine(mover);
                isMoving = false;
            }
            mover = MoveToCoroutine(transform, new Vector3(BoardManager.instance.xBar + pos, BoardManager.instance.yBar, 0), speed);
            StartCoroutine(mover);
        }
        return 0;
    }

    private int UponUndoReset(Record r)
    {
        if (!selected) return 0;
        if (gameObject != r.tile) return 0;

        if (isMoving)
        {
            StopCoroutine(mover);
            isMoving = false;
        }
        
        selected = false;
        GameEventSystem.current.onSelectedTileMove -= UponOtherTileSelected;
        GameEventSystem.current.onMatch_Destroy -= UponMatchDestruction;
        GameEventSystem.current.onDestroy_RearrangeBar -= UponMatchRearrange;
        GameEventSystem.current.onUndo -= UponUndoReset;
        doneMoving = false;
        
        mover = MoveToCoroutine(transform, new Vector3(r.prevX, r.prevY, 0), speed);
        StartCoroutine(mover);
        
        return 0;
    }

    private void OnMouseDown() //TILE IS SELECTED
    {
        Debug.Log(collisionCount);
        if (selected)
        {
            return;
        }
        selected = true;

        GameEventSystem.current.onSelectedTileMove += UponOtherTileSelected;
        GameEventSystem.current.onMatch_Destroy += UponMatchDestruction;
        GameEventSystem.current.onDestroy_RearrangeBar += UponMatchRearrange;
        GameEventSystem.current.onUndo += UponUndoReset;
        
        StartCoroutine(MoveToBar());
    }

    bool doneMoving = false;

    IEnumerator MoveToBar()
    {
        BoardManager.instance.bar.StackUp(gameObject);
        yield return new WaitUntil(() => doneMoving == true);
        doneMoving = false;                                                               //not sure how this line works but it stays because it is working
        BoardManager.instance.bar.CheckForMatch(gameObject);
        //yield return new WaitUntil(() => doneMoving == true);                           //not sure how this line works but it stays because it is working

        if (BoardManager.instance.CheckIfLost())
            Debug.Log("Lost!");
        if(BoardManager.instance.CheckIfWon())
            Debug.Log("Won!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int _this = int.Parse(GetComponent<SpriteRenderer>().sortingLayerName);
        int _other = int.Parse(collision.GetComponent<SpriteRenderer>().sortingLayerName);
        if(_this == _other + 1) //this object is one layer beneath the other
        {
            collisionCount++;
        }
        if (collisionCount > 0)
            gameObject.layer = 2;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        int _this = int.Parse(GetComponent<SpriteRenderer>().sortingLayerName);
        int _other = int.Parse(collision.GetComponent<SpriteRenderer>().sortingLayerName);
        if (_this == _other + 1) //this object is one layer beneath the other
        {
            collisionCount--;
        }
        if(collisionCount == 0) 
            gameObject.layer = 0;
    }

    private void OnDestroy()
    {
        if (selected)
        {
            GameEventSystem.current.onSelectedTileMove -= UponOtherTileSelected;
            GameEventSystem.current.onMatch_Destroy -= UponMatchDestruction;
            GameEventSystem.current.onDestroy_RearrangeBar -= UponMatchRearrange;
            GameEventSystem.current.onUndo -= UponUndoReset;
        }
    }
}
