// TODO: create config system

namespace LocalImageLiker {
    using Godot;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public partial class LocalImageLiker : Node {
        public class ImageInfo {
            public string FilePath;
            public Texture2D Texture;

            public ImageInfo(string filePath, Texture2D texture) {
                FilePath = filePath;
                Texture = texture;
            }
        }

        public static LocalImageLiker Singleton { get; private set; } = null!;

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

        public LocalImageLiker() {
            Singleton = this;

            // TODO: initialize config values (CurrentDirPath, LikeDirPath, DislikeDirPath, BatchSize)
        }

        public override void _Ready() {
            FileDialog = GetNode<FileDialog>("FileDialog");
            CurrentDirButton = GetNode<Button>("VBoxContainer/Header/Current Dir Button");
            LikeDirButton = GetNode<Button>("VBoxContainer/Body/Like/VBoxContainer/Like Dir Button");
            DislikeDirButton = GetNode<Button>("VBoxContainer/Body/Dislike/VBoxContainer/Dislike Dir Button");
            ImageTextureRect = GetNode<TextureRect>("VBoxContainer/Body/Image/VBoxContainer/Image Texture Rect");
        }

        private static void SetCurrentDirPath(string? newCurrentDirPath) {
            if (newCurrentDirPath == currentDirPath) {
                return;
            }

            currentDirPath = newCurrentDirPath;

            // TODO: update config

            RefreshCurrentTextures();

            CurrentDirButton.Text = newCurrentDirPath;
        }

        private static void SetLikeDirPath(string? newLikeDirPath) {
            if (newLikeDirPath == likeDirPath) {
                return;
            }

            likeDirPath = newLikeDirPath;

            // TODO: update config

            LikeDirButton.Text = newLikeDirPath;
        }

        private static void SetDislikeDirPath(string? newDisikeDirPath) {
            if (newDisikeDirPath == dislikeDirPath) {
                return;
            }

            dislikeDirPath = newDisikeDirPath;

            // TODO: update config

            DislikeDirButton.Text = newDisikeDirPath;
        }

        private static void SetBatchSize(int newBatchSize) {
            if (newBatchSize == batchSize) {
                return;
            }

            batchSize = newBatchSize;

            // TODO: update config

            RefreshCurrentTextures();
        }

        public static void RefreshCurrentTextures() {
            CurrentImageInfos.Clear();
            CurrentImageInfo = null;

            if (CurrentDirPath is null) {
                // TODO: warn user
                return;
            }

            string[] filePaths;
            try {
                filePaths = Directory.GetFiles(CurrentDirPath);
            } catch (Exception ex) {
                // TODO: warn user
                return;
            }

            foreach (var filePath in filePaths) {
                if (CurrentImageInfos.Count >= BatchSize) {
                    break;
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
            }

            LoadNextImageInfo(false);
        }

        private static void LoadNextImageInfo(bool refreshIfEmpty = true) {
            if (refreshIfEmpty && CurrentImageInfos.Count <= 0) {
                RefreshCurrentTextures();
            }

            CurrentImageInfo = CurrentImageInfos.Count > 0 ? CurrentImageInfos.Dequeue() : null;

            ImageTextureRect.Texture = CurrentImageInfo?.Texture;
        }

        public static void SkipCurrentImage() {
            if (CurrentImageInfo is not null) {
                CurrentImageInfos.Enqueue(CurrentImageInfo);
            }

            LoadNextImageInfo();
        }

        public static void LikeCurrentImage() {
            if (CurrentImageInfo is null) {
                // TODO: warn user
                return;
            }

            if (LikeDirPath is null) {
                // TODO: warn user
                return;
            }

            MoveFileTo(CurrentImageInfo.FilePath, LikeDirPath);

            LoadNextImageInfo();
        }

        public static void DislikeCurrentImage() {
            if (CurrentImageInfo is null) {
                // TODO: warn user
                return;
            }

            if (DislikeDirPath is null) {
                // TODO: warn user
                return;
            }

            MoveFileTo(CurrentImageInfo.FilePath, DislikeDirPath);

            LoadNextImageInfo();
        }

        private static void MoveFileTo(string filePath, string newDirPath) {
            try {
                File.Move(filePath, newDirPath);
            } catch (Exception ex) {
                // TODO: warn user
            }
        }
    }
}