using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public GameObject board;
    SpriteRenderer tile;

    void Start()
    {
        tile = board.GetComponent<SpriteRenderer>();

        float width = tile.bounds.size.x * 8;
        float height = tile.bounds.size.y * 10;

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
}