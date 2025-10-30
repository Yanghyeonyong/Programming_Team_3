using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class TitleUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private GameObject _controlsPanel;

    private CanvasGroup _settingsCanvasGroup;
    private CanvasGroup _controlsCanvasGroup;

    private void Start()
    {
        // CanvasGroup ������Ʈ �������� (������ �ڵ� �߰�)
        _settingsCanvasGroup = _settingsPanel.GetComponent<CanvasGroup>();
        if (_settingsCanvasGroup == null)
            _settingsCanvasGroup = _settingsPanel.AddComponent<CanvasGroup>();

        _controlsCanvasGroup = _controlsPanel.GetComponent<CanvasGroup>();
        if (_controlsCanvasGroup == null)
            _controlsCanvasGroup = _controlsPanel.AddComponent<CanvasGroup>();

        // ó���� ��Ȱ��ȭ ����
        _settingsPanel.SetActive(false);
        _controlsPanel.SetActive(false);
    }

    public void OnStartGameButton()
    {
        SceneManager.LoadScene(1);
    }

    public void OnSettingsButton()
    {
        _controlsPanel.SetActive(false);
        StartCoroutine(FadePanel(_settingsPanel, _settingsCanvasGroup, true));
    }

    public void OnControlsButton()
    {
        _settingsPanel.SetActive(false);
        StartCoroutine(FadePanel(_controlsPanel, _controlsCanvasGroup, true));
    }

    public void OnClosePanel()
    {
        StartCoroutine(FadePanel(_settingsPanel, _settingsCanvasGroup, false));
        StartCoroutine(FadePanel(_controlsPanel, _controlsCanvasGroup, false));
    }

    // ���̵� �ڷ�ƾ
    private IEnumerator FadePanel(GameObject panel, CanvasGroup canvasGroup, bool isFadeIn)
    {
        float duration = 0.3f; // ���̵� �ӵ�
        float start = isFadeIn ? 0f : 1f;
        float end = isFadeIn ? 1f : 0f;

        if (isFadeIn)
            panel.SetActive(true);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = end;

        if (!isFadeIn)
            panel.SetActive(false);
    }
}