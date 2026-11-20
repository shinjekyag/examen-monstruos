using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class MenuInicio : MonoBehaviour
{
    private const string EscenaJuego = "Assets/Scenes/Tarea 09.unity";

    public void Jugar()
    {
        int indiceEscena = SceneUtility.GetBuildIndexByScenePath(EscenaJuego);
        if (SceneManager.LoadSceneAsync(indiceEscena) != null)
            GetComponent<Button>().interactable = false;
    }
}
