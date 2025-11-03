using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapPattern : MonoBehaviour
{
    [SerializeField] private GameObject _boss;
    [SerializeField] private GameObject _patternObject;
    [SerializeField] private GameObject _warningObject;
    [SerializeField] float _warningMapPatternDuration;
    [SerializeField] float _mapPatternDuration;
    [SerializeField] float _mapPatternDelay;

    [SerializeField] float _patternDistance = 0.7f;
    [SerializeField] List<GameObject> _rightPattern;
    [SerializeField] List<GameObject> _leftPattern;
    [SerializeField] List<GameObject> _rightWarningPattern;
    [SerializeField] List<GameObject> _leftWarningPattern;
    

    private void Start()
    {
        StartCoroutine("PlayerSpellAttack");
    }
    IEnumerator PlayerSpellAttack()
    {
        //보스 객체가 살아있거나 존재할 경우에만 발동
        yield return _boss != null;
        yield return _boss.activeSelf == true;

        //SetPatternPos();

        while (_boss.activeSelf == true)
        {
            SetPatternPos();
            InitPos(_warningObject);
            InitPos(_patternObject);

            _warningObject.transform.SetParent(null);
            _patternObject.transform.SetParent(null);

            _warningObject.SetActive(true);
            yield return new WaitForSeconds(_warningMapPatternDuration);

            _warningObject.SetActive(false);
            _patternObject.SetActive(true);
            yield return new WaitForSeconds(_mapPatternDuration);

            _patternObject.SetActive(false);
            Debug.Log("보스 스킬");

            yield return new WaitForSeconds(_mapPatternDelay);
        }
    }

    private void InitPos(GameObject obj)
    {
        obj.transform.SetParent(_boss.transform);
        obj.transform.localPosition = Vector3.zero;
    }

    private void SetPatternPos()
    { 
        for (int i = 0; i < _rightPattern.Count; i++)
        {
            _rightPattern[i].transform.localPosition= new Vector3(_patternDistance/2 + (i) * _patternDistance, _rightPattern[i].transform.localPosition.y, _rightPattern[i].transform.localPosition.z);
            _rightWarningPattern[i].transform.localPosition = _rightPattern[i].transform.localPosition;
        }
        for (int i = 0; i < _leftPattern.Count; i++)
        {
            _leftPattern[i].transform.localPosition= new Vector3(_patternDistance / 2 + (i + 1) * (-_patternDistance), _leftPattern[i].transform.localPosition.y, _leftPattern[i].transform.localPosition.z);
            _leftWarningPattern[i].transform.localPosition = _leftPattern[i].transform.localPosition;
        }
    }
}
