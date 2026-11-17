# Quickstart: BallCollect 基本ゲームの検証手順

すべて Unity Editor に unity-cli で接続して行う。読む名前は [contracts/observation-contract.md](./contracts/observation-contract.md)、
初期値は [data-model.md](./data-model.md) を参照。

## 前提

```bash
unity-cli --output json system ping            # versionCheck.status が match
unity-cli --output json raw get_compilation_state --json '{}'   # errorCount 0
unity-cli --output json raw load_scene --json '{"scenePath":"Assets/Scenes/BallCollect.unity"}'
```

## 0. 自動テスト（Play 不要）

`run_tests`（`testMode: EditMode`）→ `get_test_status` を `completed` までポーリング。
テスト名・件数・合否を照合する（0件・全スキップ・未完了は不合格）。

## Play 検証の共通ループ

1. `play_game` → `get_editor_state` で `isPlaying: true` を確認（開始命令の成功 ≠ 移行完了）
2. 状態を読む → 短い入力（`holdSeconds` 付き）→ 入力時間より少し長く待つ → 状態を再取得して比べる
3. 終了時は入力を解除し `stop_game` → `isPlaying: false` を確認

## シナリオ

| # | 条件 | 操作 | 期待 |
|---|---|---|---|
| 1 | B1 | Play 直後に状態と画面を取得 | 球 (0,0.5,−7)、Item_1〜5 アクティブ、`collectedCount=0`、`isCleared=false`、`CollectedText`「0/5」、`ClearText` 非表示 |
| 2 | B2 | W/S/D/A をそれぞれ 1 秒、各回の前に R | 順に Z 増・Z 減・X 増・X 減 |
| 3 | B3 | W を押して Item_1 まで進める | Item_1 非アクティブ、`collectedCount=1`、「1/5」、CLEAR 非表示 |
| 4 | B6 途中 | 1/5 の状態で R | シナリオ1と同じ初期状態（速度 0 を含む） |
| 5 | B5 | 1/5 の状態で D を押し続けて床外へ落とす | 落下後にシナリオ1と同じ初期状態 |
| 6 | B4 | W を押し続けて 5 個すべて回収 | 4/5 までは CLEAR 非表示、5/5 で `isCleared=true` と CLEAR 表示 |
| 7 | B6 クリア後 | CLEAR 表示中に R | シナリオ1と同じ初期状態、再び 1/5 から数えられる |

各シナリオで状態の根拠（取得した値）を残し、シナリオ1とシナリオ6では `capture_screenshot`（`includeUI: true`）で
画面の根拠も残す。コンソールの Error / Exception が0件であることを最後に `read_console` で確認する。

## 記録

結果は `verification.md` に条件ごと数行（操作・期待・実際・根拠・判定）。判定は「確認済み／未成立／未確認」を区別する。
