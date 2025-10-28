using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneManagerDude : Singleton<SceneManagerDude>
{
    [Header("����: ���̵�(������ ����α�)")]
    [SerializeField] private CanvasGroup fadeCanvas; // Ǯ��ũ�� ���� �̹����� �޸� CanvasGroup
    [SerializeField] private float fadeDuration = 0.35f;
    [SerializeField] private bool blockInputDuringFade = true;

   
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName)) { Debug.LogWarning("[SceneManagerDude] �� �̸��� �������"); return; }
        if (fadeCanvas == null) { SceneManager.LoadScene(sceneName); return; }
        StartCoroutine(FadeLoadByName(sceneName));
    }

   
    public void LoadSceneByIndex(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        { Debug.LogWarning($"[SceneManagerDude] �߸��� �ε���: {sceneIndex}"); return; }

        if (fadeCanvas == null) { SceneManager.LoadScene(sceneIndex); return; }
        StartCoroutine(FadeLoadByIndex(sceneIndex));
    }

   
    public void ReloadCurrentScene()
    {
        var cur = SceneManager.GetActiveScene();
        LoadScene(cur.name);
    }

    /// ���� ������ �̵� (�������̸� 0������ �������� ����)
    public void LoadNextScene(bool loopToFirst = true)
    {
        int cur = SceneManager.GetActiveScene().buildIndex;
        int next = cur + 1;
        int total = SceneManager.sceneCountInBuildSettings;
        if (next >= total)
        {
            if (loopToFirst) next = 0;
            else { Debug.Log("[SceneManagerDude] ������ ���Դϴ�."); return; }
        }
        LoadSceneByIndex(next);
    }

    /// ���� ����
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    
    private System.Collections.IEnumerator FadeLoadByName(string sceneName)
    {
        yield return FadeTo(1f);
        SceneManager.LoadScene(sceneName);
        yield return null; // �� ������ ���(�ʱ�ȭ ����)
        yield return FadeTo(0f);
    }

    private System.Collections.IEnumerator FadeLoadByIndex(int sceneIndex)
    {
        yield return FadeTo(1f);
        SceneManager.LoadScene(sceneIndex);
        yield return null;
        yield return FadeTo(0f);
    }

    private System.Collections.IEnumerator FadeTo(float targetAlpha)
    {
        if (fadeCanvas == null) yield break;

        float start = fadeCanvas.alpha;
        float t = 0f;

        if (blockInputDuringFade)
        {
            fadeCanvas.blocksRaycasts = true;
            fadeCanvas.interactable = true;
        }

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime; // ��ȯ�� �ð����� ����
            float a = Mathf.Lerp(start, targetAlpha, t / fadeDuration);
            fadeCanvas.alpha = a;
            yield return null;
        }
        fadeCanvas.alpha = targetAlpha;

        if (blockInputDuringFade)
        {
            bool block = targetAlpha > 0.99f;
            fadeCanvas.blocksRaycasts = block;
            fadeCanvas.interactable = block;
        }
    }

    // ����: ��ư ���� �׽�Ʈ��
    public void Test() => Debug.Log("[SceneManagerDude] Test ȣ���");
}
