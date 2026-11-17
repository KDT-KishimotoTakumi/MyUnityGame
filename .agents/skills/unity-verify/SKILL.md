---
name: unity-verify
description: MyUnityGameのコード・アセット・パッケージ・設定・エージェント設定の変更を、変更内容に合う最小限の方法で検証する。文章だけの修正には使わない。
---

# MyUnityGameの検証

成功・失敗・未実行を区別して報告する。

1. UnityのC#、アセット、`Packages/`、`ProjectSettings/` を変更したら、unity-cli で `refresh_assets` → `get_compilation_state`（`isCompiling: false` まで待つ）で `errorCount: 0` を確認し、`read_console`（Error / Exception）を見る。
2. ロジックを変更したら EditMode テスト：`run_tests`（`testMode: EditMode`）→ `get_test_status` を `completed` までポーリング。名前・件数・合否を報告し、0件を機能検証と呼ばない。
3. 入力・物理・UI に関わる変更は、`specs/<機能>/quickstart.md` の該当シナリオを Play で行う。状態は `contracts/` の名前で読み直して比べ、CLI から状態を書き換えて「動いた」としない。表示に関わる変更は実画像を保存して開く。成功メッセージだけで判断しない。
4. エージェント設定（`AGENTS.md`、`CLAUDE.md`、`.claude/`、`.agents/`）だけの変更は、内容・リンク・差分を確認する。
5. Unity 操作が終わったら結果と保存を確認し、Play を止めて `isPlaying: false` を確認する。

検査を増やす前に、その検査が今回の変更で起こりうる失敗を検出するか考える。Editorが使えない場合は静的確認を行い、残りを未実行として報告する。関連検査が成功し受け入れ条件を満たしたら、同じ検査を繰り返さず完了する。
