namespace LocalImageLiker.Buttons {
    using Godot;

    public partial class LikeButton : Button {
        public LikeButton() => Pressed += LocalImageLiker.LikeCurrentImage;
    }
}
