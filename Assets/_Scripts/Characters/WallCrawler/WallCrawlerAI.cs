using UnityEngine;

/* FileName: WallCrawlerAI.cs
 * Description:     This script Handles Wall Crawlers AI and how they interact in game 
 * Author(s):       Matthew Perry
 * Last Modified:   04-14-2024 
 */

public class WallCrawlerAI : MonoBehaviour
{
    #region Private Variables:

    private BoxCollider2D _frontFieldOfView;
    private BoxCollider2D _backFieldOfView;
    [SerializeField] private GameObject _right;
    [SerializeField] private GameObject _left;
    [SerializeField] private bool _seeSomethingInFront;
    [SerializeField] private bool _seeSomethingInBack;
    [SerializeField] private float _sightDistance;
    [SerializeField] private LayerMask _playersLayer;
    [SerializeField] private WallCrawlerMovement _wallCrawlerMovement;

    #endregion

    #region Unity Behaviors:

    private void Awake()
    {
        _wallCrawlerMovement = gameObject.GetComponent<WallCrawlerMovement>();
    }

    private void FixedUpdate()
    {
        //RaycastHit2D rightHit = Physics2D.Raycast(_right.transform.position, transform.right, Mathf.Infinity, _playersLayer);
        //RaycastHit2D leftHit = Physics2D.Raycast(_left.transform.position, transform.right * -1f, Mathf.Infinity, _playersLayer);

        RaycastHit2D rightHit = Physics2D.BoxCast(_right.transform.position,_right.GetComponent<BoxCollider2D>().size,0f, transform.right, Mathf.Infinity, _playersLayer);
        RaycastHit2D leftHit = Physics2D.BoxCast(_right.transform.position, _right.GetComponent<BoxCollider2D>().size, 0f, transform.right * -1f, Mathf.Infinity, _playersLayer);

        if (rightHit.collider != null)
        {
            _seeSomethingInFront = true;
            _wallCrawlerMovement.MoveForward();
            _wallCrawlerMovement.Dash();
        }

        else if (leftHit.collider != null)
        {
            _seeSomethingInBack = true;
            _wallCrawlerMovement.MoveBackward();
            _wallCrawlerMovement.Dash();
        }
        else
        {
            _seeSomethingInFront = false;
            _seeSomethingInBack = false;
            _wallCrawlerMovement.Walk();
        }
    }

    #endregion
}
