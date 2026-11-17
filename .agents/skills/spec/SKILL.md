---
name: spec
description: MyUnityGameの大きな機能追加・仕様変更・大きな修正、または明示的な仕様化の依頼で、Spec Kitを使って仕様→承認→計画/タスク→実装→検証を進める入口。小さな修正には使わない。
---

# 仕様ワークフロー（Spec Kit）

各段階の手順は Spec Kit の各スキル（`speckit-*`）が正本。必要な段階のスキルだけを読む。

1. `specs/` の既存番号を確認し、対象機能を特定する（新規は `/speckit-specify`、`specs/NNN-short-name/`）。再開時は `docs/ai/HANDOFF.md` の現在状態と対象 `spec.md` を照合する。
2. 重要な未決事項は推測で埋めず、結果を左右する質問を1つずつ聞き、回答をすぐ `spec.md` に反映する（`/speckit-clarify`）。
3. 未決事項がなくなったら、保存済みの `spec.md` と検証方法（quickstart）を提示し、ユーザーの明示的な承認を得る。承認は `spec.md` の Status 行に日付と承認範囲で記録する。依頼文や過去回答から承認を推測しない。spec変更後は承認をやり直す。
4. 承認後に `/speckit-plan` と `/speckit-tasks` で計画と検証可能な小タスクを作り、`/speckit-implement` で実装する。
5. 各タスク完了ごとに `unity-verify` の手順で確認し、`tasks.md` にチェックを付ける。中断・段階移行時は `handoff` で HANDOFF を更新する。
