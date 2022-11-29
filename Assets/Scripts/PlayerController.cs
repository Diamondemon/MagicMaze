using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDir = new Vector3(0, 0, 0);
        float speedMultiplier  = 1;
        if (Input.GetAxisRaw("Sprint") == 1){
            speedMultiplier = 3;
        }
        moveDir +=  transform.forward * Input.GetAxisRaw("Vertical") + transform.right* Input.GetAxisRaw("Horizontal");

        transform.position += moveDir  * speedMultiplier * moveSpeed * Time.deltaTime;
    }
}
