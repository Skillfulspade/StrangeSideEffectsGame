using UnityEngine;

/* FileName: ObjectVisual.cs
 * Description:     This script Handles all objects color when the player dashes
 * Author(s):       Matthew Perry
 * Last Modified:   04-10-2024
 */

public class ObjectVisual : MonoBehaviour
{
    #region Private Variables:

    private PlayerHorizontalMovement _playerHorizontalMovement;
    private SpriteRenderer _spriteRenderer;
    private bool _isDashing;

    #endregion

    #region Unity Behaviors:

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _playerHorizontalMovement = PlayerHorizontalMovement.Instance;
        _playerHorizontalMovement.OnIsDashing += UpdateIsDashing;
    }

    private void FixedUpdate()
    {
        if (_isDashing)
        {
            _spriteRenderer.material.color = Color.gray;
        }
        else
        {
            _spriteRenderer.material.color = Color.white;
        }
    }

    #endregion

    #region private Function:

    private void UpdateIsDashing(object sender, PlayerHorizontalMovement.OnIsDashingUpdateEventArgs e)
    {
        _isDashing = e.IsDashing;
    }

    #endregion
}
