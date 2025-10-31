using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class TitleUIManager : MonoBehaviour
{

    [SerializeField] List<GameObject> characterPrefabs;
    [SerializeField] Transform characterSpawnPoint;

    [Header("ī�޶� Ÿ�� ������")]
    //[SerializeField] private Transform cameraTargetPrefab;

    [Header("���� ���� ��ư")]
    [SerializeField] private Button startButton;

    [Header("���� / ���۹� �г�")]
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private GameObject _controlsPanel;

    private CanvasGroup _settingsCanvasGroup;
    private CanvasGroup _controlsCanvasGroup;

    [Header("ĳ���� ���� �г�")]
    [SerializeField] private GameObject _characterSelectPanel;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;

    [Header("���� Ȯ�� �˾�")]
    [SerializeField] private GameObject _confirmPopup;
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;

    [Header("��ȯ�� ���Ӿ� �̸�")]
    [SerializeField] private string gamePlaySceneName = "GameScene";

    [Header("ī�޶� �̵� �ε巴��(LerP)")]
    [SerializeField] private float followSpeed = 5f;

    [Header("���� ��ȯ ���� ����")]
    [SerializeField] private Transform camera3DTarget;
    [SerializeField] private float transitionDuration = 2f;

    [SerializeField] private Button exitButton;


    private Transform _mainCamera;
    private bool _isTransitioning = false;
    private List<GameObject> playersInstances = new List<GameObject>();

    [SerializeField] private Button changeCharacter;
    [SerializeField] private GameObject _title;

    private void ChangeCharacter()
    {
        if (SceneChanger.Instance._selectedPlayerIndex == 0)
        {
            playersInstances[SceneChanger.Instance._selectedPlayerIndex].SetActive(false);

            SceneChanger.Instance._selectedPlayerIndex = 1;
            playersInstances[SceneChanger.Instance._selectedPlayerIndex].SetActive(true);
        }
        else if (SceneChanger.Instance._selectedPlayerIndex == 1)
        {
            playersInstances[SceneChanger.Instance._selectedPlayerIndex].SetActive(false);

            SceneChanger.Instance._selectedPlayerIndex = 0;
            playersInstances[SceneChanger.Instance._selectedPlayerIndex].SetActive(true);
            //player = Instantiate(characterPrefabs[SceneChanger.Instance._selectedPlayerIndex], characterSpawnPoint.position, characterPrefabs[SceneChanger.Instance._selectedPlayerIndex].transform.rotation);
            //player.SetActive(false);
            //SceneChanger.Instance._selectedPlayerIndex = 0;
            //player = Instantiate(characterPrefabs[SceneChanger.Instance._selectedPlayerIndex], characterSpawnPoint.position, characterPrefabs[SceneChanger.Instance._selectedPlayerIndex].transform.rotation);
        }
    }

    private void Start()
    {
        //SceneChanger.Instance.StartPreloadingGameScene();
        _mainCamera = Camera.main.transform;

        for (int i = 0; i < characterPrefabs.Count; i++)
        {
            playersInstances.Add(Instantiate(characterPrefabs[i], characterSpawnPoint.position, characterPrefabs[i].transform.rotation));
            playersInstances[i].GetComponent<MoveComponent>().enabled = false;
            playersInstances[i].GetComponent<Player>().enabled = false;
            playersInstances[i].SetActive(false);
        }
        if (SceneChanger.Instance._selectedPlayerIndex < playersInstances.Count)
        {
            playersInstances[SceneChanger.Instance._selectedPlayerIndex].SetActive(true);
        }
        //player =Instantiate(characterPrefabs[SceneChanger.Instance._selectedPlayerIndex], characterSpawnPoint.position, characterPrefabs[SceneChanger.Instance._selectedPlayerIndex].transform.rotation);

        // CanvasGroup ������Ʈ ����
        

        //_settingsCanvasGroup = EnsureCanvasGroup(_settingsPanel);
        //_controlsCanvasGroup = EnsureCanvasGroup(_controlsPanel);

        //_settingsPanel.SetActive(false);
        //_controlsPanel.SetActive(false);

        //ĳ���� ���� (�˾� �ʱ����)
        //_characterSelectPanel.SetActive(false);
        //_confirmPopup.SetActive(false);

        //��ư �̺�Ʈ ���
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartGameButton);
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(Application.Quit);
        }
        if (changeCharacter != null)
        {
            changeCharacter.onClick.AddListener(ChangeCharacter);
        }



        //if (_confirmButton != null)
        //{
        //    _confirmButton.onClick.AddListener(OnCharacterConfirm);
        //}
        //if (_cancelButton != null)
        //{
        //    _cancelButton.onClick.AddListener(OnCharacterCancel);
        //}
        //if (_yesButton != null)
        //{
        //    _yesButton.onClick.AddListener(OnConfirmYes);
        //}
        //if (_noButton != null)
        //{
        //    _noButton.onClick.AddListener(OnConfirmNo);
        //}
    }

    IEnumerator TitleDelay()
    {
        yield return new WaitForSeconds(2f);
    }
    private void OnDestroy()
    {
        //��ư �̺�Ʈ ����
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(OnStartGameButton);
        }
        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(Application.Quit);
        }
        if (changeCharacter != null)
        {
            changeCharacter.onClick.RemoveListener(ChangeCharacter);
        }
    }
    private void Update()
    {
        if (_isTransitioning)
        {
            return;
        }

        //if (cameraTargetPrefab == null)
        //{
        //    return;
        //}

        //_mainCamera.position = Vector3.Lerp(
        //    _mainCamera.position,
        //    cameraTargetPrefab.position,
        //    Time.deltaTime * followSpeed);


        //_mainCamera.rotation = Quaternion.Slerp(
        //    _mainCamera.rotation,
        //    cameraTargetPrefab.rotation,
        //    Time.deltaTime * followSpeed);
    }



    //IEnumerator

    //Start Game ��ư Ŭ����
    public void OnStartGameButton()
    {
        changeCharacter.gameObject.SetActive(false);

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        _title.SetActive(false);
        //SceneChanger.Instance._selectedPlayerIndex = 0; // �⺻�� ���� (��: 0)
        //Instantiate(characterPrefabs[SceneChanger.Instance._selectedPlayerIndex], characterSpawnPoint.position, characterPrefabs[SceneChanger.Instance._selectedPlayerIndex].transform.rotation);

        StartCoroutine(TransitionTo3D());
        //SceneChanger.Instance.LoadGameScene();

        //_characterSelectPanel.SetActive(true);
    }

    //setting ��ư Ŭ����
    public void OnSettingsButton()
    {
        _controlsPanel.SetActive(false);
        StartCoroutine(FadePanel(_settingsPanel, _settingsCanvasGroup, true));
    }


    //Controls ��ư Ŭ����
    public void OnControlsButton()
    {
        _settingsPanel.SetActive(false);
        StartCoroutine(FadePanel(_controlsPanel, _controlsCanvasGroup, true));
    }

    //Close ��ư Ŭ����
    public void OnClosePanel()
    {
        StartCoroutine(FadePanel(_settingsPanel, _settingsCanvasGroup, false));
        StartCoroutine(FadePanel(_controlsPanel, _controlsCanvasGroup, false));
    }


    //ĳ���� ���� ���� �޼���
    private void OnCharacterConfirm()
    {
        _characterSelectPanel.SetActive(false);
        StartCoroutine(TransitionTo3D());
    }

    private void OnCharacterCancel()
    {
        _characterSelectPanel.SetActive(false);
    }


    //���� ��ȯ & �˾�â Ȱ��
    private IEnumerator TransitionTo3D()
    {
        
        _isTransitioning = true;

        
        yield return new WaitForSeconds(3f);

        //yield return StartCoroutine(FadeScreen(1f));

        Vector3 startPos = _mainCamera.position;
        Quaternion startRot = _mainCamera.rotation;

        float elapsed = 0f;
        //while (elapsed < transitionDuration)
        //{
        //    elapsed += Time.deltaTime;
        //    float a = elapsed / transitionDuration;

        //    _mainCamera.position = Vector3.Lerp(startPos, camera3DTarget.position, a);
        //    _mainCamera.rotation = Quaternion.Slerp(startRot, camera3DTarget.rotation, a);
        //    yield return null;
        //}
        float a = 0;
        while ( elapsed <= transitionDuration)
        {
            elapsed += Time.deltaTime;
            a = elapsed / transitionDuration;
            //if (SceneChanger.Instance._preloadOperation.progress >= a)
            //{
                _mainCamera.position = Vector3.Lerp(startPos, camera3DTarget.position, a);
                _mainCamera.rotation = Quaternion.Slerp(startRot, camera3DTarget.rotation, a);
                yield return null;
            //}
            //else if (SceneChanger.Instance._preloadOperation.progress < a)
            //{
            //    _mainCamera.position = Vector3.Lerp(startPos, camera3DTarget.position, SceneChanger.Instance._preloadOperation.progress);
            //    _mainCamera.rotation = Quaternion.Slerp(startRot, camera3DTarget.rotation, SceneChanger.Instance._preloadOperation.progress);
            //    yield return null;
            //}

            //if (SceneChanger.Instance._preloadOperation.progress > 0.95 && a >=transitionDuration)
            //{
            //    yield return null;
            //    break;
            //}
        }
        //SceneChanger.Instance.ActivatePreloadedGameScene();
        SceneChanger.Instance.LoadGameScene();

        //yield return StartCoroutine(FadeScreen(0f));

        //_confirmPopup.SetActive(true);
        _isTransitioning = false;
    }



    private void OnConfirmYes()
    {
        SceneManager.LoadScene(gamePlaySceneName);
    }


    private void OnConfirmNo()
    {
        _confirmPopup.SetActive(false);
        _characterSelectPanel.SetActive(true);
    }

    private CanvasGroup EnsureCanvasGroup(GameObject panel)
    {
        var group = panel.GetComponent<CanvasGroup>();
        if (group == null)
        {
            group = panel.AddComponent<CanvasGroup>();
        }
        return group;
    }



    private IEnumerator FadePanel(GameObject panel, CanvasGroup canvasGroup, bool isFadeIn)
    {
        float duration = 0.3f;
        float start = isFadeIn ? 0f : 1f;
        float end = isFadeIn ? 1f : 0f;

        if (isFadeIn)
        {
            panel.SetActive(true);
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = end;

        if (!isFadeIn)
        {
            panel.SetActive(false);
        }
    }




    private IEnumerator FadeScreen(float targetAlpha)
    {
        GameObject fadeCanvas = GameObject.Find("FadeCanvas");

        if (fadeCanvas == null)
        {
            yield break;
        }

        CanvasGroup fadeGroup = fadeCanvas.GetComponent<CanvasGroup>();

        if (fadeGroup == null)
        {
            yield break;
        }


        float startAlpha = fadeGroup.alpha;
        float duration = 0.8f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);

            yield return null;
        }

        fadeGroup.alpha = targetAlpha;

    }




}
