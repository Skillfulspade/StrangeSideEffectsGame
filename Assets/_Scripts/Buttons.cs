using UnityEngine;
using UnityEngine.SceneManagement;

/* FileName: Buttons.cs
 * Description:     This is the logic for button UI
 * Author(s):       Matthew Perry
 * Last Modified:   04-15-2024 
 */

public class Buttons : MonoBehaviour
{
    public string DemoScene;

    public void GotoDemoScene()
    {
        SceneManager.LoadScene(DemoScene);
    }
}
