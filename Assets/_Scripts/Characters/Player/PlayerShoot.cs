using UnityEngine;
using UnityEngine.InputSystem;

/* FileName: PlayerShoot.cs
 * Description:     This script Handles the players shooting 
 * Author(s):       Matthew Perry
 * Last Modified:   04-03-2024 
 */

public class PlayerShoot : MonoBehaviour
{
    #region Private Variables:

    private PlayerControls _playerControls;
    private InputAction _fire;
    private InputAction _alternateFire;
    private PlayerAimWeapon _weaponAim;

    [SerializeField] private bool _canAlternateFire;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Vector3 _weaponsLocalScale;
    [SerializeField] private float _AlternateBulletSpeedMultiplyer = 2f;
    [SerializeField] private float _AlternateBulletMassMultiplyer = 2f;
    [SerializeField] private float _AlternateBulletDamgeMultiplyer = 9f;


    #endregion

    #region Unity Behaviors

    private void OnEnable()
    {
        _fire = _playerControls.Player.Fire;
        _alternateFire = _playerControls.Player.AlternateFire;
        _fire.Enable();
        _alternateFire.Enable();
    }

    private void OnDisable()
    {
        _fire.Disable();
        _alternateFire.Disable();
    }

    private void Awake()
    {
        _playerControls = new PlayerControls();
        _weaponAim = GetComponent<PlayerAimWeapon>();
    }

    private void Start()
    {
        _playerControls.Enable();
        _weaponAim.OnWeaponFlip += UpdateProjectileLocalScale;

    }

    private void Update()
    {
        _playerControls.Player.Fire.performed += context =>
        {
            var projectile = GameObject.Instantiate(_projectile, _firePoint.position, _firePoint.rotation);
            projectile.transform.localScale = new Vector3(projectile.transform.localScale.x, projectile.transform.localScale.y * _weaponsLocalScale.y, projectile.transform.localScale.z);
        };

        _playerControls.Player.AlternateFire.performed += context =>
        {
            if(_canAlternateFire) 
            {
                var projectile = GameObject.Instantiate(_projectile, _firePoint.position, _firePoint.rotation);
                var logic = projectile.GetComponent<PlayerProjectileLogic>();
                projectile.transform.localScale = new Vector3(projectile.transform.localScale.x * 2, projectile.transform.localScale.y * _weaponsLocalScale.y * 2, projectile.transform.localScale.z);
                logic.SetSpeed(logic.GetSpeed() * _AlternateBulletSpeedMultiplyer);
                logic.SetMass(logic.GetMass() * _AlternateBulletMassMultiplyer);
                logic.SetDamage(logic.GetDamge() * _AlternateBulletDamgeMultiplyer);
            }
        

        };
    }

    #endregion

    #region Private Functions:

    private void UpdateProjectileLocalScale(object sender, PlayerAimWeapon.OnWeaponFlipUpdateEventArgs e)
    {
        _weaponsLocalScale.y = e.LocalScale;
    }

    #endregion

}
