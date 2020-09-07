using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public GameObject board;
    SpriteRenderer tile;

    public GameObject Map; bool drag = false;
    Vector3 touchStart;
    Vector3 originalPosition;

    void Start()
    {
        originalPosition = Camera.main.transform.position;

        GameEventSystem.current.onControlMap += UponMapControl;

        tile = board.GetComponent<SpriteRenderer>();
        
        float width = tile.bounds.size.x * 8;
        float height = tile.bounds.size.y * 14;

        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = width / height;

        if (screenRatio >= targetRatio)
        {
            Camera.main.orthographicSize = height / 2;
        }
        else
        {
            float differenceInSize = targetRatio / screenRatio;
            Camera.main.orthographicSize = height / 2 * differenceInSize;
        }
    }

    int UponMapControl(bool active)
    {
        drag = active;
        Map.SetActive(active);
        Camera.main.transform.position = originalPosition;
        return 0;
    }

    void Update()
    {
        if (drag)
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            if (Input.GetMouseButton(0))
            {
                Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Camera.main.transform.position += direction;
            }
        }
    }

    private void OnDestroy()
    {
        GameEventSystem.current.onControlMap -= UponMapControl;
    }
}