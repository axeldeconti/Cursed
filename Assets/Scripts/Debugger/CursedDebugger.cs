using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class CursedDebugger : Singleton<CursedDebugger>
{
    [SerializeField] private TextMeshProUGUI _text = null;

    private List<DebuggerInfo> _debuggerInfoList = null;

    protected override void Awake()
    {
        base.Awake();

        _debuggerInfoList = new List<DebuggerInfo>();
        Clear();
    }

    private void LateUpdate()
    {
        //Clear();

        StringBuilder builder = new StringBuilder();

        foreach (var i in _debuggerInfoList)
        {
            builder.Append(i.Line());
            builder.AppendLine();
        }

        _text.text = builder.ToString();
    }

    public void Clear()
    {
        _text.ClearMesh();
    }

    public void Add(string name, Func<string> info)
    {
        _debuggerInfoList.Add(new DebuggerInfo(name, info, Color.white));
    }

    public void Add(string name, Func<string> info, Color color)
    {
        _debuggerInfoList.Add(new DebuggerInfo(name, info, color));
    }
}

public struct DebuggerInfo
{
    public string name;
    public Func<string> info;
    public Color color;

    public DebuggerInfo(string name, Func<string> info, Color color)
    {
        this.name = name;
        this.info = info;
        this.color = color;
    }

    public string Line()
    {
        string hexColour = ColorUtility.ToHtmlStringRGB(color);
        return $"<color=#{hexColour}>{name + " : " + info.Invoke()}</color>";
    }
}