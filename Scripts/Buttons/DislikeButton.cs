namespace LocalImageLiker.Buttons {
    using Godot;

    public partial class DislikeButton : Button {
        public DislikeButton() => Pressed += LocalImageLiker.DislikeCurrentImage;
    }
}
