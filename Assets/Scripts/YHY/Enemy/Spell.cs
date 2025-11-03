using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Spell : MonoBehaviour
{
    Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        StartCoroutine(UsingSpell());
    }

    IEnumerator UsingSpell()
    {
        _animator.Play("_spell");
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        gameObject.SetActive(false);
    }
}
