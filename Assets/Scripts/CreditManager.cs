
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _creditText;
    [SerializeField] private float _scrollSpeed = 90f;
    [SerializeField] private float _duration = 35f;
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private float _bgmFadeTime = 2f;


    private RectTransform _textTransform;

    private void Start()
    {
        _textTransform = _creditText.GetComponent<RectTransform>();
        _textTransform.anchoredPosition = Vector2.zero;

        if (_bgmSource != null)
        {
            _bgmSource.volume = 0f;
            _bgmSource.Play();
            StartCoroutine(FadeInBGM());
        }



        StartCoroutine(AutoReturnToTitle());

    }

    private void Update()
    {
        _textTransform.anchoredPosition += Vector2.up * _scrollSpeed * Time.deltaTime;
    }



    private IEnumerator FadeInBGM()
    {
        float t = 0;
        while (t < _bgmFadeTime)
        {
            _bgmSource.volume = Mathf.Lerp(0f, 0.5f, t / _bgmFadeTime);
            t += Time.deltaTime;
            yield return null;
        }
        _bgmSource.volume = 0.6f;
    }



    private IEnumerator AutoReturnToTitle()
    {
        yield return new WaitForSeconds(_duration);
        SceneManager.LoadScene(0);
    }

    public void OnSkipButtion()
    {
        SceneManager.LoadScene(0);
    }


}

