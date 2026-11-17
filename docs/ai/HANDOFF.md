# Resume state

## 現在の作業

更新: 2026-11-17 Asia/Tokyo / ブランチ: `001-ballcollect-core` / 状態: B1〜B6 実装・E2E・カバレッジまで完了（T001〜T035）。ユーザー指示で「プレイ評価→改善」の1回目の途中で停止

- 仕様: [`spec.md`](../../specs/001-ballcollect-core/spec.md)（FR-015 E2E、FR-016 カバレッジ90%を追加）/ [`tasks.md`](../../specs/001-ballcollect-core/tasks.md) / [`quickstart.md`](../../specs/001-ballcollect-core/quickstart.md)
- 検証済み: EditMode 15/15（CollectionState 7、ColorProgress 8）、PlayMode E2E 12/12（`Assets/Tests/PlayMode/BallCollectE2ETests.cs`、シナリオ1〜7＋二重回収＋R長押し）。行カバレッジ 95.6%（87/91、ColorProgress 等の追加前の測定。[`evidence/coverage-summary.md`](../../specs/001-ballcollect-core/evidence/coverage-summary.md)）
- カバレッジ測定手順: Code Coverage 1.3.0。`CoveragePreferences.instance.SetBool("EnableCodeCoverage", true)`（リフレクション、`ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json` に保存）＋ `CompilationPipeline.codeOptimization = Debug` → EditMode と PlayMode を実行 → `CodeCoverage/Report/Summary.md`。終わったら EnableCodeCoverage=false・Release に戻す（現在は戻し済み）。`StartRecording` は使わない（記録が残って結果が分かれる）
- 重要: Play・PlayMode テストは Unity Editor が OS の前面にあるときだけ進む（scratchpad の fg.ps1：Alt キー送出＋SetForegroundWindow）
- ユーザーの新しい指示: 実装完了後にプレイして総評・フィードバック→修正→を繰り返し、B1〜B6 を変えずに独自性のあるゲームにする。作業の終わりにコミット・プル・プッシュ
- プレイ評価1回目: (1) 減速がなく W を0.3秒押すだけで全回収、(2) CLEAR 直後に勢いで落下し CLEAR が約1秒で消える、(3) 見た目が単調、(4) 回収の手応えがない、(5) 独自性がない。方針＝操作感の調整（Rigidbody の Damping と moveForce）＋「色を取り戻す」表現（灰色の世界が回収ごとに色づく、玉は虹の5色、球は直近の色＋軌跡、玉が脈動）
- 改善1回目の進み: `ColorProgress`（TDD 済み）、`WorldColorizer`・`ItemPulse`（コードのみ）、`GameManager.StateChanged` イベントを追加。**シーンへの配線は未着手**
- 次の操作: (1) シーン配線＝WorldColorizer を GameManager に追加して参照設定、Item_1〜5 に ItemPulse、Player に TrailRenderer（Sprites/Default のマテリアルを作成）、カメラを SolidColor、HUD の文字を大きく (2) Player の Rigidbody に linearDamping 約1.2・angularDamping、moveForce 約15 (3) E2E とカバレッジを再実行 (4) プレイ評価2回目 (5) spec に表現の要件を追記し tasks に Phase 9 を追加 (6) T036〜T040（画像の根拠、verification.md、レビュー、unity-check、コミット）
- 未コミットのまま残すもの: `ProjectSettings/ProjectSettings.asset`（Unity が既定値を書き出しただけの差分）
