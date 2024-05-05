namespace LocalImageLiker {
    using Godot;

    public partial class Messenger : Control {
        private static readonly PackedScene MessageScene = GD.Load<PackedScene>("res://Scenes/Message.tscn");

        public static VBoxContainer MessageList { get; private set; } = null!;

        public override void _Ready() => MessageList = GetNode<VBoxContainer>("Message List");

        public static void SendInfo(string message) => AddMessage(message, Colors.White);

        public static void SendCallToAction(string message) => AddMessage(message, Colors.Blue);

        public static void SendWarning(string message) => AddMessage(message, Colors.Yellow);

        public static void SendError(string message) => AddMessage(message, Colors.Red);

        private static void AddMessage(string messageText, Color panelColor) {
            GD.Print(messageText);
            var messagePanel = MessageScene.Instantiate<Message>();
            MessageList.AddChild(messagePanel);
            MessageList.MoveChild(messagePanel, 0);
            messagePanel.Label.Text = messageText;
            messagePanel.SelfModulate = panelColor;
        }
    }
}
