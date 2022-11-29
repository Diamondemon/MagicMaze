using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Direction;

public class CharacterController : MonoBehaviour
{

    public enum Color
    {
        yellow, orange, green, purple
    }

    public Color color;
    public Square currentPosition;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Move(Direction.Direction d)
    {
        switch (d)
        {
            

        }
    }


}
