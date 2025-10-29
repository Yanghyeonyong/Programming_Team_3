using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //testing
    [SerializeField] private GameObject _attackTwoFireballPrefab;
    
    [SerializeField] private float _playerHealth = 100f;
    
    [SerializeField] private float _attackOneRange = 10f;
    [SerializeField] private float _attackTwoRange = 10f;
    [SerializeField] private float _attackOneDuration = 0.2f;
    [SerializeField] private float _attackTwoDuration = 0.3f;
    [SerializeField] private float _attackOneCooldown = 1f; 
    [SerializeField] private float _attackTwoCooldown = 1.5f;
    [SerializeField] private float  _takeDamageDuration = 0.2f;

    private float _currentAttackOneCooldown = 0f;
    private float _currentAttackTwoCooldown = 0f;

    private List<GameObject> _fireballPool = new List<GameObject>();





    private MoveComponent _moveComponent;
    private Animator _animator;
    public void TakeDamage(float damageAmount)
    {
        
        _playerHealth -= damageAmount;
        Debug.Log($"Player Health: {_playerHealth}");
        if (_playerHealth <= 0)
        {
            PlayerDeath();
        }
        StartCoroutine(TakeDamageCoroutine());
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("NextStageRecognition"))
        {
            GameStateManager.Instance.IsInNextStageRecognitionPlane = true;
        }
    }


    void PlayerDeath()
    {
        GameStateManager.Instance.IsPlayerAlive = false;
    }
    IEnumerator TakeDamageCoroutine()
    {
        _moveComponent.IsTakingDamage = true;
        _animator.SetBool("_isTakingDamage", true);
        yield return new WaitForSeconds(_takeDamageDuration);
        _moveComponent.IsTakingDamage = false;
        _animator.SetBool("_isTakingDamage", false);
        //why doesnt it stop


    }

    
    private int _layerMask = 1 << 6; // Layer 6 is "Enemy"

    IEnumerator AttackOne()
    {
        _currentAttackOneCooldown = _attackOneCooldown;
        _moveComponent.IsAttacking = true;
        _animator.SetBool("_attackOne", true);
        RaycastHit raycastHit;
        //_animator.GetBool(name: "_isRunning");

        yield return new WaitForSeconds(_attackOneDuration);

        if (Physics.Raycast(transform.position+ Vector3.up*0.1f, transform.right, out raycastHit, _attackOneRange, _layerMask))
        {
            Debug.DrawRay(transform.position + Vector3.up * 0.1f, transform.right * _attackOneRange, Color.red, 2f);
            Debug.Log(raycastHit.collider.name);

            if (raycastHit.collider.CompareTag("Enemy"))
            {
                Enemy enemy = raycastHit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.OnTakeDamage(2f);
                }
            }


            //Enemy enemy = raycastHit.collider.GetComponent<Enemy>();
            //if (enemy != null)
            //{
            //    enemy.TakeDamage(25f);
            //}
        }

        yield return new WaitForSeconds(_attackOneDuration);

        _moveComponent.IsAttacking = false;
        _animator.SetBool("_attackOne", false);

    }

    IEnumerator AttackTwo()
    {
        _currentAttackTwoCooldown = _attackTwoCooldown;
        _moveComponent.IsAttacking = true;
        _animator.SetBool("_attackTwo", true);
        //RaycastHit raycastHit;
        //_animator.GetBool(name: "_isRunning");

        yield return new WaitForSeconds(_attackTwoDuration);


        foreach (var fireball in _fireballPool)
        {
            if (fireball.activeSelf == false)
            {
                fireball.transform.position = transform.position + transform.right * 0.5f + transform.up * 0.25f;
                fireball.transform.rotation = transform.rotation;
                fireball.SetActive(true);
                break;
            }
        }


        //Instantiate(_attackTwoFireballPrefab, transform.position + transform.right *0.5f + transform.up * 0.5f, transform.rotation);

        //if (Physics.Raycast(transform.position, transform.right, out raycastHit, _attackTwoRange))
        //{
        //    //Enemy enemy = raycastHit.collider.GetComponent<Enemy>();
        //    //if (enemy != null)
        //    //{
        //    //    enemy.TakeDamage(25f);
        //    //}
        //}

        yield return new WaitForSeconds(_attackTwoDuration);

        _moveComponent.IsAttacking = false;
        _animator.SetBool("_attackTwo", false);

    }


    void OnEnable()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject temp = Instantiate(_attackTwoFireballPrefab);
            temp.SetActive(false);
            _fireballPool.Add(temp);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        


        _animator =transform.Find("Idle_0").GetComponent<Animator>();
        _moveComponent = GetComponent<MoveComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.U) && _currentAttackOneCooldown<=0)
        {
            
            StartCoroutine(AttackOne());
            

        }
        else if (Input.GetKeyDown(KeyCode.I) &&_currentAttackTwoCooldown <=0)
        {
            StartCoroutine(AttackTwo());
        }
        _currentAttackOneCooldown -= Time.deltaTime;
        _currentAttackTwoCooldown -= Time.deltaTime;

    }
}
