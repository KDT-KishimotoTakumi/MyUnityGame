# MyUnityGame — agent rules

Unity 6（6000.6.4f1）で作る小さなゲーム「BallCollect」。AI が unity-cli（CLI → Bridge → Editor）で Editor を操作し、
作る → 動かす → 結果を読む、までを行う。ユーザーの明示的な指示が優先。ユーザーの言語で返答する。
Claude Code は `CLAUDE.md` 経由でこのファイルを読み、Codex / OpenCode は直接読む。

## Read only what the task needs
- 既知のルールはこのファイルから答え、パスを示す。読み直さない。必要な読み込みはまとめて行い、根拠が揃ったら調査をやめる。読んでいない内容を作らない。
- 原則は `.specify/memory/constitution.md`、機能の仕様・計画・作業は `specs/<NNN-name>/`（spec / plan / tasks / quickstart）。特定の事実はその正本を直接読む。
- 作業を再開するとき（AI・モデルの切り替えを含む）は `docs/ai/HANDOFF.md` の「現在の作業」節だけを読み、書かれたポインタを実際の spec・ソース・Git 状態・テスト結果と照合する。
- Unity のバージョンは `ProjectSettings/ProjectVersion.txt`、パッケージは `Packages/manifest.json` / `packages-lock.json` で確認する。推測しない。
- 検索は関係するパス（`Assets/Scripts/`、`Assets/Tests/`、`Assets/Scenes/`、`Assets/Input/`、`specs/`、`docs/`）から始め、必要なときだけ広げる。
  Library、Temp、Obj、Logs、UserSettings、`.unity/`、.git、パッケージキャッシュは、特定の項目を診断するとき以外は除外する。ソース・ログ全体、生成された csproj/sln、大きなアセットをダンプしない。
- unity-cli の出力は必要なフィールドだけ取り出して読む（`grep -o` などで絞る）。ツール一覧やスキーマ全体を読まない。

## Change discipline
- 変更前に Git の状態を確認し、ユーザーの作業を残す。期待する動作・受け入れ条件を確かめてから最小の変更をする。無関係な整形・リネーム・設計変更、パッケージ／Unity／入力方式の更新をしない。秘密情報や個人情報を保存しない。
- 大きな機能・仕様変更・大きな修正は Spec Kit（`/speckit-specify` → `/speckit-plan` → `/speckit-tasks` → `/speckit-implement`）で進める。重要な曖昧さは推測せず質問する。小さな修正に文書は不要。
- 確定した事実は spec / constitution に、継続中の状態は HANDOFF に置く（古い状態は置き換え、会話履歴を貼らない）。spec にある要件は HANDOFF に複製せずリンクする。答えで作業が変わる質問だけをする。

## Unity integrity
- Unity の操作（シーン・GameObject・コンポーネント・C#・テスト・Play 検証）は unity-cli で行う（`.claude/skills/unity-*`）。`.unity` / `.asset` / `.prefab` / `.inputactions` をテキストで直接編集しない。
- C# は unity-cli の LSP 書き込み（`create_csharp_file` など）を使う。失敗したときだけ直接書き込み、`refresh_assets` → `get_compilation_state` で確認し、そのことを報告する。
- コードは名前空間 `MyUnityGame`。スクリプトは `Assets/Scripts/`（asmdef `MyUnityGame`）、テストは `Assets/Tests/EditMode/`、シーンは `Assets/Scenes/`。入力は Input System（`Assets/Input/BallCollectControls.inputactions`）に統一し、旧 `Input.GetKey` 等は使わない。Unity 6 の API（`Rigidbody.linearVelocity`）を使う。
- アセットと .meta を一緒に扱い、GUID を保つ。シリアライズ済みフィールドの名前を変えるときは `FormerlySerializedAs` を使う。
- 外部アセット・パッケージを追加しない。spec の範囲外の機能を足さない。1つの Editor を同時に複数の担当で操作しない。

## Verification
- 接続: `unity-cli --output json system ping` の `versionCheck.status` が `match`。
- コンパイル: `raw get_compilation_state` の `errorCount` が 0（リコンパイル直後は通信が切れることがあるので、`isCompiling: false` まで待って再確認）。
- テスト: `raw run_tests --json '{"testMode":"EditMode"}'` → `raw get_test_status` を `completed` までポーリングし、名前・件数・合否を報告する。0件・全スキップ・未完了は合格ではない。
- Play: `specs/<機能>/quickstart.md` に従う。開始命令の成功 ≠ Play 移行完了、入力コマンドの成功 ≠ 動作。状態を読み直して比べ、画面は `capture_screenshot`（`includeUI: true`、Game View にフォーカス）で実画像を見る。終了時は入力解除・`stop_game`・`isPlaying: false` を確認する。
- 最小限の意味のある検証をする。実行していない確認を成功に数えず、未確認を報告する。大きな継続作業では HANDOFF を更新する。
