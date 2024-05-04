namespace LocalImageLiker {
    using Godot;

    public partial class LikeDirButton : Button {
        public LikeDirButton() => Pressed += OnButtonPressed;

        private void OnButtonPressed() {
            LocalImageLiker.FileDialog.DirSelected += OnFileDialogDirSelected;
            LocalImageLiker.FileDialog.Canceled += OnFileDialogCanceled;
            LocalImageLiker.FileDialog.CurrentDir = LocalImageLiker.LikeDirPath;
            LocalImageLiker.FileDialog.Show();
        }

        private void OnFileDialogDirSelected(string selectedDir) {
            LocalImageLiker.FileDialog.DirSelected -= OnFileDialogDirSelected;
            LocalImageLiker.FileDialog.Canceled -= OnFileDialogCanceled;
            LocalImageLiker.LikeDirPath = selectedDir;
        }

        private void OnFileDialogCanceled() {
            LocalImageLiker.FileDialog.DirSelected -= OnFileDialogDirSelected;
            LocalImageLiker.FileDialog.Canceled -= OnFileDialogCanceled;
        }
    }
}
