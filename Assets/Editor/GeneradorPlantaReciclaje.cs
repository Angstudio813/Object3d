#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GeneradorPlantaReciclaje
{
    private const string RootName = "PlantaReciclaje_Autogenerada";
    private const int HeightmapResolution = 129;

    [MenuItem("Herramientas/Generar Planta de Reciclaje (automatico)")]
    public static void Generate()
    {
        if (!EditorUtility.DisplayDialog(
                "Generar planta de reciclaje",
                "Se creara una planta sencilla en la escena actual, nueve materiales y varios prefabs. " +
                "Se reutilizara el Terrain existente si lo hay. Guarda una copia de tu escena antes de continuar.",
                "Generar", "Cancelar"))
            return;

        EnsureFolder("Assets", "Generated");
        EnsureFolder("Assets", "Materials");
        EnsureFolder("Assets", "Prefabs");
        EnsureFolder("Assets", "Textures");
        EnsureFolder("Assets", "Scenes");

        GameObject oldRoot = GameObject.Find(RootName);
        if (oldRoot != null)
            Undo.DestroyObjectImmediate(oldRoot);

        Material[] mats = CreateMaterials();
        Terrain terrain = ConfigureTerrain();
        GameObject root = new GameObject(RootName);

        BuildRoads(root.transform, mats);
        BuildFactory(root.transform, mats);
        BuildConveyor(root.transform, mats);
        BuildBinsAndPrefabs(root.transform, mats);
        BuildStorageAndOffice(root.transform, mats);
        BuildDecorations(root.transform, mats);
        ConfigureCameraAndLight();

        if (terrain != null)
            terrain.Flush();

        Scene scene = SceneManager.GetActiveScene();
        string scenePath = string.IsNullOrEmpty(scene.path)
            ? "Assets/Scenes/EscenarioReciclaje_Generado.unity"
            : scene.path;
        EditorSceneManager.SaveScene(scene, scenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeGameObject = root;
        EditorUtility.DisplayDialog(
            "Listo",
            "La planta base se genero. Revisa la vista Scene y la carpeta Assets/Materials y Assets/Prefabs.\n\n" +
            "Pendiente: importar House.fbx y Car.fbx exigidos por la consigna, configurar FPS y preparar el proyecto VR.",
            "Aceptar");
    }

    private static void EnsureFolder(string parent, string folder)
    {
        string path = parent + "/" + folder;
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder(parent, folder);
    }

    private static Material[] CreateMaterials()
    {
        string[] names =
        {
            "MAT_Verde", "MAT_Azul", "MAT_Rojo", "MAT_Amarillo", "MAT_Naranja",
            "MAT_Blanco", "MAT_Gris", "MAT_Marron", "MAT_Negro"
        };
        Color[] colors =
        {
            new Color(0.16f, 0.48f, 0.23f),
            new Color(0.10f, 0.34f, 0.78f),
            new Color(0.78f, 0.12f, 0.10f),
            new Color(0.95f, 0.72f, 0.08f),
            new Color(0.95f, 0.36f, 0.06f),
            new Color(0.90f, 0.90f, 0.86f),
            new Color(0.38f, 0.41f, 0.44f),
            new Color(0.36f, 0.20f, 0.10f),
            new Color(0.08f, 0.09f, 0.10f)
        };

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        if (shader == null) shader = Shader.Find("Unlit/Color");

        Material[] result = new Material[names.Length];
        for (int i = 0; i < names.Length; i++)
        {
            string path = "Assets/Materials/" + names[i] + ".mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, path);
            }
            SetMaterialColor(material, colors[i]);
            EditorUtility.SetDirty(material);
            result[i] = material;
        }
        return result;
    }

    private static void SetMaterialColor(Material material, Color color)
    {
        if (material == null) return;
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Color")) material.SetColor("_Color", color);
        material.color = color;
    }

    private static Terrain ConfigureTerrain()
    {
        Terrain terrain = Object.FindFirstObjectByType<Terrain>();
        if (terrain == null)
        {
            TerrainData newData = CreateTerrainData();
            GameObject terrainObject = Terrain.CreateTerrainGameObject(newData);
            terrainObject.name = "Terrain";
            terrain = terrainObject.GetComponent<Terrain>();
        }
        else
        {
            TerrainData newData = CreateTerrainData();
            terrain.terrainData = newData;
            terrain.transform.position = Vector3.zero;
            terrain.name = "Terrain";
        }

        terrain.drawInstanced = true;
        terrain.heightmapPixelError = 8;
        terrain.basemapDistance = 500;
        return terrain;
    }

    private static TerrainData CreateTerrainData()
    {
        string dataPath = "Assets/Generated/PlantaTerrainData.asset";
        string texturePath = "Assets/Textures/Texture_TerrenoVerde.asset";
        string layerPath = "Assets/Generated/TerrainLayer_Planta.terrainlayer";

        if (AssetDatabase.LoadAssetAtPath<TerrainData>(dataPath) != null)
            AssetDatabase.DeleteAsset(dataPath);
        if (AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath) != null)
            AssetDatabase.DeleteAsset(texturePath);
        if (AssetDatabase.LoadAssetAtPath<TerrainLayer>(layerPath) != null)
            AssetDatabase.DeleteAsset(layerPath);

        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        texture.name = "Texture_TerrenoVerde";
        Color grass = new Color(0.30f, 0.48f, 0.22f, 1f);
        texture.SetPixels(new[] { grass, grass, grass, grass });
        texture.Apply();
        texture.wrapMode = TextureWrapMode.Repeat;
        AssetDatabase.CreateAsset(texture, texturePath);

        TerrainLayer layer = new TerrainLayer();
        layer.diffuseTexture = texture;
        layer.tileSize = new Vector2(18f, 18f);
        AssetDatabase.CreateAsset(layer, layerPath);

        TerrainData data = new TerrainData();
        data.heightmapResolution = HeightmapResolution;
        data.size = new Vector3(120f, 18f, 120f);
        float[,] heights = new float[HeightmapResolution, HeightmapResolution];

        for (int z = 0; z < HeightmapResolution; z++)
        {
            for (int x = 0; x < HeightmapResolution; x++)
            {
                float nx = (float)x / (HeightmapResolution - 1);
                float nz = (float)z / (HeightmapResolution - 1);
                // Elevaciones muy suaves en las esquinas; centro practicamente plano.
                float h1 = Gaussian(nx, nz, 0.12f, 0.14f, 0.13f) * 0.025f;
                float h2 = Gaussian(nx, nz, 0.88f, 0.15f, 0.13f) * 0.022f;
                float h3 = Gaussian(nx, nz, 0.15f, 0.87f, 0.14f) * 0.022f;
                float h4 = Gaussian(nx, nz, 0.88f, 0.87f, 0.14f) * 0.025f;
                heights[z, x] = h1 + h2 + h3 + h4;
            }
        }
        data.SetHeights(0, 0, heights);
        data.terrainLayers = new[] { layer };
        AssetDatabase.CreateAsset(data, dataPath);
        return data;
    }

    private static float Gaussian(float x, float z, float cx, float cz, float spread)
    {
        float dx = x - cx;
        float dz = z - cz;
        return Mathf.Exp(-(dx * dx + dz * dz) / (2f * spread * spread));
    }

    private static GameObject CreateBox(string name, Transform parent, Vector3 localPosition,
        Vector3 localScale, Material material)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = localPosition;
        obj.transform.localScale = localScale;
        if (material != null)
            obj.GetComponent<Renderer>().sharedMaterial = material;
        return obj;
    }

    private static void BuildRoads(Transform root, Material[] m)
    {
        CreateBox("Camino_Principal", root, new Vector3(60f, 0.12f, 18f),
            new Vector3(100f, 0.2f, 9f), m[6]);
        CreateBox("Camino_Acceso", root, new Vector3(60f, 0.13f, 42f),
            new Vector3(9f, 0.22f, 42f), m[6]);
        CreateBox("Linea_Central_Camino", root, new Vector3(60f, 0.235f, 18f),
            new Vector3(90f, 0.025f, 0.22f), m[3]);
    }

    private static void BuildFactory(Transform root, Material[] m)
    {
        Transform factory = new GameObject("Edificio_Principal_Reciclaje").transform;
        factory.SetParent(root, false);
        factory.localPosition = new Vector3(60f, 0f, 84f);
        CreateBox("Paredes", factory, new Vector3(0f, 4f, 0f), new Vector3(26f, 8f, 20f), m[5]);
        CreateBox("Techo", factory, new Vector3(0f, 8.2f, 0f), new Vector3(27f, 0.6f, 21f), m[6]);
        CreateBox("Porton", factory, new Vector3(0f, 2.3f, -10.12f), new Vector3(8f, 4.6f, 0.25f), m[8]);
        CreateBox("Ventana_Azul", factory, new Vector3(-8f, 5.4f, -10.15f), new Vector3(4f, 2f, 0.2f), m[1]);
        CreateBox("Ventana_Azul_2", factory, new Vector3(8f, 5.4f, -10.15f), new Vector3(4f, 2f, 0.2f), m[1]);
        CreateBox("Franja_Naranja", factory, new Vector3(0f, 7.2f, -10.16f), new Vector3(25f, 0.45f, 0.25f), m[4]);
        CreateBox("Chimenea", factory, new Vector3(9f, 10f, 5f), new Vector3(2f, 4f, 2f), m[6]);
    }

    private static void BuildConveyor(Transform root, Material[] m)
    {
        Transform belt = new GameObject("Cinta_Transportadora").transform;
        belt.SetParent(root, false);
        belt.localPosition = new Vector3(60f, 0f, 66f);
        CreateBox("Base_Cinta", belt, new Vector3(0f, 1.1f, 0f), new Vector3(17f, 0.6f, 2.8f), m[8]);
        CreateBox("Superficie_Cinta", belt, new Vector3(0f, 1.45f, 0f), new Vector3(16.5f, 0.12f, 2.5f), m[6]);
        for (int i = 0; i < 5; i++)
            CreateBox("Residuo_Demo_" + (i + 1), belt,
                new Vector3(-6.5f + i * 3.1f, 2.1f, (i % 2 == 0) ? -0.45f : 0.45f),
                new Vector3(1.1f, 0.9f, 0.9f), m[i % 4]);
        for (int side = -1; side <= 1; side += 2)
            for (int x = -1; x <= 1; x += 2)
                CreateBox("Soporte_Cinta", belt, new Vector3(x * 7f, 0.45f, side * 1.1f),
                    new Vector3(0.5f, 0.9f, 0.5f), m[6]);
    }

    private static void BuildBinsAndPrefabs(Transform root, Material[] m)
    {
        string[] names = { "Contenedor_Plasticos", "Contenedor_Papel", "Contenedor_Metal", "Contenedor_Vidrio" };
        Material[] colors = { m[1], m[3], m[4], m[2] };
        for (int i = 0; i < names.Length; i++)
        {
            GameObject bin = new GameObject(names[i]);
            bin.transform.SetParent(root, false);
            bin.transform.localPosition = new Vector3(43f + i * 11f, 0f, 51f);
            CreateBox("Cuerpo", bin.transform, new Vector3(0f, 1.5f, 0f),
                new Vector3(6f, 3f, 5f), colors[i]);
            CreateBox("Borde_Superior", bin.transform, new Vector3(0f, 3.1f, 0f),
                new Vector3(6.3f, 0.25f, 5.3f), m[8]);
            string safeName = "Prefab_" + names[i];
            PrefabUtility.SaveAsPrefabAsset(bin, "Assets/Prefabs/" + safeName + ".prefab");
        }
    }

    private static void BuildStorageAndOffice(Transform root, Material[] m)
    {
        Transform storage = new GameObject("Almacenamiento").transform;
        storage.SetParent(root, false);
        storage.localPosition = new Vector3(91f, 0f, 82f);
        CreateBox("Almacen", storage, Vector3.zero, new Vector3(18f, 6f, 14f), m[4]);
        CreateBox("Techo_Almacen", storage, new Vector3(0f, 3.2f, 0f), new Vector3(19f, 0.5f, 15f), m[8]);

        Transform office = new GameObject("Oficina").transform;
        office.SetParent(root, false);
        office.localPosition = new Vector3(28f, 0f, 82f);
        CreateBox("Edificio_Oficina", office, new Vector3(0f, 2.5f, 0f), new Vector3(12f, 5f, 10f), m[5]);
        CreateBox("Techo_Oficina", office, new Vector3(0f, 5.2f, 0f), new Vector3(13f, 0.5f, 11f), m[1]);
        CreateBox("Puerta_Oficina", office, new Vector3(0f, 1.2f, -5.1f), new Vector3(2f, 2.4f, 0.2f), m[7]);
    }

    private static void BuildDecorations(Transform root, Material[] m)
    {
        // Marcadores de estacionamiento y una representacion simple de vehiculo.
        CreateBox("Vehiculo_Demo_Carroceria", root, new Vector3(88f, 1.1f, 25f),
            new Vector3(7f, 2f, 3.4f), m[2]);
        CreateBox("Vehiculo_Demo_Cabina", root, new Vector3(87f, 2.3f, 25f),
            new Vector3(3.5f, 1.1f, 3f), m[5]);
        for (int i = 0; i < 3; i++)
        {
            CreateBox("Pila_Cajas_" + i, root, new Vector3(100f, 0.8f + i * 1.3f, 75f),
                new Vector3(3f, 1.5f, 3f), m[7]);
        }
        // Arboles sencillos en los limites del escenario.
        Vector3[] treePositions =
        {
            new Vector3(12f, 0f, 20f), new Vector3(15f, 0f, 100f),
            new Vector3(108f, 0f, 22f), new Vector3(108f, 0f, 105f)
        };
        for (int i = 0; i < treePositions.Length; i++)
        {
            Transform tree = new GameObject("Arbol_Decorativo_" + (i + 1)).transform;
            tree.SetParent(root, false);
            tree.localPosition = treePositions[i];
            CreateBox("Tronco", tree, new Vector3(0f, 1.5f, 0f), new Vector3(0.8f, 3f, 0.8f), m[7]);
            GameObject crown = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            crown.name = "Copa";
            crown.transform.SetParent(tree, false);
            crown.transform.localPosition = new Vector3(0f, 4f, 0f);
            crown.transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);
            crown.GetComponent<Renderer>().sharedMaterial = m[0];
        }
    }

    private static void ConfigureCameraAndLight()
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            camera = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
        }
        Vector3 cameraPosition = new Vector3(60f, 34f, -12f);
        Vector3 target = new Vector3(60f, 0f, 63f);
        camera.transform.position = cameraPosition;
        camera.transform.rotation = Quaternion.LookRotation(target - cameraPosition);
        camera.fieldOfView = 60f;
        camera.farClipPlane = 300f;

        Light directional = Object.FindFirstObjectByType<Light>();
        if (directional == null)
        {
            GameObject lightObject = new GameObject("Directional Light");
            directional = lightObject.AddComponent<Light>();
            directional.type = LightType.Directional;
        }
        directional.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        directional.intensity = 1.1f;
    }
}
#endif
