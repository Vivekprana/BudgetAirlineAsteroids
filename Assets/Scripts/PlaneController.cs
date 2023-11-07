using UnityEngine;

public class PlaneController : MonoBehaviour
{
    public Animator animator;
    public AudioSource Thump;


    [Range(0f, 180f)]
    public float pitchForce = 90f;
    [Range(0f, 180f)]
    public float rollForce = 90f;
    [Range(0f, 180f)]
    public float yawForce = 90f;
    [Range(0.01f, 0.1f)]
    public float thrustForce = 50.0f;


    // Smoothing 
    [Header("Smoothing")]
    public float smooth = 5.0f;

    private void Update()
    {
    
        // Handle input for controlling the airplane
        float pitchInput = -Input.GetAxis("Mouse Y");
        float rollInput = Input.GetAxis("Mouse X");
        float yawInput = Input.GetAxis("Horizontal");
        float thrustInput = Input.GetAxis("Vertical");

        // Add Yaw Rotation From Arrow Keys
        float pitchEffect = pitchInput * pitchForce;
        float rollEffect = rollInput * rollForce;
        float yawEffect = yawInput * yawForce;

        Vector3 rotation = new Vector3(pitchEffect, yawEffect, rollEffect);
        transform.Rotate(rotation);


        // Apply thrust
        Vector3 thrust = transform.forward * thrustInput * thrustForce;
        transform.position += thrust;
        

        // Roll Effect
        if(rollEffect < 0)
        {
            animator.SetBool("Left Roll", true);
            animator.SetBool("Right Roll", false);

        }
        if(rollEffect > 0)
        {
            animator.SetBool("Right Roll", true);
            animator.SetBool("Left Roll", false);

        }

        if(pitchEffect < 0)
        {
            animator.SetBool("Pitch Down", true);
            animator.SetBool("Pitch Up", false);

        }
        if(pitchEffect > 0)
        {
            animator.SetBool("Pitch Up", true);
            animator.SetBool("Pitch Down", false);

        }

        if (yawEffect < 0)
        {
            animator.SetBool("Yaw Left", true);
            animator.SetBool("Yaw Right", false);

        }

        if (yawEffect > 0)
        {
            animator.SetBool("Yaw Right", true);
            animator.SetBool("Yaw Left", false);

        }

        if (yawEffect == 0)
        {
            animator.SetBool("Yaw Right", false);
            animator.SetBool("Yaw Left", false);

        }


    }

    private void OnCollisionEnter(Collision other) 
    {
        if(!Thump.isPlaying)
        {
            Thump.PlayDelayed(0);
        }
    }
    


}