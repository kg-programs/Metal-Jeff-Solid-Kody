using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{

    [SerializeField] Rigidbody rb;
    [SerializeField] float speed;
    [SerializeField] Camera cam;
    Vector2 input;
    public bool sneak = false;
    float horizontalInput;
    float verticalInput;

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        input = new Vector2(horizontalInput,verticalInput);
        if (Input.GetButton("Sneaking"))
        {
            if (sneak)
            {
                sneak = false;
                speed = 200;
            }
            else
            {
                sneak = true;
                speed = 50;
            }
        }
    }
    void FixedUpdate()
    {
        var newInput = GetCameraBasedInput(input, cam);
        var newVelocity = new Vector3(newInput.x*speed*Time.fixedDeltaTime, rb.velocity.y, newInput.z*speed*Time.fixedDeltaTime);
        rb.velocity = newVelocity;

    }

    Vector3 GetCameraBasedInput(Vector2 input, Camera cam)
    {
        Vector3 camRight = cam.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 camForward = cam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        return input.x * camRight + input.y * camForward;
    }
    private void OnCollisonEnter(Collision collision)
    {
        Debug.Log("Hit");
    }
}
