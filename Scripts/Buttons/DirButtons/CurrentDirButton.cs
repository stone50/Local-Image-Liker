namespace LocalImageLiker.Buttons.DirButtons {
    using Godot;

    public partial class CurrentDirButton : Button {
        public CurrentDirButton() => Pressed += OnButtonPressed;

        private void OnButtonPressed() {
            LocalImageLiker.FileDialog.DirSelected += OnFileDialogDirSelected;
            LocalImageLiker.FileDialog.Canceled += OnFileDialogCanceled;
            LocalImageLiker.FileDialog.CurrentDir = LocalImageLiker.CurrentDirPath;
            LocalImageLiker.FileDialog.Show();
        }

        private void OnFileDialogDirSelected(string selectedDir) {
            LocalImageLiker.FileDialog.DirSelected -= OnFileDialogDirSelected;
            LocalImageLiker.FileDialog.Canceled -= OnFileDialogCanceled;
            LocalImageLiker.CurrentDirPath = selectedDir;
        }

        private void OnFileDialogCanceled() {
            LocalImageLiker.FileDialog.DirSelected -= OnFileDialogDirSelected;
            LocalImageLiker.FileDialog.Canceled -= OnFileDialogCanceled;
        }
    }
}
