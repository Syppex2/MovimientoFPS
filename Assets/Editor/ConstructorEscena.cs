using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

// Arma la escena de prueba con un clic: menú superior "Proyecto > Crear escena de prueba".
// Crea el suelo, una rampa, obstáculos, la luz, el Player con su CharacterController
// y la cámara en primera persona, con los scripts ya asignados.
public static class ConstructorEscena
{
    private const string CarpetaEscenas = "Assets/Scenes";
    private const string CarpetaMateriales = "Assets/Materiales";
    private const string RutaEscena = CarpetaEscenas + "/EscenaPrincipal.unity";

    [MenuItem("Proyecto/Crear escena de prueba")]
    public static void CrearEscena()
    {
        // Pregunta si hay que guardar la escena abierta antes de reemplazarla
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(RutaEscena) != null &&
            !EditorUtility.DisplayDialog("Crear escena de prueba",
                "Ya existe " + RutaEscena + ". ¿Quieres reemplazarla?", "Reemplazar", "Cancelar"))
            return;

        CrearCarpeta(CarpetaEscenas);
        CrearCarpeta(CarpetaMateriales);

        Scene escena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        Material matSuelo = CrearMaterial("Suelo", new Color(0.35f, 0.55f, 0.35f));
        Material matObstaculo = CrearMaterial("Obstaculo", new Color(0.75f, 0.55f, 0.30f));
        Material matJugador = CrearMaterial("Jugador", new Color(0.25f, 0.45f, 0.85f));
        Material matVisor = CrearMaterial("Visor", new Color(0.10f, 0.10f, 0.10f));

        // ---------- Mundo: agrupa el terreno, la luz y la decoración ----------
        GameObject mundo = new GameObject("Mundo");

        GameObject luz = new GameObject("Luz Direccional");
        Light componenteLuz = luz.AddComponent<Light>();
        componenteLuz.type = LightType.Directional;
        componenteLuz.intensity = 1f;
        componenteLuz.shadows = LightShadows.Soft;
        luz.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        luz.transform.SetParent(mundo.transform);

        GameObject suelo = GameObject.CreatePrimitive(PrimitiveType.Plane);
        suelo.name = "Suelo";
        suelo.transform.position = Vector3.zero;
        suelo.transform.localScale = new Vector3(5f, 1f, 5f); // 50 x 50 metros
        AsignarMaterial(suelo, matSuelo);
        suelo.transform.SetParent(mundo.transform);

        // Rampa para comprobar que el jugador sube pendientes y la gravedad lo baja
        CrearCubo("Rampa", new Vector3(0f, 0.9f, 10f), new Vector3(4f, 0.3f, 6f),
                  Quaternion.Euler(-17f, 0f, 0f), matObstaculo, mundo.transform);
        // Plataforma al final de la rampa
        CrearCubo("Plataforma", new Vector3(0f, 0.9f, 16f), new Vector3(6f, 1.8f, 6f),
                  Quaternion.identity, matObstaculo, mundo.transform);
        // Obstáculos para probar las colisiones
        CrearCubo("Obstaculo 1", new Vector3(-6f, 1f, 4f), new Vector3(2f, 2f, 2f),
                  Quaternion.identity, matObstaculo, mundo.transform);
        CrearCubo("Obstaculo 2", new Vector3(6f, 1.5f, 6f), new Vector3(2f, 3f, 2f),
                  Quaternion.identity, matObstaculo, mundo.transform);
        CrearCubo("Obstaculo 3", new Vector3(8f, 0.5f, -5f), new Vector3(3f, 1f, 3f),
                  Quaternion.Euler(0f, 30f, 0f), matObstaculo, mundo.transform);

        // ---------- Player: fuera de "Mundo" para mantener la jerarquía clara ----------
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.transform.position = new Vector3(0f, 1.1f, 0f); // Un poco sobre el suelo
        AsignarMaterial(player, matJugador);

        // El CharacterController reemplaza al CapsuleCollider que trae la cápsula
        Object.DestroyImmediate(player.GetComponent<CapsuleCollider>());
        CharacterController controlador = player.AddComponent<CharacterController>();
        controlador.height = 2f;
        controlador.radius = 0.5f;
        controlador.center = Vector3.zero;

        // Visor: cubo oscuro que muestra hacia dónde mira el jugador en la vista de escena
        GameObject visor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visor.name = "Visor";
        Object.DestroyImmediate(visor.GetComponent<BoxCollider>()); // Sin collider para no chocar con el propio jugador
        visor.transform.SetParent(player.transform, false);
        visor.transform.localPosition = new Vector3(0f, 0.35f, 0.45f);
        visor.transform.localScale = new Vector3(0.7f, 0.15f, 0.15f);
        AsignarMaterial(visor, matVisor);

        // Cámara a la altura de la cabeza, hija del Player
        GameObject camara = new GameObject("Main Camera");
        camara.tag = "MainCamera";
        Camera componenteCamara = camara.AddComponent<Camera>();
        componenteCamara.nearClipPlane = 0.1f;
        componenteCamara.fieldOfView = 70f;
        camara.AddComponent<AudioListener>();
        camara.transform.SetParent(player.transform, false);
        camara.transform.localPosition = new Vector3(0f, 0.7f, 0f);
        camara.transform.localRotation = Quaternion.identity;

        // Scripts
        PlayerMovimiento movimiento = player.AddComponent<PlayerMovimiento>();
        SerializedObject datosMovimiento = new SerializedObject(movimiento);
        datosMovimiento.FindProperty("camara").objectReferenceValue = camara.transform;
        datosMovimiento.ApplyModifiedPropertiesWithoutUndo();

        CamaraScript scriptCamara = camara.AddComponent<CamaraScript>();
        scriptCamara.Player = player.transform;

        // Guardar la escena y agregarla a la lista de escenas del build
        EditorSceneManager.SaveScene(escena, RutaEscena);
        AgregarEscenaAlBuild(RutaEscena);
        AssetDatabase.SaveAssets();

        Selection.activeGameObject = player;
        Debug.Log("Escena creada en " + RutaEscena + ". Presiona Play y muévete con WASD y el mouse. Escape libera el cursor.");
    }

    private static void CrearCubo(string nombre, Vector3 posicion, Vector3 escala, Quaternion rotacion,
                                  Material material, Transform padre)
    {
        GameObject cubo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubo.name = nombre;
        cubo.transform.position = posicion;
        cubo.transform.rotation = rotacion;
        cubo.transform.localScale = escala;
        AsignarMaterial(cubo, material);
        cubo.transform.SetParent(padre, true);
    }

    // Crea un material que funcione tanto en proyectos 3D clásicos (Built-in) como en URP
    private static Material CrearMaterial(string nombre, Color color)
    {
        string ruta = CarpetaMateriales + "/" + nombre + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(ruta);

        if (material == null)
        {
            Shader shader = null;
            if (GraphicsSettings.currentRenderPipeline != null)
                shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
                shader = Shader.Find("Standard");

            material = new Material(shader);
            AssetDatabase.CreateAsset(material, ruta);
        }

        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Color")) material.SetColor("_Color", color);
        EditorUtility.SetDirty(material);
        return material;
    }

    private static void AsignarMaterial(GameObject objeto, Material material)
    {
        Renderer renderer = objeto.GetComponent<Renderer>();
        if (renderer != null && material != null && material.shader != null)
            renderer.sharedMaterial = material;
    }

    private static void CrearCarpeta(string ruta)
    {
        if (AssetDatabase.IsValidFolder(ruta)) return;
        int ultimaBarra = ruta.LastIndexOf('/');
        AssetDatabase.CreateFolder(ruta.Substring(0, ultimaBarra), ruta.Substring(ultimaBarra + 1));
    }

    private static void AgregarEscenaAlBuild(string ruta)
    {
        foreach (EditorBuildSettingsScene existente in EditorBuildSettings.scenes)
            if (existente.path == ruta) return;

        var lista = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        lista.Insert(0, new EditorBuildSettingsScene(ruta, true));
        EditorBuildSettings.scenes = lista.ToArray();
    }
}
