using System;

[AttributeUsage(AttributeTargets.Method)]
public class EditorButtonAttribute : Attribute
{
    public string Label { get; }
    public EditorButtonAttribute(string label = null) { Label = label; }
}

[AttributeUsage(AttributeTargets.Method)]
public class GameButtonAttribute : Attribute
{
    public string Label { get; }
    public GameButtonAttribute(string label = null) { Label = label; }
}
