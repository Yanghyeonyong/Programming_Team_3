
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Spell : MonoBehaviour
{
    Animator _animator;

    public bool _isAtEndOfAnimation = false;
    public Vector3 _tempPlayerPosition;
    private void Awake()
    {
        _isAtEndOfAnimation = false;
        _animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        _isAtEndOfAnimation = false;
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        StartCoroutine(UsingSpell());
    }

    IEnumerator UsingSpell()
    {
        _animator.Play("_spell");
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length * 0.5f);
        _isAtEndOfAnimation = true;
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length * 0.5f);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_tempPlayerPosition != null)
        {
            transform.position = _tempPlayerPosition;
        }
        
    }
}


