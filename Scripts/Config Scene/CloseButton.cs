namespace LocalImageLiker.Scripts.Config_Scene {
    using Godot;

    public partial class CloseButton : Button {
        public CloseButton() => Pressed += LocalImageLiker.CloseConfigPanel;
    }
}
