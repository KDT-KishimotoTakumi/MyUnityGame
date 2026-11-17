# Implementation Plan: BallCollect 基本ゲーム

**Branch**: `001-ballcollect-core` | **Date**: 2026-11-17 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `specs/001-ballcollect-core/spec.md`

## Summary

床・球・アイテム5個・固定カメラ・HUD からなる1シーンのゲームを作り、B1〜B6（初期状態・WASD移動・回収・
クリア・落下初期化・R再開）を満たす。回収とクリア判定は MonoBehaviour から分離した純粋な C# クラス
`CollectionState` に置いて EditMode テストで先に固め、MonoBehaviour 側（`GameManager`・`PlayerController`・
`CollectibleItem`・`HudView`）は薄くする。シーン作成・スクリプト作成・参照設定・Play 検証はすべて
unity-cli → Bridge → Editor の経路で行う（Unity のみで作業する）。

## Technical Context

**Language/Version**: C#（Unity 6000.6.4f1 標準の .NET / C# 9）

**Primary Dependencies**: Input System 1.14.2、uGUI 1.0.0（Legacy Text）、Unity Physics、Code Coverage 1.3.0（2026-11-17 追加。SC-008 のカバレッジ測定に必要で、標準機能では行カバレッジを測れないため）

**Storage**: N/A（保存データなし）

**Testing**: Unity Test Framework 1.1.33 の EditMode テスト（ロジック、TDD）＋ PlayMode の E2E テスト（シーンを読み込み、テスト用キーボードで入力して状態を確認）＋ unity-cli による Play 検証。Code Coverage で `MyUnityGame` アセンブリの行カバレッジ 90% 以上を確認

**Target Platform**: Unity Editor の Play モード（Windows）。実行ファイルのビルドは範囲外

**Project Type**: 単一シーンの Unity ゲーム（Built-in Render Pipeline）

**Performance Goals**: Editor の Play で 60fps 前後。オブジェクト数が少ないため特別な最適化はしない

**Constraints**: Unity のみで作業（unity-cli 経由）。入力は Input System に統一。外部アセットなし。
状態は決まった名前で CLI から読み取れること

**Scale/Scope**: 1シーン、スクリプト5本＋テスト1本、GameObject 約12個

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| 原則 | 判定 | 根拠 |
|---|---|---|
| I. 遊べる最小単位で作る | PASS | 段階1（シーン）→ 段階2（移動）→ 段階3（ルール）の順で、各段階を Play で確認できる |
| II. テストを先に書く | PASS | `CollectionState` を純粋 C# で分離し、EditMode テストを先に書く（Red → Green） |
| III. unity-cli で再現できる | PASS | シーン・GameObject・コンポーネント・スクリプト・asmdef・入力アセットを unity-cli で作成する |
| IV. Play で確認する | PASS | [quickstart.md](./quickstart.md) の6シナリオを入力シミュレーションと状態取得で確認する |
| V. シンプルさを優先 | PASS | 追加パッケージは Code Coverage のみ（SC-008 の測定に必要、理由を Technical Context に記載）。クラスは責務ごとに5本 |
| 技術的な制約 | PASS | 名前空間 `MyUnityGame`、`Assets/Scripts`・`Assets/Tests`・`Assets/Scenes` に配置。旧 Input API 不使用 |

**Post-design re-check**: PASS（Phase 1 の設計でも違反なし。Complexity Tracking は不要）

## Project Structure

### Documentation (this feature)

```text
specs/001-ballcollect-core/
├── spec.md
├── plan.md              # 本ファイル
├── research.md          # Phase 0
├── data-model.md        # Phase 1
├── quickstart.md        # Phase 1（Play 検証の手順）
├── contracts/
│   └── observation-contract.md  # CLI から読む名前と値の約束
├── checklists/requirements.md
├── tasks.md             # /speckit-tasks で作成
└── verification.md      # 実装後に作成（操作・期待・実際・根拠・判定）
```

### Source Code (repository root)

```text
Assets/
├── Scenes/
│   └── BallCollect.unity
├── Input/
│   └── BallCollectControls.inputactions   # map "Gameplay": Move(WASD), Restart(R)
├── Materials/
│   ├── Floor.mat / Player.mat / Item.mat
├── Scripts/
│   ├── MyUnityGame.asmdef                 # refs: Unity.InputSystem, UnityEngine.UI
│   ├── CollectionState.cs                 # 純粋 C#：回収数・二重加算防止・クリア判定・初期化
│   ├── GameManager.cs                     # 落下判定・R入力・初期化の統括、状態の公開
│   ├── PlayerController.cs                # Move 入力 → Rigidbody への力、初期位置への復帰
│   ├── CollectibleItem.cs                 # トリガー接触を GameManager へ通知
│   └── HudView.cs                         # 「n/5」と CLEAR の表示
└── Tests/
    ├── EditMode/
    │   ├── MyUnityGame.Tests.EditMode.asmdef
    │   └── CollectionStateTests.cs
    └── PlayMode/
        ├── MyUnityGame.Tests.PlayMode.asmdef   # refs: MyUnityGame, Unity.InputSystem, UnityEngine.UI, test runner
        └── BallCollectE2ETests.cs              # quickstart シナリオ 1〜7
```

**Structure Decision**: Unity 標準の `Assets/` 配下に、本書の技術的な制約どおり Scripts / Tests / Scenes を置く。
ゲームコードを asmdef `MyUnityGame` にまとめ、EditMode テスト asmdef から参照できるようにする。

## 実装の段階（資料の3段階に対応）

1. **段階1 シーン**（US1 の配置部分）：シーン作成・保存 → 床・Player・Item_1〜5・カメラ位置調整・マテリアル。
   確認：`get_hierarchy` とスクリーンショット
2. **段階2 移動**（US2）：入力アセット → `PlayerController` → Player へ接続 → Play で W 入力前後の座標比較
3. **段階3 ルール**（US1 の HUD・US3・US4）：`CollectionState` のテスト → 実装 → `CollectibleItem`・`GameManager`・
   `HudView`・HUD Canvas → Play で回収・CLEAR・落下・R を確認

各段階の終わりに、コンパイルエラー0・シーン保存・Play 停止を確認する。

## Complexity Tracking

違反なし。
