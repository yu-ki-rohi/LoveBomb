using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class TimelineNode : Node
{
    // 定数
    private const float NODE_HEIGHT = 140f;
    private const float MIN_WIDTH = 50f;
    private const float WIDTH_PER_SECOND = 100f;

    // メンバー
    private readonly DurationField actionDuration = new();
    public float ActionDuration
    {
        get => actionDuration.GetValue();
        set => actionDuration.SetValue(value);
    }

    // コンストラクタ
    public TimelineNode(string title)
    {
        this.title = title;

        actionDuration.OnValueChanged += Update;

        extensionContainer.Add(actionDuration);

        Update(); // 初期サイズ設定
    }

    private void Update()
    {
        UpdateNodeSize();

    }

    private void UpdateNodeSize()
    {
        // 1秒 = 100pxとして幅を変更
        var newWidth = Mathf.Max(MIN_WIDTH, ActionDuration * WIDTH_PER_SECOND);

        SetPosition(new Rect(GetPosition().position, new Vector2(newWidth, NODE_HEIGHT)));
    }
}
