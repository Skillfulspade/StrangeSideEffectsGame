using UnityEngine;

/* FileName: CritterAI.cs
 * Description:     This script Handles the Critters AI
 * Author(s):       Matthew Perry
 * Last Modified:   04-09-2024 
 */

public class CritterAI : MonoBehaviour
{
    #region Private Variables:

    private UnitCollisionDetection _critterCollision;


    #endregion

    #region Unity Behaviors:

    private void Awake()
    {
        _critterCollision = GetComponent<UnitCollisionDetection>();
        //_currentMovementSpeed = _setMovementSpeed;
        //_currentMovementSpeedClamp = _setMovementSpeedClamp;
    }

    private void Start()
    {
        //_leaperCollision.OnIsGroundedUpdate += UpdateIsGrounded;
        //_leaperCollision.OnIsTouchingLeftWallUpdate += UpdateIsLeftWalled;
       // _leaperCollision.OnIsTouchingRightWallUpdate += UpdateIsRightWalled;
       // _currentGravity = _setGravity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            //_startMovingForward = !_startMovingForward;
        }

        if (collision.gameObject.tag == "Player")
        {
           // collision.gameObject.GetComponent<UnitHealth>().damageUnit(_attackDamge);
        }
    }

    #endregion
}
