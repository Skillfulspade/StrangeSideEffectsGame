using UnityEngine;

/* FileName: LeaperAI.cs
 * Description:     This script controls the decisions of the leaper and how they choose
 *                  to see, interact, and chase the player
 * Author(s):       Matthew Perry
 * Last Modified:   04-14-2024 
 */

public class LeaperAI : MonoBehaviour
{
    #region Private Variables:

    private UnitCollisionDetection _leaperCollsion;
    private LeaperMovement _leaperMovement;

    [SerializeField] private bool _leftLineOfSight;
    [SerializeField] private bool _rightLineOfSight;
    [SerializeField] private bool _inRangeForAttack;
    [SerializeField] private float _circleCastDistance = 0.1f;
    [SerializeField] private CircleCollider2D _rangeOfAttack;
    [SerializeField] private BoxCollider2D _leftLineOfSightTrigger;
    [SerializeField] private BoxCollider2D _rightLineOfSightTrigger;
    [SerializeField] private LayerMask _playersLayer;


    #endregion

    #region Unity Behaviors:

    private void Awake()
    {
        _leaperMovement = gameObject.GetComponent<LeaperMovement>();
    }

    private void FixedUpdate()
    {
        RaycastHit2D rightHit = Physics2D.BoxCast(_rightLineOfSightTrigger.transform.position, _rightLineOfSightTrigger.GetComponent<BoxCollider2D>().size, 0f, transform.right, Mathf.Infinity, _playersLayer);
        RaycastHit2D leftHit = Physics2D.BoxCast(_leftLineOfSightTrigger.transform.position, _leftLineOfSightTrigger.GetComponent<BoxCollider2D>().size, 0f, transform.right * -1f, Mathf.Infinity, _playersLayer);

        _inRangeForAttack = Physics2D.CircleCast(_rangeOfAttack.bounds.center, _rangeOfAttack.radius, Vector2.down, _circleCastDistance, _playersLayer);

        if (rightHit.collider != null)
        {
            _leaperMovement.MoveBackward();

            if (_inRangeForAttack)
            {
                _leaperMovement.Leap();
            }
        }

        else if (leftHit.collider != null)
        { 
            _leaperMovement.MoveForward();
            if (_inRangeForAttack)
            {
                _leaperMovement.Leap();
            }
        }

    }




    #endregion
}
