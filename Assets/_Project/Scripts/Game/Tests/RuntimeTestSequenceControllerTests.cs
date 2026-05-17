using NUnit.Framework;
using Platformer.Data;
using UnityEditor;
using UnityEngine;

namespace Platformer.Game.Tests
{
    public class RuntimeTestSequenceControllerTests
    {
        private GameObject _playerGo;
        private GameObject _controllerGo;
        private GameObject _zoneGo;
        private RespawnData _respawnData;

        [SetUp]
        public void SetUp()
        {
            _playerGo = new GameObject("Player");
            _playerGo.tag = "Player";
            _playerGo.AddComponent<BoxCollider2D>();
            _playerGo.AddComponent<Rigidbody2D>();

            _respawnData = ScriptableObject.CreateInstance<RespawnData>();
            _respawnData.SetCheckpoint(new Vector2(10f, 10f));

            _controllerGo = new GameObject("RuntimeTestSequenceController");
            var controller = _controllerGo.AddComponent<RuntimeTestSequenceController>();
            AssignControllerFields(controller);

            _zoneGo = new GameObject("PassZone");
            _zoneGo.AddComponent<BoxCollider2D>().isTrigger = true;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_zoneGo);
            Object.DestroyImmediate(_controllerGo);
            Object.DestroyImmediate(_playerGo);
            Object.DestroyImmediate(_respawnData);
        }

        [Test]
        public void ResetProgress_ClearsRespawnDataAndMovesToFirstTest()
        {
            var controller = _controllerGo.GetComponent<RuntimeTestSequenceController>();

            controller.ResetProgress();

            Assert.AreEqual(0, controller.CurrentTestIndex);
            Assert.IsFalse(_respawnData.hasCheckpoint);
            Assert.AreEqual(new Vector3(0f, 1f, 0f), _playerGo.transform.position);
        }

        [Test]
        public void HandleZoneEntered_MovePassZone_AdvancesToNextTest()
        {
            var controller = _controllerGo.GetComponent<RuntimeTestSequenceController>();
            var zone = _zoneGo.AddComponent<RuntimeTestZone>();
            AssignZoneFields(zone, controller, RuntimeTestZoneKind.Pass, 0);

            controller.ResetProgress();
            controller.HandleZoneEntered(zone, _playerGo.GetComponent<Collider2D>());

            Assert.AreEqual(1, controller.CurrentTestIndex);
            Assert.AreEqual(new Vector3(0f, -9f, 0f), _playerGo.transform.position);
        }

        private void AssignControllerFields(RuntimeTestSequenceController controller)
        {
            var serializedObject = new SerializedObject(controller);
            serializedObject.FindProperty("_player").objectReferenceValue = _playerGo.transform;
            serializedObject.FindProperty("_respawnData").objectReferenceValue = _respawnData;

            var spawnPositions = serializedObject.FindProperty("_spawnPositions");
            spawnPositions.arraySize = 3;
            spawnPositions.GetArrayElementAtIndex(0).vector2Value = new Vector2(0f, 1f);
            spawnPositions.GetArrayElementAtIndex(1).vector2Value = new Vector2(0f, -9f);
            spawnPositions.GetArrayElementAtIndex(2).vector2Value = new Vector2(0f, -29f);

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private void AssignZoneFields(
            RuntimeTestZone zone,
            RuntimeTestSequenceController controller,
            RuntimeTestZoneKind kind,
            int testIndex)
        {
            var serializedObject = new SerializedObject(zone);
            serializedObject.FindProperty("_controller").objectReferenceValue = controller;
            serializedObject.FindProperty("_kind").enumValueIndex = (int)kind;
            serializedObject.FindProperty("_testIndex").intValue = testIndex;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
