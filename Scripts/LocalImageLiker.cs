/*
 * TODO:
 *  Add a logging system
 *  Add a button to manually refresh the textures (or possibly setup a directory monitor)
 *  Add a button to manually reset the skipped images
 */

namespace LocalImageLiker {
    using Godot;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public partial class LocalImageLiker : Node {
        public class ImageInfo {
            public string FilePath;
            public Texture2D Texture;

            public ImageInfo(string filePath, Texture2D texture) {
                FilePath = filePath;
                Texture = texture;
            }
        }

        public static FileDialog FileDialog { get; private set; } = null!;
        private static Button CurrentDirButton = null!;
        private static Button LikeDirButton = null!;
        private static Button DislikeDirButton = null!;
        private static TextureRect ImageTextureRect = null!;

        private static string? currentDirPath;
        public static string? CurrentDirPath {
            get => currentDirPath;
            set => SetCurrentDirPath(value);
        }

        private static string? likeDirPath;
        public static string? LikeDirPath {
            get => likeDirPath;
            set => SetLikeDirPath(value);
        }

        private static string? dislikeDirPath;
        public static string? DislikeDirPath {
            get => dislikeDirPath;
            set => SetDislikeDirPath(value);
        }

        private static int batchSize;
        public static int BatchSize {
            get => batchSize;
            set => SetBatchSize(value);
        }

        private static readonly Queue<ImageInfo> CurrentImageInfos = new();
        private static ImageInfo? CurrentImageInfo;
        private static readonly Queue<ImageInfo> SkippedImageInfos = new();

        public override void _Ready() {
            FileDialog = GetNode<FileDialog>("FileDialog");
            CurrentDirButton = GetNode<Button>("VBoxContainer/Header/Current Dir Button");
            LikeDirButton = GetNode<Button>("VBoxContainer/Body/Like/VBoxContainer/Like Dir Button");
            DislikeDirButton = GetNode<Button>("VBoxContainer/Body/Dislike/VBoxContainer/Dislike Dir Button");
            ImageTextureRect = GetNode<TextureRect>("VBoxContainer/Body/Image/VBoxContainer/Image Texture Rect");

            if (Config.Load()) {
                LoadPropertyFromConfig<string?>(SetCurrentDirPath, nameof(CurrentDirPath), null);
                LoadPropertyFromConfig<string?>(SetLikeDirPath, nameof(LikeDirPath), null);
                LoadPropertyFromConfig<string?>(SetDislikeDirPath, nameof(DislikeDirPath), null);
                LoadPropertyFromConfig(SetBatchSize, nameof(BatchSize), 3);
            }

            RefreshCurrentTextures();
        }

        private static void LoadPropertyFromConfig<[MustBeVariant] T>(Action<T?, bool> propertySetter, string propertyName, T defaultValue) => propertySetter(Config.TryGetProperty<T>(propertyName, out var value) ? value : defaultValue, true);

        private static void SetCurrentDirPath(string? newCurrentDirPath, bool init = false) {
            if (newCurrentDirPath == currentDirPath) {
                return;
            }

            currentDirPath = newCurrentDirPath;

            if (!init) {
                Config.SaveProperty(nameof(CurrentDirPath), newCurrentDirPath ?? "");

                RefreshCurrentTextures();
            }

            CurrentDirButton.Text = newCurrentDirPath;
        }

        private static void SetLikeDirPath(string? newLikeDirPath, bool init = false) {
            if (newLikeDirPath == likeDirPath) {
                return;
            }

            likeDirPath = newLikeDirPath;

            if (!init) {
                Config.SaveProperty(nameof(LikeDirPath), newLikeDirPath ?? "");
            }

            LikeDirButton.Text = newLikeDirPath;
        }

        private static void SetDislikeDirPath(string? newDisikeDirPath, bool init = false) {
            if (newDisikeDirPath == dislikeDirPath) {
                return;
            }

            dislikeDirPath = newDisikeDirPath;

            if (!init) {
                Config.SaveProperty(nameof(DislikeDirPath), newDisikeDirPath);
            }

            DislikeDirButton.Text = newDisikeDirPath;
        }

        private static void SetBatchSize(int newBatchSize, bool init = false) {
            if (newBatchSize == batchSize) {
                return;
            }

            batchSize = newBatchSize;

            if (!init) {
                Config.SaveProperty(nameof(BatchSize), newBatchSize);

                RefreshCurrentTextures();
            }
        }

        public static void RefreshCurrentTextures(bool loadNextImageInfo = true) {
            CurrentImageInfos.Clear();
            CurrentImageInfo = null;

            if (CurrentDirPath is null) {
                Messenger.SendCallToAction("Please select a folder to go through images.");
                return;
            }

            string[] filePaths;
            try {
                filePaths = Directory.GetFiles(CurrentDirPath);
            } catch (Exception ex) {
                Messenger.SendError($"Could not get files in '{CurrentDirPath}'. Exception: {ex}");
                return;
            }

            foreach (var filePath in filePaths) {
                if (SkippedImageInfos.Any(imageInfo => imageInfo.FilePath == filePath)) {
                    continue;
                }

                var image = Image.LoadFromFile(filePath);
                if (image is null) {
                    continue;
                }

                var texture = ImageTexture.CreateFromImage(image);
                if (texture is null) {
                    continue;
                }

                CurrentImageInfos.Enqueue(new(filePath, texture));

                if (CurrentImageInfos.Count >= BatchSize) {
                    break;
                }
            }

            if (loadNextImageInfo) {
                LoadNextImageInfo();
            }
        }

        private static void LoadNextImageInfo() {
            if (CurrentImageInfos.Count <= 0) {
                RefreshCurrentTextures(false);
            }

            if (CurrentImageInfos.Count <= 0) {
                while (SkippedImageInfos.Count > 0) {
                    CurrentImageInfos.Enqueue(SkippedImageInfos.Dequeue());
                }
            }

            if (CurrentImageInfos.Count <= 0) {
                CurrentImageInfo = null;
                ImageTextureRect.Texture = null;
                Messenger.SendInfo($"Could not find more images in '{CurrentDirPath}'");
                return;
            }

            CurrentImageInfo = CurrentImageInfos.Dequeue();
            ImageTextureRect.Texture = CurrentImageInfo!.Texture;
        }

        public static void ResetSkippedImages() {
            SkippedImageInfos.Clear();
            RefreshCurrentTextures();
        }

        public static void SkipCurrentImage() {
            if (CurrentImageInfo is null) {
                Messenger.SendCallToAction("Please select a folder to go through images.");
                return;
            }

            SkippedImageInfos.Enqueue(CurrentImageInfo);

            LoadNextImageInfo();
        }

        public static void LikeCurrentImage() {
            if (CurrentImageInfo is null) {
                Messenger.SendCallToAction("Please select a folder to go through images.");
                return;
            }

            if (LikeDirPath is null) {
                Messenger.SendCallToAction("Please select a folder to send liked images to.");
                return;
            }

            if (TryMoveFileTo(CurrentImageInfo.FilePath, LikeDirPath)) {
                LoadNextImageInfo();
            }
        }

        public static void DislikeCurrentImage() {
            if (CurrentImageInfo is null) {
                Messenger.SendCallToAction("Please select a folder to go through images.");
                return;
            }

            if (DislikeDirPath is null) {
                Messenger.SendCallToAction("Please select a folder to send disliked images to.");
                return;
            }

            if (TryMoveFileTo(CurrentImageInfo.FilePath, DislikeDirPath)) {
                LoadNextImageInfo();
            }
        }

        private static bool TryMoveFileTo(string filePath, string newDirPath) {
            try {
                File.Move(filePath, Path.Combine(newDirPath, Path.GetFileName(filePath)));
            } catch (Exception ex) {
                Messenger.SendError($"Could not move file at '{filePath}' to '{newDirPath}'. Exception: {ex}");
                return false;
            }

            return true;
        }
    }
}