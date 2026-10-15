using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SetupGame : EditorWindow
{
    [MenuItem("Tools/Setup Game/1 - Setup UI Contador en SampleScene")]
    static void SetupUIContador()
    {
        var scene = SceneManager.GetActiveScene();
        if (scene.name != "SampleScene")
        {
            EditorUtility.DisplayDialog("Error", "Abre SampleScene primero.", "OK");
            return;
        }

        // Buscar si ya existe un Canvas
        Canvas existingCanvas = Object.FindFirstObjectByType<Canvas>();
        GameObject canvasGO;
        if (existingCanvas != null)
        {
            canvasGO = existingCanvas.gameObject;
        }
        else
        {
            canvasGO = new GameObject("Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        // Configurar CanvasScaler
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        // Crear EventSystem si no existe
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        // Crear Image (Marcador)
        var marcadorGO = new GameObject("Marcador", typeof(RectTransform));
        marcadorGO.transform.SetParent(canvasGO.transform, false);
        var marcadorImg = marcadorGO.AddComponent<Image>();
        var rt = marcadorGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(20, -20);
        rt.sizeDelta = new Vector2(60, 60);

        // Buscar sprite de la abeja
        string[] beeGuids = AssetDatabase.FindAssets("Fly-Sheet t:Sprite", new[] { "Assets/Sprites" });
        if (beeGuids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(beeGuids[0]);
            Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(path);
            foreach (var s in sprites)
            {
                if (s is Sprite sprite)
                {
                    marcadorImg.sprite = sprite;
                    break;
                }
            }
        }

        // Crear TextMeshPro
        var textoGO = new GameObject("TxTAbejas", typeof(RectTransform));
        textoGO.transform.SetParent(canvasGO.transform, false);
        var tmpText = textoGO.AddComponent<TextMeshProUGUI>();
        tmpText.text = "0";
        tmpText.fontSize = 36;
        tmpText.alignment = TextAlignmentOptions.Left;
        tmpText.color = Color.white;
        var rtText = textoGO.GetComponent<RectTransform>();
        rtText.anchorMin = new Vector2(0, 1);
        rtText.anchorMax = new Vector2(0, 1);
        rtText.pivot = new Vector2(0, 1);
        rtText.anchoredPosition = new Vector2(90, -25);
        rtText.sizeDelta = new Vector2(100, 50);

        // Asignar TMP_Text al Jugador
        var jugador = Object.FindFirstObjectByType<Jugador>();
        if (jugador != null)
        {
            jugador.textoAbejas = tmpText;
            EditorUtility.SetDirty(jugador);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("UI Contador de abejas creado correctamente.");
    }

    [MenuItem("Tools/Setup Game/1b - Setup Audio en SampleScene")]
    static void SetupAudio()
    {
        var scene = SceneManager.GetActiveScene();
        if (scene.name != "SampleScene")
        {
            EditorUtility.DisplayDialog("Error", "Abre SampleScene primero.", "OK");
            return;
        }

        // Crear objeto de música ambiental
        var musicGO = new GameObject("MusicaAmbiental");
        var musicAS = musicGO.AddComponent<AudioSource>();
        var musicClip = AssetDatabase.LoadAssetAtPath<AudioClip>(
            "Assets/Audio/Minifantasy_Dungeon_Music/Minifantasy_Dungeon_Music/Music/Goblins_Den_(Regular).wav");
        if (musicClip != null)
        {
            musicAS.clip = musicClip;
            musicAS.loop = true;
            musicAS.playOnAwake = true;
            musicAS.volume = 0.3f;
        }

        // Asignar AudioClips al Jugador
        var jugador = Object.FindFirstObjectByType<Jugador>();
        if (jugador != null)
        {
            jugador.audioSource = jugador.GetComponent<AudioSource>();
            if (jugador.audioSource == null)
                jugador.audioSource = jugador.gameObject.AddComponent<AudioSource>();
            jugador.audioSource.playOnAwake = false;

            var abeja = AssetDatabase.LoadAssetAtPath<AudioClip>(
                "Assets/Audio/Minifantasy_Dungeon_SFX/01_chest_open_1.wav");
            var puerquito = AssetDatabase.LoadAssetAtPath<AudioClip>(
                "Assets/Audio/Minifantasy_Dungeon_SFX/11_human_damage_1.wav");
            var caracol = AssetDatabase.LoadAssetAtPath<AudioClip>(
                "Assets/Audio/Minifantasy_Dungeon_SFX/26_sword_hit_1.wav");
            if (abeja != null) jugador.audioAbeja = abeja;
            if (puerquito != null) jugador.audioPuerquito = puerquito;
            if (caracol != null) jugador.audioCaracol = caracol;
            EditorUtility.SetDirty(jugador);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("Audio configurado: música ambiental + efectos del jugador.");
    }

    [MenuItem("Tools/Setup Game/2 - Crear Zona de Muerte")]
    static void CrearZonaDeMuerte()
    {
        var scene = SceneManager.GetActiveScene();
        if (scene.name != "SampleScene")
        {
            EditorUtility.DisplayDialog("Error", "Abre SampleScene primero.", "OK");
            return;
        }

        var zonaMuerte = new GameObject("ZonaDeMuerte");
        zonaMuerte.transform.position = new Vector3(0, -5, 0);
        zonaMuerte.tag = "puerquito";
        var boxCol = zonaMuerte.AddComponent<BoxCollider2D>();
        boxCol.isTrigger = true;
        boxCol.size = new Vector2(50, 1);

        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("Zona de muerte creada en Y=-5.");
    }

    [MenuItem("Tools/Setup Game/3 - Crear Escena MenuPrincipal")]
    static void CrearMenuPrincipal()
    {
        // Crear nueva escena
        var newScene = EditorSceneManager.NewScene(
            NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // Configurar cámara
        var cam = Object.FindFirstObjectByType<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5;
        cam.backgroundColor = new Color(0.15f, 0.15f, 0.3f);

        // Agregar script OpcionesMenu a la cámara
        cam.gameObject.AddComponent<OpcionesMenu>();

        // Crear Canvas
        var canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        // EventSystem
        var esGO = new GameObject("EventSystem");
        esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
        esGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        // --- Panel de fondo ---
        var panelBG = new GameObject("PanelFondo", typeof(RectTransform));
        panelBG.transform.SetParent(canvasGO.transform, false);
        var bgImg = panelBG.AddComponent<Image>();
        bgImg.color = new Color(0.1f, 0.1f, 0.25f, 0.8f);
        var bgRT = panelBG.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;

        // --- Título ---
        var tituloGO = new GameObject("Titulo", typeof(RectTransform));
        tituloGO.transform.SetParent(canvasGO.transform, false);
        var tituloTMP = tituloGO.AddComponent<TextMeshProUGUI>();
        tituloTMP.text = "Mi Juego 2D";
        tituloTMP.fontSize = 72;
        tituloTMP.alignment = TextAlignmentOptions.Center;
        tituloTMP.color = Color.white;
        var tituloRT = tituloGO.GetComponent<RectTransform>();
        tituloRT.anchorMin = new Vector2(0.5f, 0.8f);
        tituloRT.anchorMax = new Vector2(0.5f, 0.8f);
        tituloRT.pivot = new Vector2(0.5f, 0.5f);
        tituloRT.anchoredPosition = Vector2.zero;
        tituloRT.sizeDelta = new Vector2(800, 100);

        // Cargar sprites de botones
        string btnBasePath = "Assets/Sprites/Botones/UI_Button_Pack_Minimal_84_PNGs_By_Csmikelandre/UI_Button_Pack_Minimal_84_PNGs_By_Csmikelandre/BOTTOMS_PIXEL/";
        var playSprite = AssetDatabase.LoadAssetAtPath<Sprite>(btnBasePath + "PLAY_BOTTOM_PIXEL_1.png");
        var optionsSprite = AssetDatabase.LoadAssetAtPath<Sprite>(btnBasePath + "OPTIONS_BOTTOM_PIXEL_1.png");
        var exitSprite = AssetDatabase.LoadAssetAtPath<Sprite>(btnBasePath + "EXIT_BOTTOM_PIXEL_1.png");

        // --- Botón Jugar ---
        var btnJugar = CreateMenuButton(canvasGO.transform, "BtnJugar", "", new Vector2(0, 50), playSprite);
        var btnJugarComp = btnJugar.GetComponent<Button>();
        var camRef = cam;
        UnityEditor.Events.UnityEventTools.AddPersistentListener(
            btnJugarComp.onClick,
            camRef.GetComponent<OpcionesMenu>().vamosAjugar);

        // --- Botón Opciones ---
        var btnOpciones = CreateMenuButton(canvasGO.transform, "BtnOpciones", "", new Vector2(0, -50), optionsSprite);

        // --- Botón Salir ---
        var btnSalir = CreateMenuButton(canvasGO.transform, "BtnSalir", "", new Vector2(0, -150), exitSprite);
        var btnSalirComp = btnSalir.GetComponent<Button>();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(
            btnSalirComp.onClick,
            camRef.GetComponent<OpcionesMenu>().salirDelJuego);

        // --- Panel de Opciones (desactivado) ---
        var panelOpciones = new GameObject("PanelDeOpciones", typeof(RectTransform));
        panelOpciones.transform.SetParent(canvasGO.transform, false);
        var panelImg = panelOpciones.AddComponent<Image>();
        panelImg.color = new Color(0.2f, 0.2f, 0.4f, 0.95f);
        var panelRT = panelOpciones.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0.2f, 0.2f);
        panelRT.anchorMax = new Vector2(0.8f, 0.8f);
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;

        // Texto de opciones
        var opTitulo = new GameObject("TituloOpciones", typeof(RectTransform));
        opTitulo.transform.SetParent(panelOpciones.transform, false);
        var opTMP = opTitulo.AddComponent<TextMeshProUGUI>();
        opTMP.text = "Opciones";
        opTMP.fontSize = 48;
        opTMP.alignment = TextAlignmentOptions.Center;
        opTMP.color = Color.white;
        var opRT = opTitulo.GetComponent<RectTransform>();
        opRT.anchorMin = new Vector2(0.5f, 0.85f);
        opRT.anchorMax = new Vector2(0.5f, 0.85f);
        opRT.pivot = new Vector2(0.5f, 0.5f);
        opRT.anchoredPosition = Vector2.zero;
        opRT.sizeDelta = new Vector2(400, 60);

        // Botón cerrar opciones
        var btnCerrar = CreateMenuButton(panelOpciones.transform, "BtnCerrar", "Cerrar", new Vector2(0, -100));
        var btnCerrarComp = btnCerrar.GetComponent<Button>();
        UnityEditor.Events.UnityEventTools.AddBoolPersistentListener(
            btnCerrarComp.onClick,
            new UnityEngine.Events.UnityAction<bool>(panelOpciones.SetActive), false);

        // Botón opciones abre el panel
        var btnOpcionesComp = btnOpciones.GetComponent<Button>();
        UnityEditor.Events.UnityEventTools.AddBoolPersistentListener(
            btnOpcionesComp.onClick,
            new UnityEngine.Events.UnityAction<bool>(panelOpciones.SetActive), true);

        // Desactivar panel de opciones
        panelOpciones.SetActive(false);

        // Guardar escena
        string scenePath = "Assets/Scenes/MenuPrincipal.unity";
        EditorSceneManager.SaveScene(newScene, scenePath);

        // Agregar ambas escenas al Build Settings
        var scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene(scenePath, true),
            new EditorBuildSettingsScene("Assets/Scenes/SampleScene.unity", true)
        };
        EditorBuildSettings.scenes = scenes;

        Debug.Log("Escena MenuPrincipal creada y configurada en Build Settings (índice 0).");
    }

    static GameObject CreateMenuButton(Transform parent, string name, string label, Vector2 pos, Sprite sprite = null)
    {
        var btnGO = new GameObject(name, typeof(RectTransform));
        btnGO.transform.SetParent(parent, false);
        var img = btnGO.AddComponent<Image>();
        if (sprite != null)
        {
            img.sprite = sprite;
            img.type = Image.Type.Simple;
            img.preserveAspect = true;
            img.color = Color.white;
        }
        else
        {
            img.color = new Color(0.3f, 0.3f, 0.6f, 1f);
        }
        var btn = btnGO.AddComponent<Button>();
        btn.targetGraphic = img;
        var btnRT = btnGO.GetComponent<RectTransform>();
        btnRT.anchorMin = new Vector2(0.5f, 0.5f);
        btnRT.anchorMax = new Vector2(0.5f, 0.5f);
        btnRT.pivot = new Vector2(0.5f, 0.5f);
        btnRT.anchoredPosition = pos;
        btnRT.sizeDelta = new Vector2(300, 80);

        var textGO = new GameObject("Text", typeof(RectTransform));
        textGO.transform.SetParent(btnGO.transform, false);
        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 32;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        var textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;

        return btnGO;
    }
}
