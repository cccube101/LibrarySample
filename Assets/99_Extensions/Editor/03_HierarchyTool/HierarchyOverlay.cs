using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Helper
{
    /// <summary>
    /// ヒエラルキー上にカスタムオーバーレイ（ヘッダー・セパレーターなど）を描画・保存するエディタ拡張
    /// シーンごとに専用の HierarchyData.asset を生成・管理する
    /// </summary>
    [InitializeOnLoad]
    public static class HierarchyOverlay
    {
        private const string DATA_FOLDER = "Assets/99_Extensions/Editor/03_HierarchyTool/Data/";
        private const string DEFAULT_SCENE_NAME = "UntitledScene";

        private const float HEADER_EXTRA_WIDTH = 10f;              // ヘッダーのバー描画幅の余白
        private const float SEPARATOR_BACK_ALPHA = 0.21f;          // セパレーター背景色のアルファ

        private static GUIStyle headerStyle;                      // ヘッダー用ラベルスタイル

        public static HierarchyOverlayData asset;                 // 現在シーン用のデータ
        public static Dictionary<string, OverlayData> labelData = new(); // オーバーレイデータキャッシュ
        private static bool isLoaded = false;                     // 初期化済みフラグ

        static HierarchyOverlay()
        {
            // 初期化処理
            Initialize();

            // シーン切り替え時に保存・リロード
            EditorSceneManager.activeSceneChangedInEditMode += (oldScene, newScene) =>
            {
                SaveData();       // 既存データを保存
                asset = null;     // キャッシュクリア
                isLoaded = false;
                Load();           // 新しいシーンのデータをロード
            };

            // エディタ終了時に保存
            EditorApplication.quitting -= SaveData;
            EditorApplication.quitting += SaveData;
        }

        [InitializeOnLoadMethod]
        private static void EnsureInitialize() => Initialize();

        private static void Initialize()
        {
            if (!isLoaded) Load();
            isLoaded = true;

            // ヒエラルキー描画イベントの重複登録防止
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        /// <summary>
        /// 現在アクティブなシーンの HierarchyOverlayData をロード
        /// 存在しなければ新規作成
        /// </summary>
        private static void Load()
        {
            if (asset != null) return;

            // 保存フォルダを作成
            EnsureFolderExists(DATA_FOLDER);

            string sceneName = SceneManager.GetActiveScene().name;
            if (string.IsNullOrEmpty(sceneName)) sceneName = DEFAULT_SCENE_NAME;

            string assetPath = $"{DATA_FOLDER}HierarchyData_{sceneName}.asset";

            // 既存アセットをロード
            asset = AssetDatabase.LoadAssetAtPath<HierarchyOverlayData>(assetPath);
            if (asset == null)
            {
                // 存在しなければ新規作成
                asset = ScriptableObject.CreateInstance<HierarchyOverlayData>();
                AssetDatabase.CreateAsset(asset, assetPath);
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssets();
            }

            // Dictionary に変換してキャッシュ
            labelData = asset.ToDictionary();
        }

        /// <summary>
        /// 指定フォルダが存在しない場合、親から再帰的に作成
        /// </summary>
        private static void EnsureFolderExists(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath)) return;

            string parent = System.IO.Path.GetDirectoryName(folderPath).Replace("\\", "/");
            string folderName = System.IO.Path.GetFileName(folderPath);

            // 親フォルダも存在しなければ再帰的に作成
            EnsureFolderExists(parent);

            AssetDatabase.CreateFolder(parent, folderName);
        }

        /// <summary>
        /// ScriptableObject にデータを反映して保存
        /// </summary>
        public static void SaveData()
        {
            if (asset == null) return;

            asset.FromDictionary(labelData);
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// ヒエラルキー上でオーバーレイ描画
        /// </summary>
        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            if (!isLoaded || asset == null) { Initialize(); if (!isLoaded) return; }

            GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (obj == null) return;

            string id = GlobalObjectId.GetGlobalObjectIdSlow(obj).ToString();

            // 登録されていなければ描画しない
            if (!labelData.TryGetValue(id, out var data)) return;

            switch (data.type)
            {
                case Type.Normal: return; // 通常は描画なし
                case Type.Header: DrawHeader(selectionRect, obj.name, data); break;
                case Type.Separator: DrawSeparator(selectionRect, data); break;
            }
        }

        private static void DrawHeader(Rect rect, string name, OverlayData data)
        {
            // ヘッダー背景描画
            Rect colorRect = new(rect.x, rect.y, rect.width + HEADER_EXTRA_WIDTH, rect.height);
            EditorGUI.DrawRect(colorRect, data.headerBarColor);

            // ヘッダーテキスト描画
            EditorGUI.LabelField(rect, name, GetHeaderStyle(data.headerTextColor));
        }

        private static void DrawSeparator(Rect rect, OverlayData data)
        {
            // 背景描画
            Rect backRect = new(rect.x, rect.y, rect.width + HEADER_EXTRA_WIDTH, rect.height);
            EditorGUI.DrawRect(backRect, new Color(SEPARATOR_BACK_ALPHA, SEPARATOR_BACK_ALPHA, SEPARATOR_BACK_ALPHA));

            // 線描画
            Rect lineRect = new(rect.x, rect.y + rect.height / 2, rect.width, data.separatorLineHeight);
            EditorGUI.DrawRect(lineRect, data.separatorLineColor);
        }

        private static GUIStyle GetHeaderStyle(Color textColor)
        {
            // 1度だけ作成し再利用
            if (headerStyle == null)
            {
                headerStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter
                };
            }
            headerStyle.normal.textColor = textColor;
            return headerStyle;
        }
    }
}
