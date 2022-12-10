using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sert à bouger la tuile à laquelle ce script est rattaché
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
