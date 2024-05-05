namespace LocalImageLiker {
    using Godot;

    public partial class Message : PanelContainer {
        public enum FadeState {
            FadingIn,
            Living,
            FadingOut
        }

        public Label Label { get; private set; } = null!;
        public Timer FadeInTimer { get; private set; } = null!;
        public Timer LifeTimer { get; private set; } = null!;
        public Timer FadeOutTimer { get; private set; } = null!;

        public FadeState CurrentFadeState { get; private set; } = FadeState.FadingIn;

        public override void _Ready() {
            Label = GetNode<Label>("MarginContainer/Label");
            FadeInTimer = GetNode<Timer>("Fade In Timer");
            LifeTimer = GetNode<Timer>("Life Timer");
            FadeOutTimer = GetNode<Timer>("Fade Out Timer");

            FadeInTimer.Timeout += () => {
                CurrentFadeState = FadeState.Living;
                LifeTimer.Start();
            };

            LifeTimer.Timeout += () => {
                CurrentFadeState = FadeState.FadingOut;
                FadeOutTimer.Start();
            };

            FadeOutTimer.Timeout += QueueFree;
        }

        public override void _Process(double delta) {
            var currentModulate = Modulate;
            currentModulate.A = CurrentFadeState switch {
                FadeState.FadingIn => (float)(1f - FadeInTimer.TimeLeft / FadeInTimer.WaitTime),
                FadeState.FadingOut => (float)(FadeOutTimer.TimeLeft / FadeOutTimer.WaitTime),
                _ => 1f,
            };
            Modulate = currentModulate;
        }
    }
}
