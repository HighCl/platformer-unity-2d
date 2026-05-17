using System;
using System.IO;
using System.Linq;
using Platformer.Core;
using Platformer.Data;
using Platformer.UI;
using Newtonsoft.Json.Linq;
using UnityCliConnector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Platformer.Editor
{
    [InitializeOnLoad]
    public static class PlayerHealthUiSceneSetup
    {
        private const string SESSION_KEY = "Platformer.PlayerHealthUiSceneSetup.Done";
        private const string MAIN_SCENE_PATH = "Assets/_Project/Scenes/Main.unity";
        private const string PLAYER_PREFAB_PATH = "Assets/_Project/Prefabs/Player.prefab";
        private const string SETTINGS_PATH = "Assets/_Project/Datas/PlayerHealthSettings_Default.asset";
        private const string HEALTH_EVENT_PATH = "Assets/_Project/Datas/Events/OnPlayerHealthChanged.asset";
        private const string INVINCIBILITY_EVENT_PATH = "Assets/_Project/Datas/Events/OnPlayerInvincibilityChanged.asset";
        private const string DIED_EVENT_PATH = "Assets/_Project/Datas/Events/OnPlayerDied.asset";
        private const string INVINCIBLE_ICON_PATH = "Assets/_Project/Arts/Sprites/UI/InvincibleIcon_SquareBorder.png";
        private const string SCENE_UI_PREFAB_PATH = "Assets/_Project/Prefabs/UI/SceneUI.prefab";
        private const string INVINCIBILITY_UI_PREFAB_PATH = "Assets/_Project/Prefabs/UI/InvincibilityUI.prefab";
        private const string DONE_MARKER_PATH = "Temp/PlayerHealthUiSceneSetup.done";

        static PlayerHealthUiSceneSetup()
        {
            EditorApplication.delayCall += Apply;
        }

        public static void Apply() => Apply(false);

        public static void Apply(bool force)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.Log("[PlayerHealthUiSceneSetup] 플레이 모드에서는 UI/Player 직렬화 연결을 수정하지 않습니다.");
                return;
            }

            if (!force && SessionState.GetBool(SESSION_KEY, false))
                return;

            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += Apply;
                return;
            }

            SessionState.SetBool(SESSION_KEY, true);

            var settings = LoadOrCreateAsset<PlayerHealthSettings>(SETTINGS_PATH);
            settings.maxHealth = 3;
            settings.invincibleDuration = 0.75f;
            settings.blinkInterval = 0.1f;
            EditorUtility.SetDirty(settings);

            var onHealthChanged = LoadOrCreateAsset<PlayerHealthEvent>(HEALTH_EVENT_PATH);
            var onInvincibilityChanged = LoadOrCreateAsset<PlayerInvincibilityEvent>(INVINCIBILITY_EVENT_PATH);
            var onPlayerDied = LoadOrCreateAsset<GameEvent>(DIED_EVENT_PATH);
            var invincibleIconSprite = CreateOrLoadInvincibleIconSprite();
            var sceneUiPrefab = CreateOrUpdateSceneUiPrefab(onHealthChanged, onPlayerDied);
            var invincibilityUiPrefab = CreateOrUpdateInvincibilityUiPrefab(invincibleIconSprite, onInvincibilityChanged);

            AssignPlayerPrefab(settings, onHealthChanged, onInvincibilityChanged, onPlayerDied);
            BuildSceneHud(settings, onHealthChanged, onInvincibilityChanged, onPlayerDied, sceneUiPrefab, invincibilityUiPrefab);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Directory.CreateDirectory("Temp");
            File.WriteAllText(DONE_MARKER_PATH, DateTime.Now.ToString("O"));

            Debug.Log("[PlayerHealthUiSceneSetup] Player health UI scene setup completed.");
        }

        private static T LoadOrCreateAsset<T>(string path) where T : ScriptableObject
        {
            EnsureFolder(Path.GetDirectoryName(path)?.Replace("\\", "/"));

            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
                return asset;

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void EnsureFolder(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath) || AssetDatabase.IsValidFolder(folderPath))
                return;

            var parent = Path.GetDirectoryName(folderPath)?.Replace("\\", "/");
            var folderName = Path.GetFileName(folderPath);
            EnsureFolder(parent);

            if (!string.IsNullOrEmpty(parent) && !string.IsNullOrEmpty(folderName) && !AssetDatabase.IsValidFolder(folderPath))
                AssetDatabase.CreateFolder(parent, folderName);
        }

        private static void AssignPlayerPrefab(
            PlayerHealthSettings settings,
            PlayerHealthEvent onHealthChanged,
            PlayerInvincibilityEvent onInvincibilityChanged,
            GameEvent onPlayerDied)
        {
            if (!File.Exists(PLAYER_PREFAB_PATH))
                return;

            var prefabRoot = PrefabUtility.LoadPrefabContents(PLAYER_PREFAB_PATH);
            try
            {
                var player = prefabRoot.GetComponentInChildren<PlayerController>(true);
                if (player == null)
                    return;

                AssignPlayerFields(player, settings, onHealthChanged, onInvincibilityChanged, onPlayerDied);
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, PLAYER_PREFAB_PATH);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(prefabRoot);
            }
        }

        private static void BuildSceneHud(
            PlayerHealthSettings settings,
            PlayerHealthEvent onHealthChanged,
            PlayerInvincibilityEvent onInvincibilityChanged,
            GameEvent onPlayerDied,
            GameObject sceneUiPrefab,
            GameObject invincibilityUiPrefab)
        {
            var scene = EditorSceneManager.OpenScene(MAIN_SCENE_PATH, OpenSceneMode.Single);

            foreach (var player in UnityEngine.Object.FindObjectsByType<PlayerController>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                AssignPlayerFields(player, settings, onHealthChanged, onInvincibilityChanged, onPlayerDied);

            RemoveExistingSceneUi();

            var sceneUiRoot = PrefabUtility.InstantiatePrefab(sceneUiPrefab) as GameObject;
            if (sceneUiRoot == null)
                throw new InvalidOperationException("Scene UI prefab instance creation failed.");

            sceneUiRoot.name = "Canvas_SceneUI";
            var hudGroup = sceneUiRoot.transform.Find("GROUP_HUD");
            if (hudGroup == null)
                throw new InvalidOperationException("Scene UI prefab is missing GROUP_HUD.");

            var invincibilityUiRoot = PrefabUtility.InstantiatePrefab(invincibilityUiPrefab, hudGroup) as GameObject;
            if (invincibilityUiRoot == null)
                throw new InvalidOperationException("Invincibility UI prefab instance creation failed.");

            invincibilityUiRoot.name = "Img_InvincibleIcon";

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static Canvas EnsureCanvas()
        {
            var canvas = UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .FirstOrDefault(x => x.name == "Canvas")
                ?? UnityEngine.Object.FindFirstObjectByType<Canvas>(FindObjectsInactive.Include);

            if (canvas != null)
                return canvas;

            var go = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            return canvas;
        }

        private static T EnsureChild<T>(Transform parent, string objectName) where T : Component
        {
            var child = parent.Find(objectName);
            if (child == null)
            {
                var go = new GameObject(objectName, typeof(RectTransform));
                var rectTransform = go.GetComponent<RectTransform>();
                rectTransform.SetParent(parent, false);
                child = rectTransform;
            }

            var component = child.GetComponent<T>();
            if (component == null)
                component = child.gameObject.AddComponent<T>();

            return component;
        }

        private static void SetFullStretch(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }

        private static GameObject CreateOrUpdateSceneUiPrefab(
            PlayerHealthEvent onHealthChanged,
            GameEvent onPlayerDied)
        {
            EnsureFolder(Path.GetDirectoryName(SCENE_UI_PREFAB_PATH)?.Replace("\\", "/"));

            var root = new GameObject("Canvas_SceneUI", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            try
            {
                ConfigureSceneCanvas(root);

                var hudGroup = EnsureChild<RectTransform>(root.transform, "GROUP_HUD");
                SetFullStretch(hudGroup);

                var gauge = EnsureChild<RectTransform>(hudGroup, "Pan_HealthGauge");
                gauge.anchorMin = new Vector2(0f, 1f);
                gauge.anchorMax = new Vector2(0f, 1f);
                gauge.pivot = new Vector2(0f, 1f);
                gauge.anchoredPosition = new Vector2(24f, -24f);
                gauge.sizeDelta = new Vector2(180f, 20f);

                var back = EnsureChild<Image>(gauge, "Img_HealthGaugeBack");
                SetFullStretch(back.rectTransform);
                back.color = new Color(0.45f, 0.45f, 0.45f, 1f);
                back.raycastTarget = false;

                var fill = EnsureChild<Image>(gauge, "Img_HealthGaugeFill");
                SetFullStretch(fill.rectTransform);
                fill.color = new Color(0.9f, 0.15f, 0.12f, 1f);
                fill.type = Image.Type.Filled;
                fill.fillMethod = Image.FillMethod.Horizontal;
                fill.fillOrigin = (int)Image.OriginHorizontal.Left;
                fill.fillAmount = 1f;
                fill.raycastTarget = false;

                var slider = gauge.GetComponent<Slider>();
                if (slider == null)
                    slider = gauge.gameObject.AddComponent<Slider>();

                ConfigureHealthSlider(slider, fill.rectTransform);

                var hud = hudGroup.GetComponent<HUDController>();
                if (hud == null)
                    hud = hudGroup.gameObject.AddComponent<HUDController>();

                AssignHudFields(hud, slider, fill, onHealthChanged, onPlayerDied);

                PrefabUtility.SaveAsPrefabAsset(root, SCENE_UI_PREFAB_PATH);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }

            return AssetDatabase.LoadAssetAtPath<GameObject>(SCENE_UI_PREFAB_PATH);
        }

        private static GameObject CreateOrUpdateInvincibilityUiPrefab(
            Sprite invincibleIconSprite,
            PlayerInvincibilityEvent onInvincibilityChanged)
        {
            EnsureFolder(Path.GetDirectoryName(INVINCIBILITY_UI_PREFAB_PATH)?.Replace("\\", "/"));

            var root = new GameObject("Img_InvincibleIcon", typeof(RectTransform), typeof(Image));
            try
            {
                var iconImg = root.GetComponent<Image>();
                iconImg.rectTransform.anchorMin = new Vector2(0f, 1f);
                iconImg.rectTransform.anchorMax = new Vector2(0f, 1f);
                iconImg.rectTransform.pivot = new Vector2(0f, 1f);
                iconImg.rectTransform.anchoredPosition = new Vector2(216f, -20f);
                iconImg.rectTransform.sizeDelta = new Vector2(32f, 32f);
                iconImg.rectTransform.localRotation = Quaternion.identity;
                iconImg.sprite = invincibleIconSprite;
                iconImg.type = Image.Type.Simple;
                iconImg.preserveAspect = true;
                iconImg.color = new Color(0.2f, 0.85f, 1f, 0.25f);
                iconImg.raycastTarget = false;

                var iconView = root.GetComponent<InvincibilityIconView>();
                if (iconView == null)
                    iconView = root.AddComponent<InvincibilityIconView>();

                AssignInvincibilityIconFields(iconView, iconImg, onInvincibilityChanged);

                PrefabUtility.SaveAsPrefabAsset(root, INVINCIBILITY_UI_PREFAB_PATH);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }

            return AssetDatabase.LoadAssetAtPath<GameObject>(INVINCIBILITY_UI_PREFAB_PATH);
        }

        private static void ConfigureSceneCanvas(GameObject root)
        {
            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
        }

        private static void RemoveExistingSceneUi()
        {
            var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var rootObject in rootObjects)
            {
                if (!rootObject.TryGetComponent<Canvas>(out _))
                    continue;

                if (rootObject.name != "Canvas" && rootObject.name != "Canvas_SceneUI")
                    continue;

                if (rootObject.transform.Find("GROUP_HUD") == null)
                    continue;

                UnityEngine.Object.DestroyImmediate(rootObject);
            }
        }

        private static Sprite CreateOrLoadInvincibleIconSprite()
        {
            EnsureFolder(Path.GetDirectoryName(INVINCIBLE_ICON_PATH)?.Replace("\\", "/"));

            if (!File.Exists(INVINCIBLE_ICON_PATH))
            {
                const int size = 32;
                const int borderSize = 4;
                var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
                var pixels = new Color32[size * size];

                for (var y = 0; y < size; y++)
                {
                    for (var x = 0; x < size; x++)
                    {
                        var isBorder = x < borderSize
                            || x >= size - borderSize
                            || y < borderSize
                            || y >= size - borderSize;

                        pixels[y * size + x] = isBorder
                            ? new Color32(0, 0, 0, 255)
                            : new Color32(255, 255, 255, 255);
                    }
                }

                texture.SetPixels32(pixels);
                texture.Apply();
                File.WriteAllBytes(INVINCIBLE_ICON_PATH, texture.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(texture);
            }

            AssetDatabase.ImportAsset(INVINCIBLE_ICON_PATH, ImportAssetOptions.ForceUpdate);
            var importer = AssetImporter.GetAtPath(INVINCIBLE_ICON_PATH) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = 32f;
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(INVINCIBLE_ICON_PATH);
        }

        private static void AssignPlayerFields(
            PlayerController player,
            PlayerHealthSettings settings,
            PlayerHealthEvent onHealthChanged,
            PlayerInvincibilityEvent onInvincibilityChanged,
            GameEvent onPlayerDied)
        {
            var serializedObject = new SerializedObject(player);
            serializedObject.Update();
            SetObjectReference(serializedObject, "_healthSettings", settings);
            SetObjectReference(serializedObject, "_onHealthChanged", onHealthChanged);
            SetObjectReference(serializedObject, "_onInvincibilityChanged", onInvincibilityChanged);
            SetObjectReference(serializedObject, "_onPlayerDied", onPlayerDied);
            SetObjectReference(serializedObject, "_spriteRenderer", player.GetComponentInChildren<SpriteRenderer>(true));
            serializedObject.ApplyModifiedProperties();

            // 프리팹 인스턴스 오버라이드가 남아 있어도 런타임 필드가 같은 이벤트 채널을 보도록 보강한다.
            SetPrivateField(player, "_healthSettings", settings);
            SetPrivateField(player, "_onHealthChanged", onHealthChanged);
            SetPrivateField(player, "_onInvincibilityChanged", onInvincibilityChanged);
            SetPrivateField(player, "_onPlayerDied", onPlayerDied);
            SetPrivateField(player, "_spriteRenderer", player.GetComponentInChildren<SpriteRenderer>(true));
            EditorUtility.SetDirty(player);
        }

        private static void AssignHudFields(
            HUDController hud,
            Slider slider,
            Image fill,
            PlayerHealthEvent onHealthChanged,
            GameEvent onPlayerDied)
        {
            var serializedObject = new SerializedObject(hud);
            SetObjectReference(serializedObject, "_onPlayerDied", onPlayerDied);
            SetObjectReference(serializedObject, "_onHealthChanged", onHealthChanged);
            SetObjectReference(serializedObject, "_healthGaugeSlider", slider);
            SetObjectReference(serializedObject, "_healthGaugeFillImg", fill);
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(hud);
        }

        private static void AssignInvincibilityIconFields(
            InvincibilityIconView iconView,
            Image iconImg,
            PlayerInvincibilityEvent onInvincibilityChanged)
        {
            var serializedObject = new SerializedObject(iconView);
            SetObjectReference(serializedObject, "_onInvincibilityChanged", onInvincibilityChanged);
            SetObjectReference(serializedObject, "_iconImg", iconImg);
            SetColor(serializedObject, "_inactiveColor", new Color(0.2f, 0.85f, 1f, 0.25f));
            SetColor(serializedObject, "_activeColor", new Color(0.2f, 0.85f, 1f, 1f));
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(iconView);
        }

        private static void SetObjectReference(SerializedObject serializedObject, string propertyName, UnityEngine.Object value)
        {
            var property = serializedObject.FindProperty(propertyName);
            if (property != null)
                property.objectReferenceValue = value;
        }

        private static void SetColor(SerializedObject serializedObject, string propertyName, Color value)
        {
            var property = serializedObject.FindProperty(propertyName);
            if (property != null)
                property.colorValue = value;
        }

        private static void ConfigureHealthSlider(Slider slider, RectTransform fillRect)
        {
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 1f;
            slider.wholeNumbers = false;
            slider.direction = Slider.Direction.LeftToRight;
            slider.fillRect = fillRect;
            slider.handleRect = null;
            slider.targetGraphic = null;
            slider.interactable = false;
            slider.transition = Selectable.Transition.None;
            slider.navigation = new Navigation { mode = Navigation.Mode.None };
        }

        private static void SetPrivateField<TValue>(PlayerController player, string fieldName, TValue value)
        {
            var field = typeof(PlayerController).GetField(
                fieldName,
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            if (field != null)
                field.SetValue(player, value);
        }
    }

    [UnityCliTool(Name = "player_health_ui_scene_setup", Description = "플레이어 체력 HUD UI를 Main 씬과 Player 프리팹에 배치하고 연결한다.")]
    public static class PlayerHealthUiSceneSetupTool
    {
        public static object HandleCommand(JObject parameters)
        {
            PlayerHealthUiSceneSetup.Apply(true);
            return new SuccessResponse("Player health UI scene setup completed.");
        }
    }

    [UnityCliTool(Name = "player_health_ui_runtime_probe", Description = "플레이 모드에서 플레이어 피격 후 HUD/무적 UI 반영 상태를 진단한다.")]
    public static class PlayerHealthUiRuntimeProbeTool
    {
        public static object HandleCommand(JObject parameters)
        {
            if (!EditorApplication.isPlaying)
                return new ErrorResponse("Runtime probe requires play mode.");

            var damageAmount = parameters?["amount"]?.Value<int>() ?? 1;
            var player = UnityEngine.Object.FindFirstObjectByType<PlayerController>(FindObjectsInactive.Include);
            var hud = UnityEngine.Object.FindFirstObjectByType<HUDController>(FindObjectsInactive.Include);
            var iconView = UnityEngine.Object.FindFirstObjectByType<InvincibilityIconView>(FindObjectsInactive.Include);

            if (player == null)
                return new ErrorResponse("PlayerController not found.");

            player.TakeDamage(damageAmount);

            return new SuccessResponse("Player health UI runtime probe completed.", new
            {
                player.CurrentHealth,
                player.MaxHealth,
                player.IsInvincible,
                HealthFillAmount = hud != null ? hud.CurrentHealthFillAmount : -1f,
                IconIsInvincible = iconView != null && iconView.IsInvincible,
                IconAlpha = iconView != null ? iconView.CurrentAlpha : -1f
            });
        }
    }
}
