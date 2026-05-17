using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;

namespace Platformer.Game.PlayModeTests
{
    public class RuntimeTestScenePlayModeTests
    {
        private const string RUNTIME_TEST_SCENE_NAME = "RuntimeTest";

        [UnityTest]
        public IEnumerator RuntimeTestScene_LoadsWithPreplacedTilemaps()
        {
            yield return LoadRuntimeTestScene();

            var tilemaps = Object.FindObjectsByType<Tilemap>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            Assert.That(tilemaps, Has.Exactly(3).Matches<Tilemap>(tilemap => tilemap.name.StartsWith("Tilemap_Test")));

            foreach (var tilemap in tilemaps)
            {
                if (!tilemap.name.StartsWith("Tilemap_Test"))
                    continue;

                Assert.Greater(_CountTiles(tilemap), 0, $"{tilemap.name}에 미리 배치된 타일이 있어야 합니다.");
                Assert.NotNull(tilemap.GetComponent<TilemapCollider2D>(), $"{tilemap.name}에는 TilemapCollider2D가 있어야 합니다.");
                Assert.NotNull(tilemap.GetComponent<CompositeCollider2D>(), $"{tilemap.name}에는 CompositeCollider2D가 있어야 합니다.");
            }
        }

        [UnityTest]
        public IEnumerator RuntimeTestScene_MoveAndCheckpointFlow_Works()
        {
            yield return LoadRuntimeTestScene();

            var controller = Object.FindFirstObjectByType<RuntimeTestSequenceController>();
            var player = GameObject.FindWithTag("Player");
            var passZone = GameObject.Find("Zone_Test01_Pass").GetComponent<RuntimeTestZone>();
            var checkpointZone = GameObject.Find("Flag_Checkpoint").GetComponent<RuntimeTestZone>();

            Assert.NotNull(controller);
            Assert.NotNull(player);
            Assert.NotNull(passZone);
            Assert.NotNull(checkpointZone);

            controller.ResetProgress();
            yield return null;

            controller.HandleZoneEntered(passZone, player.GetComponent<Collider2D>());
            yield return null;

            Assert.AreEqual(1, controller.CurrentTestIndex);
            Assert.That(player.transform.position.x, Is.EqualTo(0f).Within(0.05f));
            Assert.That(player.transform.position.y, Is.EqualTo(-9f).Within(0.05f));

            controller.HandleZoneEntered(checkpointZone, player.GetComponent<Collider2D>());
            yield return null;

            Assert.AreEqual(1, controller.CurrentTestIndex, "현재 단계가 아닌 체크포인트 구역은 무시되어야 합니다.");
        }

        private static IEnumerator LoadRuntimeTestScene()
        {
            var operation = SceneManager.LoadSceneAsync(RUNTIME_TEST_SCENE_NAME, LoadSceneMode.Single);
            Assert.NotNull(operation, "RuntimeTest 씬이 Build Settings에 등록되어 있어야 합니다.");

            while (!operation.isDone)
                yield return null;

            yield return null;

            Assert.AreEqual(RUNTIME_TEST_SCENE_NAME, SceneManager.GetActiveScene().name);
        }

        private static int _CountTiles(Tilemap tilemap)
        {
            var count = 0;
            foreach (var position in tilemap.cellBounds.allPositionsWithin)
            {
                if (tilemap.GetTile(position) != null)
                    count++;
            }

            return count;
        }
    }
}
