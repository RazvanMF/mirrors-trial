///SCRIPT THAT DEALS WITH PLAYER SHOOTING
///HANDLES: SPAWNING OF BULLET AND CONDITIONS OF SPAWNING, WITH ITS DIRECTION AND ROTATION, REACTION AT HIT
///GETS: ACCESS TO CAMERA, BULLET PREFAB, PLAYER STATE, AUDIO SOURCE AND CLIP


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] Camera _camera;
    [SerializeField] GameObject _bulletPrefab;

    [SerializeField] PlayerState _modifiers;
    //[SerializeField] PowerupState _powerups;

    [SerializeField] AudioClip _hitClip;
    AudioSource _audioSource;

    Vector3 _directionVector;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //direction of bullet, will end at the crosshair
        Vector3 currentLinePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        currentLinePos += Vector3.forward * 10;
        _directionVector = (currentLinePos - this.transform.position);

        if (Input.GetButtonDown("Fire1") && _modifiers.canShoot == true)
        {
            _modifiers.canShoot = false;
            StartCoroutine(CooldownBullet());

            //rotation script on z axis, bullet instantiation
            float angleRot = Mathf.Atan2(_directionVector.y, _directionVector.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angleRot, Vector3.forward);

            if (!PowerupState.shotgunEnabled)
            {
                GameObject bullet = Instantiate(_bulletPrefab, this.transform.position, rotation);
                bullet.transform.GetComponent<SpriteRenderer>().color = new Color(0, 0.5f, 1, 1);
            }
            else 
            {
                float[] rotations = new float[5] { -30f, -15f, 0f, 15f, 30f };
                for (int i = 0; i < 5; i++)
                {
                    GameObject bullet = Instantiate(_bulletPrefab, this.transform.position, rotation);
                    bullet.transform.Rotate(Vector3.forward, rotations[i]);
                    bullet.transform.GetComponent<SpriteRenderer>().color = new Color(0, 0.5f, 1, 1);
                }
            }

        }
    }

    private IEnumerator CooldownBullet()
    {
        if (!PowerupState.fastBullet)
            yield return new WaitForSeconds(0.5f);
        else
            yield return new WaitForSeconds(0.2f);

        _modifiers.canShoot = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //killed if enemy bullet hits and the player can be hit (should be modified to 3 lives / health bar later)
        if (collision.tag == "EnemyBullet" && _modifiers.canBeHit == true) 
        {
            _audioSource.clip = _hitClip; _audioSource.loop = false;
            _audioSource.Play();

            Destroy(collision.gameObject);

            if (!PowerupState.oneShot)
                _modifiers.health--;
            else
                _modifiers.health = 0;

            if (_modifiers.health == 0)
            {
                this.transform.GetComponent<Renderer>().enabled = false;
                this.transform.GetComponent<TrailRenderer>().enabled = false;
                Destroy(this.gameObject, 0.3f);
            }
        }
    }
}
