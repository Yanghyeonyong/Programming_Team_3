using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputComponent : MonoBehaviour
{
    private float _horInput;
    private float _verInput;
    private bool _leftClickInput;
    private bool _rightClickInput;

    public float HorInput => _horInput; //return _horInput;와 동일
    public float VerInput => _verInput;
    public bool LeftClickInput => _leftClickInput;
    public bool RightClickInput => _rightClickInput;


    // Update is called once per frame
    void Update()
    {
        _horInput = Input.GetAxisRaw("Horizontal"); //수평 //GetAxis 는 손떼면 0으로 천천히 돌아가기때문에 GetAxisRaw사용해 바로 인풋이 0이됨

        _verInput = Input.GetAxisRaw("Vertical"); //수직

        _leftClickInput = Input.GetKeyDown(KeyCode.Mouse0);
        //Input.GetAxisRaw("Fire1");
        _rightClickInput =  Input.GetKeyDown(KeyCode.Mouse1);
        //Input.GetAxisRaw("Fire2");

        //Debug.Log($"HorInput : {_horInput}, VerInput : {_verInput} LeftClick : {_leftClickInput}, RightClick : {_rightClickInput}");


    }
}
