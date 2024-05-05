namespace LocalImageLiker.Buttons.DirButtons {
    using Godot;

    public partial class DislikeDirButton : Button {
        public DislikeDirButton() => Pressed += OnButtonPressed;

        private void OnButtonPressed() {
            LocalImageLiker.FileDialog.DirSelected += OnFileDialogDirSelected;
            LocalImageLiker.FileDialog.Canceled += OnFileDialogCanceled;
            LocalImageLiker.FileDialog.CurrentDir = LocalImageLiker.DislikeDirPath;
            LocalImageLiker.FileDialog.Show();
        }

        private void OnFileDialogDirSelected(string selectedDir) {
            LocalImageLiker.FileDialog.DirSelected -= OnFileDialogDirSelected;
            LocalImageLiker.FileDialog.Canceled -= OnFileDialogCanceled;
            LocalImageLiker.DislikeDirPath = selectedDir;
        }

        private void OnFileDialogCanceled() {
            LocalImageLiker.FileDialog.DirSelected -= OnFileDialogDirSelected;
            LocalImageLiker.FileDialog.Canceled -= OnFileDialogCanceled;
        }
    }
}
