using UnityEngine;

public static class UIHelper
{
    public class RectParam
    {
        public GameObject obj;
        public RectTransform rect;

        public RectParam(GameObject obj)
        {
            this.obj = obj;
            rect = obj.GetComponent<RectTransform>();
        }
    }

    public class GroupParam
    {
        public GameObject obj;
        public CanvasGroup group;
        public GroupParam(GameObject obj)
        {
            this.obj = obj;
            group = obj.GetComponent<CanvasGroup>();
        }
    }

    private readonly struct Scale
    {
        public readonly float value;
        public readonly string suffix;

        public Scale(float value, string suffix)
        {
            this.value = value;
            this.suffix = suffix;
        }
    }

    private readonly static Scale[] _scales = new[]
    {
        new Scale(1_000_000_000_000f, "T"),
        new Scale(1_000_000_000f, "G"),
        new Scale(1_000_000f, "M"),
        new Scale(1_000f, "K"),
    };

    public static string GetSuffix(float value)
    {
        foreach (var scale in _scales)
        {
            if (value >= scale.value)
            {
                var v = value / scale.value;
                // Ú”ö«—L‚è
                return $"{v:0.0}{scale.suffix}";
            }
        }
        // Ú”ö«–³‚µ
        return $"{value:0}";
    }
}