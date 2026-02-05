## 🟦 導入

**各ライブラリの導入手順**を説明します。

## 🔸 UniTask / R3 の導入（OpenUPM）

### ① Git（Windows）を導入

他ライブラリ導入時にも必要になるため、導入します。

🔗https://git-scm.com/install/windows

### ② Unityプロジェクト設定

以下を設定します。

📂 `ProjectSettings → PackageManager → Scoped Registries`

```
Name:
OpenUPM

URL:
https://package.openupm.com

Scope:
org.nuget
com.cysharp
```
設定後、**Save**してください。

### ③ PackageManagerから導入

- `My Registries` を選択
- **UniTask / R3** を検索してインストール

- UniTask → **最新バージョン**
- R3 → **NuGet版 + 無印の両方をインストール**
- ObservableCollections → 調べてみて必要だと思ったときに導入してください。

## 🔸 DOTween の導入

1. **Asset Storeで購入**
2. PackageManagerからインポート
3. 表示されるタブに従い **SetUp**

### UniTask と DOTween の連携設定

📂 `ProjectSettings → OtherSettings → ScriptCompilation`

以下を設定：

```
UNITASK_DOTWEEN_SUPPORT
```
→ **Apply**
これで **UniTask × DOTween の連携が有効化**されます。

ビルド環境ごとに設定が分かれていることに注意してください。
