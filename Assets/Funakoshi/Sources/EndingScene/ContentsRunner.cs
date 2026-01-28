using System;
using UnityEngine;

public class ContentsRunner
{
    private ContentManagement contents;

    public void SetAndRun(ContentManagement contents)
    {
        if (contents != null)
            throw new InvalidOperationException("ContentsRunnerには既にContentManagerが設定されています");
        if (contents == null)
            throw new ArgumentNullException("ContentsRunnerに渡されたContentManagerがnullでした");

        // Set
        this.contents = contents;

        // Run
        contents.RunFirstContent();
    }

    public void Update()
    {
        if (contents == null)
            throw new NullReferenceException("ContentsRunnerにContentManagerが設定されていません");

        // マウスを押すと次の停止点までスキップします
        if (Input.GetMouseButtonDown(0))
        {
            contents.SkipToNextStop();
        }
    }
}

