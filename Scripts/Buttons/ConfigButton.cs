namespace LocalImageLiker.Scripts.Buttons {
    using Godot;

    public partial class ConfigButton : Button {
        public ConfigButton() => Pressed += LocalImageLiker.OpenConfigPanel;
    }
}
