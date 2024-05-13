using UnityEngine;

/* FileName: WallCrawlerMovement.cs
 * Description:     This script Handles Wall Crawlers Movement 
 * Author(s):       Matthew Perry
 * Last Modified:   04-05-2024 
 */

public class WallCrawlerMovement : MonoBehaviour
{
    #region Private Variables:

    private UnitCollisionDetection _wallCrawlerCollision;

    [Header("Movement Variables:")]
    [SerializeField] private bool _startMovingForward;
    [SerializeField] private float _setMovementSpeed = 5f;
    [SerializeField] private float _setMovementSpeedClamp = 5f;

    [Header("Attack Variables:")]
    [SerializeField] private float _attackDamge = 1f;
    [SerializeField] private float _dashSpeed = 10f;
    [SerializeField] private float _dashSpeedClamp = 10f;

    [Header("Gravity Variables:")]
    [SerializeField] private float _setGravity = 3f;


    [Header("Read Only:")]
    [SerializeField] private bool _isRoofed;
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isLeftWalled;
    [SerializeField] private bool _isRightWalled;
    [SerializeField] private float _currentGravity;
    [SerializeField] private float _currentHorizontalSpeed;
    [SerializeField] private float _currentVerticalSpeed;
    [SerializeField] private float _currentMovementSpeed;
    [SerializeField] private float _currentMovementSpeedClamp;
    [SerializeField] private Rigidbody2D _rigidbody;

    #endregion

    #region Unity Behaviors:

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _wallCrawlerCollision = GetComponent<UnitCollisionDetection>();
        _currentMovementSpeed = _setMovementSpeed;
        _currentMovementSpeedClamp = _setMovementSpeedClamp;
    }

    private void Start()
    {
        _wallCrawlerCollision.OnIsTouchingRoofUpdate += UpdateIsRoofed;
        _wallCrawlerCollision.OnIsGroundedUpdate += UpdateIsGrounded;
        _wallCrawlerCollision.OnIsTouchingLeftWallUpdate += UpdateIsLeftWalled;
        _wallCrawlerCollision.OnIsTouchingRightWallUpdate += UpdateIsRightWalled;

        _currentGravity = _setGravity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            _startMovingForward = !_startMovingForward;
        }

        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<UnitHealth>().damageUnit(_attackDamge);
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.gravityScale = _currentGravity;

        if (_isGrounded)
        {
            _currentGravity = _setGravity;
            _currentVerticalSpeed -= _setMovementSpeed / 2;
            _currentVerticalSpeed = Mathf.Clamp(_currentVerticalSpeed, -_setMovementSpeedClamp, _setMovementSpeedClamp);

            if (_startMovingForward)
            {
                _currentHorizontalSpeed += _setMovementSpeed;
                _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_setMovementSpeedClamp, _setMovementSpeedClamp);
            }
            else
            {
                _currentHorizontalSpeed -= _setMovementSpeed;
                _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_setMovementSpeedClamp, _setMovementSpeedClamp);
            }

            _rigidbody.velocity = new Vector2(_currentHorizontalSpeed, _rigidbody.velocity.y);
        }

        if (_isRoofed)
        {
            _currentGravity = 0f;
            _currentVerticalSpeed += _setMovementSpeed / 2;
            _currentVerticalSpeed = Mathf.Clamp(_currentVerticalSpeed, -_setMovementSpeedClamp, _setMovementSpeedClamp);

            if (_startMovingForward)
            {
                _currentHorizontalSpeed -= _setMovementSpeed;
                _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_setMovementSpeedClamp, _setMovementSpeedClamp);
                if (_isLeftWalled)
                {
                    _currentGravity = _setGravity;
                }
            }
            else
            {
                _currentHorizontalSpeed += _setMovementSpeed;
                _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_setMovementSpeedClamp, _setMovementSpeedClamp);

                if (_isRightWalled)
                {
                    _currentGravity = _setGravity;
                }
            }

            _rigidbody.velocity = new Vector2(_currentHorizontalSpeed, _currentVerticalSpeed);
        }


        if (_isLeftWalled)
        {
            _currentGravity = 0f;
            _currentHorizontalSpeed -= _setMovementSpeed / 2f;
            _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_setMovementSpeedClamp, _setMovementSpeedClamp);

            if (!_isRoofed)
            {
                _currentGravity = 0f;
            }

            if (_startMovingForward)
            {
                _currentVerticalSpeed -= _setMovementSpeed;
                _currentVerticalSpeed = Mathf.Clamp(_currentVerticalSpeed, -_setMovementSpeedClamp, _setMovementSpeedClamp);
            }
            else
            {
                _currentVerticalSpeed += _setMovementSpeed;
                _currentVerticalSpeed = Mathf.Clamp(_currentVerticalSpeed, -_setMovementSpeedClamp, _setMovementSpeedClamp);
            }

            _rigidbody.velocity = new Vector2(_currentHorizontalSpeed, _currentVerticalSpeed);
        }

        if (_isRightWalled)
        {
            _currentHorizontalSpeed += _setMovementSpeed / 2f;
            _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_setMovementSpeedClamp, _setMovementSpeedClamp);

            if (!_isRoofed)
            {
                _currentGravity = 0f;
            }

            if (_startMovingForward)
            {
                _currentVerticalSpeed += _setMovementSpeed;
                _currentVerticalSpeed = Mathf.Clamp(_currentVerticalSpeed, -_setMovementSpeedClamp, _setMovementSpeedClamp);
            }
            else
            {
                _currentVerticalSpeed -= _setMovementSpeed;
                _currentVerticalSpeed = Mathf.Clamp(_currentVerticalSpeed, -_setMovementSpeedClamp, _setMovementSpeedClamp);
            }

            _rigidbody.velocity = new Vector2(_currentHorizontalSpeed, _currentVerticalSpeed);
        }

        if (!_isGrounded && !_isRoofed)
        {
            if (!_isLeftWalled && !_isRightWalled || _isLeftWalled && _isRightWalled)
            {
                _currentGravity = _setGravity;
            }
        }
    }

    #endregion

    #region Private Functions:
    private void UpdateIsRoofed(object sender, UnitCollisionDetection.OnIsTouchingRoofUpdateEventArgs e)
    {
        _isRoofed = e.IsTouchingRoof;
    }

    private void UpdateIsGrounded(object sender, UnitCollisionDetection.OnIsGroundedUpdateEventArgs e)
    {
        _isGrounded = e.IsGrounded;
    }

    private void UpdateIsLeftWalled(object sender, UnitCollisionDetection.OnIsTouchingLeftWallUpdateEventArgs e)
    {
        _isLeftWalled = e.IsTouchingLeftWall;
    }

    private void UpdateIsRightWalled(object sender, UnitCollisionDetection.OnIsTouchingRightWallUpdateEventArgs e)
    {
        _isRightWalled = e.IsTouchingRightWall;
    }

    #endregion

    #region Public Functions:

    public void MoveForward()
    {
        _startMovingForward = true;
    }

    public void MoveBackward()
    {
        _startMovingForward = false;
    }

    public void Dash()
    {
        _setMovementSpeed = _dashSpeed;
        _setMovementSpeedClamp = _dashSpeedClamp;
    }

    public void Walk()
    {
        _setMovementSpeed = _currentMovementSpeed;
        _setMovementSpeedClamp = _currentMovementSpeedClamp;
    }

    #endregion
}
