namespace LocalImageLiker.ConfigPanel {
    using Godot;

    public partial class CloseButton : Button {
        public CloseButton() => Pressed += LocalImageLiker.CloseConfigPanel;
    }
}
