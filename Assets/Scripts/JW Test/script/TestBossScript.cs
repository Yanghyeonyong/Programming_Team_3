using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBossScript : MonoBehaviour
{
    Animator _animator;

    [SerializeField] private GameObject _fireBallPrefab;



    // Start is called before the first frame update
    void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
    }



    //_isBossAttacking

    


    // Update is called once per frame
    void Update()
    {
        
        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    _animator.SetBool("_isBossAttacking", true);

        //    Instantiate (_fireBallPrefab, transform.position + transform.right, _fireBallPrefab.transform.rotation);

        //}

        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    _animator.SetBool("_isBossAttacking", false);
        //}


    }
}



