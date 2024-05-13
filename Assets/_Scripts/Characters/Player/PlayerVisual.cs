using UnityEngine;

/* FileName: PlayerVisual.cs
 * Description:     This script Handles the players movement horizontally, 
 *                  allowing the player to move left or right.
 * Author(s):       Matthew Perry
 * Last Modified:   04-09-2024 
 */

public class PlayerVisual : MonoBehaviour
{
    #region Private Variables:

    private PlayerHorizontalMovement _playerHorizontalMovement;
    private SpriteRenderer _spriteRenderer;
    public bool _isDashing;

    #endregion

    #region Unity Behaviors:

    private void Awake()
    {
        _playerHorizontalMovement = GetComponentInParent<PlayerHorizontalMovement>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
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
