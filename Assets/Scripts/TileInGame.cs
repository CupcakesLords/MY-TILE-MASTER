using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInGame : MonoBehaviour
{
    private int bar_layer = 30000;
    private int layer;
    //
    private float originX, originY;
    private float speed = 0.3f;
    //
    private int collisionCount = 0;
    private bool selected = false;
    //
    private bool isMoving = false;
    private IEnumerator mover;

    public IEnumerator MoveToCoroutine(Transform targ, Vector3 pos, float dur)
    {
        isMoving = true; GetComponent<BoxCollider2D>().enabled = false;

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

        isMoving = false; GetComponent<BoxCollider2D>().enabled = true;
        doneMoving = true;
    }

    void Start()
    {
        layer = GetComponent<SpriteRenderer>().sortingOrder;
        originX = transform.position.x; originY = transform.position.y;
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
            GetComponent<SpriteRenderer>().sortingOrder = bar_layer + pivot;
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
            GetComponent<SpriteRenderer>().sortingOrder = bar_layer + pos;
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
        
        mover = MoveToCoroutine(transform, new Vector3(r.prevX, r.prevY, 0), speed);
        StartCoroutine(mover);

        selected = false; doneMoving = false;
        GameEventSystem.current.onSelectedTileMove -= UponOtherTileSelected;
        GameEventSystem.current.onMatch_Destroy -= UponMatchDestruction;
        GameEventSystem.current.onDestroy_RearrangeBar -= UponMatchRearrange;
        GameEventSystem.current.onUndo -= UponUndoReset;

        GetComponent<SpriteRenderer>().sortingOrder = layer;

        return 0;
    }

    private void OnMouseDown() //TILE IS SELECTED
    {
        if (selected)
            return;
        if (BoardManager.instance.CheckIfFull())
            return;

        selected = true;

        GameEventSystem.current.onSelectedTileMove += UponOtherTileSelected;
        GameEventSystem.current.onMatch_Destroy += UponMatchDestruction;
        GameEventSystem.current.onDestroy_RearrangeBar += UponMatchRearrange;
        GameEventSystem.current.onUndo += UponUndoReset;

        if (doneMoving == true)
            doneMoving = false;
        StartCoroutine(MoveToBar());
    }

    bool doneMoving = false;

    IEnumerator MoveToBar()
    {
        BoardManager.instance.bar.StackUp(gameObject, originX, originY);
        yield return new WaitUntil(() => doneMoving == true);
        doneMoving = false;                                                               //not sure how this line works but it stays because it is working
        BoardManager.instance.bar.CheckForMatch();
        //yield return new WaitUntil(() => doneMoving == true);                           //not sure how this line works but it stays because it is working
        
        if (BoardManager.instance.CheckIfLost())
            Debug.Log("Lost!");
        if(BoardManager.instance.CheckIfWon())
            Debug.Log("Won!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int _this = GetComponent<SpriteRenderer>().sortingOrder;
        int _other = collision.GetComponent<SpriteRenderer>().sortingOrder;
        if (_this < _other) 
        {
            collisionCount++; 
        }
        if (collisionCount > 0)
        {
            GetComponent<SpriteRenderer>().material.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            gameObject.layer = 2;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        int _this = GetComponent<SpriteRenderer>().sortingOrder;
        int _other = collision.GetComponent<SpriteRenderer>().sortingOrder;
        if (_this < _other) 
        {
            collisionCount--; 
        }
        if (collisionCount == 0)
        {
            GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1f);
            gameObject.layer = 0;
        }
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
