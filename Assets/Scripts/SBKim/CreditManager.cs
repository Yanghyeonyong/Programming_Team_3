using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _creditText;
    [SerializeField] private float _scrollSpeed = 40f;
    [SerializeField] private float _duration = 25f;

    private RectTransform _textTransform;

    
    private void Start()
    {
        _textTransform = _creditText.GetComponent<RectTransform>();
        _textTransform.anchoredPosition = Vector2.zero;
        StartCoroutine(AutoReturnToTitle());
    }

    
    private void Update()
    {
        _textTransform.anchoredPosition += Vector2.up * _scrollSpeed * Time.deltaTime;
    }

   
    //Scene(0) = TitleScene
    private IEnumerator AutoReturnToTitle()
    { 
        yield return new WaitForSeconds(_duration);
        SceneManager.LoadScene(0);
    }

    public void OnSkipButton()
    {
        SceneManager.LoadScene(0);
    }
}
