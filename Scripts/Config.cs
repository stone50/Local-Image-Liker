namespace LocalImageLiker {
    using Godot;

    public static class Config {
        public const string ConfigFilePath = "user://config.cfg";

        private static readonly ConfigFile ConfigFile = new();

        public static bool Initialize() {
            var loadError = ConfigFile.Load(ConfigFilePath);
            if (loadError == Error.Ok) {
                return true;
            }

            Messenger.SendMessage(Messenger.MessageType.Warning, "Could not load config file. Attempting to create a new config file.");

            var saveError = ConfigFile.Save(ConfigFilePath);
            if (saveError == Error.Ok) {
                Messenger.SendMessage(Messenger.MessageType.Info, $"New config file created at '{ProjectSettings.GlobalizePath(ConfigFilePath)}'");
                return true;
            }

            Messenger.SendMessage(Messenger.MessageType.Error, $"Could not create a new config file at '{ProjectSettings.GlobalizePath(ConfigFilePath)}'");
            return false;
        }

        public static void SaveProperty<[MustBeVariant] T>(string propertyName, T? propertyValue) {
            if (propertyValue is null) {
                ConfigFile.EraseSectionKey("", propertyName);
                return;
            }

            ConfigFile.SetValue("", propertyName, Variant.From(propertyValue));

            var saveError = ConfigFile.Save(ConfigFilePath);
            if (saveError != Error.Ok) {
                Messenger.SendMessage(Messenger.MessageType.Error, $"Could not save property '{propertyName}' with value '{propertyValue}' to config file.");
            }
        }

        public static bool TryGetProperty<[MustBeVariant] T>(string propertyName, out T value) {
            var variant = ConfigFile.GetValue("", propertyName);
            value = variant.As<T>();
            return variant.VariantType != Variant.Type.Nil;
        }
    }
}
