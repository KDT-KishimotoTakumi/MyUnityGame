# Resume state

## 現在の作業

更新: 2026-11-17 Asia/Tokyo / ブランチ: `001-ballcollect-core` / 状態: 001 BallCollect を `/speckit-implement` 中。MVP（US1・US2）完了＝T001〜T015 完了。次は T016（US3、テスト先行）

- 仕様: [`spec.md`](../../specs/001-ballcollect-core/spec.md) / [`plan.md`](../../specs/001-ballcollect-core/plan.md) / [`tasks.md`](../../specs/001-ballcollect-core/tasks.md) / [`quickstart.md`](../../specs/001-ballcollect-core/quickstart.md)。ユーザーが実装の継続を依頼済み。追加ルール（30秒制限など）は範囲外（002）。
- 完了: シーン `Assets/Scenes/BallCollect.unity`（Floor、Player＋Rigidbody＋`PlayerController`、Item_1〜5 Trigger、カメラ、HUD の CollectedText「0/5」・ClearText 非アクティブ）、asmdef 2つ、マテリアル3つ、`Assets/Input/BallCollectControls.inputactions`、`Assets/Scripts/PlayerController.cs`。コンパイルエラー 0。
- 検証済み: US1 の配置を画像で確認（[`evidence/us1-initial-state.png`](../../specs/001-ballcollect-core/evidence/us1-initial-state.png)）。T015：W/S/D/A を各1秒 → Z −7.00→5.66、Z −7.00→−20.28（床外へ落下）、X 0→12.49、X 0→−12.35。4方向とも期待どおり（verification.md は T029 で作る）。
- 重要: **Play のフレームは Unity Editor が OS の前面にあるときだけ進む**。バックグラウンドでは `Time.frameCount` が 1 のまま（Run In Background は効果なし、元に戻した）。Play 検証の前に PowerShell の `SetForegroundWindow`（Unity の MainWindowHandle）で前面に出し、`eval_csharp` で frameCount の増加を確かめる。`.agents/skills/unity-editor/SKILL.md` と quickstart に記載済み。
- C# の LSP 書き込み（`create_csharp_file`）は GitHub API の HTTP 403 で使えない。ファイルを直接書き、`refresh_assets` → `get_compilation_state` で確認している。複雑な JSON・C# は scratchpad のファイルから Python で JSON 化して渡すと安全。
- 未コミットのまま残すもの: `ProjectSettings/ProjectSettings.asset`（Run In Background を戻した後に Unity が既定値を書き出した差分のみ。機能上の変更なし）。
- Unity Editor: Play 停止済み、シーン保存済み。
- 次の操作: T016 `CollectionState` の骨組み → T017 テスト7件 → T018 失敗確認 → T019 実装して成功確認 → T020〜T024。
