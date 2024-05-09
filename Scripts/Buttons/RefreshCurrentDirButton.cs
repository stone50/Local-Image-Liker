namespace LocalImageLiker.Buttons {
    using Godot;

    public partial class RefreshCurrentDirButton : Button {
        public RefreshCurrentDirButton() => Pressed += () => LocalImageLiker.RefreshCurrentTextures();
    }
}
