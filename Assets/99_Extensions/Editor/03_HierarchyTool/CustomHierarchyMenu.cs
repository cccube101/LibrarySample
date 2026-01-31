using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Helper
{
    /// <summary>
    /// ヒエラルキー右クリックメニュー拡張
    /// </summary>
    public class CustomHierarchyMenu
    {
        // メニューアイテム優先順位
        private const int MENU_PRIORITY = 10;

        [MenuItem("GameObject/C3/Create Header", false, MENU_PRIORITY)]
        private static void CreateHeader(MenuCommand menuCommand) => Create(menuCommand, Type.Header);

        [MenuItem("GameObject/C3/Create Separator", false, MENU_PRIORITY)]
        private static void CreateSeparator(MenuCommand menuCommand) => Create(menuCommand, Type.Separator);

        /// <summary>
        /// 指定タイプのオブジェクトを作成し Overlay に登録
        /// </summary>
        private static void Create(MenuCommand menuCommand, Type type)
        {
            // オブジェクト名をタイプごとに決定
            string name = type switch
            {
                Type.Header => "Header",
                Type.Separator => "Separator",
                _ => "Unknown",
            };

            // 新規 GameObject 作成
            GameObject obj = new(name);

            // EditorOnly タグが存在すれば付与
            if (UnityEditorInternal.InternalEditorUtility.tags.Contains("EditorOnly"))
                obj.tag = "EditorOnly";

            // 親オブジェクトが指定されていればアラインして配置
            GameObjectUtility.SetParentAndAlign(obj, menuCommand.context as GameObject);

            // Undo に登録
            Undo.RegisterCreatedObjectUndo(obj, $"Create {name}");
            if (menuCommand.context is GameObject parent)
                Undo.SetTransformParent(obj.transform, parent.transform, "Set Parent");

            // Header は作成時に選択状態にする
            if (type == Type.Header)
                Selection.activeObject = obj;

            // Overlay データ登録（未登録の場合のみ）
            string id = GlobalObjectId.GetGlobalObjectIdSlow(obj).ToString();
            if (!HierarchyOverlay.labelData.ContainsKey(id))
                HierarchyOverlay.labelData[id] = new OverlayData { type = type };

            // データ保存
            HierarchyOverlay.SaveData();
        }
    }
}
