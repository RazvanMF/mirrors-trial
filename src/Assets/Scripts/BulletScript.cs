///SCRIPT THAT DEALS WITH BULLET MOVEMENT
///HANDLES: BULLET DESTRUCTION AFTER SET PERIOD, SPEED AND MOVEMENT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector2 directionVector { get; set; }

    //[SerializeField] PowerupState _powerups;

    float _speed = 12.5f;
    int _hits = 1;


    [SerializeField] LayerMask _layerWall;

    void Start()
    {
        if (!PowerupState.ricochet)
            _hits = 1;
        else
            _hits = 2;

        if (!PowerupState.fastBullet)
            _speed = 12.5f;
        else
            _speed = 20f;
    }

    // Update is called once per frame
    void Update()
    {
        //movement on the right position vector, modified at instantiation
        this.transform.Translate(Vector3.right * Time.deltaTime * _speed);

        var hit = Physics2D.Raycast(this.transform.position, this.transform.right, 1, _layerWall);
        if (hit.point != Vector2.zero)
        {
            this.transform.right = Vector2.Reflect(this.transform.right, hit.normal);
            _hits--;
        }

        if (_hits == 0)
            Destroy(this.gameObject);
    }
}
