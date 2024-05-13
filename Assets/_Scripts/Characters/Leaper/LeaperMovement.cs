using UnityEngine;

/* FileName: LeaperMovement.cs
 * Description:     This script is meant to handle the way the leaper moves
 *                  around the world.
 * Author(s):       Matthew Perry
 * Last Modified:   04-14-2024 
 */

public class LeaperMovement : MonoBehaviour
{
    #region Private Variables:

    private UnitCollisionDetection _leaperCollision;
    private UnitHealth _Player;

    [Header("Movement Variables:")]
    [SerializeField] private bool _startMovingForward = true;
    [SerializeField] private float _setMovementSpeed = 1f;
    [SerializeField] private float _setMovementSpeedClamp = 2f;
    [SerializeField] private float _hopHeight = 7f;
    [SerializeField] private float _setHopHeightClamp = 7f;

    [Header("Jump Variables:")]
    [SerializeField] private bool _canJump;
    [SerializeField] private float _jumpForce = 14f;

    [Header("Attack Variables:")]
    [SerializeField] private float _attackDamge = 1f;

    [Header("Gravity Variables:")]
    [SerializeField] private float _setGravity = 3f;

    [Header("Read Only:")]
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isLeftWalled;
    [SerializeField] private bool _isRightWalled;
    [SerializeField] private bool _isTouchingEnemyAbove;
    [SerializeField] private bool _isTouchingEnemyBelow;
    [SerializeField] private bool _isTouchingEnemyLeft;
    [SerializeField] private bool _isTouchingEnemyRight;
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
        _leaperCollision = GetComponent<UnitCollisionDetection>();
        _currentMovementSpeed = _setMovementSpeed;
        _currentMovementSpeedClamp = _setMovementSpeedClamp;
        _currentGravity = _setGravity;
    }

    private void Start()
    {
        _leaperCollision.OnIsGroundedUpdate += UpdateIsGrounded;
        _leaperCollision.OnIsTouchingLeftWallUpdate += UpdateIsLeftWalled;
        _leaperCollision.OnIsTouchingRightWallUpdate += UpdateIsRightWalled;

        _leaperCollision.OnIsTouchingEnemyAboveUpdate += UpdateIsTouchingEnemyAbove;
        _leaperCollision.OnIsTouchingEnemyBelowUpdate += UpdateIsTouchingEnemyBelow;
        _leaperCollision.OnIsTouchingEnemyLeftUpdate += UpdateIsTouchingEnemyLeft;
        _leaperCollision.OnIsTouchingEnemyRightUpdate += UpdateIsTouchingEnemyRight;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            _startMovingForward = !_startMovingForward;
        }

        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Touch Player");
            if (_isTouchingEnemyAbove || _isTouchingEnemyBelow || _isTouchingEnemyLeft || _isTouchingEnemyRight)
            {
                Debug.Log("Touch damage");
                collision.gameObject.GetComponent<UnitHealth>().damageUnit(_attackDamge);
            }
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.gravityScale = _currentGravity;
        if (_isGrounded)
        {
            _currentGravity = _setGravity;

            if (_startMovingForward)
            {
                _currentHorizontalSpeed -= _setMovementSpeed;
                _currentVerticalSpeed += _hopHeight;
                _currentVerticalSpeed = Mathf.Clamp(_currentVerticalSpeed, -_setHopHeightClamp, _setHopHeightClamp);
                _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_setMovementSpeedClamp, _setMovementSpeedClamp);
            }
            else
            {
                _currentHorizontalSpeed += _setMovementSpeed;
                _currentVerticalSpeed += _hopHeight;
                _currentVerticalSpeed = Mathf.Clamp(_currentVerticalSpeed, -_setHopHeightClamp, _setHopHeightClamp);
                _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_setMovementSpeedClamp, _setMovementSpeedClamp);

            }

            if (_canJump)
            {
                _currentVerticalSpeed = _jumpForce;
            }

            if (_isLeftWalled || _isRightWalled)
            {
                _canJump = true;
            }

            if ((_rigidbody.velocity.x == 0f) && _isRightWalled)
            {
                //_canJump = true;
                _startMovingForward = true;
            }

            else if ((_rigidbody.velocity.x == 0f) && _isLeftWalled)
            {
                //_canJump = true;
                _startMovingForward = false;
            }

            _rigidbody.velocity = new Vector2(_currentHorizontalSpeed, _currentVerticalSpeed);
        }
        else
        {
            _canJump = false;
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

    private void UpdateIsTouchingEnemyAbove(object sender, UnitCollisionDetection.OnIsTouchingEnemyAboveUpdateEventArgs e)
    {
        _isTouchingEnemyAbove = e.IsTouchingEnemyAbove;
    }

    private void UpdateIsTouchingEnemyBelow(object sender, UnitCollisionDetection.OnIsTouchingEnemyBelowUpdateEventArgs e)
    {
        _isTouchingEnemyBelow = e.IsTouchingEnemyBelow;
    }

    private void UpdateIsTouchingEnemyLeft(object sender, UnitCollisionDetection.OnIsTouchingEnemyLeftUpdateEventArgs e)
    {
        _isTouchingEnemyLeft = e.IsTouchingEnemyLeft;
    }

    private void UpdateIsTouchingEnemyRight(object sender, UnitCollisionDetection.OnIsTouchingEnemyRightUpdateEventArgs e)
    {
        _isTouchingEnemyRight = e.IsTouchingEnemyRight;
    }

    #endregion

    #region Public Functions:

    public void Leap()
    {
        _canJump = true;
    }

    public void MoveForward()
    {
        _startMovingForward = true;
    }

    public void MoveBackward()
    {
        _startMovingForward = false;
    }


    #endregion
}
