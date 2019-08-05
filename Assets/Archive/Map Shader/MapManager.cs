using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance
    {
        get
        {
            if (Instance)
                return Instance;
            else
                Instance = new MapManager();
            return Instance;
        }
        private set
        {
            Instance = value;
        }
    }

    private MapManager()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckSates()
    {

    }
}
