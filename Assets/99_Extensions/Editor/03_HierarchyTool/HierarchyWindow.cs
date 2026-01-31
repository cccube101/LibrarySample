using UnityEditor;
using UnityEngine;

namespace Helper
{
    /// <summary>
    /// ヒエラルキー対象オブジェクトのカスタムプロパティ編集ウィンドウ
    /// </summary>
    public class HierarchyWindow : EditorWindow
    {
        private GameObject target;              // 編集対象 GameObject
        private OverlayData _data;              // 対応するオーバーレイデータ
        private string _cachedId;               // 高コストな GlobalObjectId をキャッシュ

        // 定数化：ウィンドウサイズとオフセット
        private static readonly Vector2 WINDOW_SIZE = new(300f, 150f);
        private const float VERTICAL_OFFSET = 134f;

        public static void ShowWindow(GameObject go, Rect hierarchyRect)
        {
            if (go == null) return;

            var window = CreateInstance<HierarchyWindow>();
            window.target = go;

            // GlobalObjectId をキャッシュして高コスト計算を避ける
            window._cachedId = GlobalObjectId.GetGlobalObjectIdSlow(go).ToString();

            // 既存ラベルデータを取得、なければデフォルト Normal を作成
            window._data = HierarchyOverlay.labelData.TryGetValue(window._cachedId, out var data)
                ? data
                : new OverlayData { type = Type.Normal };

            // ヒエラルキー座標 → スクリーン座標変換
            var screenPos = GUIUtility.GUIToScreenPoint(new Vector2(hierarchyRect.x, hierarchyRect.y - VERTICAL_OFFSET));
            var pos = new Rect(screenPos, WINDOW_SIZE);

            // ドロップダウンウィンドウとして表示
            window.ShowAsDropDown(pos, WINDOW_SIZE);
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical("box");

            // 対象が存在しない場合は警告表示
            if (target == null)
            {
                EditorGUILayout.HelpBox("No Target", MessageType.Warning);
                GUILayout.EndVertical();
                return;
            }

            // タイトル表示
            EditorGUIEx.DrawSubTitle($"Target [ {target.name} ]");
            EditorGUILayout.Space();
            EditorGUIEx.DrawSeparator();

            // =========================
            // タグ編集
            // =========================
            string newTag = EditorGUILayout.TagField("Tag", target.tag);

            // タグが変更された場合のみ反映
            if (!target.CompareTag(newTag))
            {
                Undo.RecordObject(target, "Change Tag"); // Undo 登録
                target.tag = newTag;
                EditorUtility.SetDirty(target);           // 変更を通知
            }

            // ボタンで EditorOnly タグを強制設定
            if (GUILayout.Button("Set EditorOnly"))
            {
                Undo.RecordObject(target, "Set EditorOnly Tag");
                target.tag = "EditorOnly";
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.Space();
            EditorGUIEx.DrawSeparator();

            // =========================
            // Overlay プロパティ編集
            // =========================
            EditorGUI.BeginChangeCheck();

            // 種類をドロップダウンで変更
            _data.type = (Type)EditorGUILayout.EnumPopup("Draw Type", _data.type);

            // 種類ごとにプロパティを表示
            switch (_data.type)
            {
                case Type.Header:
                    _data.headerBarColor = EditorGUILayout.ColorField("Bar Color", _data.headerBarColor);
                    _data.headerTextColor = EditorGUILayout.ColorField("Text Color", _data.headerTextColor);
                    break;
                case Type.Separator:
                    _data.separatorLineColor = EditorGUILayout.ColorField("Line Color", _data.separatorLineColor);
                    _data.separatorLineHeight = EditorGUILayout.FloatField("Line Height", _data.separatorLineHeight);
                    break;
                case Type.Normal:
                    EditorGUILayout.Space(35f);
                    break;
            }

            // 変更があれば反映
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(HierarchyOverlay.asset, "Update LabelData");
                HierarchyOverlay.labelData[_cachedId] = _data;
                HierarchyOverlay.SaveData();
            }

            GUILayout.EndVertical();
        }

        private void OnDisable()
        {
            // ウィンドウ閉じるときに確実に保存
            if (target == null) return;
            HierarchyOverlay.labelData[_cachedId] = _data;
            HierarchyOverlay.SaveData();
        }
    }
}
