using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Platformer.Core;
using Platformer.Data;
using Platformer.Game;
using UnityCliConnector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Platformer.Editor
{
    public static class RuntimeTestSceneSetup
    {
        #region 상수
        private const string SCENE_PATH = "Assets/_Project/Scenes/RuntimeTest.unity";
        private const string PLAYER_PREFAB_PATH = "Assets/_Project/Prefabs/Player.prefab";
        private const string RESPAWN_DATA_PATH = "Assets/_Project/Datas/RuntimeTestRespawnData.asset";
        private const string PLAYER_DIED_EVENT_PATH = "Assets/_Project/Datas/Events/OnRuntimeTestPlayerDied.asset";
        private const string TILESET_FOLDER = "Assets/_Project/Arts/Tilesets";
        private const string OBJECTS_SPRITE_SHEET_PATH = "Assets/Simple 2D Platformer BE2/Sprites/Objects.png";
        #endregion

        #region Public 메서드
        [MenuItem("Platformer/Tests/Build Runtime Test Scene")]
        public static void BuildScene()
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning("[RuntimeTestSceneSetup] 플레이 모드 종료 후 다시 실행하십시오.");
                return;
            }

            EnsureFolder("Assets/_Project/Scenes");
            EnsureFolder("Assets/_Project/Datas/Events");

            var visualAssets = RuntimeTestVisualAssets.LoadFromMainScene();
            var respawnData = LoadOrCreateAsset<RespawnData>(RESPAWN_DATA_PATH);
            respawnData.Reset();
            EditorUtility.SetDirty(respawnData);

            var onPlayerDied = LoadOrCreateAsset<GameEvent>(PLAYER_DIED_EVENT_PATH);
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "RuntimeTest";
            var tileSet = RuntimeTestTileSet.Load();

            var player = CreatePlayer(respawnData, onPlayerDied);
            CreateCamera(player.transform);
            CreateRuntimeController(player.transform, respawnData);
            CreateGlobalLight();
            CreateSectionOne(tileSet);
            CreateSectionTwo(tileSet, visualAssets);
            CreateSectionThree(respawnData, tileSet, visualAssets);

            EditorSceneManager.SaveScene(scene, SCENE_PATH);
            AddSceneToBuildSettings(SCENE_PATH);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[RuntimeTestSceneSetup] 테스트 씬 생성 완료: {SCENE_PATH}");
        }

        public static object DebugLoadTileSet()
        {
            var tileSet = RuntimeTestTileSet.Load();
            return new
            {
                top = AssetDatabase.GetAssetPath(tileSet.top),
                mid = AssetDatabase.GetAssetPath(tileSet.mid),
                bot = AssetDatabase.GetAssetPath(tileSet.bot),
                single = AssetDatabase.GetAssetPath(tileSet.single)
            };
        }
        #endregion

        #region Private 메서드
        private static GameObject CreatePlayer(RespawnData respawnData, GameEvent onPlayerDied)
        {
            GameObject player;
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PLAYER_PREFAB_PATH);
            if (prefab != null)
                player = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            else
                player = CreateFallbackPlayer();

            player.name = "Player_RuntimeTest";
            player.tag = "Player";
            player.transform.position = new Vector3(0f, 1f, 0f);

            var controller = player.GetComponent<PlayerController>();
            if (controller == null)
                controller = player.AddComponent<PlayerController>();

            var groundCheck = player.transform.Find("GroundCheck");
            if (groundCheck == null)
                groundCheck = player.transform.Find("Go_GroundCheck");
            if (groundCheck == null)
            {
                var groundCheckGo = new GameObject("Go_GroundCheck");
                groundCheckGo.transform.SetParent(player.transform, false);
                groundCheckGo.transform.localPosition = new Vector3(0f, -0.55f, 0f);
                groundCheck = groundCheckGo.transform;
            }

            var serializedObject = new SerializedObject(controller);
            serializedObject.FindProperty("_respawnData").objectReferenceValue = respawnData;
            serializedObject.FindProperty("_onPlayerDied").objectReferenceValue = onPlayerDied;
            serializedObject.FindProperty("_groundCheck").objectReferenceValue = groundCheck;
            serializedObject.FindProperty("_groundLayer").intValue = 1 << 0;
            serializedObject.FindProperty("_moveSpeed").floatValue = 5f;
            serializedObject.FindProperty("_jumpForce").floatValue = 9f;
            serializedObject.FindProperty("_fallDeathThreshold").floatValue = -100f;
            serializedObject.FindProperty("_deathReloadDelay").floatValue = 0.35f;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(controller);

            return player;
        }

        private static GameObject CreateFallbackPlayer()
        {
            var player = new GameObject("Player_RuntimeTest");
            var rb = player.AddComponent<Rigidbody2D>();
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

            var collider = player.AddComponent<CapsuleCollider2D>();
            collider.direction = CapsuleDirection2D.Vertical;
            collider.size = new Vector2(0.8f, 1.2f);

            var renderer = player.AddComponent<SpriteRenderer>();
            renderer.sprite = LoadFirstSprite("Assets/Simple 2D Platformer BE2/Sprites/Player.png");

            return player;
        }

        private static void CreateCamera(Transform target)
        {
            var cameraGo = new GameObject("Main Camera");
            cameraGo.tag = "MainCamera";
            cameraGo.transform.position = new Vector3(0f, 1.5f, -10f);

            var camera = cameraGo.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 6f;
            camera.backgroundColor = new Color(0.92f, 0.94f, 0.96f, 1f);

            var follow = cameraGo.AddComponent<CameraFollow>();
            var serializedObject = new SerializedObject(follow);
            serializedObject.FindProperty("_target").objectReferenceValue = target;
            serializedObject.FindProperty("_offset").vector3Value = new Vector3(3f, 1.5f, -10f);
            serializedObject.FindProperty("_smoothTime").floatValue = 0.12f;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void CreateGlobalLight()
        {
            var lightGo = new GameObject("Global Light 2D");
            var lightType = System.Type.GetType("UnityEngine.Rendering.Universal.Light2D, Unity.RenderPipelines.Universal.Runtime");
            if (lightType == null)
                return;

            var light = lightGo.AddComponent(lightType);
            var lightTypeProperty = lightType.GetProperty("lightType");
            if (lightTypeProperty != null)
            {
                var globalValue = System.Enum.Parse(lightTypeProperty.PropertyType, "Global");
                lightTypeProperty.SetValue(light, globalValue);
            }

            var intensityProperty = lightType.GetProperty("intensity");
            intensityProperty?.SetValue(light, 1f);
        }

        private static void CreateRuntimeController(Transform player, RespawnData respawnData)
        {
            var go = new GameObject("RuntimeTestSequenceController");
            var controller = go.AddComponent<RuntimeTestSequenceController>();

            var serializedObject = new SerializedObject(controller);
            serializedObject.FindProperty("_player").objectReferenceValue = player;
            serializedObject.FindProperty("_respawnData").objectReferenceValue = respawnData;
            SetVector2Array(serializedObject.FindProperty("_spawnPositions"), new[]
            {
                new Vector2(0f, 1f),
                new Vector2(0f, -9f),
                new Vector2(0f, -29f)
            });
            SetStringArray(serializedObject.FindProperty("_testNames"), new[]
            {
                "1번 이동 테스트",
                "2번 사망 테스트",
                "3번 세이브 테스트"
            });
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void CreateSectionOne(RuntimeTestTileSet tileSet)
        {
            CreateLabel("Txt_Test01", "0,0\n이동 테스트", new Vector2(-3.6f, 2.2f));
            var tilemap = CreateGroundTilemap("Tilemap_Test01_Ground");
            PaintSolidRect(tilemap, tileSet, new Vector3Int(-1, -1, 0), 10, 2);
            PaintArrow(tilemap, tileSet.single, new Vector3Int(2, 2, 0));
            CreateZone("Zone_Test01_Start", RuntimeTestZoneKind.Start, 0, new Vector2(0f, 1f), new Vector2(1.5f, 2f));
            CreateZone("Zone_Test01_Pass", RuntimeTestZoneKind.Pass, 0, new Vector2(7.2f, 1f), new Vector2(1f, 2f));
        }

        private static void CreateSectionTwo(RuntimeTestTileSet tileSet, RuntimeTestVisualAssets visualAssets)
        {
            CreateLabel("Txt_Test02", "0,-10\n2번 사망 테스트", new Vector2(-3.3f, -7.8f));
            var tilemap = CreateGroundTilemap("Tilemap_Test02_Ground");
            PaintSolidRect(tilemap, tileSet, new Vector3Int(-3, -17, 0), 6, 2);
            PaintSolidRect(tilemap, tileSet, new Vector3Int(-3, -16, 0), 1, 8);
            PaintSolidRect(tilemap, tileSet, new Vector3Int(2, -16, 0), 1, 8);

            for (var i = 0; i < 3; i++)
                CreateSpike($"Spike_Test02_{i + 1}", new Vector2(-1.5f + i * 1.5f, -15.95f), visualAssets);

            CreateZone("Zone_Test02_Damage", RuntimeTestZoneKind.Damage, 1, new Vector2(0f, -16.2f), new Vector2(5f, 1.5f));
        }

        private static void CreateSectionThree(RespawnData respawnData, RuntimeTestTileSet tileSet, RuntimeTestVisualAssets visualAssets)
        {
            CreateLabel("Txt_Test03", "0,-30\n3번 세이브 테스트", new Vector2(-3.9f, -27.7f));
            var tilemap = CreateGroundTilemap("Tilemap_Test03_Ground");
            PaintSolidRect(tilemap, tileSet, new Vector3Int(-4, -31, 0), 5, 2);
            PaintSolidRect(tilemap, tileSet, new Vector3Int(1, -33, 0), 8, 2);
            CreateFlag("Flag_Checkpoint", new Vector2(-2.4f, -29.55f), respawnData, visualAssets);

            for (var i = 0; i < 2; i++)
                CreateSpike($"Spike_Test03_{i + 1}", new Vector2(1.5f + i * 1.8f, -31.95f), visualAssets);
        }

        private static GameObject CreateZone(string name, RuntimeTestZoneKind kind, int testIndex, Vector2 position, Vector2 size)
        {
            var go = new GameObject(name);
            go.transform.position = position;
            var collider = go.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = size;

            var zone = go.AddComponent<RuntimeTestZone>();
            var controller = Object.FindFirstObjectByType<RuntimeTestSequenceController>();
            var serializedObject = new SerializedObject(zone);
            serializedObject.FindProperty("_controller").objectReferenceValue = controller;
            serializedObject.FindProperty("_kind").enumValueIndex = (int)kind;
            serializedObject.FindProperty("_testIndex").intValue = testIndex;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();

            return go;
        }

        private static Tilemap CreateGroundTilemap(string name)
        {
            var grid = GameObject.Find("Grid_RuntimeTest");
            if (grid == null)
                grid = new GameObject("Grid_RuntimeTest", typeof(Grid));

            var tilemapGo = new GameObject(name);
            tilemapGo.transform.SetParent(grid.transform, false);

            var tilemap = tilemapGo.AddComponent<Tilemap>();
            var renderer = tilemapGo.AddComponent<TilemapRenderer>();
            renderer.sortingOrder = 0;

            var rb = tilemapGo.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;

            var collider = tilemapGo.AddComponent<TilemapCollider2D>();
#pragma warning disable CS0618
            collider.usedByComposite = true;
#pragma warning restore CS0618
            tilemapGo.AddComponent<CompositeCollider2D>();

            return tilemap;
        }

        private static void PaintSolidRect(Tilemap tilemap, RuntimeTestTileSet tileSet, Vector3Int origin, int width, int height)
        {
            if (tilemap == null || tileSet == null)
                return;

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var isTop = y == height - 1;
                    var isBottom = y == 0;
                    var isLeft = x == 0;
                    var isRight = x == width - 1;
                    var tile = tileSet.GetSolidTile(isTop, isBottom, isLeft, isRight);
                    if (tile == null)
                        continue;

                    tilemap.SetTile(origin + new Vector3Int(x, y, 0), tile);
                }
            }

            tilemap.RefreshAllTiles();
            tilemap.CompressBounds();
            EditorUtility.SetDirty(tilemap);
        }

        private static void CreateFlag(string name, Vector2 position, RespawnData respawnData, RuntimeTestVisualAssets visualAssets)
        {
            var pole = new GameObject(name);
            pole.transform.position = position;
            var renderer = pole.AddComponent<SpriteRenderer>();
            renderer.sprite = visualAssets.checkpointSprite != null
                ? visualAssets.checkpointSprite
                : visualAssets.finishFlagSprite;
            renderer.sharedMaterial = visualAssets.spriteMaterial;
            renderer.sortingOrder = 5;

            var collider = pole.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.8f, 1.6f);

            var checkpoint = pole.AddComponent<Checkpoint>();
            var checkpointSo = new SerializedObject(checkpoint);
            checkpointSo.FindProperty("_respawnData").objectReferenceValue = respawnData;
            checkpointSo.ApplyModifiedPropertiesWithoutUndo();

            var zone = pole.AddComponent<RuntimeTestZone>();
            var zoneSo = new SerializedObject(zone);
            zoneSo.FindProperty("_controller").objectReferenceValue = Object.FindFirstObjectByType<RuntimeTestSequenceController>();
            zoneSo.FindProperty("_kind").enumValueIndex = (int)RuntimeTestZoneKind.Checkpoint;
            zoneSo.FindProperty("_testIndex").intValue = 2;
            zoneSo.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void PaintArrow(Tilemap tilemap, TileBase tile, Vector3Int origin)
        {
            if (tile == null)
                return;

            for (var x = 0; x < 4; x++)
                tilemap.SetTile(origin + new Vector3Int(x, 0, 0), tile);

            tilemap.SetTile(origin + new Vector3Int(4, 0, 0), tile);
            tilemap.SetTile(origin + new Vector3Int(3, 1, 0), tile);
            tilemap.SetTile(origin + new Vector3Int(3, -1, 0), tile);
            tilemap.SetTile(origin + new Vector3Int(2, 2, 0), tile);
            tilemap.SetTile(origin + new Vector3Int(2, -2, 0), tile);
            tilemap.RefreshAllTiles();
            tilemap.CompressBounds();
            EditorUtility.SetDirty(tilemap);
        }

        private static GameObject CreateSpike(string name, Vector2 position, RuntimeTestVisualAssets visualAssets)
        {
            var spike = new GameObject(name);
            spike.transform.position = position;

            var renderer = spike.AddComponent<SpriteRenderer>();
            renderer.sprite = visualAssets.spikeSprite != null
                ? visualAssets.spikeSprite
                : visualAssets.enemySprite;
            renderer.sharedMaterial = visualAssets.spriteMaterial;
            renderer.sortingOrder = 4;

            if (renderer.sprite == null)
                spike.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
            else
                spike.transform.localScale = Vector3.one;

            return spike;
        }

        private static void CreateLabel(string name, string text, Vector2 position)
        {
            var go = new GameObject(name);
            go.transform.position = position;
            var textMesh = go.AddComponent<TextMesh>();
            textMesh.text = text;
            textMesh.characterSize = 0.22f;
            textMesh.fontSize = 48;
            textMesh.anchor = TextAnchor.UpperLeft;
            textMesh.color = Color.black;
        }

        private static Material CreateMaterial(Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
                shader = Shader.Find("Sprites/Default");
            if (shader == null)
                shader = Shader.Find("Standard");

            var material = new Material(shader);
            material.color = color;
            return material;
        }

        private static Sprite LoadFirstSprite(string path)
        {
            var sprites = AssetDatabase.LoadAllAssetsAtPath(path)
                .OfType<Sprite>()
                .ToArray();
            return sprites.Length > 0 ? sprites[0] : AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static Sprite LoadSpriteByName(string path, params string[] nameParts)
        {
            var sprites = AssetDatabase.LoadAllAssetsAtPath(path)
                .OfType<Sprite>()
                .ToArray();

            foreach (var sprite in sprites)
            {
                var spriteName = sprite.name.ToLowerInvariant();
                if (nameParts.All(part => spriteName.Contains(part.ToLowerInvariant())))
                    return sprite;
            }

            return sprites.Length > 0 ? sprites[0] : AssetDatabase.LoadAssetAtPath<Sprite>(path);
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

        private static void AddSceneToBuildSettings(string scenePath)
        {
            var scenes = EditorBuildSettings.scenes;
            foreach (var scene in scenes)
            {
                if (scene.path == scenePath)
                    return;
            }

            var newScenes = new EditorBuildSettingsScene[scenes.Length + 1];
            scenes.CopyTo(newScenes, 0);
            newScenes[newScenes.Length - 1] = new EditorBuildSettingsScene(scenePath, true);
            EditorBuildSettings.scenes = newScenes;
        }

        private static void SetVector2Array(SerializedProperty property, Vector2[] values)
        {
            property.arraySize = values.Length;
            for (var i = 0; i < values.Length; i++)
                property.GetArrayElementAtIndex(i).vector2Value = values[i];
        }

        private static void SetStringArray(SerializedProperty property, string[] values)
        {
            property.arraySize = values.Length;
            for (var i = 0; i < values.Length; i++)
                property.GetArrayElementAtIndex(i).stringValue = values[i];
        }
        #endregion

        private sealed class RuntimeTestTileSet
        {
            public TileBase topLeft;
            public TileBase top;
            public TileBase topRight;
            public TileBase midLeft;
            public TileBase mid;
            public TileBase midRight;
            public TileBase botLeft;
            public TileBase bot;
            public TileBase botRight;
            public TileBase single;

            public static RuntimeTestTileSet Load()
            {
                var tileSet = new RuntimeTestTileSet
                {
                    topLeft = LoadTile("Tile_TopLeft"),
                    top = LoadTile("Tile_Top"),
                    topRight = LoadTile("Tile_TopRight"),
                    midLeft = LoadTile("Tile_MidLeft"),
                    mid = LoadTile("Tile_Mid"),
                    midRight = LoadTile("Tile_MidRight"),
                    botLeft = LoadTile("Tile_BotLeft"),
                    bot = LoadTile("Tile_Bot"),
                    botRight = LoadTile("Tile_BotRight"),
                    single = LoadTile("Tile_Single")
                };

                if (tileSet.top == null)
                    tileSet.top = tileSet.single;
                if (tileSet.mid == null)
                    tileSet.mid = tileSet.top;
                if (tileSet.bot == null)
                    tileSet.bot = tileSet.mid;
                if (tileSet.topLeft == null)
                    tileSet.topLeft = tileSet.top;
                if (tileSet.topRight == null)
                    tileSet.topRight = tileSet.top;
                if (tileSet.midLeft == null)
                    tileSet.midLeft = tileSet.mid;
                if (tileSet.midRight == null)
                    tileSet.midRight = tileSet.mid;
                if (tileSet.botLeft == null)
                    tileSet.botLeft = tileSet.bot;
                if (tileSet.botRight == null)
                    tileSet.botRight = tileSet.bot;

                return tileSet;
            }

            public TileBase GetSolidTile(bool isTop, bool isBottom, bool isLeft, bool isRight)
            {
                if (isTop && isBottom)
                {
                    if (isLeft && isRight)
                        return single;
                    if (isLeft)
                        return topLeft;
                    if (isRight)
                        return topRight;
                    return top;
                }

                if (isTop)
                {
                    if (isLeft)
                        return topLeft;
                    if (isRight)
                        return topRight;
                    return top;
                }

                if (isBottom)
                {
                    if (isLeft)
                        return botLeft;
                    if (isRight)
                        return botRight;
                    return bot;
                }

                if (isLeft)
                    return midLeft;
                if (isRight)
                    return midRight;
                return mid;
            }

            private static TileBase LoadTile(string assetName)
            {
                var path = $"{TILESET_FOLDER}/{assetName}.asset";
                var tile = AssetDatabase.LoadAssetAtPath<TileBase>(path);
                if (tile != null)
                    return tile;

                return AssetDatabase.LoadAllAssetsAtPath(path)
                    .OfType<TileBase>()
                    .FirstOrDefault();
            }
        }

        private sealed class RuntimeTestVisualAssets
        {
            public Sprite checkpointSprite;
            public Sprite finishFlagSprite;
            public Sprite spikeSprite;
            public Sprite enemySprite;
            public Material spriteMaterial;

            public static RuntimeTestVisualAssets LoadFromMainScene()
            {
                var assets = new RuntimeTestVisualAssets();
                foreach (var renderer in Object.FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                {
                    if (renderer.sprite == null)
                        continue;

                    if (renderer.name.Contains("Checkpoint") && assets.checkpointSprite == null)
                        assets.checkpointSprite = renderer.sprite;
                    else if (renderer.name.Contains("FinishFlag") && assets.finishFlagSprite == null)
                        assets.finishFlagSprite = renderer.sprite;
                    else if (renderer.name.Contains("Enemy") && assets.enemySprite == null)
                        assets.enemySprite = renderer.sprite;

                    if (assets.spriteMaterial == null && renderer.sharedMaterial != null)
                        assets.spriteMaterial = renderer.sharedMaterial;
                }

                if (assets.checkpointSprite == null)
                    assets.checkpointSprite = LoadSpriteByName(OBJECTS_SPRITE_SHEET_PATH, "checkpoint");
                if (assets.finishFlagSprite == null)
                    assets.finishFlagSprite = LoadSpriteByName(OBJECTS_SPRITE_SHEET_PATH, "flag");
                if (assets.spikeSprite == null)
                    assets.spikeSprite = LoadSpriteByName(OBJECTS_SPRITE_SHEET_PATH, "spike");
                if (assets.enemySprite == null)
                    assets.enemySprite = LoadFirstSprite("Assets/Simple 2D Platformer BE2/Sprites/Enemy.png");

                if (assets.spriteMaterial == null)
                {
                    var shader = Shader.Find("Sprites/Default");
                    if (shader != null)
                        assets.spriteMaterial = new Material(shader);
                }

                return assets;
            }
        }
    }

    [UnityCliTool(Name = "runtime_test_scene_setup", Description = "RuntimeTest 씬을 생성하고 런타임 순차 테스트 구역을 배치한다.")]
    public static class RuntimeTestSceneSetupTool
    {
        public static object HandleCommand(JObject parameters)
        {
            RuntimeTestSceneSetup.BuildScene();
            return new SuccessResponse("Runtime test scene setup completed.");
        }
    }

    [UnityCliTool(Name = "runtime_test_scene_inspect", Description = "RuntimeTest 씬의 미리 배치된 타일맵 타일 수와 사용 타일 에셋을 조회한다.")]
    public static class RuntimeTestSceneInspectTool
    {
        public static object HandleCommand(JObject parameters)
        {
            var results = Object.FindObjectsByType<Tilemap>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Select(tilemap =>
                {
                    var usedTiles = new System.Collections.Generic.List<TileBase>();
                    foreach (var position in tilemap.cellBounds.allPositionsWithin)
                    {
                        var tile = tilemap.GetTile(position);
                        if (tile != null)
                            usedTiles.Add(tile);
                    }

                    return new
                    {
                        name = tilemap.name,
                        tileCount = usedTiles.Count,
                        tileAssets = usedTiles
                            .Select(AssetDatabase.GetAssetPath)
                            .Where(path => !string.IsNullOrEmpty(path))
                            .Distinct()
                            .OrderBy(path => path)
                            .ToArray()
                    };
                })
                .ToArray();

            var tileProbePath = "Assets/_Project/Arts/Tilesets/Tile_Top.asset";
            var tileProbe = AssetDatabase.LoadAllAssetsAtPath(tileProbePath)
                .Select(asset => new { name = asset.name, type = asset.GetType().FullName })
                .ToArray();
            var tileSet = RuntimeTestSceneSetup.DebugLoadTileSet();

            return new SuccessResponse("Runtime test scene tilemaps inspected.", new
            {
                tileProbePath,
                tileProbe,
                tileSet,
                tilemaps = results
            });
        }
    }

    [UnityCliTool(Name = "runtime_test_playmode_run", Description = "RuntimeTest PlayMode 테스트를 실제 Test Runner API로 실행하고 결과를 반환한다.")]
    public static class RuntimeTestPlayModeRunTool
    {
        public static async Task<object> HandleCommand(JObject parameters)
        {
            var timeoutMs = parameters?["timeoutMs"]?.Value<int>() ?? 120000;
            var api = ScriptableObject.CreateInstance<TestRunnerApi>();
            var callbacks = new RuntimeTestRunCallbacks();
            api.RegisterCallbacks(callbacks);

            try
            {
                var filter = new Filter
                {
                    testMode = TestMode.PlayMode,
                    assemblyNames = new[] { "Platformer.Game.PlayModeTests" }
                };

                api.Execute(new ExecutionSettings(filter));
                var completedTask = await Task.WhenAny(callbacks.Completion.Task, Task.Delay(timeoutMs));
                if (completedTask != callbacks.Completion.Task)
                    return new ErrorResponse($"RuntimeTest PlayMode tests timed out after {timeoutMs}ms.");

                return callbacks.Completion.Task.Result;
            }
            finally
            {
                api.UnregisterCallbacks(callbacks);
                Object.DestroyImmediate(api);
            }
        }

        private sealed class RuntimeTestRunCallbacks : ICallbacks
        {
            public readonly TaskCompletionSource<object> Completion = new TaskCompletionSource<object>();
            private readonly System.Collections.Generic.List<object> _finishedTests = new System.Collections.Generic.List<object>();

            public void RunStarted(ITestAdaptor testsToRun)
            {
            }

            public void RunFinished(ITestResultAdaptor result)
            {
                var data = new
                {
                    result = result.ResultState,
                    passed = result.PassCount,
                    failed = result.FailCount,
                    skipped = result.SkipCount,
                    inconclusive = result.InconclusiveCount,
                    duration = result.Duration,
                    tests = _finishedTests.ToArray()
                };

                if (result.FailCount > 0)
                    Completion.TrySetResult(new ErrorResponse("RuntimeTest PlayMode tests failed.", data));
                else
                    Completion.TrySetResult(new SuccessResponse("RuntimeTest PlayMode tests passed.", data));
            }

            public void TestStarted(ITestAdaptor test)
            {
            }

            public void TestFinished(ITestResultAdaptor result)
            {
                if (result.Test == null || result.Test.HasChildren)
                    return;

                _finishedTests.Add(new
                {
                    name = result.FullName,
                    state = result.ResultState,
                    duration = result.Duration,
                    message = result.Message
                });
            }
        }
    }
}
