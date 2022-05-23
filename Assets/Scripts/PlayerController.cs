using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    Animator anim;
    private SphereCollider col;
    private Vector3 dir;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity;
    [SerializeField] private int pins;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private Text pinsText;
    [SerializeField] private Score scoreScript;
    private bool isImmortal;

    private int lineToMove = 1;
    public float lineDistance = 4;
    private float maxSpeed = 110;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
        col = GetComponent<SphereCollider>();
        Time.timeScale = 1;
        pins = PlayerPrefs.GetInt("Pins");
        pinsText.text = pins.ToString();
        StartCoroutine(SpeedIncrease());
        isImmortal = false;

    }

    private void Update()
    {
        if (SwipeController.swipeRight)
        {
            if (lineToMove < 2)
                lineToMove++;
        }

        if (SwipeController.swipeLeft)
        {
            if (lineToMove > 0)
                lineToMove--;
        }

        if (SwipeController.swipeUp)
        {
            if (controller.isGrounded)
                Jump();
        }
        if(SwipeController.swipeDown)
        {
            StartCoroutine(Slide());
        }

        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;
        if (lineToMove == 0)
            targetPosition += Vector3.left * lineDistance;
        else if (lineToMove == 2)
            targetPosition += Vector3.right * lineDistance;

        if (transform.position == targetPosition)
            return;
        Vector3 diff = targetPosition - transform.position;
        Vector3 moveDir = diff.normalized * 25 * Time.deltaTime;
        if (moveDir.sqrMagnitude < diff.sqrMagnitude)
            controller.Move(moveDir);
        else
            controller.Move(diff);
        
        
    }

    private void Jump()
    {
        dir.y = jumpForce;
    }

    void FixedUpdate()
    {
        dir.z = speed;
        dir.y += gravity * Time.fixedDeltaTime;
        controller.Move(dir * Time.fixedDeltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if(hit.gameObject.tag == "obstacle")
        {
            if (isImmortal)
                Destroy(hit.gameObject);
            else
            {
                losePanel.SetActive(true);
                int lastRunScore = int.Parse(scoreScript.scoreText.text.ToString());
                PlayerPrefs.SetInt("lastRunScore", lastRunScore);
                Time.timeScale = 0;
            }
            
        }
    }
    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.tag == "Pins")
        {
            pins++;
            PlayerPrefs.SetInt("Pins", pins);
            pinsText.text = pins.ToString();
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Armor")
        {
            StartCoroutine(ArmorBonus());
            Destroy(other.gameObject);
        }
        
    }
    private IEnumerator SpeedIncrease()
    {
        yield return new WaitForSeconds(4);
        if(speed < maxSpeed)
        {
            speed += 1;
            StartCoroutine(SpeedIncrease());

        }
        
    }
    private IEnumerator Slide()
    {
        col.center = new Vector3(0, -0.5f, 0);
        
        yield return new WaitForSeconds(1);
        col.center = new Vector3 (0, 0, 0);

    }
    private IEnumerator ArmorBonus()
    {
        isImmortal = true;
        yield return new WaitForSeconds(5);
        isImmortal = false;
    }
}