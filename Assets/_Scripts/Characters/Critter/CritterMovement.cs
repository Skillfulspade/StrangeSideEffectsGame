using System;
using UnityEngine;

/* FileName: CritterAI.cs
 * Description:     This script Handles the Critters Movement
 * Author(s):       Matthew Perry
 * Last Modified:   04-14-2024 
 */

public class CritterMovement : MonoBehaviour
{
    #region Public Events:

    public event EventHandler<OnDirectionUpdateUpdateEventArgs> OnDirectionUpdate;
    public class OnDirectionUpdateUpdateEventArgs : EventArgs { public float Direction; }

    #endregion

    #region Private Variables:

    private UnitCollisionDetection _critterCollision;
    private float _direction;

    [Header ("Movement Variables:")]
    [SerializeField] private bool _startMovingForward;
    [SerializeField] private float _setMovementSpeed;
    [SerializeField] private float _setMovementSpeedClamp;

    [Header("Jump Variables:")]
    [SerializeField] private float _jumpForce = 10f;
    public bool _canJump;

    [Header("Attack Variables:")]
    [SerializeField] private float _attackDamge = 1f;
    //[SerializeField] private float _dashSpeed = 10f;
    //[SerializeField] private float _dashSpeedClamp = 10f;

    [Header("Gravity Variables:")]
    [SerializeField] private float _setGravity = 3f;

    [Header ("Read Only:")]
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isLeftWalled;
    [SerializeField] private bool _isRightWalled;
    [SerializeField] private float _currentMovementSpeed = 5f;
    [SerializeField] private float _currentMovementSpeedClamp = 5f;
    [SerializeField] private float _currentGravity;
    [SerializeField] private float _currentHorizontalSpeed;
    [SerializeField] private float _currentVerticalSpeed;
    [SerializeField] private Rigidbody2D _rigidbody;

    #endregion

    #region Unity Behaviors:

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _critterCollision = GetComponent<UnitCollisionDetection>();
        _currentMovementSpeed = _setMovementSpeed;
        _currentMovementSpeedClamp = _setMovementSpeedClamp;
        _currentGravity = _setGravity;
    }

    private void Start()
    {
        _critterCollision.OnIsGroundedUpdate += UpdateIsGrounded;
        _critterCollision.OnIsTouchingLeftWallUpdate += UpdateIsLeftWalled;
        _critterCollision.OnIsTouchingRightWallUpdate += UpdateIsRightWalled;
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

            if(_startMovingForward)
            {
                _currentHorizontalSpeed -= _setMovementSpeed;
                _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_setMovementSpeedClamp, _setMovementSpeedClamp);
            }
            else
            {
                _currentHorizontalSpeed += _setMovementSpeed;
                _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_setMovementSpeedClamp, _setMovementSpeedClamp);

            }
        }

        if (_isLeftWalled || _isRightWalled)
        {
            _canJump = true;
        }

        if (!_isGrounded)
        {
            _canJump = false;
        }

        if (_canJump)
        {
            MakeCritterJump();
        }

        if((_rigidbody.velocity.x <= 0f && _rigidbody.velocity.x > -1f) && _currentHorizontalSpeed > 0f)
        {
            _canJump = true;
            _startMovingForward = true;
        }

        else if ((_rigidbody.velocity.x >= 0f && _rigidbody.velocity.x < 1f) && _currentHorizontalSpeed < 0f)
        {
            _canJump = true;
            _startMovingForward = false;
        }

        _rigidbody.velocity = new Vector2(_currentHorizontalSpeed, _rigidbody.velocity.y);

        if (_currentHorizontalSpeed != _direction)
        {
            _direction = _currentHorizontalSpeed;
            OnDirectionUpdate?.Invoke(this, new OnDirectionUpdateUpdateEventArgs { Direction = _direction });
        }
    }

    #endregion

    #region Private Functions:

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

    public void MakeCritterJump()
    {
        _rigidbody.velocity = new Vector2(_currentHorizontalSpeed, _jumpForce);
    }

    #endregion
}
