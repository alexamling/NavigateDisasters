using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for controlling all of the acitons nessicary for the map to run
/// this includes all the behind the scenes data processing and tracking as well as organizing the presentation of data
/// Writeen by Alexander Amling
/// </summary>

    /*NOTES:
     * must be attatched to a camera object 
     * requires heightmap
     * generate data maps should be relocated from firemanager to here, or to the base manager class
     */

// struct to simplify the organization of data maps
[System.Serializable]
public struct MapData
{
    public Texture2D heightMap;
    public Texture2D baseFuelMap;
    public Texture2D baseWaterMap;
    public Texture2D waterBodyMap;
    public Texture2D firePattern;
}

// struct to simplify the organization of compute shaders
[System.Serializable]
public struct ShaderCollection
{
    public ComputeShader fireTrackingShader;
    public ComputeShader floodTrackingShader;
    public ComputeShader viewMapShader;
}

public class MapController : MonoBehaviour
{
    /// <summary>
    /// Unused. Left in for future implementation
    /// </summary>
    /*
    public Terrain terrain;
    private TerrainData terrainData;
    [Range(64, 8192)]
    public int mapWidth = 4096;
    [Range(64, 8192)]
    public int mapHeight = 4096;
    [Range(1, 50)]
    public float heightScale;
    [Space(5)]
    public bool floodEnabled;
    public bool fireEnabled;
    [Space(5)]
    public MapData dataMaps;
    [Space(5)]
    public ShaderCollection shaders;
    [Space(5)]
    public Material mapMaterial;
    [Space(10)]
    */

    [Header("Objective Variables")]
    [Range(0, 60)]
    public float objectiveFrequency;
    [Range(0, 60)]
    public float objectiveVariance;
    public List<GameObject> objectives;
    private float timeSinceLastObjective;
    private float nextObjective;

    [Space(5)]

    [Header("Inject Variables")]
    [Range(0, 300)]
    public float injectFrequency;
    [Range(0,60)]
    public float injectVariance;
    [Range(0, 60)]
    public float injectStepTimer;
    [Range(0, 60)]
    public float injectStepVariance;
    private InjectsManager injectManager;
    private float timeSinceLastInject;
    private float nextInject;

    [Space(5)]

    [Header("Flood Variables")]
    //public GameObject waterPrefab;
    //public AnimationCurve floodCurve;
    //public float baseFloodHeight;
    //public float maxFloodHeight;
    public GameObject floodLocationRoot;
    private Transform[] floodLocations;
    //[HideInInspector]
    //public FloodManager floodManager;

    [Space(5)]

    [Header("Fire Variables")]
    //public ParticleSystem fireParticles;
    //public ParticleSystem explosionParticles;
    public GameObject fireLocationRoot;
    private Transform[] fireLocations;
    //[HideInInspector]
    //public FireManager fireManager;

    [Space(5)]

    [Header("PersonalIncidents Variables")]
    public GameObject personalLocationRoot;
    private Transform[] personalLocations;

    [Space(5)]

    [Header("Accident Variables")]
    public GameObject accidentLocationRoot;
    private Transform[] accidentLocations;

    [Header("UI Variables")]
    public GameObject iconRoot;
    
    [Space(10)]
    public float score;

    [HideInInspector]
    public objectiveReader objectiveReader;
    [HideInInspector]
    public gameTimer gameTimer;
    public PlayerControls playerControls;
    

    //private Texture2D fireSnapshot;
    //private Texture2D viewSnapshot;
    //private Texture2D replacement;
    
    //[HideInInspector]
    //public TerrainGenerator terrainGenerator;

    //private new Renderer renderer;

    //private ParticleSystem.ShapeModule shapeModule;

    public void QuitGame()
    {
        Application.Quit();
    }


    // Adds managers and passes values to them
    void Start()
    {
        //initialize locations
        fireLocations = fireLocationRoot.GetComponentsInChildren<Transform>();
        accidentLocations = accidentLocationRoot.GetComponentsInChildren<Transform>();
        personalLocations = personalLocationRoot.GetComponentsInChildren<Transform>();
        floodLocations = floodLocationRoot.GetComponentsInChildren<Transform>();

        fireLocations = SelectObjectiveAreas(fireLocations, 6);
        accidentLocations = SelectObjectiveAreas(accidentLocations, 8);
        personalLocations = SelectObjectiveAreas(personalLocations, 11);


        playerControls = FindObjectOfType<PlayerControls>();
        injectManager = FindObjectOfType<InjectsManager>();
        objectiveReader = FindObjectOfType<objectiveReader>();
        gameTimer = FindObjectOfType<gameTimer>();

        timeSinceLastObjective = 0;
        timeSinceLastInject = 0;
        nextObjective = 1;
        nextInject = injectFrequency + Random.Range(-injectVariance, injectVariance);

        #region old shader functionality
        /*

        shapeModule = fireParticles.shape;
        terrainData = terrain.terrainData;

        fireSnapshot = new Texture2D(mapWidth, mapHeight, TextureFormat.RGBA32, false, true);
        viewSnapshot = new Texture2D(mapWidth, mapHeight, TextureFormat.RGBA32, false, false);

        if (floodEnabled)
        {
            floodManager = gameObject.AddComponent<FloodManager>();

            floodManager.mapWidth = mapWidth;
            floodManager.mapHeight = mapHeight;

            floodManager.floodCurve = floodCurve;

            floodManager.baseHeight = baseFloodHeight;
            floodManager.maxHeight = maxFloodHeight;

            floodLocations = floodLocationRoot.GetComponentsInChildren<Transform>();

            // load availible data maps
            if (dataMaps.heightMap)
                floodManager.heightMap = dataMaps.heightMap;

            floodManager.waterObject = Instantiate(waterPrefab);
        }

        if (fireEnabled)
        {
            fireParticles.gameObject.SetActive(true);
            fireManager = gameObject.AddComponent<FireManager>();

            fireManager.trackingShader = shaders.fireTrackingShader;

            fireManager.fireEffect = dataMaps.firePattern;
            
            fireManager.mapWidth = mapWidth;
            fireManager.mapHeight = mapHeight;

            //fireLocations = fireLocationRoot.GetComponentsInChildren<Transform>();

            // load availible data maps
            if (dataMaps.heightMap)
                fireManager.heightMap = dataMaps.heightMap;
            if (dataMaps.baseFuelMap)
                fireManager.baseFuelMap = dataMaps.baseFuelMap;
            if (dataMaps.baseWaterMap)
                fireManager.baseWaterMap = dataMaps.baseWaterMap;
            if (dataMaps.waterBodyMap)
                fireManager.waterBodyMap = dataMaps.waterBodyMap;
        }
        else
        {
            fireParticles.gameObject.SetActive(false);
        }

        terrainGenerator = gameObject.AddComponent<TerrainGenerator>();
        terrainGenerator.width = mapWidth;
        terrainGenerator.height = mapHeight;
        //terrainGenerator.LOD = LOD;
        terrainGenerator.heightMap = dataMaps.heightMap;
        terrainGenerator.scale = heightScale;
        terrainGenerator.terrainData = terrainData;
        */
        #endregion

        //StartCoroutine(Load());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            SpawnFloodObjective();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            SpawnFireObjective();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            SpawnAccidentObjective();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            SpawnPersonalObjective();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            injectManager.StartInject(injectStepTimer, injectStepVariance);
        }


    }

    void FixedUpdate()
    {
        if (gameTimer.gameState == GameState.Running)
        {
            timeSinceLastObjective += Time.deltaTime;
            timeSinceLastInject += Time.deltaTime;

            if(timeSinceLastObjective > nextObjective)
            {
                timeSinceLastObjective -= nextObjective;
                nextObjective = objectiveFrequency + Random.Range(-objectiveVariance, objectiveVariance);
                SpawnRandomObjective();
            }

            if(timeSinceLastInject > nextInject)
            {
                timeSinceLastInject -= nextInject;
                nextInject = injectFrequency + Random.Range(-injectVariance, injectVariance);
                injectManager.StartInject(injectStepTimer, injectStepVariance);
            }
        }
    }

    /// <summary>
    /// Unused. Left in for possible future implementation
    /// </summary>
    /// <returns></returns>
    /*
    IEnumerator Load()
    {
        yield return StartCoroutine(terrainGenerator.Load());
        shapeModule.mesh = terrainGenerator.mesh;
        if (fireEnabled)
            yield return StartCoroutine(fireManager.Load());
        if (floodEnabled)
            yield return StartCoroutine(floodManager.Load());

        if (fireEnabled)
        for (int i = 0; i < 25; i++)
        {
            fireManager.StartFire();
        }
    }

    /// <summary>
    /// Used to pass data from the fire rendertexture to a texture2D, so that it can be used as a texture for particle effect emmision
    /// </summary>
    void OnPostRender()
    {
        if (Time.frameCount % 20 == 0)
            UpdateMap();
    }

    void UpdateMap()
    {
        mapMaterial.SetTexture("_ViewMap", viewSnapshot);

        

        if (fireEnabled)
        {
            RenderTexture.active = fireManager.output;
            replacement.ReadPixels(new Rect(0, 0, mapWidth, mapHeight), 0, 0);
            replacement.Apply();
            RenderTexture.active = null;
            mapMaterial.SetTexture("_FireMap", replacement);
            shapeModule.texture = replacement;
        }

        if (renderer)
            renderer.material = mapMaterial;
        else
            renderer = gameObject.GetComponent<Renderer>();
    }
    */

    Transform[] SelectObjectiveAreas(Transform[] arr, int numLocations)
    {
        numLocations = (arr.Length > numLocations) ? numLocations : arr.Length;

        Transform[] newArr = new Transform[numLocations];

        for (int i = 0; i < numLocations; i++)
        {
            newArr[i] = arr[Random.Range(1, arr.Length)];
        }

        return newArr;
    }

    void SpawnRandomObjective()
    {
        int val = Random.Range(0, 5);

        switch (val)
        {
            case (0):
            case (1):
                SpawnFloodObjective();
                break;
            case (2):
                SpawnFireObjective();
                break;
            case (4):
                SpawnAccidentObjective();
                break;
            case (5):
                SpawnPersonalObjective();
                break;
        }
    }

    void SpawnFloodObjective()
    {
        PlayerObjective objective = Instantiate(objectiveReader.floodList[Random.Range(0, objectiveReader.floodList.Count)]);

        Transform placementValues = floodLocations[Random.Range(1, floodLocations.Length)].transform;

        Vector2 pos = Random.insideUnitCircle * placementValues.localScale.x * .5f;

        objective.transform.position = new Vector3(pos.x + placementValues.position.x, 0, pos.y + placementValues.position.z);

        SpawnObjective(objective);
    }

    void SpawnFireObjective()
    {
        PlayerObjective objective = Instantiate(objectiveReader.fireList[Random.Range(0, objectiveReader.fireList.Count)]);
        Transform placementValues = fireLocations[Random.Range(1, fireLocations.Length)].transform;

        Vector2 pos = Random.insideUnitCircle * placementValues.localScale.x * .5f;

        objective.transform.position = new Vector3(pos.x + placementValues.position.x, 0, pos.y + placementValues.position.z);

        SpawnObjective(objective);
    }

    void SpawnAccidentObjective()
    {
        PlayerObjective objective = Instantiate(objectiveReader.accidentList[Random.Range(0, objectiveReader.accidentList.Count)]);
        Transform placementValues = accidentLocations[Random.Range(1, accidentLocations.Length)].transform;

        Vector2 pos = Random.insideUnitCircle * placementValues.localScale.x * .5f;

        objective.transform.position = new Vector3(pos.x + placementValues.position.x, 0, pos.y + placementValues.position.z);

        SpawnObjective(objective);
    }

    void SpawnPersonalObjective()
    {
        PlayerObjective objective = Instantiate(objectiveReader.personalList[Random.Range(0, objectiveReader.personalList.Count)]);
        Transform placementValues = personalLocations[Random.Range(1, personalLocations.Length)].transform;

        Vector2 pos = Random.insideUnitCircle * placementValues.localScale.x * .5f;

        objective.transform.position = new Vector3(pos.x + placementValues.position.x, 0, pos.y + placementValues.position.z);

        SpawnObjective(objective);
    }

    private void SpawnObjective(PlayerObjective objective)
    {

        Notification newNotification = Instantiate(playerControls.notificationPrefab, playerControls.notificationPanel.panel.transform);
        newNotification.transform.SetAsFirstSibling();
        newNotification.text.text = objective.notificationTitle;
        newNotification.severity = 0;
        newNotification.objective = objective;
        newNotification.manager = playerControls;
        objective.notification = newNotification;
        objective.iconRoot = iconRoot;
        playerControls.notifications.Add(newNotification);

        objective.objectiveState = ObjectiveState.Requesting;

        if (!objective.needsResponse)
        {
            playerControls.ignoredObjectivesIdeal++;
        }
    }
}
