# Resume state

## 現在の作業

更新: 2026-11-17 Asia/Tokyo / ブランチ: `001-ballcollect-core` / 状態: 001 BallCollect 基本ゲームを `/speckit-implement` 中。T001〜T014 完了、T015（WASD の Play 検証）で停止中

- 仕様: [`spec.md`](../../specs/001-ballcollect-core/spec.md) / [`plan.md`](../../specs/001-ballcollect-core/plan.md) / [`tasks.md`](../../specs/001-ballcollect-core/tasks.md) / [`quickstart.md`](../../specs/001-ballcollect-core/quickstart.md)。ユーザーが `/speckit-tasks` → `/speckit-implement` を実行して実装開始を依頼済み。追加ルール（30秒制限など）は範囲外（002 で扱う）。
- 完了: シーン `Assets/Scenes/BallCollect.unity`（Floor 20×20、Player (0,0.5,−7) Rigidbody＋tag Player、Item_1〜5 Trigger、Main Camera (0,18,−16)/(50,0,0)、HUD Canvas と CollectedText「0/5」・ClearText 非アクティブ）、asmdef 2つ、マテリアル3つ、`Assets/Input/BallCollectControls.inputactions`（Gameplay: Move=WASD 2D composite、Restart=R）、`Assets/Scripts/PlayerController.cs`（Player に追加、inputActions 設定済み、シーン保存済み）。コンパイルエラー 0。US1 のスクリーンショットで配置を目視確認済み。
- unity-cli の LSP 書き込み（`create_csharp_file`）は GitHub API の HTTP 403（csharp-lsp の取得失敗）で使えなかった。C# はファイルを直接書き、`refresh_assets` → `get_compilation_state` で確認している。
- ブロッカー（T015）: Play 中に `input_keyboard` で W を押すと、デバイス状態では W が押されているが球が動かない。`eval_csharp` で調べると `Time.frameCount=1`、`Time.time=0` のままで、Play のフレームが進んでいない（`isPaused=false`、`timeScale=1`、`Application.isFocused=True`）。ゲーム側の `Keyboard.current.wKey.isPressed` は False、Move アクションの値は (0,0)。入力設定は `backgroundBehavior=ResetAndDisableNonBackgroundDevices`、`editorInputBehaviorInPlayMode=PointersAndKeyboardsRespectGameViewFocus`。
- 次の操作: (1) フレームが進まない原因を調べる（Editor がバックグラウンドで更新を止めていないか、`EditorApplication.isPlaying` 中に `Time.frameCount` が増えるか時間をおいて再測定、Game View のフォーカス）。(2) フレームが進んだら、入力が届くか確認（Game View にフォーカスしてから入力、必要なら Input System の Play Mode 入力設定を確認）。(3) T015 を quickstart シナリオ 2 で完了させ、T016 以降（US3 はテスト先行）へ進む。
- Unity Editor: Play は停止済み（`isPlaying=false`）。シーンは保存済み。
- 未コミット: `Assets/` 一式、`ProjectSettings/SceneTemplateSettings.json`、`specs/001-ballcollect-core/tasks.md`。
