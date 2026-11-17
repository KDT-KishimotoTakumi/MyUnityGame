# Observation Contract: CLI から読む名前と値

FR-013 のための約束。検証（quickstart.md・verification.md）はこの名前だけを使う。名前を変える場合は本書も更新する。

## 状態の読み取り（`get_component_values`）

| 確認したいこと | gameObjectName | componentType | 読む値 |
|---|---|---|---|
| 球の位置 | `Player` | `Transform` | `position` |
| 球の速度 | `Player` | `Rigidbody` | `linearVelocity` |
| 回収数 | `GameManager` | `GameManager` | `collectedCount`（int、0〜5） |
| クリア状態 | `GameManager` | `GameManager` | `isCleared`（bool） |
| アイテムの有無 | `Item_1`〜`Item_5` | — | GameObject の `isActive`（`get_gameobject_details` / `get_hierarchy`） |

`GameManager` は `CollectionState` の値を毎フレーム `[SerializeField]` の `collectedCount` / `isCleared` に
書き写し、CLI から読めるようにする（読み取り専用。CLI からこの値を書き換えて検証してはならない）。

## UI（`find_ui_elements` / `get_ui_element_state`）

| 要素 | 名前 | 期待 |
|---|---|---|
| 回収数 | `CollectedText` | テキストが `"{collectedCount}/5"` |
| CLEAR | `ClearText` | `isCleared` が true のときだけアクティブ、文字列 `"CLEAR"` |

## 入力（`input_keyboard`）

| キー | アクション | 期待 |
|---|---|---|
| `w` / `s` / `d` / `a` | `Gameplay/Move` | +Z / −Z / +X / −X へ移動 |
| `r` | `Gameplay/Restart` | 初期化（data-model.md の初期値に戻る） |
