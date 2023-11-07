using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PursuitAI : MonoBehaviour
{
    [Header("Target")]
    public GameObject TargetPlane;

    [Header("Obstacle Detection")]
    public GameObject WingObstacleDetectionPointL;
    public GameObject WingObstacleDetectionPointR;

    [Header("Smoothness Variables")]
    public float Speed = 1.0f;
    public float TurnTime = 1.0f;
    public float BufferDistance = 2.0f;
    float startTime;

    public List<Transform> ManeuverOptions;

    [Header("Animations")]
    public Animator animator;



    private bool finishedTurning = true;
    // Start is called before the first frame update
    private enum PursuitState
    {
        DirectFollow,
        ManeuverObstacle
    }

    private enum ManeuverState
    {
        TurnAway,
        Turning,
        MoveForward,
        BackOnTrack,
    }

    private enum ManeuverMode
    {
        None,
        LWing,
        RWing,
        Body,
        All
    }

    ManeuverMode currentManeuverMode;

    ManeuverState currentManeuverState = ManeuverState.TurnAway;

    Vector3 avoidanceManeuverPosition;

    PursuitState currentState = PursuitState.DirectFollow;

    // Update is called once per frame

    void Start()
    {
        startTime = Time.time;
    }
    void Update()
    {
        if (currentState == PursuitState.DirectFollow)
        {
            detectObstactles();
            directPursuit();
        }
        if (currentState == PursuitState.ManeuverObstacle)
        {
            maneuverAround();
        }

    }

    private void directPursuit()
    {
        turnToTarget(TargetPlane.transform.position);
        moveToTarget(TargetPlane.transform.position);
    }

    private void detectObstactles()
    {
        checkSensors();
    }

    private void checkSensors()
    {
        bool lWing, rWing, body;
        lWing = rWing = body = false;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, BufferDistance))
        {
            if (hit.collider.name.Contains("Asteroid"))
            {
                body = true;
            }
        }
        if (Physics.Raycast(WingObstacleDetectionPointL.transform.position, transform.TransformDirection(Vector3.forward), out hit, BufferDistance))
        {
            if (hit.collider.name.Contains("Asteroid"))
            {
                lWing = true;
            }
        }
        if (Physics.Raycast(WingObstacleDetectionPointR.transform.position, transform.TransformDirection(Vector3.forward), out hit, BufferDistance))
        {
            if (hit.collider.name.Contains("Asteroid"))
            {
                rWing = true;
            }
        }

        if (body)
        {
            currentManeuverMode = ManeuverMode.Body;
            if (lWing && rWing)
            {
                currentManeuverMode = ManeuverMode.All;
            }
        }
        else if (lWing)
        {
            currentManeuverMode = ManeuverMode.LWing;
        }
        else if (rWing)
        {
            currentManeuverMode = ManeuverMode.RWing;
        }


        if (body || lWing || rWing)
        {
            currentState = PursuitState.ManeuverObstacle;
            currentManeuverState = ManeuverState.TurnAway;
        }
    }

    private void maneuverAround()
    {
        // checkSensors();
        if (currentManeuverState == ManeuverState.TurnAway)
        {
            switch (currentManeuverMode)
            {
                case ManeuverMode.All:
                    avoidCollision();
                    break;

                case ManeuverMode.Body:
                    avoidCollision();
                    break;


                case ManeuverMode.RWing:
                    avoidCollision(2);
                    break;
                
                case ManeuverMode.LWing:
                    avoidCollision();
                    break;

                default:
                    break;
            }
        }

        if (currentManeuverState == ManeuverState.Turning)
        {
            // Turn Towards Target
            turnToTarget(avoidanceManeuverPosition);

            // Move Towards Target
            moveToTarget(avoidanceManeuverPosition); // need to switch the target
            if (finishedTurning) { checkSensors(); }

            if (Vector3.Distance(avoidanceManeuverPosition, transform.position) < 0.01)
            {
                currentManeuverState = ManeuverState.MoveForward;
                startTime = Time.time;
            }
        }
        if (currentManeuverState == ManeuverState.MoveForward)
        {
            // Turn Towards Target
            turnToTarget(TargetPlane.transform.position);
            // Move Towards Target
            moveToTarget(TargetPlane.transform.position);

            if(Vector3.Distance(avoidanceManeuverPosition, transform.position) > 1)
            {
                currentManeuverState = ManeuverState.BackOnTrack;
                startTime = Time.time;
            }
        }

        if (currentManeuverState == ManeuverState.BackOnTrack)
        {
            currentState = PursuitState.DirectFollow;
            currentManeuverState = ManeuverState.TurnAway;
            currentManeuverMode = ManeuverMode.None;
        }
    }

    private void avoidCollision(int maneuverIndex = 0)
    {
        int maneuverI = maneuverIndex;
        bool SearchingAlternateRoutes = true;

        while (SearchingAlternateRoutes)
        {
            avoidanceManeuverPosition = ManeuverOptions[maneuverI].position;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(avoidanceManeuverPosition), out hit, 1.5f))
            {
                if(maneuverI < ManeuverOptions.Count - 1)
                {
                    maneuverI += 1;
                }
                else
                {
                    SearchingAlternateRoutes = false;
                }
            }
            else
            {
                SearchingAlternateRoutes = false;
            }
        }
        currentManeuverState = ManeuverState.Turning;
        startTime = Time.time;
    }

    private void turnToTarget(Vector3 target)
    {
        Quaternion lookRotation = Quaternion.LookRotation(target - transform.position);
        setTurningAnimations(lookRotation);
        if (currentManeuverMode == ManeuverMode.RWing) { lookRotation.eulerAngles += new Vector3(0, 0, 90); }
        if (currentManeuverMode == ManeuverMode.LWing) { lookRotation.eulerAngles += new Vector3(0, 0, -90);}
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, (Time.time - startTime) / TurnTime);

        // Determine the angle left to turn
        finishedTurning = Vector3.Angle(target - transform.position, transform.forward) < 1 ? true : false;
    }

    private void moveToTarget(Vector3 target)
    {
        float step = Speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);
    }

    private void setTurningAnimations(Quaternion lookRotation)
    {
        if(lookRotation.eulerAngles.x < 0){
            animator.SetBool("Left Roll", true);
            animator.SetBool("Right Roll", false);
        }
        if(lookRotation.eulerAngles.x > 0){
            animator.SetBool("Right Roll", true);
            animator.SetBool("Left Roll", false);
        }
        if(lookRotation.eulerAngles.x == 0){
            animator.SetBool("Right Roll", false);
            animator.SetBool("Left Roll", false);
        }
        if(lookRotation.eulerAngles.y < 0) {
            animator.SetBool("Pitch Down", true);
            animator.SetBool("Pitch Up", false);
        }
        if(lookRotation.eulerAngles.y > 0) {
            animator.SetBool("Pitch Down", false);
            animator.SetBool("Pitch Up", true);

        }
        if(lookRotation.eulerAngles.y == 0) {
            animator.SetBool("Pitch Down", false);
            animator.SetBool("Pitch Up", false);

        }
        if(lookRotation.eulerAngles.z < 0) {
            animator.SetBool("Yaw Left", true);
            animator.SetBool("Yaw Right", false);
        }
        if(lookRotation.eulerAngles.z > 0) {
            animator.SetBool("Yaw Left", false);
            animator.SetBool("Yaw Right", true);
        }
        if(lookRotation.eulerAngles.z == 0) {
            animator.SetBool("Yaw Left", false);
            animator.SetBool("Yaw Right", false);
        }

    }

}



