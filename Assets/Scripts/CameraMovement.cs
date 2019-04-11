using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    public List<Transform> targets = new List<Transform>();
    public Image fade;
    Transform lookPoint;
    Transform camera;

    [Header("Speed")]
        [Tooltip("This is the speed at which the camera will go.")]
        public float speed = 3f;

    [Header("Stopping Distance"), Range(0.05f, 0.5f)]
        [Tooltip("How close the camera will get to its target before moving on to the next one.")]
        public float stoppingDistance = 0.15f;

        [Tooltip("This is the amount of time that the camera will go idle for at each stop."), VectorLabels("Timer", "CurrentTime")]
        public Vector2 idleTime = new Vector2(0.0f, 0.0f);

    [Header("Targeting")]
        [Tooltip("This is what the camera is currently targeting."), SerializeField]
        private Transform currentTarget;
        private Transform previousTarget;
        int target;

        [Tooltip("This will randomise targets making the camera go to a random target each time."), SerializeField]
        private bool randomTargeting;

        [Tooltip("Enable if you what the camera to face its next target."), SerializeField]
        private bool faceTarget;

    // Start is called before the first frame update
    void Start()
    {
        camera = transform.GetChild(0);
        lookPoint = transform.Find("LookPoint");
        Transform path = transform.Find("Path");

        if (transform.Find("Path").childCount == 0)
            Debug.LogError("No paths found on " + this.name + " game object. " + this.GetType() + ".cs requires paths.");
        else if (transform.Find("Path").childCount == 1)
            Destroy(this);

        for (int i = 0; i < path.childCount; i++)
            targets.Add(path.GetChild(i));

        foreach (Transform node in targets)
        {
            Destroy(node.GetComponent<MeshRenderer>());
            Destroy(node.GetComponent<MeshFilter>());
        }

        Destroy(lookPoint.GetComponent<MeshRenderer>());
        Destroy(lookPoint.GetComponent<MeshFilter>());

        ChangeTarget(0);
    }

    // Update is called once per frame
    void Update()
    {
        FadeIn();

        Move();

        UpdateSpeed();

        Rotate();

        Targeting();
    }

    void Move()
    {
        camera.position = Vector3.MoveTowards(camera.position, currentTarget.position, speed * Time.deltaTime);
    }

    public void Rotate()
    {
        if (faceTarget)
        {
            Vector3 direction = (currentTarget.position - transform.GetChild(0).position).normalized;
            if ((new Vector3(direction.x, 0, direction.z)) != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.GetChild(0).rotation = Quaternion.Slerp(transform.GetChild(0).rotation, lookRotation, Time.deltaTime * 0.5f);
            }
        }
        else
            camera.LookAt(lookPoint.position);
    }

    void Targeting()
    {
        float distance = Vector3.Distance(currentTarget.position, transform.GetChild(0).position);
        if (distance <= stoppingDistance)
        {
            idleTime.y -= Time.deltaTime;
            if (idleTime.y < 0)
            {
                if (randomTargeting)
                    ChangeTarget(Random.Range(0, targets.Count));
                else
                    ChangeTarget(target+1);

                idleTime.y = idleTime.x;
            }
        }
    }

    void ChangeTarget(int targetNumber)
    {
        if (targetNumber == targets.Count)
            targetNumber = 3;

        target = targetNumber;
        previousTarget = currentTarget;
        currentTarget = targets[targetNumber];
    }

    void FadeIn()
    {
        if(target == 2 || target == 3)
        {
            Color fadeColor = fade.color;
            fadeColor.a -= Time.deltaTime * 0.1f;
            fade.color = fadeColor;
        }
    }

    void UpdateSpeed()
    {
        SpeedChecker(2, 3, 5.0f);
        SpeedChecker(3, 4, 7.0f);

    }

    void SpeedChecker(int _previousTarget, int _target, float _speed)
    {
        if (previousTarget == targets[_previousTarget - 1] && currentTarget == targets[_target - 1])
            speed = _speed;
    }
}
