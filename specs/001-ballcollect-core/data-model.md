# Data Model: BallCollect 基本ゲーム

## CollectionState（純粋 C#、テスト対象）

| フィールド | 型 | 初期値 | 説明 |
|---|---|---|---|
| `TotalCount` | int（読み取り専用） | 5 | アイテム総数 |
| `CollectedCount` | int | 0 | 回収済みの数（0〜TotalCount） |
| `IsCleared` | bool | false | `CollectedCount == TotalCount` のときのみ true |
| （内部）回収済み ID の集合 | 整数の集合 | 空 | 二重加算防止に使う |

**操作**

- `bool TryCollect(int itemId)`：範囲外 ID（0 未満・TotalCount 以上）または回収済み ID なら何もせず false。
  それ以外は集合に追加し、`CollectedCount` を1増やして true。
- `void Reset()`：集合を空にし、`CollectedCount = 0`、`IsCleared = false`。

**検証ルール**: `0 ≤ CollectedCount ≤ TotalCount`。`IsCleared ⇔ CollectedCount == TotalCount`。

## 状態遷移

```text
Playing(n<5) --TryCollect(新ID)--> Playing(n+1) / Cleared(n+1==5)
Playing(n)   --TryCollect(既存ID)--> Playing(n)（変化なし）
Playing / Cleared --落下 or R--> Playing(0)  ※位置・速度・アイテム・HUD も初期化
Cleared      --移動--> Cleared（落下するまで CLEAR のまま）
```

## シーン上のエンティティ

| エンティティ | GameObject 名 | 主な構成 | 初期値 |
|---|---|---|---|
| 床 | `Floor` | Plane（scale 2,1,2 = 20×20）、Floor.mat | 位置 (0,0,0) |
| プレイヤー | `Player` | Sphere、Rigidbody、`PlayerController`、タグ `Player`、Player.mat | 位置 (0,0.5,−7)、速度 0 |
| アイテム | `Item_1`〜`Item_5` | Sphere（scale 0.6）、Trigger Collider、`CollectibleItem`（`itemId` 0〜4）、Item.mat | Z = −4, −1, 2, 5, 8（X=0, Y=0.5）、アクティブ |
| ゲーム管理 | `GameManager` | `GameManager`（player・items・hud・inputActions を参照） | 回収数 0、未クリア |
| HUD | `HUD` | Canvas（Overlay）、`HudView` | — |
| 回収数表示 | `HUD/CollectedText` | Text | 「0/5」 |
| CLEAR 表示 | `HUD/ClearText` | Text | 非アクティブ |
| カメラ | `Main Camera` | 既存を流用 | 位置 (0,18,−16)、回転 (50,0,0) |
| ライト | `Directional Light` | 既存を流用 | 変更なし |
