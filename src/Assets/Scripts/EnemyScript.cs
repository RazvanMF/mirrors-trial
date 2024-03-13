///SCRIPT THAT DEALS WITH ENEMY BEHAVIOURS
///HANDLES: ENEMY MOVEMENT, SHOOTING, DESTRUCTION (PLAYING OF PARTICLES, SCORE INCREMENT ETC.), BULLET SPAWNING, REACTION AT HIT
///GETS: ACCESS TO BULLET PREFAB, PLAYER TRANSFORM
///ACCESSIBLE: SPEED MODIFIER

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform _playerLocation;
    [SerializeField] float _speed = 2.5f;
    [SerializeField] int _health = 3;

    [SerializeField] GameObject _enemyBulletPrefab;

    [SerializeField] AudioClip[] _sounds;
    AudioSource _audioSource;

    public Animator anim;
    private SpriteRenderer sprite;

    private Vector3 directionVector;

    //[SerializeField] PowerupState _powerups;

    void Start()
    {
        _playerLocation = GameObject.Find("Player").transform;
        StartCoroutine(BulletSpawnCoRoutine());
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _sounds[0]; _audioSource.loop = false;
        _audioSource.Play();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //stopping distance from player, moves to player until the distance between them is this 
        float distance = 1.25f;
        if (Vector3.Distance(_playerLocation.position, this.transform.position) > distance)
        {
            directionVector = (_playerLocation.position - this.transform.position).normalized;
            this.transform.Translate(directionVector * Time.deltaTime * _speed);
        }

        Animate();

        if (directionVector.x > 0f)
        {
            sprite.flipX = false;
        }
        else
        {
            sprite.flipX = true;
        }

        //trajectory
        //Debug.DrawRay(this.transform.position, _playerLocation.position - this.transform.position, Color.magenta);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //killed if a PLAYER bullet hits it, plays particle system
        if (other.tag == "Bullet")
        {
            Destroy(other.gameObject);

            if (!PowerupState.oneShot)
                _health--;
            else
                _health = 0;

            this.GetComponent<ParticleSystem>().Play();
            _audioSource.clip = _sounds[1]; _audioSource.loop = false;
            _audioSource.Play();

            if (_health == 0)
            {
                PlayerState.score++;
                this.GetComponent<Renderer>().enabled = false;

                Destroy(this.gameObject, 0.3f);
                Destroy(this);
            }
        }
    }

    private IEnumerator BulletSpawnCoRoutine()
    {
        while (true)
        {
            //cooldown
            if (!PowerupState.fastBullet)
                yield return new WaitForSeconds(Random.Range(0.8f, 1.2f));
            else
                yield return new WaitForSeconds(Random.Range(0.4f, 0.8f));

            //direction vector for bullet trajectory and rotation on z axis
            Vector3 _directionVector = (_playerLocation.position - this.transform.position).normalized;
            float angleRot = Mathf.Atan2(_directionVector.y, _directionVector.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angleRot, Vector3.forward);

            //instantiantion and tag
            //GameObject bullet = Instantiate(_enemyBulletPrefab, this.transform.position, rotation);
            //bullet.tag = "EnemyBullet";
            //bullet.transform.GetComponent<SpriteRenderer>().color = Color.red;


            if (!PowerupState.shotgunEnabled)
            {
                GameObject bullet = Instantiate(_enemyBulletPrefab, this.transform.position, rotation);
                bullet.tag = "EnemyBullet";
                bullet.transform.GetComponent<SpriteRenderer>().color = Color.red;
            }
            else
            {
                float[] rotations = new float[5] { -30f, -15f, 0f, 15f, 30f };
                for (int i = 0; i < 5; i++)
                {
                    GameObject bullet = Instantiate(_enemyBulletPrefab, this.transform.position, rotation);
                    bullet.tag = "EnemyBullet";
                    bullet.transform.Rotate(Vector3.forward, rotations[i]);
                    bullet.transform.GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
        }
    }

    void Animate()
    {
        anim.SetFloat("AnimMoveX", directionVector.x);
        anim.SetFloat("AnimMoveY", directionVector.y);
    }
}
