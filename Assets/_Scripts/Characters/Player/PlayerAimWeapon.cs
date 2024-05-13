using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

/* FileName: PlayerAimWeapon.cs
 * Description:     This script Handles the players aim 
 *                  while firing a weapon.
 * Author(s):       Matthew Perry
 * Last Modified:   04-04-2024 
 */

public class PlayerAimWeapon : MonoBehaviour
{
    #region Public Events:

    public event EventHandler<OnWeaponFlipUpdateEventArgs> OnWeaponFlip;
    public class OnWeaponFlipUpdateEventArgs : EventArgs { public float LocalScale; }

    #endregion

    #region Private Variables:

    private Transform _aimTransform;

    [SerializeField] private Vector3 _currentMousePosition;
    [SerializeField] private Vector3 _aimDirection;
    [SerializeField] private Vector3 _localScale;
    [SerializeField] private float _aimAngle;

    #endregion

    #region Unity Behaviors:
    private void Awake()
    {
        _aimTransform = transform.Find("AimPoint");
    }

    private void Update()
    {

        _currentMousePosition = GetMouseWorldPosition();

        _aimDirection = (_currentMousePosition - transform.position).normalized;
        _aimAngle = Mathf.Atan2(_aimDirection.y, _aimDirection.x) * Mathf.Rad2Deg;
        _aimTransform.eulerAngles = new Vector3(0, 0, _aimAngle);
        _localScale = Vector3.one;

        if (_aimAngle > 90 || _aimAngle < -90)
        {
            _localScale.y *= -1f;
            OnWeaponFlip?.Invoke(this, new OnWeaponFlipUpdateEventArgs { LocalScale = _localScale.y});
        }
        else
        {
            _localScale.y = Mathf.Abs(_localScale.y);
            OnWeaponFlip?.Invoke(this, new OnWeaponFlipUpdateEventArgs { LocalScale = _localScale.y });
        }

        _aimTransform.localScale = _localScale;

    }



    #endregion

    #region private Functions:

    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vec.z = 0f;
        return vec;
    }

    public static Vector3 GetMouseWorldPositionWithZ()
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    }

    public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
    }

    public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
    {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }

    #endregion
}
