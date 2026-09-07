using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PresentationGraphNodeView : Node
{
    public PresentationNodeData Data { get; private set; }

    public List<Port> InputPorts { get; private set; } = new();
    public List<Port> OutputPorts { get; private set; } = new();

    private EnumField _typeField;
    private TextField _titleField;

    private TextField _param1Field;
    private TextField _param2Field;
    private TextField _param3Field;

    public PresentationGraphNodeView(PresentationNodeData data)
    {
        Data = data;

        viewDataKey = data.guid;

        title = string.IsNullOrEmpty(data.title)
            ? data.nodeType.ToString()
            : data.title;

        style.left = data.position.x;
        style.top = data.position.y;

        CreatePorts();
        CreateContents();

        RefreshExpandedState();
        RefreshPorts();
    }

    private void CreatePorts()
    {
        InputPorts.Clear();
        OutputPorts.Clear();

        switch (Data.nodeType)
        {
            case PresentationNodeType.Start:
                AddOutputPort("Out", Port.Capacity.Single);
                break;

            case PresentationNodeType.End:
                AddInputPort("In", Port.Capacity.Multi);
                break;

            case PresentationNodeType.Fork:
                AddInputPort("In", Port.Capacity.Single);
                AddOutputPort("Out", Port.Capacity.Multi);
                break;

            case PresentationNodeType.Join:
                AddInputPort("In", Port.Capacity.Multi);
                AddOutputPort("Out", Port.Capacity.Single);
                break;

            case PresentationNodeType.Branch:
                AddInputPort("In", Port.Capacity.Single);
                AddOutputPort("True", Port.Capacity.Single);
                AddOutputPort("False", Port.Capacity.Single);
                break;

            case PresentationNodeType.Choice:
                AddInputPort("In", Port.Capacity.Single);
                AddOutputPort("Option1", Port.Capacity.Single);
                AddOutputPort("Option2", Port.Capacity.Single);
                break;

            default:
                AddInputPort("In", Port.Capacity.Single);
                AddOutputPort("Out", Port.Capacity.Single);
                break;
        }
    }

    private Port AddInputPort(string name, Port.Capacity capacity)
    {
        var port = InstantiatePort(
            Orientation.Horizontal,
            Direction.Input,
            capacity,
            typeof(bool));

        port.portName = name;

        inputContainer.Add(port);
        InputPorts.Add(port);

        return port;
    }

    private Port AddOutputPort(string name, Port.Capacity capacity)
    {
        var port = InstantiatePort(
            Orientation.Horizontal,
            Direction.Output,
            capacity,
            typeof(bool));

        port.portName = name;

        outputContainer.Add(port);
        OutputPorts.Add(port);

        return port;
    }

    private void CreateContents()
    {
        _typeField = new EnumField("Type", Data.nodeType);
        _typeField.SetEnabled(false); // 수정 불가

        extensionContainer.Add(_typeField);

        _titleField = new TextField("Title")
        {
            value = Data.title
        };

        _titleField.RegisterValueChangedCallback(evt =>
        {
            Data.title = evt.newValue;

            title = string.IsNullOrEmpty(evt.newValue)
                ? Data.nodeType.ToString()
                : evt.newValue;
        });

        extensionContainer.Add(_titleField);

        _param1Field = new TextField("Param1")
        {
            value = Data.param1
        };
        _param1Field.RegisterValueChangedCallback(evt => Data.param1 = evt.newValue);
        extensionContainer.Add(_param1Field);

        _param2Field = new TextField("Param2")
        {
            value = Data.param2
        };
        _param2Field.RegisterValueChangedCallback(evt => Data.param2 = evt.newValue);
        extensionContainer.Add(_param2Field);

        _param3Field = new TextField("Param3")
        {
            value = Data.param3
        };
        _param3Field.RegisterValueChangedCallback(evt => Data.param3 = evt.newValue);
        extensionContainer.Add(_param3Field);
    }

    private void RebuildPorts()
    {
        inputContainer.Clear();
        outputContainer.Clear();

        CreatePorts();

        RefreshPorts();
        RefreshExpandedState();
    }

    public void UpdateNodePosition()
    {
        Data.position = GetPosition().position;
    }
}