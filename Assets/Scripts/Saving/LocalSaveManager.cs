using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;


public class LocalSaveManager : MonoBehaviour
{
    
    public PlayerData playerData;                                               //data-object --> class that holds data for player
    private string filename = "localRafHousingPlayerData.json";                 //name of file stored in persistent path

    [SerializeField] GameObject housePrefab;

    private void Start()
    {
        LoadFromLocal();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SaveToLocal();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            LoadFromLocal();
        }
    }

    public void FindAllHousesAndUpdateData()
    {

        GameObject[] housesInLevel = GameObject.FindGameObjectsWithTag("house");
        List<Vector3> location = new List<Vector3>();
        List<Vector3> rotation = new List<Vector3>();
        

        foreach (GameObject house in housesInLevel)
        {
            location.Add(house.transform.position);
            rotation.Add(house.transform.rotation.eulerAngles);
        }

        if(housesInLevel.Length <= 0)
        {
            location.Add(new Vector3(0,0,0));
            rotation.Add(new Vector3(0,0,0));
        }

        playerData.houseLocationRotations.houseLocation = location;
        playerData.houseLocationRotations.houseRotation = rotation;

    }

    public void UpdatePlayerDataObject()
    {
        playerData.level = 1;
        playerData.sliderHeight = GameManager.Instance.GetSliderHieght();
        playerData.currentScore = GameManager.Instance.GetScore();
    }

    private void PopulateBoardAndLoadData()
    {
        foreach(Vector3 location in playerData.houseLocationRotations.houseLocation)
        {
            GameObject newHouse = GameObject.Instantiate(housePrefab);
            newHouse.gameObject.transform.position = location;
        }
        foreach (Vector3 rotation in playerData.houseLocationRotations.houseRotation)
        {
            GameObject newHouse = GameObject.Instantiate(housePrefab);
            newHouse.gameObject.transform.rotation = Quaternion.Euler(rotation);
        }

        GameManager.Instance.slider.value = playerData.sliderHeight;
    }

    public void ClearBoardNewGame()
    {
        GameObject[] housesInLevel = GameObject.FindGameObjectsWithTag("house");

        foreach(GameObject house in housesInLevel)
        {
            Destroy(house);
        }

        GameManager.Instance.slider.value = 0;
    }


    //LOAD DATA - get data from local file
    public void LoadFromLocal()
    {
        string path = Path.Combine(Application.persistentDataPath, filename);

        
        if (File.Exists(path))                                                  //check --> if file path and file exist. The filename is defined within 'path'
        {
            string json = File.ReadAllText(path);                               //read data in file, stringify and save to var - json
            playerData = JsonUtility.FromJson<PlayerData>(json);                //convert json to data-object PlayerData and save to local variable playerData
            PopulateBoardAndLoadData();
            Debug.Log("game data loaded from + " + path);
            Debug.Log($"Player:  {playerData.playerName}, lastupdated : {playerData.lastupdated}");
        }
        else                                                                    //check --> if data can't be loaded, crate a new data object with default values.
        {
            Debug.Log("no saved file found at " + path + "; creating a new data file ");
            playerData = new PlayerData()
            {
                playerId = "00000",
                playerName = "default name",
                lastupdated = 0,
                level = 1,
                currentScore = 0,
                sliderHeight = 0,
                houseLocationRotations = new HouseLocationRotation(),
            };
        }
    }

    //SAVE TO LOCAL - send data TO local file
    public void SaveToLocal()
    {
        //Update data
        playerData.lastupdated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();         //Get the date of last update.
        FindAllHousesAndUpdateData();
        UpdatePlayerDataObject();

        //Convert to Json and save to local file
        string json = JsonUtility.ToJson(playerData);                               //convert data-object 'playerData' to Json.                     
        string path = Path.Combine(Application.persistentDataPath, filename);       //define the path. Concatinate persisent path with file name
        File.WriteAllText(path, json);                                              //write the data in the json file to the file path
        Debug.Log("local save completed");
    }
}
