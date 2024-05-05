namespace LocalImageLiker.Buttons {
    using Godot;

    public partial class SkipButton : Button {
        public SkipButton() => Pressed += LocalImageLiker.SkipCurrentImage;
    }
}
