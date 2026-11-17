# MyUnityGame Constitution

## Core Principles

### I. 遊べる最小単位で作る

- 各機能は、それ単体で Play して確認できる単位（ユーザーストーリー）に分けて実装しなければならない（MUST）。
- 最初のマイルストーンは「操作する・ボールを回収する・クリアする」が一通り遊べる状態とし、
  演出や追加ルールはその後に積み上げる。
- 1つのタスクが完了した時点で、プロジェクトはコンパイルエラーのない状態でなければならない（MUST）。

理由: 学習用の小規模ゲームでは、常に動く状態を保つことが手戻りを最も減らす。

### II. ゲームロジックはテストを先に書く（NON-NEGOTIABLE）

- 回収・スコア・クリア判定・初期化など、入力や描画に依存しないロジックは、
  MonoBehaviour から切り離したプレーンな C# クラスに置かなければならない（MUST）。
- そのロジックは Unity Test Framework の EditMode テストを先に書き、失敗を確認してから実装する
  （Red → Green → Refactor）。
- バグ修正では、まず再現するテストを追加してから修正しなければならない（MUST）。

理由: ロジックを分離すると、エディタを Play しなくても数秒で正しさを確かめられる。

### III. エディタ操作は unity-cli で再現できるようにする

- シーン作成、GameObject・コンポーネントの追加や変更、設定変更は、
  unity-cli（`.claude/skills/unity-*`）で行い、手順として記録できる形にしなければならない（MUST）。
- エディタの GUI だけで行った変更は、その内容を tasks.md またはコミットメッセージに書き残す。
- 作業の前に `unity-cli system ping` で CLI と Bridge のバージョン一致（`match`）を確認する。

理由: AI エージェントと人の両方が同じ手順を再実行・検証できるようにするため。

### IV. Play モードで実際に動かして確認する

- 入力・物理・UI が関わる機能は、unity-cli の Play モード操作（入力シミュレーション、
  コンソール確認、スクリーンショット等）で動作を確認してから完了としなければならない（MUST）。
- 完了の判断には、spec.md の受け入れ条件を1つずつ照合した結果を残す。
- Play 中にコンソールへ Error / Exception が出ている状態を完了としてはならない（MUST NOT）。

理由: EditMode テストでは、入力や衝突判定などの実行時の不具合は見つけられないため。

### V. シンプルさを優先する

- 今必要な機能だけを実装し、将来のための抽象化や汎用化は行わない（YAGNI）。
- 外部パッケージやアセットの追加は、標準機能で実現できない理由を plan.md に書いた場合に限る。
- スクリプトは1クラス1ファイルとし、責務が2つ以上になったら分割する。

理由: 小規模なゲームでは、構造の複雑さが学習と保守のコストに直結する。

## 技術的な制約

- エンジン: Unity 6（6000.6.4f1）。バージョンを上げる場合は本書の改定として扱う。
- 言語: C#。名前空間は `MyUnityGame` 配下に置く。
- 入力: 新しい Input System（`.inputactions` アセット）を使う。旧 Input Manager の API
  （`Input.GetKey` 等）は新規コードで使わない。
- テスト: Unity Test Framework（EditMode / PlayMode）。テストは `Assets/Tests/` 配下に置く。
- 配置: スクリプトは `Assets/Scripts/`、シーンは `Assets/Scenes/` に置く。
- 自動化: unity-cli / unity-cli-bridge 0.19.1。両者のバージョンを揃える。

## 開発フローと品質ゲート

- 機能は Spec Kit の順序で進める:
  `/speckit-specify` → （必要なら `/speckit-clarify`）→ `/speckit-plan` → `/speckit-tasks`
  → `/speckit-implement` → `/speckit-converge`。
- plan.md の Constitution Check では、原則 I〜V への適合を確認し、違反がある場合は理由を記録する。
- 機能を完了とする条件（すべて満たすこと）:
  1. コンパイルエラーがない
  2. EditMode テストがすべて成功している
  3. Play モードで受け入れ条件を確認済み（原則 IV）
  4. `code-review-and-quality` スキルでレビュー済み
- コミットは機能またはタスク単位で小さく行い、`main` へ直接大きな変更を積まない。

## Governance

- 本書はプロジェクト内の他のあらゆる慣習・手順より優先する。
- 改定は本ファイルを編集して行い、Sync Impact Report に変更点を記録したうえでコミットする。
- バージョンはセマンティックバージョニングに従う:
  原則の削除・再定義は MAJOR、原則や節の追加・大幅な拡充は MINOR、文言の修正は PATCH。
- 各機能の plan.md とレビュー時に、本書への適合を確認する。複雑さを持ち込む場合は理由を明記する。
- 日々の開発手順の詳細は `README.md` と `.claude/skills/` の各スキルを参照する。

**Version**: 1.0.0 | **Ratified**: 2026-11-17 | **Last Amended**: 2026-11-17
