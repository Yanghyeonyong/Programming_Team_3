using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestButton : MonoBehaviour
{
    Button _button;

    Player _player;
    //GameStateManager _gameStateManager;
    // Start is called before the first frame update
    void Start()
    {

        _button = GetComponent<Button>();
        StartCoroutine(TimeDelay());
        while (_player == null)
        {
        _player = FindObjectOfType<Player>(); //.gameObject.GetComponent<Player>();
            if (_player != null)
            {
        _button.onClick.AddListener(GameStateManager.Instance.PauseUnpauseMusic);
                break;
            }
        }


        _button.onClick.AddListener(PlayerDamageTest);
    }
    IEnumerator TimeDelay()
    {
        yield return null;
        yield return null;
        yield return null;
    }

    void PlayerDamageTest()
    {
               _player.TakeDamage(10f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
