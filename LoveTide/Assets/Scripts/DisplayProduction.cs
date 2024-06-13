using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisplayProduction : MonoBehaviour
{
    [SerializeField] private DirtyTrickCtrl darkCtrl;
    // Start is called before the first frame update
    async void Start()
    {
        SceneManager.LoadScene("Scenes/StartUP");
        /*
        await Task.Delay(3000);
        darkCtrl.OnExitGamePlayScenes();
        await Task.Delay(800);
        SceneManager.LoadScene("Scenes/StartUP");*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
