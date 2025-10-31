using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSecondSpell : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 1f;
    [SerializeField] float _duration = 5f;
    public bool _onShot = false;
    [SerializeField] private float _attack;
    [SerializeField] private Transform _originTransform;
    [SerializeField] Transform _boss;

    private void OnEnable()
    {
        _onShot = false;

        gameObject.transform.localPosition = _originTransform.localPosition;
        gameObject.transform.localScale = _originTransform.localScale;
        //_originPos=transform.localPosition;
        //Debug.Log(_originPos);
        //_originScale=transform.localScale;
        //Debug.Log(_originScale);
    }
    void Update()
    {
        if (_onShot)
        {
            transform.position += transform.right * _moveSpeed * Time.deltaTime;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            other.GetComponent<Player>().TakeDamage(_attack);
        }
        if (other.gameObject.CompareTag("Wall"))
        {
            gameObject.SetActive(false);
        }
    }

    public void CancelSpell()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {

        transform.SetParent(_boss);
    }
    public IEnumerator Shooting()
    {
        yield return new WaitForSeconds(_duration);
        //if (gameObject.activeSelf)
        //{
        //    gameObject.SetActive(false);
        //}
        CancelSpell();
    }
}
