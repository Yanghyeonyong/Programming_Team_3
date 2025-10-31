using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollowingCamera : MonoBehaviour
{



    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FollowPlayer());
    }

    IEnumerator FollowPlayer()
    {
        while (true)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Vector3 newPosition = player.transform.position + new Vector3(0, 0.377f, -0.33f );
                
                //newPosition.z = transform.position.z;
                transform.position = newPosition;
                transform.LookAt(player.transform.position + new Vector3(0,0.377f,0));
            }
            yield return null; // Wait for the next frame
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
