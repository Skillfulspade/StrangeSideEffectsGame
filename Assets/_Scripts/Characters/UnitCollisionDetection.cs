using System;
using UnityEngine;

/* FileName: UnitCollisionDetection.cs
 * Description:     This script Handles the a units collision, 
 *                  preventing the unit from getting stuck,
 *                  jumping when they are not supposed to, etc.
 * Author(s):       Matthew Perry
 * Last Modified:   04-14-2024 
 */

public class UnitCollisionDetection : MonoBehaviour
{
    #region Public Events:

    public event EventHandler<OnIsGroundedUpdateEventArgs> OnIsGroundedUpdate;
    public class OnIsGroundedUpdateEventArgs : EventArgs { public bool IsGrounded; }

    public event EventHandler<OnIsTouchingRightWallUpdateEventArgs> OnIsTouchingRightWallUpdate;
    public class OnIsTouchingRightWallUpdateEventArgs : EventArgs { public bool IsTouchingRightWall; }

    public event EventHandler<OnIsTouchingLeftWallUpdateEventArgs> OnIsTouchingLeftWallUpdate;
    public class OnIsTouchingLeftWallUpdateEventArgs : EventArgs { public bool IsTouchingLeftWall; }

    public event EventHandler<OnIsTouchingRoofUpdateEventArgs> OnIsTouchingRoofUpdate;
    public class OnIsTouchingRoofUpdateEventArgs : EventArgs { public bool IsTouchingRoof; }

    public event EventHandler<OnIsTouchingEnemyAboveUpdateEventArgs> OnIsTouchingEnemyAboveUpdate;
    public class OnIsTouchingEnemyAboveUpdateEventArgs : EventArgs { public bool IsTouchingEnemyAbove; }

    public event EventHandler<OnIsTouchingEnemyBelowUpdateEventArgs> OnIsTouchingEnemyBelowUpdate;
    public class OnIsTouchingEnemyBelowUpdateEventArgs : EventArgs { public bool IsTouchingEnemyBelow; }

    public event EventHandler<OnIsTouchingEnemyLeftUpdateEventArgs> OnIsTouchingEnemyLeftUpdate;
    public class OnIsTouchingEnemyLeftUpdateEventArgs : EventArgs { public bool IsTouchingEnemyLeft; }

    public event EventHandler<OnIsTouchingEnemyRightUpdateEventArgs> OnIsTouchingEnemyRightUpdate;
    public class OnIsTouchingEnemyRightUpdateEventArgs : EventArgs { public bool IsTouchingEnemyRight; }

    #endregion

    #region Private Variables:

    [Header("Collision Variables")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isTouchingEnemyBelow;

    [SerializeField] private bool _isTouchingRightWall;
    [SerializeField] private bool _isTouchingEnemyRight;

    [SerializeField] private bool _isTouchingLeftWall;
    [SerializeField] private bool _isTouchingEnemyLeft;

    [SerializeField] private bool _isTouchingRoof;
    [SerializeField] private bool _isTouchingEnemyAbove;

    [SerializeField] private bool _isColidingFromAbove;
    [SerializeField] private bool _isColidingFromBelow;
    [SerializeField] private bool _isColidingFromLeft;
    [SerializeField] private bool _isColidingFromRight;
    [SerializeField] private bool _isColidingOnEnemyAbove;
    [SerializeField] private bool _isColidingOnEnemyBelow;
    [SerializeField] private bool _isColidingOnEnemyLeft;
    [SerializeField] private bool _isColidingOnEnemyRight;
    [SerializeField] private BoxCollider2D _aboveColisionTrigger;
    [SerializeField] private BoxCollider2D _belowColisionTrigger;
    [SerializeField] private BoxCollider2D _leftColisionTrigger;
    [SerializeField] private BoxCollider2D _rightColisionTrigger;
    [SerializeField] private float _boxCastDistance = 0.1f;
    [SerializeField] private float _boxCastAngle;

    #endregion

    #region Unity Behaviors:

    private void FixedUpdate()
    {
        
        _isColidingFromAbove = Physics2D.BoxCast(_aboveColisionTrigger.bounds.center, _aboveColisionTrigger.bounds.size, _boxCastAngle, Vector2.up, _boxCastDistance, _groundLayer);
        _isColidingFromBelow = Physics2D.BoxCast(_belowColisionTrigger.bounds.center, _belowColisionTrigger.bounds.size, _boxCastAngle, Vector2.down, _boxCastDistance, _groundLayer);
        _isColidingFromLeft = Physics2D.BoxCast(_leftColisionTrigger.bounds.center, _leftColisionTrigger.bounds.size, _boxCastAngle, Vector2.left, _boxCastDistance, _groundLayer);
        _isColidingFromRight = Physics2D.BoxCast(_rightColisionTrigger.bounds.center, _rightColisionTrigger.bounds.size, _boxCastAngle, Vector2.right, _boxCastDistance, _groundLayer);

        _isColidingOnEnemyAbove= Physics2D.BoxCast(_aboveColisionTrigger.bounds.center, _aboveColisionTrigger.bounds.size, _boxCastAngle, Vector2.up, _boxCastDistance, _enemyLayer);
        _isColidingOnEnemyBelow = Physics2D.BoxCast(_belowColisionTrigger.bounds.center, _belowColisionTrigger.bounds.size, _boxCastAngle, Vector2.down, _boxCastDistance, _enemyLayer);
        _isColidingOnEnemyLeft = Physics2D.BoxCast(_leftColisionTrigger.bounds.center, _leftColisionTrigger.bounds.size, _boxCastAngle, Vector2.left, _boxCastDistance, _enemyLayer);
        _isColidingOnEnemyRight = Physics2D.BoxCast(_rightColisionTrigger.bounds.center, _rightColisionTrigger.bounds.size, _boxCastAngle, Vector2.right, _boxCastDistance, _enemyLayer);

        if (_isTouchingRoof != _isColidingFromAbove)
        {
            _isTouchingRoof = _isColidingFromAbove;
            OnIsTouchingRoofUpdate?.Invoke(this, new OnIsTouchingRoofUpdateEventArgs { IsTouchingRoof = _isTouchingRoof});
        }
        else if (_isTouchingEnemyAbove != _isColidingOnEnemyAbove)
        {
            _isTouchingEnemyAbove = _isColidingOnEnemyAbove;
            OnIsTouchingEnemyAboveUpdate?.Invoke(this, new OnIsTouchingEnemyAboveUpdateEventArgs { IsTouchingEnemyAbove = _isTouchingEnemyAbove });
        }

        if (_isGrounded != _isColidingFromBelow)
        {
            _isGrounded = _isColidingFromBelow;
            OnIsGroundedUpdate?.Invoke(this, new OnIsGroundedUpdateEventArgs { IsGrounded = _isGrounded } );
            
        }
        else if (_isTouchingEnemyBelow != _isColidingOnEnemyBelow)
        {
            _isTouchingEnemyBelow = _isColidingOnEnemyBelow;
            OnIsTouchingEnemyBelowUpdate?.Invoke(this, new OnIsTouchingEnemyBelowUpdateEventArgs { IsTouchingEnemyBelow = _isTouchingEnemyBelow});
        }

        if (_isTouchingRightWall != _isColidingFromRight)
        {
            _isTouchingRightWall = _isColidingFromRight;
            OnIsTouchingRightWallUpdate?.Invoke(this, new OnIsTouchingRightWallUpdateEventArgs { IsTouchingRightWall = _isTouchingRightWall });
        }
        else if (_isTouchingEnemyRight != _isColidingOnEnemyRight)
        {
            _isTouchingEnemyRight = _isColidingOnEnemyRight;
            OnIsTouchingEnemyRightUpdate?.Invoke(this, new OnIsTouchingEnemyRightUpdateEventArgs { IsTouchingEnemyRight = _isTouchingEnemyRight });
        }

        if (_isTouchingLeftWall != _isColidingFromLeft)
        {
            _isTouchingLeftWall = _isColidingFromLeft;
            OnIsTouchingLeftWallUpdate?.Invoke(this, new OnIsTouchingLeftWallUpdateEventArgs { IsTouchingLeftWall = _isTouchingLeftWall });
        }
        else if (_isTouchingEnemyLeft != _isColidingOnEnemyLeft)
        {
            _isTouchingEnemyLeft = _isColidingOnEnemyLeft;
            OnIsTouchingEnemyLeftUpdate?.Invoke(this, new OnIsTouchingEnemyLeftUpdateEventArgs { IsTouchingEnemyLeft = _isTouchingEnemyLeft });
        }
    }

    #endregion
}
