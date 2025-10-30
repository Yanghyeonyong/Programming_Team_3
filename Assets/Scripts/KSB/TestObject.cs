using UnityEngine;
using UnityEngine.SceneManagement; // 폴백용(선택)

public class TestObject : MonoBehaviour
{
    void Update()
    {
        var gm = GameManager.Instance;
        if (Input.GetKeyDown(KeyCode.A))
            gm.Score += 10;                 // OK

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (gm) gm.Load(0);             // ✅ 이렇게
            else SceneManager.LoadScene(0); // (옵션) 폴백
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (gm) gm.Load(1);             // ✅ 이렇게
            else SceneManager.LoadScene(1); // (옵션) 폴백
        }
    }
}
