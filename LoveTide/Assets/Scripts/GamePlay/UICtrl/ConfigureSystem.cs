using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfigureSystem : MonoBehaviour
{
    [SerializeField] private GameObject quitObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReadyQuitGame()
    {
        quitObject.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public async void OnBackMenu()
    {
        FindObjectOfType<DirtyTrickCtrl>().OnExitGamePlayScenes();
        await Task.Delay(1500);
        SceneManager.LoadScene("Scenes/StartUP");
    }

    public async void ClickDisplayImage()
    {
        FindObjectOfType<DirtyTrickCtrl>().OnExitGamePlayScenes();
        await Task.Delay(1500);
        SceneManager.LoadScene("Scenes/DisplayCG");
    }

}
