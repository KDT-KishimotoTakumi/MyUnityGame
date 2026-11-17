---
name: unity-editor
description: MyUnityGameのUnity Editor操作（シーン・GameObject・マテリアル・入力アセット・Play・スクリーンショット・テスト）をunity-cliで行うときの共通ルール。具体的な手順は該当する unity-* スキルを使う。C#の編集だけなら不要。
---

# Unity Editor操作（unity-cli）

詳細な手順は `.claude/skills/unity-*`（scene-create、gameobject-edit、csharp-edit、input-system、playmode-testing など）が正本。必要なものだけ読む。

- 入口は `unity-cli --output json raw <tool> --json '<json>'`（`%USERPROFILE%\.unity\tools\unity-cli\win-x64\unity-cli.exe`）。ツールの形は `unity-cli tool schema <tool> --output json` で必要な分だけ確認する。複雑な JSON はファイルから組み立てて渡す。
- 操作前に `system ping`（`versionCheck: match`）と `get_scene_info` で対象シーンを確認する。Play・リコンパイル後は再確認する（ドメインリロード中は通信が切れる。`get_compilation_state` が `isCompiling: false` になるまで待つ）。
- 変更系がタイムアウト・通信断になっても未実行とは限らない。状態を読み直し、同じ変更を再送しない。
- シーンの変更は `save_scene` まで確認する。`save_scene` の `saveAs` は開いているシーンを切り替えないので、保存後に `load_scene` で開き直す。.meta と GUID を壊す手作業の YAML 編集はしない。
- 画像は `manage_windows`（`focus` / `GameView`）の後に `capture_screenshot`（`captureMode: game`、`includeUI: true`）で保存し、PNG を開いて確認する。根拠として残す画像は `specs/<機能>/evidence/` にコピーする。
- Play のフレームは Unity Editor が OS の前面にあるときだけ進む（バックグラウンドでは `Time.frameCount` が 1 のまま止まる。Run In Background を有効にしても変わらない）。Play 検証の前に Editor を前面に出し（PowerShell の `SetForegroundWindow` など）、`eval_csharp` で `Time.frameCount` が増えていることを確かめてから入力する。
- Play を使ったら最後に入力解除・`stop_game`・`isPlaying: false` を確認する。1つの Editor を同時に複数の担当で操作しない。
