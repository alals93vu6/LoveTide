using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class QuitGameObject : MonoBehaviour
{
    [SerializeField] private GameObject configureObject;
    [SerializeField] private DirtyTrickCtrl quitEffect;

    public async void OnQuitGame()
    {
        quitEffect.OnExitGamePlayScenes();
        await Task.Delay(1500);
        Application.Quit();
    }

    public void BackConfigure()
    {
        configureObject.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
    
    public void BackMenu()
    {
        this.gameObject.SetActive(false);
    }
}
