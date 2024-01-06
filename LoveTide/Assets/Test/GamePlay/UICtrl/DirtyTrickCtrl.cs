using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DirtyTrickCtrl : MonoBehaviour
{
    [SerializeField] public DirtyTrickObject dirtyTrickObject;
    // Start is called before the first frame update
    public void TimeDown()
    {
        dirtyTrickObject.gameObject.SetActive(true);
    }

    public async void OnChangeScenes()
    {
        dirtyTrickObject.gameObject.SetActive(true);
        dirtyTrickObject.gameObject.GetComponent<Animator>().Play("ChangeScenes");
    }

    public void OnExitGamePlayScenes()
    {
        dirtyTrickObject.gameObject.SetActive(true);
        dirtyTrickObject.gameObject.GetComponent<Animator>().Play("OnExit");
    }
}
