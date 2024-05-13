using UnityEngine;

/* FileName: CritterVisual.cs
 * Description:     This script handles the visuals of the critter
 * Author(s):       Matthew Perry
 * Last Modified:   04-10-2024 
 */

public class CritterVisual : MonoBehaviour
{
    #region Private Variables:

    [SerializeField] float _direction;
    [SerializeField] CritterMovement _critterMovement;

    #endregion

    #region Unity Behaviors:

    private void Awake()
    {
        _critterMovement = GetComponentInParent<CritterMovement>();
    }

    private void Start()
    {
        _critterMovement.OnDirectionUpdate += UpdateDirection;
    }

    private void Update()
    {
        if (_direction > 0)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * -1f, transform.localScale.y);
        }
        else if (_direction < 0)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    #endregion

    #region Private Functions:

    private void UpdateDirection(object sender, CritterMovement.OnDirectionUpdateUpdateEventArgs e)
    {
        _direction = e.Direction;
    }

    #endregion
}
