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
        _upperBorder = upperFrame.transform.position.y - upperFrame.GetComponent<BoxCollider2D>().bounds.extents.y - GetComponent<BoxCollider2D>().bounds.extents.y;
        _lowerBorder = lowerFrame.transform.position.y + lowerFrame.GetComponent<BoxCollider2D>().bounds.extents.y + GetComponent<BoxCollider2D>().bounds.extents.y;
    }
    
    // Update is called once per frame
    void Update()
    {
        float verticalInput = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(0, verticalInput, 0) * (speed * Time.deltaTime));
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, _lowerBorder, _upperBorder), 0);
    }
}
