using Mono.Cecil;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class TerrainGenerator : MonoBehaviour
{
    #region bools used as buttons
    [Header("Buttons")]
    public bool generateNewTerrain;
    public bool placeDecorations;
    public bool placeResources;
    #endregion

    [Header("Terrain Settings")]
    #region seed settings
    public int seed;
    private int minSeedValue = 0;
    private int maxSeedValue = 10000;
    private List<int> previousSeeds = new List<int>();
    #endregion

    #region mesh-specific variables
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    public int terrainSize = 500;
    private int zSize;
    private int xSize;

    Vector2[] uvs;
    Color[] colors;
    #endregion

    //public Gradient gradient;
    private float minTerrainHeight;
    private float maxTerrainHeight;

    private int centerPoint; //centre of the terrain

    [Header("Object to place references")]
    //decorative objects to place
    [SerializeField]
    private GameObject treeToPlace;
    [SerializeField]
    private GameObject objToPlace2;
    [SerializeField]
    private GameObject objToPlace3;
    private GameObject objContainer; //holds all decorative gameobjects in the inspector

    private GameObject resourceContainer; //holds all cluster gameobjects in the inspector (for resources)
    private ResourceClusterSpawner resourceClusterSpawner;

    [SerializeField]
    private GameObject house;
    private int houseSize = 6; //should be an even number, used for flattening the terrain in the centre


    #region biome specific settings
    [Header("Biome Settings")]
    [Tooltip("This determines how much the biomes will blend. For no blending use 0.")]
    [Range(0, 10)]
    public int biomeBlendRadius = 5;

    public int biomeInnerRadius = 200;
    public int biomeMiddleRadius = 350;
    //public int biomeFarRadius = 0;

    [Tooltip("This determines how much the terrain will be divided for biomes. A higher number gives more detail, with many smaller biome pieces added together. 8 is a solid number for this.")]
    public int voronoiGridSize = 8; 
    private int verticesPerCell;

    [Tooltip("This adds randomness to the biome placement in regards to the distance from origo (0,0). For no added randomness use 0.")]
    public float biomePlacementRandomness = 1.5f; //added randomness to biome placement

    public float scale = 25f; //höjden på noiset för biomerna

    private Vector2Int[,] pointPositions;
    //private int[] possibleBiomes = { 0, 1, 2 };
    private int[,] biomes;
    private int[] biomeMap;
    private BiomePercent[] biomeMapPercentages;

    class BiomePercent
    {
        public float pBiome1 = 0;
        public float pBiome2 = 0;
        public float pBiome3 = 0;

        BiomePercent() { }
        public BiomePercent(float a, float b, float c)
        {
            pBiome1 = a; pBiome2 = b; pBiome3 = c;
        }
    }


    Snowfields biomeSnowfields;
    CrystalIcefields biomeCrystalIcefields;
    RockyHighlands biomeRockyHighlands;

    abstract class Biome
    {
        public abstract int ChanceForScrapMetal { get; }
        public abstract int ChanceForPlanks { get; }
        public abstract int ChanceForFabric { get; }
        public abstract int ChanceForMagnets { get; }
        public abstract int ChanceForElectronics { get; }
        public abstract int ChanceForThermalCrystals { get; }

        public int GetTotal()
        {
            return ChanceForScrapMetal + ChanceForPlanks 
                + ChanceForFabric + ChanceForMagnets 
                + ChanceForElectronics + ChanceForThermalCrystals;
        }

    }

    class Snowfields : Biome
    {
        override public int ChanceForScrapMetal { get { return 25; } }
        override public int ChanceForPlanks { get { return 70; } }
        override public int ChanceForFabric { get { return 50; } }
        override public int ChanceForMagnets { get { return 10; } }
        override public int ChanceForElectronics { get { return 0; } }
        override public int ChanceForThermalCrystals { get { return 0; } }

    }

    class CrystalIcefields : Biome
    {
        override public int ChanceForScrapMetal { get { return 10; } }
        override public int ChanceForPlanks { get { return 0; } }
        override public int ChanceForFabric { get { return 0; } }
        override public int ChanceForMagnets { get { return 25; } }
        override public int ChanceForElectronics { get { return 30; } }
        override public int ChanceForThermalCrystals { get { return 40; } }

    }

    class RockyHighlands : Biome
    {
        override public int ChanceForScrapMetal { get { return 55; } }
        override public int ChanceForPlanks { get { return 20; } }
        override public int ChanceForFabric { get { return 30; } }
        override public int ChanceForMagnets { get { return 50; } }
        override public int ChanceForElectronics { get { return 35; } }
        override public int ChanceForThermalCrystals { get { return 10; } }

    }

    #endregion


    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        //CreateNewMap();

        biomeSnowfields = new Snowfields();
        biomeCrystalIcefields = new CrystalIcefields();
        biomeRockyHighlands = new RockyHighlands();

        resourceClusterSpawner = this.GetComponent<ResourceClusterSpawner>();
    }

    void Update()
    {
        if (generateNewTerrain)
        {
            CreateNewTerrain();
            generateNewTerrain = false;
        }

        if (placeDecorations)
        {
            PlaceDecorations();
            placeDecorations = false;
        }

        if (placeResources)
        {
            PlaceResources();
            placeResources = false;
        }
    }

    public void NewSeed()
    {
        seed = UnityEngine.Random.Range(minSeedValue, maxSeedValue);

        //safety precaution
        if (previousSeeds.Count >= maxSeedValue)
        {
            Debug.Log("All available seeds have been used. Clearing seed list.");
            previousSeeds.Clear();
        }

        //if seed has already been used, pick a new one
        if (previousSeeds.Contains(seed))
        {
            Debug.Log("Seed: " + seed + " has already been used, picking a new one.");
            NewSeed();
            return;
        }
        previousSeeds.Add(seed);
    }

    public void CreateNewTerrain()
    {
        if (terrainSize <= 0)
        {
            Debug.LogWarning("ChunkSize is too small on " + this.gameObject.name + " it needs to be larger than 0.");
            return;
        }
        zSize = xSize = terrainSize;
        centerPoint = terrainSize / 2;


        CreateMeshShape();
        CreateTriangles();
        CreateColors();
        UpdateMesh();

        PlaceResources();

        PlaceDecorations();
    }

    private void CreateBiomes()
    {
        biomeMap = new int[vertices.Length];
        biomeMapPercentages = new BiomePercent[vertices.Length];

        //1. Seed choses how many biomes to use
        //4 är placeholder för vad som bör vara Biomes.length eller liknande
        //int biomeCount = 3;// UnityEngine.Random.Range(0, 4);

        //nu skapa voronoi och på punkterna sätt ut ett nummer från 0 till biomeCount
        verticesPerCell = terrainSize / voronoiGridSize;

        if (verticesPerCell <= 0)
        {
            Debug.LogWarning("Too small map for biomes");
            return;
        }

        GeneratePoints();

        for (int i = 0; i <= terrainSize; i++)
        {
            for (int j = 0; j <= terrainSize; j++)
            {

                int gridX = i / verticesPerCell;
                int gridY = j / verticesPerCell;

                float nearestDistance = Mathf.Infinity;
                Vector2Int nearestPoint = new Vector2Int();
                //kollar Moore's neighbourhood runt cellen
                for (int a = -1; a < 2; a++)
                {
                    for (int b = -1; b < 2; b++)
                    {
                        int X = gridX + a;
                        int Y = gridY + b;
                        if (X < 0 || Y < 0 || X >= voronoiGridSize || Y >= voronoiGridSize)
                            continue;
                        
                        float distance = Vector2Int.Distance(new Vector2Int(i, j), pointPositions[X, Y]);
                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestPoint = new Vector2Int(X, Y);//pointPositions[X, Y];
                        }    
                    }
                }

                int pos = i * xSize + j;

                if (pos >= 0 && pos < biomeMap.Length)
                    biomeMap[pos] = biomes[nearestPoint.x, nearestPoint.y];
                else
                    Debug.LogWarning("too large pos!!");
            }
        }

    }

    private void GeneratePoints()
    {
        Vector2 centre = new Vector2((voronoiGridSize / 2), (voronoiGridSize / 2));
        if (voronoiGridSize % 2 == 0) //solves distance favouring one side when the grid is evenly divided
            centre -= new Vector2(0.5f, 0.5f);


        pointPositions = new Vector2Int[voronoiGridSize, voronoiGridSize];
        biomes = new int[voronoiGridSize, voronoiGridSize];
        for (int i = 0; i < voronoiGridSize; i++)
        {
            for (int j = 0; j < voronoiGridSize; j++)
            {
                //placing points randomly within the biome cells
                pointPositions[i, j] = new Vector2Int(i * verticesPerCell + UnityEngine.Random.Range(0, verticesPerCell), j * verticesPerCell + UnityEngine.Random.Range(0, verticesPerCell));

                float dist = Vector2.Distance(new Vector2(i, j), centre);

                //adding a random number to the distance to get some more randomness in the selected biomes
                dist += UnityEngine.Random.Range(-biomePlacementRandomness, biomePlacementRandomness);
                dist *= 100; //translated for an easier distance value to work with

                int biome = -1;
                if (dist < biomeInnerRadius)
                {
                    //biome snowfield
                    biome = 0;

                }
                else if (dist < biomeMiddleRadius)
                {
                    //biome highlands
                    biome = 2;
                }
                else
                {
                    //biome ice crystal fields
                    biome = 1;
                }

                //biome = possibleBiomes[UnityEngine.Random.Range(0, possibleBiomes.Length)];

                //och väljer vilken färg aka biome punkten ska ha
                //biomes[i, j] = possibleBiomes[UnityEngine.Random.Range(0, possibleBiomes.Length)];
                biomes[i, j] = biome;
            }
        }
    }

    private void BlendBiomes()
    {
        int minBlendLength = 0 - biomeBlendRadius;
        int maxBlendLength = biomeBlendRadius + 1;


        //create temporary map with the size of the biomeMap
        //int[] tBiomeMap = new int[biomeMap.Length];
        BiomePercent[] tBiomeMap = new BiomePercent[biomeMap.Length];

        //Go through the biome map
        for (int x = 0; x <= terrainSize; x++)
        {
            for (int z = 0; z <= terrainSize; z++)
            {
                int pos = x * xSize + z;

                //get this biome
                //int biome = biomeMap[pos];


                int biome1Count = 0;
                int biome2Count = 0;
                int biome3Count = 0;
                float divideLength = 0;

                //kollar Moore's neighbourhood runt cellen
                //och samlar ihop vilka biomer vi har
                for (int a = minBlendLength; a < maxBlendLength; a++)
                {
                    for (int b = minBlendLength; b < maxBlendLength; b++)
                    {
                        int tX = x + a;
                        int tZ = z + b;
                        int tPos = (tX * xSize) + tZ;

                        if (tX < 0 || tZ < 0 || tPos >= biomeMap.Length )
                        {
                            //Debug.Log("skipped saving this position " + tPos + " x: "  + x  + " z: " + z);
                            continue;
                        }



                        int tBiome = biomeMap[tPos];

                        if(tBiome  == 0)
                        {
                            biome1Count++;
                        }
                        else if(tBiome == 1)
                        {
                             biome2Count++;
                        }
                        else
                        {
                            biome3Count++;
                        }
                        divideLength++;
                    }
                }




                //räknar ut procenten för varje biom
                if(divideLength == 0)
                    divideLength = 1;
                float pBiome1 = biome1Count / (float)divideLength;
                float pBiome2 = biome2Count / (float)divideLength;
                float pBiome3 = biome3Count / (float)divideLength;


                //och lägg till detta i arrayen

                BiomePercent biomePercent = new BiomePercent(pBiome1,pBiome2,pBiome3);

                if (pos >= 0 && pos < biomeMap.Length)
                    tBiomeMap[pos] = biomePercent;
            }
        }

        //set biomeMap = tempMap
        biomeMapPercentages = tBiomeMap;
    }


    private void CreateMeshShape()
    {
        //offset för att hålla kartans centre i origo
        float offsetX =  -xSize / 2;
        float offsetZ =  -zSize / 2;

        //1. create seed
        UnityEngine.Random.InitState(seed);

        int newNoise = UnityEngine.Random.Range(0, 10000); //variabel som används just nu för att lägga offset på perlin noise så att det blir nytt med nytt seed

        //2. force scale att vara större än 0

        //3. create array med verts
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        //skapa biomes 
        CreateBiomes();

        //och blenda dessa
        BlendBiomes();


        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                //set the height for each vert
                float y = getBiomeHeight(newNoise, x, z);

                if (x == centerPoint && z == centerPoint)
                {
                    house.transform.position = new Vector3(0, y+ house.transform.lossyScale.y/2, 0);
                }


                //läger till offset för att hålla kartans mitt i origo
                vertices[i] = new Vector3(x + offsetX, y, z + offsetZ);
                i++;
            }
        }

        //place house on correct height
        //float height = house.transform.lossyScale.y + Mathf.PerlinNoise((centerPoint + newNoise) * .03f, (centerPoint + newNoise) * .03f) * scale;
        //house.transform.position = new Vector3 (0, height, 0);

    }



    private float getBiomeHeight(int newNoise, int x, int z)
    {
        float y = 0;

        //SOME NOISE????
        float2 pos = new float2(x + newNoise, z + newNoise);  //                    double value = noise(x / 32.0, y / 32.0);
        float value = noise.srnoise(pos / 5 * 1, seed);//Unity.Mathematics.noise.snoise(pos);
        value = value * 0.5f;// + 0.5f;
        float absValue = Mathf.Abs(value);
        absValue *= 5;
        absValue += scale / 2;
        absValue += Mathf.PerlinNoise((x + newNoise) * .06f, (z + newNoise) * .06f) * (scale / 4);
        if (absValue > 15f)
        {
            absValue = noise.snoise(new float2(z * .03f, x * .03f));
            absValue = 1;
        }



        //PERLIN
        float perlin = Mathf.PerlinNoise((x + newNoise) * .01f, (z + newNoise) * .01f) * scale;  //där * .03f zoomar in på the noise, och * 25f gör hjöjdskilnaderna tydligare!!
        //för mer komplex noise lägger på stuff
        // perlin += Mathf.PerlinNoise((x + newNoise) * .06f, (z + newNoise) * .06f) * (scale / 4);
        // perlin += Mathf.PerlinNoise((x + newNoise) * .5f, (z + newNoise) * .5f) * (scale / 64);

        float perlin2 = Mathf.PerlinNoise((x + newNoise) * .005f, (z + newNoise) * .005f) * scale/2;

        float perlin3 = Mathf.PerlinNoise((x + newNoise) * .005f, (z + newNoise) * .005f) * 5;


        //SIMPLEX
        float simplex = noise.snoise(new float2(z * .03f, x * .03f));
        simplex += scale;





        if (y > maxTerrainHeight)
            maxTerrainHeight = y;
        if (y < minTerrainHeight)
            minTerrainHeight = y;

        //  i = (pos - j) /xSize
        // pos = i * xSize + j;   är   pos = x * xSize + y;
        int pos2 = (z * xSize) + x;
        BiomePercent bp = biomeMapPercentages[pos2];
        if (bp != null) 
            y = (bp.pBiome1 * perlin) + (bp.pBiome2 * ((perlin * absValue) )) + (bp.pBiome3 * ((perlin * perlin3)));



        //kolla om det är i center zonen
        /*if (centerPoint >= x - houseSize && centerPoint <= x + houseSize && centerPoint >= z - houseSize && centerPoint <= z + houseSize)
        {
            y = Mathf.PerlinNoise((centerPoint + newNoise) * .03f, (centerPoint + newNoise) * .03f) * scale;
        }*/

        return y;
    }

    private void CreateTriangles()
    {
        //6 verts are used to create a square (which is 2 triangels)
        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;

        //loop through rows
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

    }

    private void CreateColors()
    {
        colors = new Color[vertices.Length];
        uvs = new Vector2[vertices.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                uvs[i] = new Vector2((float)x / xSize, (float)z / zSize);

                //färger baserat på höjden av mappen
                //float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                //colors[i] = gradient.Evaluate(height);

                //biome färger med användning av gamla gradient slidern
                //colors[i] = gradient.Evaluate(Mathf.InverseLerp(0, possibleBiomes.Length, biomeMap[pos2]));

                //biome färger som använder biome percentages för att skapa rgb färg
                int pos = (z * xSize) + x;
                BiomePercent bp = biomeMapPercentages[pos];
                if (bp != null)
                    colors[i] = new Color(bp.pBiome1, bp.pBiome2, bp.pBiome3, 0.5f);

                i++;
            }
        }
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; //la till för att man ska kunna ha 1000 i storlek
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        mesh.uv = uvs;
        mesh.colors = colors;
        GetComponent<MeshCollider>().sharedMesh = mesh;

        //gameObject.transform.localScale = new Vector3(xSize * zSize, xSize * zSize, xSize * zSize);
    }

    private void PlaceDecorations()
    {
        if (treeToPlace == null)
        {
            Debug.LogWarning("The Object to place 'objToPlace' on " + this.gameObject.name + " is NULL! It needs a gameobject to have anything to place.");
            return;
        }

        Destroy(objContainer);
        objContainer = new GameObject("Decorations Container"); //för att hålla det clean i inspectorn


        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                //kolla om det är i center zonen, och isf skippa att lägga ut något här
                if (x >= centerPoint - houseSize && x <= centerPoint + houseSize && z >= centerPoint - houseSize && z <= centerPoint + houseSize)
                {
                    i++;
                    continue;
                }



                //olika saker beroende på biome   PLACHOLDER SKA SE FINARE UT!!!!!
                GameObject obj = treeToPlace;

                Renderer r = obj.GetComponent<Renderer>(); //kommer ta as mycket kraft
                if (r != null) 
                {
                    obj.GetComponent<Renderer>().sharedMaterial.color = Color.red;
                }
                
                
                float perlin = Mathf.PerlinNoise(x * .3f, z * .3f);
                int pos2 = (z * xSize) + x;

                //OBS, biomemap måste använda pos2, medan vertices måste använda i!!!!
                //sätter ut träd i plains biomen
                if (biomeMap[pos2] == 0)
                {
                    if (perlin > 0.05f)
                    {

                        //2% ish Chans
                        if (UnityEngine.Random.Range(1, 101) < 2)
                            Instantiate(obj, vertices[i] + new Vector3(UnityEngine.Random.Range(-0.8f, 0.8f), UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.8f, 0.8f)), obj.transform.rotation, objContainer.transform);
                    }
                }

                i++;
            }
        }


    }


    int GetWeightedResource(Biome b)
    {
        int maxChance = b.GetTotal();
        int rand = UnityEngine.Random.Range(0, maxChance);

        if (rand < b.ChanceForScrapMetal)
        {
            return 0;
        }
        else if(rand < b.ChanceForScrapMetal + b.ChanceForPlanks)
        {
            return 1;
        }
        else if (rand < b.ChanceForScrapMetal + b.ChanceForPlanks +b.ChanceForFabric)
        {
            return 2;
        }
        else if (rand < b.ChanceForScrapMetal + b.ChanceForPlanks + b.ChanceForFabric +b.ChanceForMagnets)
        {
            return 3;
        }
        else if (rand < b.ChanceForScrapMetal + b.ChanceForPlanks + b.ChanceForFabric + b.ChanceForMagnets + b.ChanceForElectronics)
        {
            return 4;
        }
        else if (rand < b.ChanceForScrapMetal + b.ChanceForPlanks + b.ChanceForFabric + b.ChanceForMagnets + b.ChanceForElectronics + b.ChanceForThermalCrystals)
        {
            return 5;
        }
        return -1;
    }

    private void PlaceResources()
    {
        if (resourceClusterSpawner == null)
        {
            Debug.LogError("resourceClusterSpawner on " + this.name + " is unnassigned, put the resourceclusterspawner-script on the terrain gameobject!");
            return;
        }

        Destroy(resourceContainer);
        resourceContainer = new GameObject("Resource Container");

        //ENSURE CRITICAL RESOURCES NEAR CENTRE
        ResourceType[] criticalResourcesToSpawn = new ResourceType[2];
        criticalResourcesToSpawn[0] = ResourceType.Wood;
        criticalResourcesToSpawn[1] = ResourceType.Wood;
        //criticalResourcesToSpawn[2] = ResourceType.Wood;
        resourceClusterSpawner.SpawnResourceClusterWithSpecificResource(criticalResourcesToSpawn, new Vector3(20, 20, 10), resourceContainer.transform);

        criticalResourcesToSpawn[0] = ResourceType.Scrap;
        criticalResourcesToSpawn[1] = ResourceType.Scrap;
        //criticalResourcesToSpawn[2] = ResourceType.Scrap;
        resourceClusterSpawner.SpawnResourceClusterWithSpecificResource(criticalResourcesToSpawn, new Vector3(-20, 20, -10), resourceContainer.transform);


        //GÅ IGENOM BIOMER
        foreach (Vector2Int pos in pointPositions)
        {
            //skapa array med de saker som spawnern ska spawna, och skicka med
            int amountOfResourceClustersToSpawn = UnityEngine.Random.Range(1, 3);
            ResourceType[] resourcesToSpawn = new ResourceType[amountOfResourceClustersToSpawn];


            int i = (xSize * pos.y) + pos.x;
            Vector3 vecPos = new Vector3(pos.x - xSize / 2, 0, pos.y - zSize / 2);
            for (int j = 0; j< amountOfResourceClustersToSpawn; j++)
            {
                int resource = -1;

                //kan simplifiera detta om jag gör enums av biomerna så jag inte behöver ha if, utan bara kan skicka med biomeMap[i] i GetWeightedResource()
                if (biomeMap[i] == 0)
                {
                    resource = GetWeightedResource(biomeSnowfields);
                }
                else if (biomeMap[i] == 1)
                {
                    resource = GetWeightedResource(biomeCrystalIcefields);
                }
                else if(biomeMap[i] == 2)
                {
                    resource = GetWeightedResource(biomeRockyHighlands);
                }

                if (resource == -1) //SENTINEL
                { continue; }

                resourcesToSpawn[j] = (ResourceType)resource;
            }
            resourceClusterSpawner.SpawnResourceClusterWithSpecificResource(resourcesToSpawn, vecPos, resourceContainer.transform);
        }
    }

    
}

