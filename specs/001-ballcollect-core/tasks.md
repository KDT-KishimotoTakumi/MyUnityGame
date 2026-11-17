---

description: "Task list for BallCollect core game (001)"
---

# Tasks: BallCollect 基本ゲーム

**Input**: Design documents from `specs/001-ballcollect-core/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/observation-contract.md, quickstart.md

**Tests**: 憲章 原則 II（NON-NEGOTIABLE）と SC-005 により、`CollectionState` の EditMode テストを先に書く。
入力・物理・UI は原則 IV により Play 検証（quickstart.md）で確認する。

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (US1〜US4)

## 共通ルール（全タスク）

- Unity の操作はすべて unity-cli で行う（`unity-cli --output json raw <tool> --json '{...}'`）。`.unity` / `.asset` / `.inputactions` を
  テキストで直接編集しない。C# は `create_csharp_file`（`waitForCompile: true`）で作る。
- unity-cli は `%USERPROFILE%\.unity\tools\unity-cli\win-x64\unity-cli.exe`。PATH に無ければ追加してから使う。
- コードは名前空間 `MyUnityGame`。Unity 6 の API を使う（`Rigidbody.linearVelocity`。`velocity` は使わない）。旧 `Input.GetKey` 等は使わない。
- Editor を変更するタスクの後は `get_compilation_state` で `errorCount: 0` を確認する。シーンを変えたら `save_scene` する。
- Editor を操作する担当は同時に1人（`AGENTS.md`）。[P] は「Editor の状態に依存しないファイル作成」に限る。
- 読む名前は `contracts/observation-contract.md` に従う。CLI から状態を書き換えて「動いた」としない。

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: シーンとアセンブリの土台

- [X] T001 接続と状態を確認する：`system ping` の `versionCheck.status` が `match`、`get_compilation_state` の `errorCount` が 0、`get_editor_state` の `isPlaying` が false（記録先なし、結果を報告に含める）
- [X] T002 現在の未保存シーン（`Main Camera`・`Directional Light` のみ）を `save_scene` で `Assets/Scenes/BallCollect.unity` として保存し、`list_scenes` で存在を確認する
- [X] T003 ゲームコード用 asmdef を `eval_csharp` で作成し `AssetDatabase.ImportAsset` する：`Assets/Scripts/MyUnityGame.asmdef`（`"name": "MyUnityGame"`, `"rootNamespace": "MyUnityGame"`, `"references": ["Unity.InputSystem", "UnityEngine.UI"]`）
- [X] T004 EditMode テスト用 asmdef を同じ方法で作成する：`Assets/Tests/EditMode/MyUnityGame.Tests.EditMode.asmdef`（`"references": ["MyUnityGame", "UnityEngine.TestRunner", "UnityEditor.TestRunner"]`, `"includePlatforms": ["Editor"]`, `"overrideReferences": true`, `"precompiledReferences": ["nunit.framework.dll"]`, `"defineConstraints": ["UNITY_INCLUDE_TESTS"]`）。作成後 `get_compilation_state` で `errorCount: 0`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: 全ストーリーが使うアセット

- [X] T005 [P] `create_material` で Standard シェーダーのマテリアルを3つ作る：`Assets/Materials/Floor.mat`（灰 0.6,0.6,0.6）、`Assets/Materials/Player.mat`（青 0.2,0.4,1.0）、`Assets/Materials/Item.mat`（黄 1.0,0.85,0.1）
- [X] T006 [P] `Assets/Input/BallCollectControls.inputactions` を作成し（`unity-input-system` スキル）、map `Gameplay` に `Move`（Value / Vector2、2D composite：up=`<Keyboard>/w`、down=`<Keyboard>/s`、left=`<Keyboard>/a`、right=`<Keyboard>/d`）と `Restart`（Button、`<Keyboard>/r`）を追加する。`analyze_input_actions_asset` で2アクション・5バインドを確認

**Checkpoint**: シーン・asmdef・マテリアル・入力アセットが揃い、コンパイルエラー 0

---

## Phase 3: User Story 1 - 初期状態のステージが表示される（B1）(Priority: P1) 🎯 MVP

**Goal**: 床・球・アイテム5個・固定カメラ・「0/5」表示が開始時に正しく並ぶ

**Independent Test**: `get_hierarchy` と `get_component_values`（Transform）で配置を確認し、`capture_screenshot`（`includeUI: true`）で全体が1画面に見えることを確認する

### Implementation for User Story 1

- [X] T007 [US1] `create_gameobject` で Plane `Floor` を作る：位置 (0,0,0)、scale (2,1,2)（20×20）、マテリアル `Assets/Materials/Floor.mat`。シーン `Assets/Scenes/BallCollect.unity`
- [X] T008 [US1] Sphere `Player` を作る：位置 (0,0.5,−7)、タグ `Player`（組み込みタグ）、`Rigidbody`（mass 1）を追加、マテリアル `Assets/Materials/Player.mat`
- [X] T009 [US1] Sphere `Item_1`〜`Item_5` を作る：scale 0.6、位置 X=0・Y=0.5・Z=−4, −1, 2, 5, 8（Item_1 が Z=−4）、`SphereCollider.isTrigger = true`、マテリアル `Assets/Materials/Item.mat`
- [X] T010 [US1] 既存の `Main Camera` を位置 (0,18,−16)・回転 (50,0,0) に変更する。カメラとライトが重複していないことを `find_by_component`（Camera / Light）で確認
- [X] T011 [US1] HUD を作る：Canvas `HUD`（Screen Space - Overlay、CanvasScaler: Scale With Screen Size 1920×1080）、子に Legacy `Text` の `CollectedText`（文字列 `0/5`、左上、size 48、フォント `LegacyRuntime.ttf`）と `ClearText`（文字列 `CLEAR`、中央、size 96、**非アクティブ**）。UI 作成で typed ツールが足りない場合は `eval_csharp` で作成する
- [X] T012 [US1] `save_scene` 後、`get_hierarchy` で Floor / Player / Item_1〜5 / Main Camera / Directional Light / HUD（CollectedText、ClearText）を確認し、`capture_screenshot`（Game View、`includeUI: true`）で配置と「0/5」表示の根拠を残す

**Checkpoint**: US1 の受け入れシナリオ 1・2 を満たすシーンが保存されている

---

## Phase 4: User Story 2 - WASD で球を動かす（B2）(Priority: P1)

**Goal**: W/S/D/A で +Z/−Z/+X/−X に球が転がる

**Independent Test**: Play 中に各キーを `input_keyboard`（`holdSeconds: 1.0`）で押し、約1.2秒待ってから `Player` の Transform を再取得して座標の変化を比べる

### Implementation for User Story 2

- [X] T013 [US2] `Assets/Scripts/PlayerController.cs` を作成する：`namespace MyUnityGame`、`[RequireComponent(typeof(Rigidbody))]`、`[SerializeField] InputActionAsset inputActions`、`[SerializeField] float moveForce = 10f`、`[SerializeField] Vector3 startPosition = new Vector3(0f, 0.5f, -7f)`。`OnEnable` で `inputActions.FindAction("Gameplay/Move", true)` を取得して Enable、`OnDisable` で Disable。`FixedUpdate` で `rb.AddForce(new Vector3(move.x, 0f, move.y) * moveForce)`。公開メソッド `ResetToStart()` は `rb.linearVelocity = Vector3.zero`、`rb.angularVelocity = Vector3.zero`、`rb.position` と `transform.position` を `startPosition` にする
- [X] T014 [US2] `Player` に `PlayerController` を追加し、`inputActions` に `Assets/Input/BallCollectControls.inputactions` を `set_component_field` で設定して `save_scene`
- [X] T015 [US2] Play 検証（quickstart シナリオ 2）：`play_game` → `isPlaying: true` を確認 → W を1秒 → Z が増えたことを確認。`stop_game` → `play_game` で戻してから S・D・A も同様に確認（順に Z 減・X 増・X 減）。最後に入力解除・`stop_game`・`isPlaying: false`

**Checkpoint**: US1＋US2 で「操作できるステージ」になっている

---

## Phase 5: User Story 3 - アイテムを回収して CLEAR する（B3・B4）(Priority: P2)

**Goal**: 触れたアイテムが消えて回収数が1増え、5個でのみ CLEAR

**Independent Test**: EditMode テストが全件成功し、Play で 1個目回収で「1/5」、5個回収で CLEAR になる

### Tests for User Story 3（先に書いて失敗を確認する）⚠️

- [ ] T016 [US3] `Assets/Scripts/CollectionState.cs` に API だけの骨組みを作る（テストがコンパイルできるようにするため）：`public sealed class CollectionState`、コンストラクタ `CollectionState(int totalCount = 5)`、`int TotalCount { get; }`、`int CollectedCount { get; }`、`bool IsCleared { get; }`、`bool TryCollect(int itemId)`、`void Reset()`。メソッド本体は `throw new System.NotImplementedException()`
- [ ] T017 [US3] `Assets/Tests/EditMode/CollectionStateTests.cs` に NUnit テストを書く：(1) 初期値は `CollectedCount == 0` かつ `IsCleared == false`、(2) 新しい ID の `TryCollect` は true を返し回収数 1、(3) 同じ ID を2回 `TryCollect` しても回収数は 1 で2回目は false、(4) 範囲外 ID（−1 と 5）は false で回収数は変わらない、(5) 4個回収では `IsCleared == false`、(6) 5個回収で `IsCleared == true`、(7) `Reset()` 後は回収数 0・未クリアで、同じ ID を再び回収できる
- [ ] T018 [US3] `run_tests`（`testMode: EditMode`）→ `get_test_status` を `completed` までポーリングし、7件が**失敗**すること（Red）を名前・件数で確認する

### Implementation for User Story 3

- [ ] T019 [US3] `Assets/Scripts/CollectionState.cs` を実装する。data-model.md の規則をそのまま守る：「範囲外 ID（0 未満・TotalCount 以上）または回収済み ID なら何もせず false」「`0 ≤ CollectedCount ≤ TotalCount`」「`IsCleared ⇔ CollectedCount == TotalCount`」「`Reset()`：集合を空にし、`CollectedCount = 0`、`IsCleared = false`」。回収済み ID は `HashSet<int>` で持つ。再度テストを実行し 7件**成功**（Green）を確認
- [ ] T020 [P] [US3] `Assets/Scripts/HudView.cs` を作成する：`[SerializeField] UnityEngine.UI.Text collectedText`、`[SerializeField] GameObject clearText`、`public void Refresh(int collected, int total, bool cleared)` で `collectedText.text = $"{collected}/{total}"`、`clearText.SetActive(cleared)`
- [ ] T021 [P] [US3] `Assets/Scripts/CollectibleItem.cs` を作成する：`[SerializeField] int itemId`、`[SerializeField] GameManager gameManager`、`public int ItemId => itemId`。`OnTriggerEnter(Collider other)` で `other.CompareTag("Player")` かつ `gameManager.TryCollect(itemId)` が true なら `gameObject.SetActive(false)`
- [ ] T022 [US3] `Assets/Scripts/GameManager.cs` を作成する（回収部分）：`[SerializeField] PlayerController player`、`[SerializeField] CollectibleItem[] items`、`[SerializeField] HudView hud`、`[SerializeField] InputActionAsset inputActions`、観測用 `[SerializeField] int collectedCount` と `[SerializeField] bool isCleared`（observation-contract.md）。`CollectionState state = new CollectionState(5)`。`public bool TryCollect(int itemId)` は state に委譲し、成功時に観測用フィールドと `hud.Refresh` を更新する。`Start` で `hud.Refresh(0, 5, false)`
- [ ] T023 [US3] シーンを配線する：空の GameObject `GameManager` に `GameManager` を追加。`Item_1`〜`Item_5` に `CollectibleItem` を追加し `itemId` を 0〜4、`gameManager` を設定。`HUD` に `HudView` を追加し `collectedText` / `clearText` を設定。`GameManager` の `player` / `items`（5個、順番どおり）/ `hud` を設定して `save_scene`
- [ ] T024 [US3] Play 検証（quickstart シナリオ 3・6）：W で Item_1 に触れて `Item_1` 非アクティブ・`collectedCount=1`・「1/5」・CLEAR 非表示を確認。続けて W を保持して全回収し、4/5 までは CLEAR 非表示、5/5 で `isCleared=true` と `ClearText` アクティブを確認、`capture_screenshot`（`includeUI: true`）で CLEAR 画面の根拠を残す。`stop_game`

**Checkpoint**: 回収とクリアが動き、ロジックのテストが全件成功

---

## Phase 6: User Story 4 - 落下と R キーで初期状態に戻る（B5・B6）(Priority: P2)

**Goal**: 落下・途中R・クリア後R のどれでも、位置・速度・回収数・アイテム5個・HUD がすべて開始時に戻る

**Independent Test**: 1個回収後に R／落下、クリア後に R を行い、data-model.md の初期値5項目がすべて一致する

### Implementation for User Story 4

- [ ] T025 [US4] `Assets/Scripts/GameManager.cs` に初期化を追加する：`public void ResetGame()` は research.md R5 の順で (1) `player.ResetToStart()`、(2) `state.Reset()`、(3) `items` をすべて `SetActive(true)`、(4) 観測用フィールドと `hud.Refresh(0, 5, false)`。`OnEnable` で `inputActions.FindAction("Gameplay/Restart", true)` を Enable し、`Update` で `WasPressedThisFrame()` なら `ResetGame()`。`Update` で `player.transform.position.y < -5f` なら `ResetGame()`
- [ ] T026 [US4] `GameManager` の `inputActions` に `Assets/Input/BallCollectControls.inputactions` を設定して `save_scene`。`get_compilation_state` で `errorCount: 0`
- [ ] T027 [US4] Play 検証（quickstart シナリオ 4・5・7）：1/5 で R → 初期状態、1/5 で D を保持して床外へ落とす → 初期状態、CLEAR 中に R → 初期状態で再び 1/5 から数えられる。各回で `Player` の位置・`linearVelocity`・`collectedCount`・`isCleared`・Item_1〜5 のアクティブ・`CollectedText`/`ClearText` を取得して比べる。R を保持し続けても状態が壊れないことも確認。`stop_game`

**Checkpoint**: B1〜B6 がすべて Play で確認できる

---

## Phase 7: Polish & Cross-Cutting Concerns

- [ ] T028 quickstart.md のシナリオ 1〜7 を通しで実行し、`read_console` で Error / Exception が 0 件であることを確認する
- [ ] T029 `specs/001-ballcollect-core/verification.md` を作成する：B1〜B6 ごとに「操作・期待・実際・根拠・判定（確認済み／未成立／未確認）」を数行。EditMode テストの名前・件数・合否と、スクリーンショット2枚以上の保存場所も書く
- [ ] T030 `code-review-and-quality` スキルで差分をレビューし（基準：constitution、spec.md、本 tasks.md）、指摘があれば修正して再確認する
- [ ] T031 入力解除・`stop_game`（`isPlaying: false`）・`save_scene` を確認し、変更をコミットする

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)** → **Foundational (Phase 2)** → **US1** → **US2** → **US3** → **US4** → **Polish**
- このゲームは1シーン・1 Editor を共有するため、ストーリーは優先度順に**順番に**進める（並列に Editor を操作しない）

### User Story Dependencies

- **US1 (P1)**: Phase 2 の後。他に依存しない
- **US2 (P1)**: US1 の `Player` が必要
- **US3 (P2)**: US1 の Item・HUD が必要。移動の確認に US2 を使う
- **US4 (P2)**: US3 の `GameManager` と `CollectionState.Reset()` を拡張する（T025 は T022 と同じファイル）

### Within Each User Story

- テスト（T017）は実装（T019）より先に書き、失敗を確認する（T018）
- スクリプト作成 → シーンへの配線 → Play 検証 の順
- 各ストーリーの最後にコンパイルエラー 0・シーン保存・Play 停止

### Parallel Opportunities

- T005 と T006（別アセット、Editor の状態に依存しない）
- T020 と T021（別ファイルの新規スクリプト）。ただし作成中のコンパイルが重ならないよう、`waitForCompile` の完了を待ってから次を作る
- 定型作業にサブエージェントは使わない（`CLAUDE.md`）。場所が分かっている小さな C# 修正だけ `code-task` に任せられる

---

## Parallel Example: User Story 3

```bash
# 骨組み（T016）の後、別ファイルのスクリプトは並行して下書きできる：
Task: "Create HudView in Assets/Scripts/HudView.cs"
Task: "Create CollectibleItem in Assets/Scripts/CollectibleItem.cs"
# Editor への書き込み（create_csharp_file）は1つずつ実行し、コンパイル完了を確認する
```

---

## Implementation Strategy

### MVP First（US1 → US2）

1. Phase 1・2 を完了
2. US1（ステージ表示）→ スクリーンショットで確認
3. US2（移動）→ 座標の変化で確認 → **ここで一度止めて遊べることを確認**

### Incremental Delivery

1. US3（回収・クリア）をテスト先行で追加 → Play で確認
2. US4（落下・R）を追加 → 3パターンの初期化を確認
3. Polish で通し検証・記録・検収・コミット

### 担当の振り分け（`CLAUDE.md`）

- Editor を操作するタスク（T001〜T015、T018、T023〜T028、T031）はメイン会話が行う
- 接続・シーン・コンソールの読み取り専用の点検は `unity-check` に任せてよい
- 各ストーリーの終わりで `handoff` スキルにより `docs/ai/HANDOFF.md` を更新する

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- 範囲外（敵・外部アセット・効果音・複数ステージ・ランキング・オンライン・動画収録・ビルド）は追加しない
- 時間制限などの追加ルールは別機能（002）で扱う
