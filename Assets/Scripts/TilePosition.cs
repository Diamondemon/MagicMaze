using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePosition : MonoBehaviour
{
    public float speed = 3;
    public Vector3 initialPosition = new Vector3(24, 0, 24);
    void Start()
    {
        transform.position = initialPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
