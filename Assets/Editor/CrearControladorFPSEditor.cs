using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CrearControladorFPSEditor
{
    [MenuItem("Herramientas/Crear controlador FPS propio")]
    private static void CrearControlador()
    {
        if (EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog("No se puede crear", "Detén Play Mode antes de crear el controlador.", "Aceptar");
            return;
        }

        bool confirmar = EditorUtility.DisplayDialog(
            "Crear controlador FPS",
            "Se creará un jugador en primera persona con CharacterController y una cámara propia. " +
            "También se eliminará la cámara principal independiente de la escena para evitar cámaras duplicadas.\n\n" +
            "Guarda la escena antes de continuar.",
            "Crear",
            "Cancelar");

        if (!confirmar) return;

        // Retira la cámara predeterminada independiente, si existe.
        Camera camaraPrincipal = Camera.main;
        if (camaraPrincipal != null && camaraPrincipal.transform.parent == null)
            Undo.DestroyObjectImmediate(camaraPrincipal.gameObject);
        else
        {
            GameObject camaraVieja = GameObject.Find("Main Camera");
            if (camaraVieja != null && camaraVieja.transform.parent == null)
                Undo.DestroyObjectImmediate(camaraVieja);
        }

        GameObject jugador = new GameObject("FPSController_Propio");
        Undo.RegisterCreatedObjectUndo(jugador, "Crear controlador FPS");
        jugador.tag = "Player";
        jugador.transform.position = ObtenerPosicionInicial();

        CharacterController cc = Undo.AddComponent<CharacterController>(jugador);
        cc.height = 1.8f;
        cc.radius = 0.3f;
        cc.center = new Vector3(0f, 0.9f, 0f);
        cc.stepOffset = 0.3f;
        cc.slopeLimit = 45f;

        GameObject camaraObjeto = new GameObject("PlayerCamera");
        Undo.RegisterCreatedObjectUndo(camaraObjeto, "Crear cámara FPS");
        camaraObjeto.transform.SetParent(jugador.transform, false);
        camaraObjeto.transform.localPosition = new Vector3(0f, 1.6f, 0f);
        camaraObjeto.transform.localRotation = Quaternion.identity;
        camaraObjeto.tag = "MainCamera";

        Camera camara = Undo.AddComponent<Camera>(camaraObjeto);
        camara.fieldOfView = 70f;
        camara.nearClipPlane = 0.05f;
        Undo.AddComponent<AudioListener>(camaraObjeto);

        ControladorFPS controlador = Undo.AddComponent<ControladorFPS>(jugador);
        controlador.ConfigurarCamara(camara);

        Selection.activeGameObject = jugador;
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorUtility.DisplayDialog(
            "Controlador creado",
            "Se creó FPSController_Propio.\n\nPulsa Play para probar. Usa W, A, S y D para caminar, el mouse para mirar y Escape para liberar el cursor.",
            "Aceptar");
    }

    private static Vector3 ObtenerPosicionInicial()
    {
        Terrain terreno = Terrain.activeTerrain;
        if (terreno == null)
            return new Vector3(0f, 1f, 0f);

        Vector3 origen = terreno.transform.position;
        Vector3 tamano = terreno.terrainData.size;
        float x = origen.x + Mathf.Clamp(tamano.x * 0.08f, 5f, 12f);
        float z = origen.z + Mathf.Clamp(tamano.z * 0.08f, 5f, 12f);
        Vector3 punto = new Vector3(x, 0f, z);
        float y = terreno.SampleHeight(punto) + origen.y + 0.1f;
        return new Vector3(x, y, z);
    }
}
