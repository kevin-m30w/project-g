using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public void OnLeverPulled()
    {
        StartCoroutine(LoadLevelRoutine("Map_Forest"));
    }

    private IEnumerator LoadLevelRoutine(string sceneName)
    {
        // 1. Play train moving sounds / lock lever here

        // 2. Load the lever scene in the background without destroying the lobby train scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // Wait until scene geometry is loaded into memory
        while (!asyncLoad.isDone)
        {
            float progress = asyncLoad.progress; // Can be used for UI progress bars
            yield return null;
        }

        // 3. Trigger your Procedural Map Generator script here!
        // 4. Once generated, open train doors.
    }
}
