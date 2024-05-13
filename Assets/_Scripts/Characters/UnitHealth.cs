using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/* FileName: UnitHealth.cs
 * Description:     This script Handles A Units Health
 * Author(s):       Matthew Perry
 * Last Modified:   04-05-2024 
 */

public class UnitHealth : MonoBehaviour
{
    #region Public Events:

    public event EventHandler<OnTookDamageUpdateEventArgs> OnTookDamageUpdate;
    public class OnTookDamageUpdateEventArgs : EventArgs { public float currentHealth; }

    #endregion

    #region Private Variables:

    [Header ("Health Variables:")]
    [SerializeField] private bool _upSize;
    [SerializeField] GameObject SpawnPoint;
    [SerializeField] private float _lives = 2f;
    [SerializeField] private float _unitHealth = 10f;
    [SerializeField] private float _unitHealthMax = 10f;
    [SerializeField] private float _upSizedHealth = 10f;
    [SerializeField] private float _upSizedHealthMax = 20f;
    [SerializeField] private float _upSizeScale = 2f;
    [SerializeField] private TMP_Text _lifeText;
    [SerializeField] private TMP_Text _healthText;


    [Header ("Read Only")]
    [SerializeField] private float _currentHealth;
    [SerializeField] private bool _hasUpSizedHealth;

    #endregion

    #region Unity Behaviors:

    private void Start()
    {
        _currentHealth = _unitHealth;

        if (gameObject.tag == "Player")
        {
            _lifeText.text = "Lives: " + _lives.ToString();
            _healthText.text = "Health: " + _currentHealth.ToString();
        }
    }

    private void FixedUpdate()
    {
        if (_unitHealth != _currentHealth)
        {
            if (_unitHealth > _currentHealth)
            {
                OnTookDamageUpdate?.Invoke(this, new OnTookDamageUpdateEventArgs { currentHealth = _currentHealth});
            }

            _unitHealth = _currentHealth;

            if (_unitHealth <= 0f && gameObject.tag != "Player")
            {
                GameObject.Destroy(gameObject);
            }

            else if(_unitHealth <= 0f && gameObject.tag == "Player")
            {
                if (!_upSize)
                {
                    _currentHealth = _unitHealthMax;
                }
                else
                {
                    _currentHealth = _upSizedHealthMax;
                }
                
                _lives -= 1f;
                transform.position = SpawnPoint.transform.position;

            }

            if (gameObject.tag == "Player")
            {
                if (_lives <= 0f)
                {
                    SceneManager.LoadScene("GameOver");
                }

                _lifeText.text = "Lives: " + _lives.ToString();
                _healthText.text = "Health: " + _currentHealth.ToString();
            }

         

        }

        if (_upSize)
        {
            
            if (!_hasUpSizedHealth)
            {
                gameObject.transform.localScale = new Vector3(_upSizeScale, _upSizeScale, 1f);
                _hasUpSizedHealth = true;
                healUnit(_upSizedHealth);
            }
        }
    }

    #endregion

    #region Private Functions:

    public void damageUnit(float damage)
    {
        _currentHealth -= damage;
    }

    public void healUnit(float health)
    {
        _currentHealth += health;
    }

    #endregion
}
