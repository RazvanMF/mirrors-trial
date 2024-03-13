///SCRIPT THAT DEALS WITH PLAYER MOVEMENT ONLY
///HANDLES: PLAYER MOVEMENT, DASH CONTROL, CROSSHAIR POSITION(tbd), GIZMO LINE DEBUG
///GETS: ACCESS TO CAMERA, CROSSHAIR AND THE PLAYER STATE
///ACCESSIBLE: SPEED MODIFIER, STATIC POSITION BETWEEN SCENES

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Camera _camera;
    [SerializeField] float _speed = 5f;
    [SerializeField] PlayerState _modifiers;

    //animator and animation states
    public Animator anim;
    private SpriteRenderer sprite;
    private bool isDashing = false;


    //rigidbody for movement, via direction vector
    Rigidbody2D _rigidBody;
    Vector2 _directionVector;
    public static Vector2 _positionRetainerVector = Vector2.zero;

    //crosshair, should be moved out of here sometime
    GameObject _crosshair;

    //dash cooldown, depends on movement, and is local
    float _dashCooldown = 0;

    void Start()
    {
        this.transform.position = _positionRetainerVector;
        _crosshair = GameObject.Find("Crosshair");
        _rigidBody = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float _horizontalInput = Input.GetAxis("Horizontal");
        float _verticalInput = Input.GetAxis("Vertical");
        _directionVector = new Vector2(_horizontalInput, _verticalInput); //variable for fixed update
        _positionRetainerVector = this.transform.position;

        //dash cooldown will be decremented, dashTimer is 1.5f, flag for fixed update
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetMouseButtonDown(1)) && (_modifiers.canDash && _dashCooldown == 0))
        {
            _modifiers.canDash = false;
            _dashCooldown = _modifiers.dashTimer;
            isDashing = true;
        }

        //crosshair dependencies
        Vector3 currentLinePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        currentLinePos += Vector3.forward * 10;
        Debug.DrawRay(this.transform.position, currentLinePos - this.transform.position, Color.white);
        _crosshair.transform.position = currentLinePos;

        Animate();

        if (_directionVector.x > 0f)
        {
            sprite.flipX = false;
        }
        else
        {
            sprite.flipX = true;
        }
    }

    private void FixedUpdate()
    {
        //movement
        _rigidBody.velocity = _directionVector * _speed;

        //dash function
        if (!_modifiers.canDash)
        {
            StartCoroutine(DashAbilityCoRoutine());
        }
    }

    private IEnumerator DashAbilityCoRoutine()
    {
        _modifiers.canDash = true; //to avoid multiple coroutines
        _modifiers.canBeHit = false; //to PlayerShoot

        _rigidBody.AddForce(_directionVector * 3000f); //looks nice with trailrenderer
        yield return new WaitForSeconds(1);
        isDashing = false;

        _modifiers.canBeHit = true;
        yield return new WaitForSeconds(0.5f);

        _dashCooldown = 0;
    }

    //animation manager
    void Animate()
    {
        anim.SetFloat("AnimMoveX", _directionVector.x);
        anim.SetFloat("AnimMoveY", _directionVector.y);
        anim.SetFloat("AnimMoveMagnitude", _directionVector.magnitude);
        anim.SetBool("isDashing", isDashing);
    }
}
