# Research: BallCollect 基本ゲーム

調査は Unity Editor へ unity-cli で接続して行った（2026-11-17、CLI / Bridge とも 0.19.1、`versionCheck: match`）。

## 現状（Editor から取得）

- Editor: 6000.6.4f1、`isPlaying=false`、コンパイルエラー 0
- シーン: 未保存の新規シーン。Hierarchy は `Main Camera` と `Directional Light` のみ（`list_scenes` は 0 件）
- パッケージ: `com.unity.inputsystem` 1.14.2、`com.unity.ugui` 1.0.0、`com.unity.test-framework` 1.1.33 が
  Bridge の依存として解決済み。Render Pipeline は Built-in
- 入力方式: `activeInputHandler: 2`（Both）

## R1. 入力の定義方法

- **Decision**: `.inputactions` アセット（`Assets/Input/BallCollectControls.inputactions`）に map `Gameplay` を作り、
  `Move`（Value/Vector2、WASD 2D composite）と `Restart`（Button、`<Keyboard>/r`）を定義する。スクリプトは
  `InputActionAsset` を参照し、`FindAction("Gameplay/Move")` で取得する。
- **Rationale**: 技術的な制約（`.inputactions` を使う）に従い、unity-cli の `create_action_map` /
  `add_input_action` / `create_composite_binding` で作成・確認できる。`InputActionReference` より
  シーンへの参照設定が単純。
- **Alternatives**: コード内で `new InputAction` を作る（CLI から確認できないため不採用）、`PlayerInput`
  コンポーネント（イベント配線が増えるため不採用）。

## R2. 回収ロジックの分離とテスト

- **Decision**: 純粋 C# クラス `CollectionState`（回収済み ID の集合、回収数、クリア判定、初期化）を作り、
  EditMode テストを先に書く。MonoBehaviour は状態を持たず、`CollectionState` に委譲する。
- **Rationale**: 原則 II。二重加算防止（B3）、5個でのみクリア（B4）、初期化（B5/B6）をエディタの Play なしで確認できる。
- **Alternatives**: `GameManager` に直接カウンタを持つ（テストに Play が必要になるため不採用）。

## R3. asmdef の作成

- **Decision**: `Assets/Scripts/MyUnityGame.asmdef` と `Assets/Tests/EditMode/MyUnityGame.Tests.EditMode.asmdef` を
  unity-cli 経由で作る（`eval_csharp` で JSON を書き込み `AssetDatabase.ImportAsset`、作成後
  `get_compilation_state` で確認）。
- **Rationale**: テスト asmdef から Assembly-CSharp は参照できないため、ゲームコード側にも asmdef が必要。
  `create_csharp_file` は `.cs` 専用のため、asmdef は Editor 内の C# 評価で作る（Unity のみで完結）。
- **Alternatives**: asmdef なしでテストを `Editor` フォルダに置く（テスト配置の制約 `Assets/Tests/` に反するため不採用）。

## R4. 回収の判定とアイテムの消し方

- **Decision**: アイテムは Sphere（直径 0.6）に `isTrigger` の Collider。`CollectibleItem.OnTriggerEnter` で
  `Player` タグを確認し `GameManager.TryCollect(itemId)` を呼ぶ。成功時に `SetActive(false)`。初期化で `SetActive(true)`。
- **Rationale**: 非アクティブ化は「消える」を満たし、Destroy と違い初期化で同じオブジェクトを戻せる。
  二重加算は `CollectionState` の ID 集合で防ぐため、同フレームの複数トリガーでも安全。
- **Alternatives**: Destroy して再生成（参照が切れて CLI からの確認が複雑になるため不採用）。

## R5. 落下と初期化

- **Decision**: `GameManager.Update` で `player.position.y < -5` を判定し `ResetGame()`。`ResetGame()` は
  (1) 球の位置を (0, 0.5, −7)、`linearVelocity` と `angularVelocity` を 0、(2) `CollectionState.Reset()`、
  (3) 全アイテム `SetActive(true)`、(4) HUD 更新、の順で行う。R も同じ `ResetGame()` を呼ぶ。
- **Rationale**: B5/B6 の「位置だけ戻さない」を1か所にまとめ、経路による差をなくす。Unity 6 では
  `Rigidbody.velocity` ではなく `linearVelocity` を使う。
- **Alternatives**: シーン再読み込み（状態が CLI から追いにくく、Play 中の参照が切れるため不採用）。

## R6. HUD

- **Decision**: Screen Space Overlay の Canvas `HUD` に uGUI Legacy `Text` を2つ（`CollectedText`「n/5」、
  `ClearText`「CLEAR」）。フォントは組み込みの `LegacyRuntime.ttf`。
- **Rationale**: 追加パッケージ不要。`find_ui_elements` / `get_ui_element_state` と `includeUI` 付きスクリーンショットで確認できる。
- **Alternatives**: TextMeshPro（追加インポートが必要）、UI Toolkit（CLI での確認手順が増える）。

## R7. カメラと見た目

- **Decision**: Main Camera を (0, 18, −16)、回転 (50, 0, 0) に固定（+Z が画面の奥＝上方向）。
  マテリアルは Standard シェーダーで Floor=灰、Player=青、Item=黄。
- **Rationale**: 20×20 の床全体と全アイテムが1画面に入り、W=画面上方向で直感に合う（B2）。

## R8. 移動の物理

- **Decision**: `Rigidbody`（mass 1）に `FixedUpdate` で `AddForce(new Vector3(x, 0, y) * moveForce)`、`moveForce = 10`。
- **Rationale**: 転がる動きで、1秒の W 入力で Z が数単位増える（資料の記録例 −7.0 → −3.2 程度）。
