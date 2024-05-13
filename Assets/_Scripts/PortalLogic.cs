using UnityEngine.SceneManagement;
using UnityEngine;

public class PortalLogic : MonoBehaviour
{
    #region Private Variables:

    [SerializeField] GameObject SpawnPoint;
    [SerializeField] string _gotoScene;
    [SerializeField] bool _switchscene;

    #endregion

    #region Unity Behaviors:

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (SpawnPoint != null)
        {
            if (collision.gameObject.tag == "Player")
            {
                if (_switchscene)
                {
                    SceneManager.LoadScene(_gotoScene);
                }
                else
                {
                    collision.gameObject.transform.position = SpawnPoint.transform.position;
                }
            }
        }
    }

    #endregion
}
