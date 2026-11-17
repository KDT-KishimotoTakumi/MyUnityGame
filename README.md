# MyUnityGame

Unity 6 (6000.6.4f1) で作成しているゲームプロジェクトです。

## 開き方
1. Unity Hub で「Add」→「Add project from disk」を選び、このフォルダを指定します。
2. エディタのバージョンは 6000.6.4f1 を使います。

## AIエージェント用スキル
`.claude/skills/` に [akiojin/unity-cli](https://github.com/akiojin/unity-cli) v0.19.1（MIT License）のスキルから、このゲームで使う11個を入れています。

| 用途 | スキル |
|---|---|
| 接続・診断 | unity-cli-usage, unity-editor-tools |
| シーン作成・確認 | unity-scene-create, unity-scene-inspect, unity-gameobject-edit |
| C# | unity-csharp-edit, unity-csharp-navigate |
| 入力・Playでの確認 | unity-input-system, unity-playmode-testing, unity-ui-automation, unity-development-loop |

CLI本体とUnity側のBridgeは次のコマンドで入れます（Windows PowerShell）。

```powershell
$env:UNITY_CLI_VERSION = 'v0.19.1'; irm https://raw.githubusercontent.com/akiojin/unity-cli/v0.19.1/scripts/install.ps1 | iex
unity-cli --output json setup --launch-editor
unity-cli --output json system ping   # versionCheck.status が match なら接続OK
```

### 開発全般のスキル
[KDT-KishimotoTakumi/AI-Development-Skills](https://github.com/KDT-KishimotoTakumi/AI-Development-Skills) の8個のうち、このプロジェクトに合う5個を入れています（出典は addyosmani/agent-skills、MIT License）。補助資料は `.claude/references/` にあります。

| スキル | このゲームでの用途 |
|---|---|
| test-driven-development | 回収・クリア・初期化ロジックのEditModeテスト（実習⑧） |
| debugging-and-error-recovery | compile errorや「動かない」の原因調査 |
| code-review-and-quality | 追加ルール実装後のレビュー |
| code-simplification | 挙動を変えずに整理する |
| context-engineering | requirements / plan / verification の引継ぎ |

C++用（cpp-coding-standards, cpp-testing）と、別途MCP実行環境が必要な context-mode は入れていません。

## Spec Kit（仕様駆動開発）
[github/spec-kit](https://github.com/github/spec-kit) v1.4.1 を Claude 連携・PowerShellスクリプトで初期化しています（`.specify/` と `.claude/skills/speckit-*`）。
Claude Code のチャットで次の順に使います。

```text
/speckit-constitution  → /speckit-specify → /speckit-clarify（任意）→ /speckit-plan → /speckit-tasks → /speckit-implement → /speckit-converge
```
