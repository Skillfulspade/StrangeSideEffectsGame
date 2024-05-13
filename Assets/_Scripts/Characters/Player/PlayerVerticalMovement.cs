using UnityEngine;
using UnityEngine.InputSystem;

/* FileName: PlayerVerticalMovement.cs
 * Description:     This script Handles the players movement Vertically, 
 *                  allowing the player to move Up or down.
 * Author(s):       Matthew Perry
 * Last Modified:   04-03-2024 
 */

public class PlayerVerticalMovement : MonoBehaviour
{
    #region Variables:

    private PlayerControls _playerControls;
    private UnitCollisionDetection _playerCollisionDetection;
    private PlayerHorizontalMovement _playerHorizontalMovement;
    private InputAction _jump;
    private bool _isDashing;
    private float _horizontalInput;

    [Header("Jump Variables")]
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isOnEnemy;
    [SerializeField] private bool _triedToJump;
    [SerializeField] private bool _endedJumpEarly;
    [SerializeField] private float _currentVerticalSpeed;
    [SerializeField] private float _jumpHeight = 11f;
    [SerializeField] private float _jumpStopForce = 5.5f;
    [SerializeField] private float _setJumpBufferTimeSec = 0.7f;
    [SerializeField] private float _curretJumpBufferTime;
    [SerializeField] private float _jumpHangTimeThreshold = 0.5f;
    [SerializeField] private float _jumpHangGravityMultiplier = 1.5f;
    [SerializeField] private Rigidbody2D _rigidbody;

    [Header("Double Jump Controls")]
    [SerializeField] private bool _canDoubleJump;
    [SerializeField] private bool _hasDoubleJumped;
    [SerializeField] private float _doubleJumpHeight = 11f;
    [SerializeField] private float _hopHeight = 11f / 2f;

    [Header("Coyote Jump Variables")]
    [SerializeField] private bool _coyoteTimeJumped;
    [SerializeField] private float _coyoteJumpMultiplyer = 1.1f;
    [SerializeField] private float _setCoyoteTimeInSec = 0.7f;
    [SerializeField] private float _currentCoyoteTime;

    [Header("Gravity Variables")]
    [SerializeField] private float _setGravity = 1f;
    [SerializeField] private float _currentGravity;
    [SerializeField] private float _fallGravityModifier = 2f;
    [SerializeField] private float _setMaxFallSpeed = 25f;
    [SerializeField] private float _knockUpSpeed = 10f;

    #endregion

    #region Unity Behaviors:

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            float horizontalPos = collision.gameObject.transform.position.x - gameObject.transform.position.x;
            float verticalPos = collision.gameObject.transform.position.y - gameObject.transform.position.y;

            if (verticalPos > 0f)
            {
                _currentVerticalSpeed = -_knockUpSpeed;
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _currentVerticalSpeed);
            }
            else if (verticalPos <= 0f)
            {
                _currentVerticalSpeed = _knockUpSpeed;
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _currentVerticalSpeed);
            }
            
        }
    }

    private void OnEnable()
    {
        _jump = _playerControls.Player.Jump;
        _jump.Enable();
    }

    private void OnDisable()
    {
        _jump.Disable();
    }

    private void Awake()
    {
        _playerControls = new PlayerControls();
        _playerCollisionDetection = gameObject.GetComponent<UnitCollisionDetection>();
        _playerHorizontalMovement = gameObject.GetComponent<PlayerHorizontalMovement>();
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _rigidbody.freezeRotation = true;
    }

    private void Start()
    {
        _playerControls.Enable();
        _playerCollisionDetection.OnIsGroundedUpdate += UpdateIsGrounded;
        _playerCollisionDetection.OnIsTouchingEnemyBelowUpdate += UpdateIsOnEnemy;
        _playerHorizontalMovement.OnIsDashing += UpdateIsDashing;
        _playerHorizontalMovement.OnHorizontalInput += UpdateHorizontalInput;
        _currentGravity = _setGravity;
    }

    private void FixedUpdate()
    {
        _rigidbody.gravityScale = _currentGravity;

        if (_isDashing)
        {
            _currentGravity = 0;
        }
        else
        {
            if (!_isDashing)
            {
                _currentGravity = _setGravity;
            }
            // Reset conditions
            if (_isGrounded || _isOnEnemy)
            {
                _currentCoyoteTime = _setCoyoteTimeInSec;
                _currentGravity = _setGravity;
                _coyoteTimeJumped = false;
                _endedJumpEarly = false;


                if (_canDoubleJump)
                {
                    if (_hasDoubleJumped)
                    {
                        _hasDoubleJumped = false;
                    }

                    if (_horizontalInput != 0f)
                    {
                        _currentVerticalSpeed = _hopHeight;
                        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _currentVerticalSpeed);
                    }
                }
            }
            else
            {
                _currentCoyoteTime -= 0.1f;

                if (_triedToJump)
                {
                    _curretJumpBufferTime -= 0.1f;
                }

                if (_currentCoyoteTime <= 0)
                {
                    _currentCoyoteTime = 0;
                }

                if (_curretJumpBufferTime <= 0)
                {
                    _curretJumpBufferTime = 0;
                }
            }

            // Detecting Jump Input
            _playerControls.Player.Jump.started += context =>
            {
                _curretJumpBufferTime = _setJumpBufferTimeSec;
                if (!_isGrounded && !_isOnEnemy)
                {
                    _triedToJump = true;
                }
            };
            _playerControls.Player.Jump.performed += context =>
            {
                if (_isGrounded || _isOnEnemy || (_canDoubleJump && !_hasDoubleJumped))
                {
                    _currentVerticalSpeed = _jumpHeight;

                    if (!_isGrounded && !_isOnEnemy)
                    {
                        _currentVerticalSpeed = _doubleJumpHeight;
                        _hasDoubleJumped = true;
                        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);
                    }


                    _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _currentVerticalSpeed);
                }
            };
            _playerControls.Player.Jump.canceled += context =>
            {
                if (!_isGrounded && _rigidbody.velocity.y > 0 && !_isOnEnemy)
                {
                    _endedJumpEarly = true;
                }
            };

            // Buffer Jump Logic
            if (_triedToJump && (_isGrounded || _isOnEnemy))
            {
                if (_curretJumpBufferTime > 0f)
                {
                    _currentVerticalSpeed = _jumpHeight;
                    _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _currentVerticalSpeed);
                }
                _triedToJump = false;
            }

            // Coyote Jump Logic
            else if (_triedToJump && !_isGrounded && !_isOnEnemy)
            {
                if (_currentCoyoteTime > 0f && !_coyoteTimeJumped)
                {
                    _currentVerticalSpeed = _jumpHeight;
                    _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _currentVerticalSpeed * _coyoteJumpMultiplyer);
                    _coyoteTimeJumped = true;
                    _triedToJump = false;
                }
            }

            // Varied Jump Logic
            if (_endedJumpEarly && _rigidbody.velocity.y > 0f)
            {
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y - _jumpStopForce);
                _endedJumpEarly = false;
            }

            // Quicker fall
            if (_rigidbody.velocity.y < 0f && !_isOnEnemy)
            {
                _currentGravity = _setGravity * _fallGravityModifier;
            }

            // Max Fall _speed:
            if (_rigidbody.velocity.y < _setMaxFallSpeed)
            {
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, Mathf.Max(_rigidbody.velocity.y, -_setMaxFallSpeed));
            }

            // Bonus Air Time
            if (!_isGrounded && !_isOnEnemy && Mathf.Abs(_rigidbody.velocity.y) < _jumpHangTimeThreshold)
            {
                _currentGravity *= _jumpHangGravityMultiplier;
            }
        }

        
    }

    #endregion

    #region Private Functions:

    public void EnableDoubleJump()
    {
        _canDoubleJump = true;
    }

    public void DisableDoubleJump()
    {
        _canDoubleJump = false;
    }

    private void UpdateIsGrounded(object sender, UnitCollisionDetection.OnIsGroundedUpdateEventArgs e)
    {
        _isGrounded = e.IsGrounded;
    }

    private void UpdateIsOnEnemy(object sender, UnitCollisionDetection.OnIsTouchingEnemyBelowUpdateEventArgs e)
    {
        _isOnEnemy = e.IsTouchingEnemyBelow;
    }

    private void UpdateIsDashing(object sender, PlayerHorizontalMovement.OnIsDashingUpdateEventArgs e)
    {
        _isDashing = e.IsDashing;
    }

    private void UpdateHorizontalInput(object sender, PlayerHorizontalMovement.OnHorizontalInputUpdateEventArgs e)
    {
        _horizontalInput = e.HorizontalInput;
    }

    #endregion
}
