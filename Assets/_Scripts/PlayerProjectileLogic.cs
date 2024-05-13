using UnityEngine;

/* FileName: PlayerProjectileLogic.cs
 * Description:     This script Handles the players shooting 
 * Author(s):       Matthew Perry
 * Last Modified:   04-03-2024 
 */

public class PlayerProjectileLogic : MonoBehaviour
{
    #region Private Variables:

    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _mass = 1f;
    [SerializeField] private float _damage = 1f;

    #endregion

    #region Unity Behaviors:

    private void Start()
    {
        _rigidbody.velocity = transform.right * _speed;
        _rigidbody.mass = _mass;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Player" && collision.gameObject.tag != "PlayerProjectile" )
        {
            if (collision.gameObject.tag == "Enemy") 
            {
                collision.gameObject.GetComponent<UnitHealth>().damageUnit(_damage);
            }
            Destroy(gameObject);
        }
    }

    #endregion

    #region Public Functions:

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void SetMass(float mass)
    {
        _mass = mass;
    }

    public void SetDamage(float damage)
    {
        _damage = damage;
    }

    public float GetSpeed()
    {
        return _speed;
    }

    public float GetMass()
    {
        return _mass;
    }

    public float GetDamge()
    {
        return _damage;
    }

    #endregion
}
