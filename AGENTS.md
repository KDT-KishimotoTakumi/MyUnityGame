# AGENTS.md

## Purpose

MyUnityGame は Unity 6（6000.6.4f1）で作る小さなゲーム「BallCollect」のプロジェクトです。
AI エージェントが unity-cli 経由で Unity Editor を操作し、作る → 動かす → 結果を読む、までを行います。

## 前提として読むもの

- `.specify/memory/constitution.md` — プロジェクトの原則（最優先）
- `specs/<機能>/spec.md`・`plan.md`・`tasks.md` — 機能ごとの仕様・計画・作業
- `docs/handoff/HANDOFF-*.md` — 渡された場合は、その作業専用の指示書

## Rules for coding agents

- 関係のないファイルを変更しない。差分は小さく、1作業 = レビュー可能な1単位にする。
- 前の会話の文脈は前提にしない。必要な情報はハンドオフ文書かこのリポジトリにある。
- Unity の操作（シーン・GameObject・コンポーネント・C# 作成・テスト・Play 検証）は unity-cli で行う。
  `.unity` / `.asset` / `.prefab` を直接テキスト編集しない。
- 入力は Input System（`.inputactions`）に統一する。旧 `Input.GetKey` 等は使わない。
- コードは名前空間 `MyUnityGame`。スクリプトは `Assets/Scripts/`、テストは `Assets/Tests/`、シーンは `Assets/Scenes/`。
- 外部アセット・パッケージを追加しない。spec.md の範囲外の機能を足さない。
- 1つの Unity Editor を同時に複数の担当で操作しない。

## Validation（テストコマンド）

```bash
unity-cli --output json system ping                                   # versionCheck.status = match
unity-cli --output json raw get_compilation_state --json '{}'         # errorCount = 0
unity-cli --output json raw run_tests --json '{"testMode":"EditMode"}'
unity-cli --output json raw get_test_status --json '{}'               # status = completed、失敗 0（exit 8 は失敗あり）
```

Play での確認手順は `specs/<機能>/quickstart.md` に従う。開始成功・0件・全スキップ・未完了は合格ではない。

## 完了時の報告

変更したファイル、実行したテストと結果、Play で確認した内容（確認済み／未成立／未確認を区別）、残るリスクを報告する。
