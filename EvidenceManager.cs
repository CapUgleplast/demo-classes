using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class EvidenceManager : MonoBehaviour {

    public PhotonView photonMine;

    public DynamicObject door;

    [Header("Evidence Generator")]
    public List<GameObject> evidenceList = new List<GameObject>();
    public string[] EvidencePathsSpawnList;
    public string[] EvidenceAOPathsSpawnList;
    public GameObject[] AnxiousObjectsList;
    public List<GameObject[]> evidenceSpawnList = new List<GameObject[]>();

    [Header("Enemy Search")]
    private int rand;
    private int AOrand;
    private int TrueDemon;
    public bool[] evTrueList = new bool[10];
    public bool[] evFalse1List = new bool[10];
    public Toggle[] evIRLList;
    public Text demonType;
    public Button AcceptTheory;
    public string[] demonTypesList;
    private int demonFound;


    private void Start() {
        photonMine = PhotonView.Get(this);
        Random.InitState(System.DateTime.Now.Millisecond);

        Debug.LogError("start");

    }

    public void BindToggleToListener() {
        foreach(var t in evIRLList) {
            t.onValueChanged.AddListener(EvListCompare);
        }
        Debug.LogError("bind");
    }

 
    void GenerateEvidences() {
        Debug.LogError("rpcStart");

        photonMine.RPC("RPCGenerateEvidences", RpcTarget.MasterClient);
        photonMine.RPC("RPCCloseDoor", RpcTarget.All);//Photon

        Debug.LogError("rpcend");
    }

    void EvListCompare(bool _)
    {
        Debug.LogError("compare");

        bool equalEvs = true;
        for (int i = 0; i < 10; i++)
        {
            if (evIRLList[i].isOn != evTrueList[i])
            {
                equalEvs = false;
            }
        }
        if (equalEvs)
        {
            demonType.text = demonTypesList[0];
            AcceptTheory.gameObject.SetActive(true);
            demonFound = 0;
            return;
        }

        equalEvs = true;
        for (int i = 0; i < 10; i++)
        {
            if (evIRLList[i].isOn != evFalse1List[i])
            {
                equalEvs = false;
            }
        }
        if (equalEvs)
        {
            demonType.text = demonTypesList[1];
            AcceptTheory.gameObject.SetActive(true);
            demonFound = 1;
            return;
        }

        demonType.text = "";
        AcceptTheory.gameObject.SetActive(false);
    }


    [PunRPC]
    public void RPCCloseDoor() {
        door.CloseDoors();
        door.isLocked = true;
    }

    [PunRPC]
    public void RPCGenerateEvidences() {
        Debug.LogError("rpcStart");

        door.CloseDoors();
        door.isLocked = true;
        SceneManager.UnloadSceneAsync("Shop");

        TrueDemon = (int)Mathf.Ceil(Random.Range(-1f, demonTypesList.Length - 1));
        string SwapDemon;
        SwapDemon = demonTypesList[0];
        demonTypesList[0] = demonTypesList[TrueDemon];
        demonTypesList[TrueDemon] = SwapDemon;
        Debug.LogError("demon");


        AOrand = (int)Mathf.Ceil(Random.Range(-1f, AnxiousObjectsList.Length - 1));
        EvidencePathsSpawnList[9] = EvidenceAOPathsSpawnList[AOrand];
        foreach(var t in AnxiousObjectsList) {
            if(t != AnxiousObjectsList[AOrand]) {
                t.SetActive(false);
            }
        }
        Debug.LogError("AO");
        

        for(int i = 0; i < EvidencePathsSpawnList.Length; i++) {
            rand = (int)Mathf.Ceil(Random.Range(-1f, evidenceSpawnList[i].Length - 1));
            GameObject evidence = PhotonNetwork.Instantiate("Prefabs/" + EvidencePathsSpawnList[i], evidenceSpawnList[i][rand].transform.position, Quaternion.identity);
            
            photonMine.RPC("EvlistSynch", RpcTarget.All, EvidencePathsSpawnList[i]); //Photon
        }
        Debug.LogError("evidencelist");


        foreach (var j in evTrueList)
        {
            rand = (int)Mathf.Ceil(Random.Range(-1f, evidenceList.Count - 1));
            if(evTrueList[rand] != true) {
                photonMine.RPC("TrueEvsAssignment", RpcTarget.All, rand); //Photon
                j++;
                Debug.LogError("trueEv " + (rand + 1));
            }
        }

        foreach (var j in evFalse1List)
        {
            rand = (int)Mathf.Ceil(Random.Range(-1f, evidenceList.Count - 1));
            if(evFalse1List[rand] != true) {
                photonMine.RPC("FalseEvs1Assignment", RpcTarget.All, rand); //Photon
                j++;
            }
        }

        CursedManager.gameStart = true;
    }

}
