using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    //[SerializeField] public NumericalRecords playerData;
    [SerializeField] private bool isContinue;
    [SerializeField] private GameObject saveMenu;
    [SerializeField] public SaveLocation[] saveObject;
    [SerializeField] public DirtyTrickCtrl darkCtrl;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void OnClickStartGame(bool isContinue)
    {
        darkCtrl.OnChangeScenes();
        await Task.Delay(500);
        saveMenu.SetActive(true);
        for (int i = 0; i < saveObject.Length; i++)
        {
            saveObject[i].DisplayDataInformation(isContinue);
        }
    }

    public async void BackMenu()
    {
        darkCtrl.OnChangeScenes();
        await Task.Delay(500);
        saveMenu.SetActive(false);
    }
}
