using UnityEngine;

/* FileName: DeathBarrier.cs
 * Description:     This script Handles when the player passes this barrier they respawn
 * Author(s):       Matthew Perry
 * Last Modified:   04-07-2024 
 */

public class DeathBarrier : MonoBehaviour
{
    #region Private Variables:

    [SerializeField] GameObject SpawnPoint;

    #endregion

    #region Unity Behaviors:

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (SpawnPoint != null)
        {
            if (collision.gameObject.tag == "Player")
            {
                collision.gameObject.transform.position = SpawnPoint.transform.position;
            }
        }
    }

    #endregion
}
