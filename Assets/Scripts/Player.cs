using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject player;
    public GameObject top;
    public GameObject bottom;

    public float speed = 2f;

    private float playerHeight = 0;

    private float maxY;
    private float minY;
    private float currentX;

    private Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            Vector2 size = boxCollider.size;
            playerHeight = size.y;
        }

        maxY = top.transform.position.y + playerHeight / -2;
        minY = bottom.transform.position.y + playerHeight / 2;
        currentX = player.transform.position.x;
    }
    
    // Update is called once per frame
    void Update()
    {
        float verticalInput = Input.GetAxis("Vertical");
        player.transform.Translate(new Vector3(0, verticalInput, 0) * (speed * Time.deltaTime));
        float currentY = player.transform.position.y;
        player.transform.position = new Vector3(currentX,Mathf.Clamp(currentY, minY, maxY), 0);
    }
}
