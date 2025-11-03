using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(InputComponent))] //InputComponent가 없으면 자동으로 추가해줌
public class MoveComponent : MonoBehaviour
{

    [SerializeField] private GameObject _movingPlane;
    //[SerializeField] private InputComponent _inputCompontent;
    [SerializeField] private float _moveSpeed = 20;
    [SerializeField] private float _rotationSpeed = 700f;


    //flat mode
    [SerializeField] private float _rotationLimitMaxRight = 30f;
    [SerializeField] private float _rotationLimitMinRight = -30f;
    [SerializeField] private float _rotationLimitMaxLeft = 210f;
    [SerializeField] private float _rotationLimitMinLeft = 150f;


    //line mode limits
    [SerializeField] private float _rotationLimitMaxUp = -60f;
    [SerializeField] private float _rotationLimitMinUp = -120f;
    [SerializeField] private float _rotationLimitMaxDown = 120f;
    [SerializeField] private float _rotationLimitMinDown = 60f;

    //flip parameters
    [SerializeField] private float _flipJumpForce = 10f;
    [SerializeField] private float _flipDuration = 0.5f;
    [SerializeField] private float _flipRecoveryDelay = 0.2f;
    [SerializeField] private float _flipCooldown = 1f;


    //251028 물체 충돌시 힘이 가해지는 것을 초기화
    private Rigidbody _rigidbody;


    private float _currentFlipCooldown = 0f;

    //[SerializeField] private float _xBoundValue = 2f;
    //[SerializeField] private float _zBoundValue = 2f;

    //[SerializeField] private float _dashDistance = 2f;
    [SerializeField] private float _dashCooldown = 2f;
    //[SerializeField] private float _dashSpeed = 0.5f;
    [SerializeField] private float _dashDuration = 0.2f;
    //[SerializeField] private float _dashInterpolation = 0.1f;
    //[SerializeField] private float _dashBarrelRollSpeed = 5f;
    //[SerializeField] private float _dashColliderShrinkFactor = 2f;
    //[SerializeField] private float _dashAfterDashShrinkRevertDelay = 0.1f;




    private InputComponent _inputComponent;
    //private Vector3 _minWorldBounds;
    //private Vector3 _maxWorldBounds;
    //private Vector3 _playerExtents;
    //private Vector3 _dashStartPosition;
    //private Vector3 _dashEndPosition;
    //private float _currentRotationYZ = 0f;
    //private float _currentDashTime = 0f;
    //private float _currentDashCooldown = 0f;
    //private int _dashDirection = 0; //-1 left, 1 right

    public float DashCooldown { get { return _dashCooldown; } set { _dashCooldown = value; } }
    public float DashDuration { get { return _dashDuration; } set { _dashDuration = value; } }


    Quaternion nextRotation;


    private Animator _animator;

    private float _lastInput = 1f;
    private float _lastVerInput = 1f;

    private bool _flatMode = true;
    private bool _isChangingMode = false;

    //죽음시 이동을 못하게
    private bool _isPlayerAlive =true;

    public void SetIsPlayerAlive(bool isAlive)
    {
        _isPlayerAlive = isAlive;
    }


    //_isChangingMode = true;


    private void OnCollisionStay(Collision collision)
    {
        if (_rigidbody != null)
        
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

        }
    }



    void ChangeFlatMode()
    {
        _flatMode = !_flatMode;

        StartCoroutine(FlippingAnimation());


        //if (_flatMode == true)
        //{


        //    transform.rotation = Quaternion.Euler(0, 0, 0);

        //}

        //if (_flatMode == false)
        //{
        //    transform.rotation = Quaternion.Euler(0, 90f, 0);


        //    //transform.rotation =
        //}
    }

    IEnumerator FlippingAnimation()
    {
        float _elapsedFlipTime = 0f;
        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        transform.GetComponent<Rigidbody>().AddForce(Vector3.up * _flipJumpForce);

        while (true)
        {
            transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
            _elapsedFlipTime += Time.deltaTime;

            if (_elapsedFlipTime - _flipDuration <= 0.1f && _elapsedFlipTime - _flipDuration >= -0.1f)
            {
                _elapsedFlipTime = 0f;
                break;
            }

            yield return null;

        }





        while (true)
        {
            if (_flatMode == true)
            {
                _elapsedFlipTime += Time.deltaTime;
                float currentYRotation = transform.rotation.eulerAngles.y;
                float clampedYRotation = Mathf.LerpAngle(currentYRotation, 0, Time.deltaTime * _rotationSpeed);
                transform.rotation = Quaternion.Euler(0, clampedYRotation, 0);
                if (_elapsedFlipTime - _flipRecoveryDelay >= -0.1f && _elapsedFlipTime - _flipRecoveryDelay <= 0.1f)
                {
                    _isChangingMode = false;
                    transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
                    break;
                }
                yield return null;

            }
            else if (_flatMode == false)
            {
                _elapsedFlipTime += Time.deltaTime;
                float currentYRotation = transform.rotation.eulerAngles.y;
                float clampedYRotation = Mathf.LerpAngle(currentYRotation, -90f, Time.deltaTime * _rotationSpeed);
                transform.rotation = Quaternion.Euler(0, clampedYRotation, 0);

                if (_elapsedFlipTime - _flipRecoveryDelay >= -0.1f && _elapsedFlipTime - _flipRecoveryDelay <= 0.1f)
                {
                    _isChangingMode = false;
                    transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
                    break;
                }
                yield return null;

            }
        }

        //_isChangingMode = false;

    }




    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _animator = transform.Find("Idle_0").GetComponent<Animator>();
        _inputComponent = GetComponent<InputComponent>();
        //StartCoroutine(CheckAndDodge());


    }
    private void OnEnable()
    {
        //_rigidbody = GetComponent<Rigidbody>();
        //_animator = transform.Find("Idle_0").GetComponent<Animator>();
        //_inputComponent = GetComponent<InputComponent>();
        StartCoroutine(CheckAndDodge());
    }



    private float NormalizeAngle(float angle)
    {
        while (angle > 180f)
        {
            angle -= 360f;
        }
        while (angle < -180f)
        {
            angle += 360f;
        }
        return angle;
    }

    private void MoveLine()
    {

        Vector3 inputVec = new Vector3(_inputComponent.HorInput, 0, _inputComponent.VerInput).normalized;
        Vector3 deltaMovement = inputVec * _moveSpeed * Time.deltaTime; //벡터를 맨뒤에 옮겨놓으면 성능이 조금 좋아짐
        //데이터 가격이 싼 순서대로 곱셈을 하는게 좋음


        Vector3 nextPosition = transform.position + deltaMovement;

        if (inputVec != Vector3.zero)
        {
            _animator.SetBool("_isRunning", true);
            _animator.SetBool("_isIdle", false);
            nextRotation = Quaternion.LookRotation(inputVec);
        }
        //Vector3 dashNextPosition;
        //nextRotation = Quaternion.LookRotation(inputVec);
        float currentYRotation = transform.rotation.eulerAngles.y;

        if (_inputComponent.VerInput > 0 && _lastVerInput > 0)
        {
            _lastVerInput = _inputComponent.VerInput;
            float clampedYRotation = nextRotation.eulerAngles.y - 90f;
            //currentYRotation = NormalizeAngle(currentYRotation);
            clampedYRotation = NormalizeAngle(clampedYRotation);
            //Debug.Log("Up Rotation: " + clampedYRotation);
            clampedYRotation = Mathf.Clamp(clampedYRotation, _rotationLimitMinUp, _rotationLimitMaxUp);
            transform.rotation = Quaternion.Euler(0, clampedYRotation, 0);
        }

        if (_inputComponent.VerInput < 0 && _lastVerInput < 0)
        {
            _lastVerInput = _inputComponent.VerInput;
            float clampedYRotation = nextRotation.eulerAngles.y - 90f;
            //currentYRotation = NormalizeAngle(currentYRotation);
            clampedYRotation = NormalizeAngle(clampedYRotation);
            clampedYRotation = Mathf.Clamp(clampedYRotation, _rotationLimitMinDown, _rotationLimitMaxDown);
            transform.rotation = Quaternion.Euler(0, clampedYRotation, 0);
        }



        if (_inputComponent.VerInput < 0 && _lastVerInput > 0)
        {
            _lastVerInput = _inputComponent.VerInput;
            transform.rotation = Quaternion.Euler(0, 90f, 0);
        }
        if (_inputComponent.VerInput > 0 && _lastVerInput < 0)
        {
            _lastVerInput = _inputComponent.VerInput;
            transform.rotation = Quaternion.Euler(0, -90f, 0);
        }



        if (_inputComponent.VerInput == 0 && _lastVerInput > 0)


        {

            if (_inputComponent.HorInput == 0)
            {
                _animator.SetBool("_isRunning", false);
                _animator.SetBool("_isIdle", true);
            }
            transform.rotation = Quaternion.Euler(0, -90f, 0);
        }

        if (_inputComponent.VerInput == 0 && _lastVerInput < 0)
        {
            if (_inputComponent.HorInput == 0)
            {
                _animator.SetBool("_isRunning", false);
                _animator.SetBool("_isIdle", true);
            }
            transform.rotation = Quaternion.Euler(0, 90f, 0);
        }





        transform.position = nextPosition;
    }


    private void MoveFlat()
    {

        Vector3 inputVec = new Vector3(_inputComponent.HorInput, 0, _inputComponent.VerInput).normalized;
        Vector3 deltaMovement = inputVec * _moveSpeed * Time.deltaTime; //벡터를 맨뒤에 옮겨놓으면 성능이 조금 좋아짐
        //데이터 가격이 싼 순서대로 곱셈을 하는게 좋음
        Vector3 nextPosition = transform.position + deltaMovement;
        //Vector3 dashNextPosition;
        float currentYRotation = transform.rotation.eulerAngles.y;

        if (inputVec != Vector3.zero)
        {
            _animator.SetBool("_isRunning", true);
            _animator.SetBool("_isIdle", false);    
            nextRotation = Quaternion.LookRotation(inputVec);
        }


        if (_inputComponent.HorInput > 0 && _lastInput > 0)
        {
            _lastInput = _inputComponent.HorInput;
            float clampedYRotation = nextRotation.eulerAngles.y - 90f;
            //currentYRotation = NormalizeAngle(currentYRotation);
            clampedYRotation = NormalizeAngle(clampedYRotation);
            //Debug.Log("Right Rotation: " + clampedYRotation);
            clampedYRotation = Mathf.Clamp(clampedYRotation, _rotationLimitMinRight, _rotationLimitMaxRight);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, clampedYRotation, 0), _rotationSpeed * Time.deltaTime);
        }


        if (_inputComponent.HorInput < 0 && _lastInput < 0)
        {
            _lastInput = _inputComponent.HorInput;
            float clampedYRotation = nextRotation.eulerAngles.y - 90f;
            //currentYRotation = NormalizeAngle(currentYRotation);
            //clampedYRotation = NormalizeAngle(clampedYRotation);
            //Debug.Log("Left Rotation: " + clampedYRotation);
            clampedYRotation = Mathf.Clamp(clampedYRotation, _rotationLimitMinLeft, _rotationLimitMaxLeft);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, clampedYRotation, 0), _rotationSpeed * Time.deltaTime);
        }

        if (_inputComponent.HorInput < 0 && _lastInput > 0)
        {
            _lastInput = _inputComponent.HorInput;
            transform.rotation = Quaternion.Euler(0, -180f, 0);
        }
        if (_inputComponent.HorInput > 0 && _lastInput < 0)
        {
            _lastInput = _inputComponent.HorInput;
            transform.rotation = Quaternion.Euler(0, 0f, 0);
        }



        if (_inputComponent.HorInput == 0 && _lastInput > 0)


        {

            if (_inputComponent.VerInput == 0)
            {
                _animator.SetBool("_isRunning", false);
                _animator.SetBool("_isIdle", true);
            }
           
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        if (_inputComponent.HorInput == 0 && _lastInput < 0)
        {

            if (_inputComponent.VerInput == 0)
            {
                _animator.SetBool("_isRunning", false);
                _animator.SetBool("_isIdle", true);
            }
            transform.rotation = Quaternion.Euler(0, -180f, 0);
        }






        transform.position = nextPosition;
    }

    //}
    private float _timeBetweenInput = 0f;
    private bool _inputTimerActive = false;
    private float _dodgeElapsedTime = 0f;
    [SerializeField] private float _dodgeCooldown = 1f;
    [SerializeField] private float _maxDodgeInputDelay = 0.3f;
    [SerializeField] private float _dodgeDistance = 1f;
    [SerializeField] private float __dodgeSpeed = 10f;
    [SerializeField] private float __dodgeDuration = 0.2f;
    private bool _isDodging = false;
    private bool _isAttacking = false;
    public bool IsAttacking { get { return _isAttacking; } set { _isAttacking = value; } }
    private bool _isTakingDamage = false;
    public bool IsTakingDamage { get { return _isTakingDamage; } set { _isTakingDamage = value; } }
    public bool IsDodging { get { return _isDodging; } set { _isDodging = value; } }
    public void StopCoroutinesForReset()
    {
        StopAllCoroutines();
        Debug.Log("DashStopped");
    }

    public void StartCheckAndDodge()
    {
        StartCoroutine(CheckAndDodge());
    }

    IEnumerator CheckAndDodge()
    {

        while (SceneManager.GetSceneByBuildIndex(1).isLoaded) //&& GameStateManager.Instance._currentPlayer.GetComponent<Player>().PlayerHealth >0)
        {
            yield return null;
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {


                _inputTimerActive = true;
                _timeBetweenInput = 0f;
                while (_inputTimerActive) //프레임마다 반복?
                {
                    _timeBetweenInput += Time.deltaTime;

                    if (_timeBetweenInput >= _maxDodgeInputDelay)
                    {
                        _inputTimerActive = false;
                        break;
                        //yield break; //코루틴종료

                    }
                    yield return null;
                    if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                    {
                        //_animator.SetBool("_isDodging",_isDodging);
                        _isDodging = true;
                        _dodgeElapsedTime = 0f;
                        while (true)
                        {

                            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.forward * _dodgeDistance, __dodgeSpeed * Time.deltaTime);
                            yield return null;
                            _dodgeElapsedTime += Time.deltaTime;
                            if (_dodgeElapsedTime >= __dodgeDuration)
                            {
                                yield return null;
                                break; 
                            }

                        }
                        _isDodging = false;


                    }
                }

            }


            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {


                _inputTimerActive = true;
                _timeBetweenInput = 0f;
                while (_inputTimerActive) //프레임마다 반복?
                {
                    _timeBetweenInput += Time.deltaTime;

                    if (_timeBetweenInput >= _maxDodgeInputDelay)
                    {
                        _inputTimerActive = false;
                        break;
                        //yield break; //코루틴종료

                    }
                    yield return null;
                    if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                    {
                        _isDodging = true;
                        _dodgeElapsedTime = 0f;
                        while (true)
                        {

                            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.left * _dodgeDistance, __dodgeSpeed * Time.deltaTime);
                            yield return null;
                            _dodgeElapsedTime += Time.deltaTime;
                            if (_dodgeElapsedTime >= __dodgeDuration)
                            {
                                yield return null;
                                break;
                            }

                        }
                        _isDodging = false;


                    }
                }

            }

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {


                _inputTimerActive = true;
                _timeBetweenInput = 0f;
                while (_inputTimerActive) //프레임마다 반복?
                {
                    _timeBetweenInput += Time.deltaTime;

                    if (_timeBetweenInput >= _maxDodgeInputDelay)
                    {
                        _inputTimerActive = false;
                        break;
                        //yield break; //코루틴종료

                    }
                    yield return null;
                    if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                    {
                        _isDodging = true;
                        _dodgeElapsedTime = 0f;
                        while (true)
                        {

                            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.right * _dodgeDistance, __dodgeSpeed * Time.deltaTime);
                            yield return null;
                            _dodgeElapsedTime += Time.deltaTime;
                            if (_dodgeElapsedTime >= __dodgeDuration)
                            {
                                yield return null;
                                break;
                            }

                        }
                        _isDodging = false;


                    }
                }

            }



            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {


                _inputTimerActive = true;
                _timeBetweenInput = 0f;
                while (_inputTimerActive) //프레임마다 반복?
                {
                    _timeBetweenInput += Time.deltaTime;

                    if (_timeBetweenInput >= _maxDodgeInputDelay)
                    {
                        _inputTimerActive = false;
                        break;
                        //yield break; //코루틴종료

                    }
                    yield return null;
                    if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                    {
                        _isDodging = true;
                        _dodgeElapsedTime = 0f;
                        while (true)
                        {
                            _isDodging = true;
                            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.back * _dodgeDistance, __dodgeSpeed * Time.deltaTime);
                            yield return null;
                            _dodgeElapsedTime += Time.deltaTime;
                            if (_dodgeElapsedTime >= __dodgeDuration)
                            {
                                yield return null;  
                                break;

                            }

                        }
                        _isDodging = false;


                    }
                }

            }






        }


    }



    

    // Update is called once per frame
    void Update()
    {

       // _isPlayerAlive = GameStateManager.Instance.IsPlayerAlive;

        if (Input.GetKeyDown(KeyCode.LeftShift) && _currentFlipCooldown <= 0 && _isDodging == false && _isAttacking == false && _isTakingDamage == false && _isPlayerAlive )
        {
            _isChangingMode = true;
            _currentFlipCooldown = _flipCooldown;
            ChangeFlatMode();
            transform.position =new Vector3 (transform.position.x, 0f, transform.position.z);
        }
        _currentFlipCooldown -= Time.deltaTime;


        if (_flatMode == true && _isChangingMode == false && _isDodging == false && _isAttacking == false && _isTakingDamage == false && _isPlayerAlive)
        {
            MoveFlat();
        }
        else if (_flatMode == false && _isChangingMode == false && _isDodging == false && _isAttacking == false && _isTakingDamage == false && _isPlayerAlive)
        {

            MoveLine();
        }



        //Dash();
    }

}

