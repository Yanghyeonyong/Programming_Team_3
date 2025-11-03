using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class  MapPatternTrigger : MonoBehaviour
{
    [SerializeField] private float _attack;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            other.GetComponent<Player>().TakeDamage(_attack);
        }
    }
}
