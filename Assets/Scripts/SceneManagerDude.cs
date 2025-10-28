using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneManagerDude : Singleton<SceneManagerDude>
{
    [Header("선택: 페이드(없으면 비워두기)")]
    [SerializeField] private CanvasGroup fadeCanvas; // 풀스크린 검정 이미지가 달린 CanvasGroup
    [SerializeField] private float fadeDuration = 0.35f;
    [SerializeField] private bool blockInputDuringFade = true;

   
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName)) { Debug.LogWarning("[SceneManagerDude] 씬 이름이 비어있음"); return; }
        if (fadeCanvas == null) { SceneManager.LoadScene(sceneName); return; }
        StartCoroutine(FadeLoadByName(sceneName));
    }

   
    public void LoadSceneByIndex(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        { Debug.LogWarning($"[SceneManagerDude] 잘못된 인덱스: {sceneIndex}"); return; }

        if (fadeCanvas == null) { SceneManager.LoadScene(sceneIndex); return; }
        StartCoroutine(FadeLoadByIndex(sceneIndex));
    }

   
    public void ReloadCurrentScene()
    {
        var cur = SceneManager.GetActiveScene();
        LoadScene(cur.name);
    }

    /// 다음 씬으로 이동 (마지막이면 0번으로 루프할지 선택)
    public void LoadNextScene(bool loopToFirst = true)
    {
        int cur = SceneManager.GetActiveScene().buildIndex;
        int next = cur + 1;
        int total = SceneManager.sceneCountInBuildSettings;
        if (next >= total)
        {
            if (loopToFirst) next = 0;
            else { Debug.Log("[SceneManagerDude] 마지막 씬입니다."); return; }
        }
        LoadSceneByIndex(next);
    }

    /// 게임 종료
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
        yield return null; // 한 프레임 대기(초기화 여유)
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
            t += Time.unscaledDeltaTime; // 전환은 시간정지 무시
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

    // 선택: 버튼 연결 테스트용
    public void Test() => Debug.Log("[SceneManagerDude] Test 호출됨");
}
