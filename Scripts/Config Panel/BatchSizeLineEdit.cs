namespace LocalImageLiker.ConfigPanel {
    using Godot;

    public partial class BatchSizeLineEdit : LineEdit {
        public BatchSizeLineEdit() {
            Text = LocalImageLiker.BatchSize.ToString();

            TextSubmitted += OnTextSubmitted;
        }

        private void OnTextSubmitted(string newText) {
            int newBatchSize;
            try {
                newBatchSize = int.Parse(newText);
            } catch {
                Text = LocalImageLiker.BatchSize.ToString();
                return;
            }

            if (newBatchSize <= 0) {
                Text = LocalImageLiker.BatchSize.ToString();
                return;
            }

            Text = newBatchSize.ToString();
            LocalImageLiker.BatchSize = newBatchSize;
        }
    }
}
