using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//외부 호출용 프로퍼티, 해다 탕타입의 싱글톤이 없으면 찾아보고 그래도 없으면 새로 생성 후 설정

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>(); //씬에서 해당 타입의 싱글톤을 찾음
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<T>();
                    singletonObject.name = typeof(T).ToString() + " (Singleton)";
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }


    //중복 체크 및 연결 기능 구현
    protected virtual void Awake() //상속받는 클래스에서 오버라이드 가능, 프로젝티드로 자식만 접근 가능
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (_instance != this) // for returning to a previous scene where the singleton already exists

            {
                Destroy(gameObject);
            }

        }
    }


    // Singleton<T> 클래스 내부에 추가

    //private static bool applicationIsQuitting = false;

    // 애플리케이션이 종료될 때 플래그를 true로 설정합니다.
    //protected virtual void OnApplicationQuit()
    //{
    //    applicationIsQuitting = true;
    //}

    //protected virtual void OnDestroy()
    //{
        // 오브젝트가 파괴될 때, 애플리케이션 종료 과정이 아니라면
        // _instance를 비워주거나 필요한 처리를 합니다.
        // DontDestroyOnLoad 때문에 씬 종료 시 OnDestroy가 호출되지 않을 수 있지만,
        // 이 플래그가 있으면 유니티가 '정리되지 않은 오브젝트'로 판단하는 것을 피하는 데 도움이 됩니다.
    //}

}

