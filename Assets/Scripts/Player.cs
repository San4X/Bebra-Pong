using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject upperFrame;
    public GameObject lowerFrame;

    public float speed = 2f;
    
    private float _upperBorder;
    private float _lowerBorder;
    
    
    // Start is called before the first frame update
    void Start()
    {
        float playerSizeY = GetComponent<BoxCollider2D>().size.y;
        float upperFrameSizeY = upperFrame.GetComponent<BoxCollider2D>().size.y;
        float lowerFrameSizeY = lowerFrame.GetComponent<BoxCollider2D>().size.y;
        
        _upperBorder = upperFrame.transform.position.y - upperFrameSizeY / 2f - playerSizeY / 3f;
        _lowerBorder = lowerFrame.transform.position.y + lowerFrameSizeY / 2f + playerSizeY / 3f;
    }
    
    // Update is called once per frame
    void Update()
    {
        float verticalInput = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(0, verticalInput, 0) * (speed * Time.deltaTime));
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, _lowerBorder, _upperBorder), 0);
    }
}
