using Unity.Mathematics;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public bool GenerateNew;
    public int seed;

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    public int chunkSize;

    private int zSize;
    private int xSize;

    Vector2[] uvs;
    Color[] colors;
    public Gradient gradient;

    float maxTerrainHeight;
    float minTerrainHeight;

    [SerializeField]
    GameObject objToPlace;
    [SerializeField]
    GameObject objToPlace2;
    [SerializeField]
    GameObject objToPlace3;
    private GameObject objContainer;

    public bool PlaceStuff;
    public bool placeResources;
    [SerializeField]
    GameObject[] resources;


    int centerPoint;
    int houseSize = 6; //måste vara ett jämnt nummer?


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateNewMap();
    }

    public void CreateNewMap()
    {
        DestroyThings();

        if (chunkSize <= 0)
        {
            Debug.LogWarning("ChunkSize is too small on " + this.gameObject.name + " it needs to be larger than 0.");
            return;
        }
        zSize = xSize = chunkSize;
        centerPoint = chunkSize / 2;


        CreateMeshShape();




        CreateTriangles();
        CreateColors();
        UpdateMesh();
    }




    private int voronoiGridSize = 2;
    private int verticesPerCell;

    private Vector2Int[,] pointPositions;
    private int[] possibleBiomes = { 1, 2, 3 };
    private int[,] biomes;

    private int[] biomeMap;

    private void CreateBiomes()
    {
        biomeMap = new int[vertices.Length];

        //1. Seed choses how many biomes to use
        //4 är placeholder för vad som bör vara Biomes.length eller liknande
        int biomeCount = UnityEngine.Random.Range(0, 4);

        //nu skapa voronoi och på punkterna sätt ut ett nummer från 0 till biomeCount
        verticesPerCell = chunkSize / voronoiGridSize;

        if (verticesPerCell <= 0)
        {
            Debug.LogWarning("Too small map for biomes");
            return;
        }

        GeneratePoints();

        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
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
                        if (X < 0 || Y < 0 || X >= voronoiGridSize || Y >= voronoiGridSize) continue;

                        float distance = Vector2Int.Distance(new Vector2Int(i, j), pointPositions[X, Y]);
                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestPoint = new Vector2Int(X, Y);//pointPositions[X, Y];
                        }
                    }
                }

                //SÄTT TEXTUR STUFF!!!!!
                //det är viktigt att pos hittar rätt vertice här!!!!!!!
                int pos = i * xSize + j;

                if (pos >= 0 && pos < biomeMap.Length)
                    biomeMap[pos] = biomes[nearestPoint.x, nearestPoint.y];
                else
                    Debug.LogWarning("too large pos!!");
            }
        }

        //nu bestäm vilken biom som ska korrespondera till vilken int

    }

    private void GeneratePoints()
    {
        pointPositions = new Vector2Int[voronoiGridSize, voronoiGridSize];
        biomes = new int[voronoiGridSize, voronoiGridSize];
        for (int i = 0; i < voronoiGridSize; i++)
        {
            for (int j = 0; j < voronoiGridSize; j++)
            {
                //sätter ut punkterna
                pointPositions[i, j] = new Vector2Int(i * verticesPerCell + UnityEngine.Random.Range(0, verticesPerCell), j * verticesPerCell + UnityEngine.Random.Range(0, verticesPerCell));

                //och väljer vilken färg aka biome punkten ska ha
                biomes[i, j] = possibleBiomes[UnityEngine.Random.Range(0, possibleBiomes.Length)];
            }
        }
    }




    private void CreateMeshShape()
    {

        int length = 20;
        float scale = 25f; //höjden på noiset
        //offset för att hålla kartans centre i origo
        float offsetX = -xSize / 2;
        float offsetZ = -zSize / 2;



        //1. create seed
        UnityEngine.Random.InitState(seed);

        int newNoise = UnityEngine.Random.Range(0, 10000); //variabel som används just nu för att lägga offset på perlin noise så att det blir nytt med nytt seed

        //2. force scale att vara större än 0

        //3. create array med verts
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        //skapa biomes 
        CreateBiomes();

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                //set the height for each vert


                float2 pos = new float2(x + newNoise, z + newNoise);  //                    double value = noise(x / 32.0, y / 32.0);

                float value = noise.srnoise(pos / length * 10, seed);//Unity.Mathematics.noise.snoise(pos);
                value = value * 0.5f;// + 0.5f;
                float absValue = Mathf.Abs(value); //   
                absValue *= 5;

                if (absValue < 0.25f)
                {
                    //absValue += -2;
                }

                //float perlinSeed = noise.pnoise(new float2(x * .03f, z * .03f), seed) *25f;
                float perlin = Mathf.PerlinNoise((x + newNoise) * .03f, (z + newNoise) * .03f) * scale;  //där * .03f zoomar in på the noise, och * 25f gör hjöjdskilnaderna tydligare!!

                //för mer komplex noise lägger på stuff
                perlin += Mathf.PerlinNoise((x + newNoise) * .06f, (z + newNoise) * .06f) * (scale / 4);
                //perlin += Mathf.PerlinNoise((x + newNoise) * .5f, (z + newNoise) * .5f) * (scale / 64);



                //float simplex = noise.snoise(new float2(z * .03f, x * .03f)) * 5f;

                float simplex = noise.snoise(new float2(z * .03f, x * .03f));
                float simplexV = Mathf.Abs(simplex);
                if (simplexV > 0.35f)
                {
                    simplex *= 5f;
                }


                float y = perlin;


                if (y > maxTerrainHeight)
                    maxTerrainHeight = y;
                if (y < minTerrainHeight)
                    minTerrainHeight = y;

                /*
                //kolla biome och välj noise accordingly
                if (biomeMap[i] == 1)
                {
                    y = simplex + maxTerrainHeight/2;
                }
                else if(biomeMap[i] == 2)
                {
                    y = absValue + maxTerrainHeight / 2; 
                }
                */




                //kolla om det är i center zonen
                if (centerPoint >= x - houseSize && centerPoint <= x + houseSize && centerPoint >= z - houseSize && centerPoint <= z + houseSize)
                {
                    y = Mathf.PerlinNoise((centerPoint + newNoise) * .03f, (centerPoint + newNoise) * .03f) * scale;
                }



                //läger till offset för att hålla kartans mitt i origo
                vertices[i] = new Vector3(x + offsetX, y, z + offsetZ);
                i++;
            }
        }

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

                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);


                //för att testa biomes
                //colors[i] = gradient.Evaluate(Mathf.InverseLerp(0, possibleBiomes.Length, biomeMap[i]));

                //Debug.Log(biomeMap[i]);

                /*
                if (i+zSize < vertices.Length && Mathf.Abs(vertices[i].y - vertices[i+1].y) > .3f )
                {
                    colors[i] = Color.black;
                }
                else
                    colors[i] = gradient.Evaluate(height);
                */


                //test för slope
                //vert    vs    vert + xSize + 1;
                /*
                float ang = 0;
                if (i%3== 0 && i+2 < vertices.Length)
                {
                    Vector3 side1 = vertices[i + 1] - vertices[i];
                    Vector3 side2 = vertices[i + 2] - vertices[i];
                    Vector3 triangle = Vector3.Cross(side1, side2);//.normalized;


                    Plane plane = new Plane(vertices[i], vertices[i + 1], vertices[i + 2]);

                    Vector3 norm = plane.normal;


                    ang = Vector3.Angle(norm, Vector3.up);//Vector3.Angle(triangle, Vector3.down);
                    Debug.Log("Angle between ( " + triangle + " and " + Vector3.up + ") is angle: " + ang);
                }


               

                if (Mathf.Abs(ang) > 30f)
                    colors[i] = Color.black;
                else
                    colors[i] = gradient.Evaluate(height);

                */
                i++;
            }
        }


        Debug.Log("Vertices length: " + vertices.Length + " triangles length: " + triangles.Length);

        //trying for steepness
        /*for (int i = 0; i < triangles.Length; i+=3)
        {
            if (i+2 > triangles.Length)
                return;
            Vector3 tri = new Vector3(triangles[i + 0], triangles[i + 1], triangles[i + 2]);
            float ang = Vector3.Angle(Vector3.up, tri);
            Debug.Log("Angle between i: " + i + " and up is angle: " +  ang);

            if(ang > 40f && i+2 < colors.Length)
            {
                colors[i] = Color.green;
                colors[i+1] = Color.green;
                colors[i+2] = Color.green;
            }
        }*/

        /*
        int vert = 0;
        int tris = 0;

        //loop through rows
        for (int z = 0; z < zSize; z++) 
        {
            for (int x = 0; x < xSize; x++)
            {
                Vector3 tri = new Vector3(triangles[tris + 0], triangles[tris + 1], triangles[tris + 2]);
                float ang = Vector3.Angle(Vector3.up, tri.normalized);
                Debug.Log("Angle between i: " + z + " and up is angle: " + ang);

                if (ang > 45f)
                {
                    colors[vert + 0] = Color.green;
                    colors[vert + xSize + 1] = Color.green;
                    colors[vert + 1] = Color.green;
                }

                vert++;
                tris += 6;
            }
            vert++;
        }
        */

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

    private void DestroyThings()
    {
        //probably borde börja med att ta bort saker först?
        Destroy(objContainer);
    }

    private void PlaceThings()
    {
        DestroyThings();


        objContainer = new GameObject("Object Container"); //för att hålla det clean i inspectorn
        if (objToPlace == null)
        {
            Debug.LogWarning("The Object to place 'objToPlace' on " + this.gameObject.name + " is NULL! It needs a gameobject to have anything to place.");
            return;
        }

        //note: vid en storm map aka många objekt bör de probably delas in i mindre chunks, där bara vissa är aktiva åt gången?

        Vector3 origoOffset = new Vector3(-(xSize / 2), 0, -(xSize / 2));

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



                float perlin = Mathf.PerlinNoise(x * .3f, z * .3f);


                //olika saker beroende på biome   PLACHOLDER SKA SE FINARE UT!!!!!
                GameObject obj = objToPlace;
                if (biomeMap[i] == 2)
                {
                    obj = objToPlace2;

                    if (perlin > 0.85f)
                    {
                        Instantiate(obj, vertices[i] + new Vector3(UnityEngine.Random.Range(-0.8f, 0.8f), UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.8f, 0.8f)), transform.rotation, objContainer.transform);
                    }
                }
                else if (biomeMap[i] == 3)
                {
                    //obj = objToPlace3;

                    if (perlin > 0.55f)
                    {
                        Instantiate(obj, vertices[i] + new Vector3(UnityEngine.Random.Range(-0.8f, 0.8f), UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.8f, 0.8f)), transform.rotation, objContainer.transform);
                    }
                }
                else
                {
                    if (perlin > 0.75f)
                    {
                        Instantiate(obj, vertices[i] + new Vector3(UnityEngine.Random.Range(-0.8f, 0.8f), UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.8f, 0.8f)), transform.rotation, objContainer.transform);
                    }
                }


                i++;
            }
        }


    }

    private void PlaceResources(GameObject[] resources)
    {
        //det finns en liiten risk att resources hamnar på samma plats om dess x och z blir densamma som för en tidigare resource
        //och det här hanterar inte clipping eller något, så resources kan hamna halvt innuti ett berg ex


        foreach (GameObject obj in resources)
        {
            //skapa random position
            int x = UnityEngine.Random.Range(0, xSize);
            int z = UnityEngine.Random.Range(0, xSize);

            //vertices har antal platser som är zSize * xSize stor (detta är i)
            //för att hitta positionen vid x,z 's vertice
            //xSize * z + x
            int i = (xSize * z) + x;

            //bara för säkerhet
            if (i < 0 || i >= vertices.Length)
                return;
            if (obj == null)
            {
                Debug.LogWarning("A resource is NULL in the resources array on " + this.gameObject.name);
                continue;
            }
            Vector3 offset = new Vector3(0, 0, 0); //new Vector3(UnityEngine.Random.Range(-0.8f, 0.8f), UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.8f, 0.8f))
            Instantiate(obj, vertices[i] + offset, transform.rotation);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (GenerateNew)
        {
            CreateNewMap();
            GenerateNew = false;
        }

        if (PlaceStuff)
        {
            PlaceThings();
            PlaceStuff = false;
        }

        if (placeResources)
        {
            PlaceResources(resources);
            placeResources = false;
        }
    }
}

