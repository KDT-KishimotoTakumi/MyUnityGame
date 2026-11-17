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
