using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float horizontalInput;
    public float verticalInput;
    //Nếu nhấn chuột trái thì chuyển sang trạng thái tấn công 
    public bool attackInput;


    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        if(!attackInput && Time.timeScale > 0)
        {
            attackInput = Input.GetMouseButtonDown(0); //Nhấn chuột trái 
        }
        
    }
    private void OnDisable()
    {
        horizontalInput = 0;
        verticalInput = 0;
        attackInput = false;
    }
}
