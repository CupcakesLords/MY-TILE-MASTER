using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileInGame : MonoBehaviour, IPointerDownHandler
{
    private int layer;
    private float originX, originY;
    private int collisionCount = 0;
    private bool selected = false;
    private bool isMoving = false;

    private IEnumerator mover;

    private IEnumerator MoveToCoroutine(Transform targ, Vector3 pos, float dur)
    {
        isMoving = true; GetComponent<BoxCollider2D>().enabled = false; gameObject.tag = "Moving";

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

        isMoving = false; GetComponent<BoxCollider2D>().enabled = true; gameObject.tag = "Untagged";

        doneMoving = true;
    }

    private IEnumerator WitherAway(float dur)
    {
        yield return StartCoroutine(Wither(0.3f));
        Destroy(gameObject);
    }

    private IEnumerator Wither(float dur)
    {
        float t = 0f;
        while (t < dur && transform.localScale.x >= 0)
        {
            t += Time.deltaTime;
            transform.localScale -= new Vector3(1f, 1f, 0) * (t * t / dur);
            yield return null;
        }
    }

    void Start()
    {
        layer = GetComponent<SpriteRenderer>().sortingOrder;
        originX = transform.position.x; originY = transform.position.y;

        GameEventSystem.current.onRefresh += UponRefresh;
        GameEventSystem.current.onHint += UponHint;
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
            mover = MoveToCoroutine(transform, new Vector3(BoardManager.instance.xBar + pivot, BoardManager.instance.yBar, 0), BoardManager.instance.speed);
            StartCoroutine(mover);
            GetComponent<SpriteRenderer>().sortingOrder = BoardManager.instance.bar_layer + pivot;
        }
        return 0;
    }

    private int UponMatchDestruction(GameObject identity)
    {
        if (identity == gameObject && selected)
        {
            BoardManager.instance.OnTileDestroy();
            StartCoroutine(WitherAway(BoardManager.instance.destruction_speed));
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
            mover = MoveToCoroutine(transform, new Vector3(BoardManager.instance.xBar + pos, BoardManager.instance.yBar, 0), BoardManager.instance.speed);
            StartCoroutine(mover);
            GetComponent<SpriteRenderer>().sortingOrder = BoardManager.instance.bar_layer + pos;
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
        
        mover = MoveToCoroutine(transform, new Vector3(r.prevX, r.prevY, 0), BoardManager.instance.speed);
        StartCoroutine(mover);

        selected = false; doneMoving = false;
        GameEventSystem.current.onSelectedTileMove -= UponOtherTileSelected;
        GameEventSystem.current.onMatch_Destroy -= UponMatchDestruction;
        GameEventSystem.current.onDestroy_RearrangeBar -= UponMatchRearrange;
        GameEventSystem.current.onUndo -= UponUndoReset;

        GameEventSystem.current.onRefresh += UponRefresh;
        GameEventSystem.current.onHint += UponHint;

        GetComponent<SpriteRenderer>().sortingOrder = layer;

        return 0;
    }

    private int UponRefresh(int layer, int direction)
    {
        if (selected) return 0;
        int dif = GetComponent<SpriteRenderer>().sortingOrder - layer;
        if (!(dif >= 0 && dif < 100)) return 0;

        Vector3 temp = transform.position;
        if (direction == 0)
            transform.position -= new Vector3(15, 0, 0); //to the left
        else if (direction == 1)
            transform.position += new Vector3(0, 15, 0); //go up
        else if (direction == 2)
            transform.position += new Vector3(15, 0, 0); //to the right

        if (isMoving)
        {
            StopCoroutine(mover);
            isMoving = false;
        }
        mover = MoveToCoroutine(transform, temp, BoardManager.instance.speed);
        StartCoroutine(mover);

        return 0;
    }

    private int UponHint(GameObject obj)
    {
        if (obj == gameObject)
        {
            PlayerClick();
        }
        return 0;
    }

    public void OnPointerDown(PointerEventData eventData) //TILE IS SELECTED
    {
        PlayerClick();
    }

    private void PlayerClick()
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

        GameEventSystem.current.onRefresh -= UponRefresh;
        GameEventSystem.current.onHint -= UponHint;

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
        
        BoardManager.instance.CheckIfLost();
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
        else
        {
            GameEventSystem.current.onRefresh -= UponRefresh;
            GameEventSystem.current.onHint -= UponHint;
        }
    }
}
