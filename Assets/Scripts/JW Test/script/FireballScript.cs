using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FireballScript : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _damage = 2f;
    [SerializeField] private float _fireballLifetime = 5f;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.OnTakeDamage(_damage);
            }
            gameObject.SetActive(false);
        }
    }

    IEnumerator _delayDisable()
    {
        yield return new WaitForSeconds(_fireballLifetime);
        gameObject.SetActive(false);
    }


    void OnEnable()
    {
        StartCoroutine(_delayDisable());
    }
    
    void Awake()
    {
        //Destroy(gameObject, _fireballLifetime);
    }

    void Update()
    {
        transform.position += transform.right * _speed * Time.deltaTime;
    }


}
