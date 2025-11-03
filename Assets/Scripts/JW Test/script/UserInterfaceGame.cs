using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UserInterfaceGame : MonoBehaviour
{
    //[SerializeField] List<Sprite> _playerIcons;

    private GameObject _player;
    private Image _playerHealthBar;
    //private Image _playerIcon;

    //private bool _isSearchingForPlayer = true;
    //void Awake()
    //{

        
    //}

    private void OnEnable()
    {
        StartCoroutine(SearchForPlayer());
        //StartCoroutine(SearchForPlayer());
        foreach (Transform child in transform)
        {
            if (child.name == "GoNext")
            {
                GameStateManager.Instance._goNext = child.gameObject;
                child.gameObject.SetActive(false);
            }
        }

        //_playerIcon.sprite = _playerIcons[GameStateManager.Instance.CurrentPlayerType];
    }

   

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(SearchForPlayer());
        //_playerIcon.sprite = _playerIcons[GameStateManager.Instance.CurrentPlayerType];
    }

    public void NotifyHealthBar()
    {
        if (_player != null)
        {
            Player playerComponent = _player.GetComponent<Player>();
            if (playerComponent != null)
            {
                 
                _playerHealthBar.fillAmount = playerComponent.PlayerHealth / playerComponent.MaxHealth;
            }
        }

    }

    public bool _isReady = false;

    IEnumerator SearchForPlayer()
    {
        float _quickTimer = 5f;

        while (_player == null && _quickTimer > 0f)
        {
            //Debug.Log("Searching for Player...");
            if (FindObjectOfType<Player>() != null)
            {
                _player = FindObjectOfType<Player>().gameObject;
            }
            _quickTimer -= Time.deltaTime;
            yield return null;
        }
        if (_player == null)
        {
            Debug.LogWarning("Player not found within the time limit.");
        }
        else
        {
            _player.GetComponent<Player>()._userInterfaceGame = this;
            _isReady = true;
            Debug.Log("Player found!");
            NotifyHealthBar();
        }

    }

    private void Awake()
    {
        GameStateManager.Instance._userInterface = gameObject;
        foreach (Transform child in transform)
        {
            if (child.name == "PlayerHealth")
            {
                foreach (Transform grandChild in child)
                {
                    //if (grandChild.name == "PlayerIcon")
                    //{
                    //    _playerIcon = grandChild.GetComponent<Image>();
                    //}
                    if (grandChild.name == "HealthBar")
                    {
                        _playerHealthBar = grandChild.GetComponent<Image>();
                    }
                }

                //_playerIcon = GetComponentIn
            }

        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (_isSearchingForPlayer && _player != null)
        //{
        //    _isSearchingForPlayer = false;
        //    //NotifyHealthBar();
        //}
    }
}
