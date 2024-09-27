using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{

    [SerializeField] Rigidbody rb;
    [SerializeField] float speed;
    Vector2 input;
    public bool sneak = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (Input.GetButton("Sneaking"))
        {
            if (sneak)
            {
                sneak = false;
                speed = speed * 2;
            }
            else
            {
                sneak = true;
                speed = speed / 2;
            }
        }
    }
    void FixedUpdate()
    {
        var newInput = GetCameraBasedInput(input, Camera.main);
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

    public bool returnSneak()
    {
        return sneak;
    }
}
