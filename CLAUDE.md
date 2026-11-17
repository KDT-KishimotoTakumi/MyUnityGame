@AGENTS.md

## Claude Code specifics
- unity-cli は `%USERPROFILE%\.unity\tools\unity-cli\win-x64\unity-cli.exe`。シェルの PATH に無ければ追加して使う。長い Unity 操作はバックグラウンドで実行して完了通知を待ち、同じ変更系の操作を二重に送らない。
- Project skills: `unity-verify`（変更に合う最小限の検証）、`unity-editor`（unity-cli での Editor 操作の共通ルール）、`spec`（大きな機能の入口。手順は Spec Kit の `speckit-*`）、`handoff`（節目の引き継ぎと再開）。unity-cli の `unity-*` スキル、開発全般の `test-driven-development` などは、その作業に当てはまるときだけ読み込む。他のスキルを前もって読み込まない。
- Agents: `unity-check`（読み取り専用の接続・シーン・コンソール点検）、`code-task`（場所が分かっている小さな修正）。広い作業・大きな機能・Editor を変更する作業は通常のセッションで行う。定型作業にサブエージェントを使わない。
- Context hygiene: ユーザーは定期的にセッションをクリアする。自然な区切り（`tasks.md` のフェーズやユーザーストーリーの完了、段階の切り替え、無関係な作業へ移る前）で、Play を止めて保存を確認し、`handoff` スキルで `docs/ai/HANDOFF.md` を更新し、ユーザーの流れに沿ってコミットしてから、`/clear` して「HANDOFF を読んで続きから」で再開するのに良いタイミングだと伝える。`/clear` の後は HANDOFF の「現在の作業」節と、そこが指すファイルだけを読んで再開する。
- MCP サーバーは常駐登録しない（ツールのスキーマをコンテキストに載せないため）。Editor の操作は unity-cli の CLI で行う。
