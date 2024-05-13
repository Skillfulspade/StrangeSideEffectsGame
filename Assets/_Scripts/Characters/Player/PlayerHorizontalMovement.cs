using System;
using UnityEngine;
using UnityEngine.InputSystem;

/* FileName: PlayerHorizontalMovement.cs
 * Description:     This script Handles the players movement horizontally, 
 *                  allowing the player to move left or right.
 * Author(s):       Matthew Perry
 * Last Modified:   04-03-2024 
 */

public class PlayerHorizontalMovement : MonoBehaviour
{
    #region Public Events:

    public event EventHandler<OnIsDashingUpdateEventArgs> OnIsDashing;
    public class OnIsDashingUpdateEventArgs : EventArgs { public bool IsDashing; }

    public event EventHandler<OnHorizontalInputUpdateEventArgs> OnHorizontalInput;
    public class OnHorizontalInputUpdateEventArgs : EventArgs { public float HorizontalInput; }

    #endregion

    #region Public Variables:

    public static PlayerHorizontalMovement Instance;

    #endregion

    #region Private Variables:

    private PlayerControls _playerControls;
    private PlayerVisual _playerVisual;
    private InputAction _movment;
    private InputAction _run;
    private InputAction _dash;
    private UnitCollisionDetection _playerCollisionDetection;

    [Header("Movement Variables")]
    [SerializeField] private bool _isTouchingRightWall;
    [SerializeField] private bool _isTouchingLeftWall;
    [SerializeField] private float _setMovmentSpeed = 5f;
    [SerializeField] private float _setMovementSpeedClamp = 5f;
    [SerializeField] private float _setRunSpeed = 10f;
    [SerializeField] private float _runSpeedClamp = 10f;
    [SerializeField] private float _knockBackSpeed = 10f;
    [SerializeField] private float _stopForce = 10f;

    [Header("Dashing Variables")]
    [SerializeField] private bool _canDash;
    [SerializeField] private bool _isDashing;
    [SerializeField] private float _setDashingSpeed = 10f;
    [SerializeField] private float _setDashingDuration = 1f;

    [Header("ReadOnly Variables")]
    [SerializeField] private Material _colorFiler;
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isOnEnemy;
    [SerializeField] private float _currentHorizontalInput;
    [SerializeField] private float _horizontalInput;
    [SerializeField] private float _currentHorizontalSpeed;
    [SerializeField] private float _currentMovmentSpeed;
    [SerializeField] private float _currentMovementSpeedClamp;
    [SerializeField] private float _currentDashingDuration;
    [SerializeField] private Rigidbody2D _rigidbody;

    


    #endregion

    #region Unity Behaviors:

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            float horizontalPos = collision.gameObject.transform.position.x - gameObject.transform.position.x;
            float verticalPos = collision.gameObject.transform.position.y - gameObject.transform.position.y;

            if (horizontalPos >= 0f && verticalPos <= 0f && verticalPos > -0.9f)
            {
                _currentHorizontalSpeed = -_knockBackSpeed;
                _rigidbody.velocity = new Vector2(_currentHorizontalSpeed, _rigidbody.velocity.y);
            }
            else if (horizontalPos <= 0f && verticalPos <= 0f && verticalPos > -0.9f)
            {
                _currentHorizontalSpeed = _knockBackSpeed;
                _rigidbody.velocity = new Vector2(_currentHorizontalSpeed, _rigidbody.velocity.y);
            }
        }
    }

    private void OnEnable()
    {
        _movment = _playerControls.Player.Movement;
        _run = _playerControls.Player.Run;
        _dash = _playerControls.Player.Dash;

        _movment.Enable();
        _run.Enable();
        _dash.Enable();
    }

    private void OnDisable()
    {
        _movment.Disable();
        _run.Disable();
        _dash.Disable();
    }

    private void Awake()
    {
        Instance = this;
        _playerControls = new PlayerControls();
        _playerCollisionDetection = gameObject.GetComponent<UnitCollisionDetection>();
        _playerVisual = gameObject.GetComponentInChildren<PlayerVisual>();
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _colorFiler = _playerVisual.gameObject.GetComponent<SpriteRenderer>().material;
    }

    private void Start()
    {
        _playerControls.Enable();
        _playerCollisionDetection.OnIsTouchingRightWallUpdate += UpdateIsTouchingRightWall;
        _playerCollisionDetection.OnIsTouchingLeftWallUpdate += UpdateIsTouchingLeftWall;
        _playerCollisionDetection.OnIsGroundedUpdate += UpdateIsGrounded;
        _playerCollisionDetection.OnIsTouchingEnemyBelowUpdate += UpdateIsOnEnemy;
        _currentMovmentSpeed = _setMovmentSpeed;
        _currentMovementSpeedClamp = _setMovementSpeedClamp;
    }

    private void FixedUpdate()
    {
        //Reading Input:
        _currentHorizontalInput = _movment.ReadValue<Vector2>().x;

        if (_currentHorizontalInput != _horizontalInput)
        {
            _horizontalInput = _currentHorizontalInput;
            OnHorizontalInput?.Invoke(this, new OnHorizontalInputUpdateEventArgs { HorizontalInput = _horizontalInput });
        }

        if (_rigidbody.velocity.x > 0f)
        {
            _playerVisual.transform.localScale = new Vector3( 1f, _playerVisual.transform.localScale.y, transform.localScale.z);
        }
        else if (_rigidbody.velocity.x < 0f)
        {
            _playerVisual.transform.localScale = new Vector3( -1f, _playerVisual.transform.localScale.y, transform.localScale.z);
        }

        _playerControls.Player.Dash.performed += context =>
        {
            if(_canDash)
            {
                _isDashing = true;

                OnIsDashing?.Invoke(this, new OnIsDashingUpdateEventArgs { IsDashing = _isDashing});

                _currentHorizontalSpeed = _setDashingSpeed * _playerVisual.transform.localScale.x;
            }
            
        };

        if (_isDashing)
        {
            _colorFiler.color = Color.gray;
            _currentDashingDuration -= 0.1f;

            if (_currentDashingDuration <= 0f)
            {
                _isDashing = false;
                _colorFiler.color = Color.white;
                OnIsDashing?.Invoke(this, new OnIsDashingUpdateEventArgs { IsDashing = _isDashing });
            }

            _rigidbody.velocity = new Vector2(_currentHorizontalSpeed, 0f);

        }
        else
        {
            if (_isGrounded || _isOnEnemy)
            {
                _currentDashingDuration = _setDashingDuration;
            }

            _playerControls.Player.Run.performed += context =>
            {
                _currentMovmentSpeed = _runSpeedClamp;
                _currentMovementSpeedClamp = _runSpeedClamp;
            };

            _playerControls.Player.Run.canceled += context =>
            {
                _currentMovmentSpeed = _setMovmentSpeed;
                _currentMovementSpeedClamp = _setMovementSpeedClamp;
            };

            // Moving Player:
            if (_currentHorizontalInput != 0)
            {
                _currentHorizontalSpeed += _currentHorizontalInput * _currentMovmentSpeed;
                _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_currentMovementSpeedClamp, _currentMovementSpeedClamp);
            }

            // Slowing player:
            else if (_currentHorizontalInput == 0)
            {
                _currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _stopForce);
            }

            _rigidbody.velocity = new Vector2(_currentHorizontalSpeed, _rigidbody.velocity.y);
        }

        
    }

    #endregion

    #region Private Functions:

    private void UpdateIsTouchingRightWall(object sender, UnitCollisionDetection.OnIsTouchingRightWallUpdateEventArgs e)
    {
        _isTouchingRightWall = e.IsTouchingRightWall;
    }

    private void UpdateIsTouchingLeftWall(object sender, UnitCollisionDetection.OnIsTouchingLeftWallUpdateEventArgs e)
    {
        _isTouchingLeftWall = e.IsTouchingLeftWall;
    }

    #endregion

    #region Public Functions:

    public float GetHorizontalInput()
    {
        return _currentHorizontalInput;
    }

    public void EnableDash()
    {
        _canDash = true;
    }

    public void DisableDash()
    {
        _canDash = false;
    }

    private void UpdateIsGrounded(object sender, UnitCollisionDetection.OnIsGroundedUpdateEventArgs e)
    {
        _isGrounded = e.IsGrounded;
    }

    private void UpdateIsOnEnemy(object sender, UnitCollisionDetection.OnIsTouchingEnemyBelowUpdateEventArgs e)
    {
        _isOnEnemy = e.IsTouchingEnemyBelow;
    }

    #endregion
}
