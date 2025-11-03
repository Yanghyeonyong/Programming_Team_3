using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeathUI : MonoBehaviour
{

    [Header("Restart Button Scripts")]
    [SerializeField] private List<string> _restartButtonScripts;
    [SerializeField] private float _timeBetweenScripts = 2f;
    //[SerializeField] private float _restartButtonTargetXPos = -400f;


    Button _restartButton;
    RectTransform _restartButtonRectTransform;
    Button _menuButton;
    GameObject _menu;

    TextMeshProUGUI _restartButtonText;

    // Start is called before the first frame update
    void Awake()
    {
        _restartButtonScripts = new List<string>()
        {
            "Did it hurt?",
            "You can do it!",
            "I believe in you",
        };


        foreach (Transform child in transform)
        {

            if (child.name == "Restart")
            {
                _restartButton = child.GetComponent<Button>();
                _restartButtonRectTransform = _restartButton.GetComponent<RectTransform>();
                _restartButton.onClick.AddListener(RestartGameFromUI);
                _restartButton.onClick.AddListener(DeactivateUI);
                //_restartButton.onClick.AddListener(DisableMenuButton);
                _restartButtonText = _restartButton.GetComponentInChildren<TextMeshProUGUI>();
            }

            else if (child.name == "Menu")
            {
                _menuButton = child.GetComponent<Button>();
                _menuButton.onClick.AddListener(SceneChanger.Instance.LoadMenu);
                _menu = child.gameObject;
            }
        }
    }

    //void DisableMenuButton()
    //{
    //           _menu.SetActive(false);
    //}

    void OnEnable()
    {
        _restartButtonText.text = " ";
        StartCoroutine(_restartButtonDialogue());
        //StartCoroutine(_restartButtonMove());

    }

    //private void OnDisable()
    //{
    //    _restartButtonRectTransform.localPosition = new Vector3(0, 0, 0);
    //}


    IEnumerator _restartButtonDialogue()
    {
        foreach (string script in _restartButtonScripts)
        {
            _restartButtonText.text = script;
            //_restartButtonRectTransform.localPosition = new Vector3(_restartButtonTargetXPos,  0, 0);
            //_restartButton.rectTransfrom.localPosition = new Vector3.Lerp(_restartButton.rectTransform.localPosition, new Vector3(_restartButtonTargetXPos, _restartButton.rectTransform.localPosition.y, _restartButton.rectTransform.localPosition.z), 0.5f);
            yield return new WaitForSeconds(_timeBetweenScripts);
        }
        _restartButtonText.text = "Restart";
        _menu.SetActive(true);
    }
    //IEnumerator _restartButtonMove()
    //{
    //    while (_restartButtonRectTransform.localPosition.x > _restartButtonTargetXPos)
    //    {
    //        _restartButtonRectTransform.localPosition = new Vector3(_restartButtonRectTransform.localPosition.x - 0.5f, 0, 0);
    //        yield return null;
    //    }
    //}


    private void OnDestroy()
    {
        _restartButton.onClick.RemoveListener(RestartGameFromUI);
        _restartButton.onClick.RemoveListener(DeactivateUI);
        //_restartButton.onClick.RemoveListener(DisableMenuButton);
        _menuButton.onClick.RemoveListener(SceneChanger.Instance.LoadMenu);

    }

    private void DeactivateUI()
    {
               gameObject.SetActive(false);
    }

    private void RestartGameFromUI()
    {
        GameStateManager.Instance.RequestGameRestart();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
