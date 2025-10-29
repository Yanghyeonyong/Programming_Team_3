using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyRange : MonoBehaviour
{
    [SerializeField] float _plusMoveSpeed;
    Enemy _enemy;

    private void Start()
    {
        _enemy = GetComponentInParent<Enemy>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _enemy.PlusMoveSpeed(_plusMoveSpeed);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _enemy.PlusMoveSpeed(-_plusMoveSpeed);
        }
    }
}