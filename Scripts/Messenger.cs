namespace LocalImageLiker {
    using Godot;
    using System;

    public partial class Messenger : Control {
        public enum MessageType {
            Info,
            CallToAction,
            Warning,
            Error
        }

        private static readonly PackedScene MessageScene = GD.Load<PackedScene>("res://Scenes/Message.tscn");

        public static VBoxContainer MessageList { get; private set; } = null!;

        public override void _Ready() => MessageList = GetNode<VBoxContainer>("Message List");

        public static void SendMessage(MessageType messageType, string message) {
            GD.Print($"[{DateTime.UtcNow}] ({messageType}) {message}");

            var messagePanel = MessageScene.Instantiate<Message>();
            MessageList.AddChild(messagePanel);
            MessageList.MoveChild(messagePanel, 0);
            messagePanel.Label.Text = message;
            messagePanel.SelfModulate = GetMessageTypeColor(messageType);
        }

        private static Color GetMessageTypeColor(MessageType messageType) => messageType switch {
            MessageType.CallToAction => Colors.Blue,
            MessageType.Warning => Colors.Yellow,
            MessageType.Error => Colors.Red,
            _ => Colors.White,
        };
    }
}
