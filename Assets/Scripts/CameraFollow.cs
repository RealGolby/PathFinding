using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    public GameObject SquareSprite;
    public bool Debug;
    private float x = 0;
    private float y = 0;
    public float limitX;
    public float limitY;
    // Start is called before the first frame update
    void Start()
    {
        if (Debug)
        {
            SquareSprite.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.x < limitX && player.transform.position.x > -limitX)
        {
            x = player.transform.position.x;
        }
        if (player.transform.position.y < limitY && player.transform.position.y > -limitY)
        {
            y = player.transform.position.y;
        }

        SquareSprite.transform.localScale = new Vector3(limitX * 2, limitY * 2, 1);
        transform.position = new Vector3(x, y, -10);
    }
}