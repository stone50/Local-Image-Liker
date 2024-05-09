namespace LocalImageLiker.Buttons {
    using Godot;

    public partial class ResetSkippedImagesButton : Button {
        public ResetSkippedImagesButton() => Pressed += LocalImageLiker.ResetSkippedImages;
    }
}
