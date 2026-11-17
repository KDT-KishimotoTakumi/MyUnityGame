using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace MyUnityGame.Tests
{
    /// <summary>End-to-end checks of quickstart scenarios 1-7: real scene, keyboard events in, observed state out.</summary>
    public class BallCollectE2ETests
    {
        private static readonly Vector3 StartPosition = new Vector3(0f, 0.5f, -7f);
        private const float Timeout = 10f;

        private Keyboard keyboard;
        private InputSettings.EditorInputBehaviorInPlayMode savedBehavior;
        private GameManager game;
        private Rigidbody player;
        private GameObject[] items;
        private Text collectedText;
        private GameObject clearText;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            savedBehavior = InputSystem.settings.editorInputBehaviorInPlayMode;
            InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            keyboard = InputSystem.AddDevice<Keyboard>();

            yield return SceneManager.LoadSceneAsync("BallCollect", LoadSceneMode.Single);
            yield return null;

            game = UnityEngine.Object.FindAnyObjectByType<GameManager>();
            player = GameObject.Find("Player").GetComponent<Rigidbody>();
            items = new GameObject[5];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = GameObject.Find("Item_" + (i + 1));
            }

            Transform hud = GameObject.Find("HUD").transform;
            collectedText = hud.Find("CollectedText").GetComponent<Text>();
            clearText = hud.Find("ClearText").gameObject;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            Release();
            yield return null;
            InputSystem.RemoveDevice(keyboard);
            InputSystem.settings.editorInputBehaviorInPlayMode = savedBehavior;
        }

        // Scenario 1 (B1)
        [UnityTest]
        public IEnumerator Scenario1_Start_IsInitialState()
        {
            yield return new WaitForFixedUpdate();
            AssertInitialState();
        }

        // Scenario 2 (B2): each direction from a freshly loaded scene
        [UnityTest]
        public IEnumerator Scenario2_W_MovesPlusZ()
        {
            yield return Move(Key.W, p => Assert.Greater(p.z, StartPosition.z + 0.5f));
        }

        [UnityTest]
        public IEnumerator Scenario2_S_MovesMinusZ()
        {
            yield return Move(Key.S, p => Assert.Less(p.z, StartPosition.z - 0.5f));
        }

        [UnityTest]
        public IEnumerator Scenario2_D_MovesPlusX()
        {
            yield return Move(Key.D, p => Assert.Greater(p.x, 0.5f));
        }

        [UnityTest]
        public IEnumerator Scenario2_A_MovesMinusX()
        {
            yield return Move(Key.A, p => Assert.Less(p.x, -0.5f));
        }

        // Scenario 3 (B3)
        [UnityTest]
        public IEnumerator Scenario3_FirstItem_CountsOne_NoClear()
        {
            yield return CollectFirstItem();
            Assert.IsFalse(items[0].activeSelf, "Item_1 hidden");
            for (int i = 1; i < items.Length; i++)
            {
                Assert.IsTrue(items[i].activeSelf, $"Item_{i + 1} still shown");
            }

            Assert.AreEqual(1, game.CollectedCount);
            Assert.AreEqual("1/5", collectedText.text);
            Assert.IsFalse(game.IsCleared);
            Assert.IsFalse(clearText.activeSelf);
        }

        // B3 / FR-006: the same item is never counted twice
        [UnityTest]
        public IEnumerator SameItemReportedTwice_CountsOnce()
        {
            yield return null;
            Assert.IsTrue(game.TryCollect(2));
            Assert.IsFalse(game.TryCollect(2));
            Assert.AreEqual(1, game.CollectedCount);
            Assert.AreEqual("1/5", collectedText.text);
        }

        // Scenario 6 (B4): CLEAR never shows before 5, shows at 5
        [UnityTest]
        public IEnumerator Scenario6_AllFive_ShowsClear_OnlyAtFive()
        {
            yield return CollectAll();
            Assert.AreEqual(5, game.CollectedCount);
            Assert.AreEqual("5/5", collectedText.text);
            Assert.IsTrue(game.IsCleared);
            Assert.IsTrue(clearText.activeSelf);
            foreach (GameObject item in items)
            {
                Assert.IsFalse(item.activeSelf, item.name + " hidden");
            }
        }

        // Scenario 4 (B6 mid-game)
        [UnityTest]
        public IEnumerator Scenario4_R_MidGame_ResetsEverything()
        {
            yield return CollectFirstItem();
            yield return PressR();
            AssertInitialState();
        }

        // Scenario 5 (B5)
        [UnityTest]
        public IEnumerator Scenario5_FallOff_ResetsEverything()
        {
            yield return CollectFirstItem();
            Press(Key.D);
            bool fell = false;
            yield return WaitUntil(() =>
            {
                if (player.position.y < 0f)
                {
                    fell = true;
                }

                return fell && game.CollectedCount == 0;
            }, "reset after falling");
            Release();
            AssertInitialState();
        }

        // Scenario 7 (B6 after clear) and counting again
        [UnityTest]
        public IEnumerator Scenario7_R_AfterClear_ResetsEverything_AndCountsAgain()
        {
            yield return CollectAll();
            yield return PressR();
            AssertInitialState();
            yield return CollectFirstItem();
            Assert.AreEqual(1, game.CollectedCount);
            Assert.AreEqual("1/5", collectedText.text);
        }

        // Edge case: holding R keeps the game in its initial state
        [UnityTest]
        public IEnumerator HoldingR_KeepsInitialState()
        {
            yield return CollectFirstItem();
            yield return PressR();
            for (int i = 0; i < 10; i++)
            {
                yield return null;
            }

            AssertInitialState(velocityTolerance: 0.5f);
        }

        private IEnumerator Move(Key key, Action<Vector3> check)
        {
            Press(key);
            yield return new WaitForSeconds(0.5f);
            Release();
            check(player.position);
        }

        private IEnumerator CollectFirstItem()
        {
            Press(Key.W);
            yield return WaitUntil(() => game.CollectedCount >= 1, "first item collected");
            Release();
        }

        private IEnumerator CollectAll()
        {
            Press(Key.W);
            yield return WaitUntil(() =>
            {
                Assert.AreEqual(game.CollectedCount == 5, clearText.activeSelf, $"CLEAR shown only at 5 (count {game.CollectedCount})");
                return game.CollectedCount == 5;
            }, "all items collected");
            Release();
        }

        // Leaves R held; TearDown or a later Press/Release lets it go.
        private IEnumerator PressR()
        {
            Press(Key.R);
            yield return WaitUntil(() => game.CollectedCount == 0, "reset by R");
        }

        private void AssertInitialState(float velocityTolerance = 0.01f)
        {
            Assert.That(Vector3.Distance(player.position, StartPosition), Is.LessThan(0.01f), $"position {player.position}");
            Assert.That(player.linearVelocity.magnitude, Is.LessThan(velocityTolerance), $"velocity {player.linearVelocity}");
            Assert.That(player.angularVelocity.magnitude, Is.LessThan(velocityTolerance), $"angular velocity {player.angularVelocity}");
            Assert.AreEqual(0, game.CollectedCount);
            Assert.IsFalse(game.IsCleared);
            foreach (GameObject item in items)
            {
                Assert.IsTrue(item.activeSelf, item.name + " shown");
            }

            Assert.AreEqual("0/5", collectedText.text);
            Assert.IsFalse(clearText.activeSelf);
        }

        private IEnumerator WaitUntil(Func<bool> condition, string what)
        {
            float end = Time.time + Timeout;
            while (!condition())
            {
                if (Time.time > end)
                {
                    Assert.Fail($"Timed out waiting for {what} (count {game.CollectedCount}, position {player.position})");
                }

                yield return null;
            }
        }

        private void Press(Key key) => InputSystem.QueueStateEvent(keyboard, new KeyboardState(key));

        private void Release() => InputSystem.QueueStateEvent(keyboard, new KeyboardState());
    }
}
