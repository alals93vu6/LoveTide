using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DirtyTrickCtrl : MonoBehaviour
{
    [SerializeField] public GameObject dirtyTrickObject;
    // Start is called before the first frame update
    void Start()
    {
        ActiveOff_OnStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OnChangeScenes();
        }
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            OnExitGamePlayScenes();
        }
    }

    private async void ActiveOff_OnStart()
    {
        await Task.Delay(1600);
        dirtyTrickObject.SetActive(false);
    }

    public async void OnChangeScenes()
    {
        dirtyTrickObject.SetActive(true);
        dirtyTrickObject.gameObject.GetComponent<Animator>().Play("ChangeScenes");
        await Task.Delay(1050);
        dirtyTrickObject.SetActive(false);
    }

    public void OnExitGamePlayScenes()
    {
        dirtyTrickObject.SetActive(true);
        dirtyTrickObject.gameObject.GetComponent<Animator>().Play("OnExit");
    }
}
