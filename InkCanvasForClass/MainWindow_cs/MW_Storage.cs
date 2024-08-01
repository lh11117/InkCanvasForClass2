﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Ink_Canvas {
    public partial class MainWindow : Window {

        public class StorageLocationItem {
            public string Path { get; set; }
            public ImageSource Icon { get; set; }
            public string Title { get; set; }
            public bool IsDrive { get; set; }
            public DriveType DriveType { get; set; }
            public string SelectItem { get; set; }
        }

        public static long GetDirectorySize(System.IO.DirectoryInfo directoryInfo, bool recursive = true)
        {
            var startDirectorySize = default(long);
            if (directoryInfo == null || !directoryInfo.Exists)
                return startDirectorySize; //Return 0 while Directory does not exist.

            //Add size of files in the Current Directory to main size.
            foreach (var fileInfo in directoryInfo.GetFiles())
                System.Threading.Interlocked.Add(ref startDirectorySize, fileInfo.Length);

            if (recursive) //Loop on Sub Direcotries in the Current Directory and Calculate it's files size.
                System.Threading.Tasks.Parallel.ForEach(directoryInfo.GetDirectories(), (subDirectory) =>
                    System.Threading.Interlocked.Add(ref startDirectorySize, GetDirectorySize(subDirectory, recursive)));

            return startDirectorySize;
        }

        public async Task<long> GetDirectorySizeAsync(System.IO.DirectoryInfo directoryInfo, bool recursive = true) {
            var size = await Task.Run(()=> GetDirectorySize(directoryInfo, recursive));
            return size;
        }

        private static string FormatBytes(long bytes) {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i; double dblSByte = bytes;
            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024) {
                dblSByte = bytes / 1024.0;
            }
            return String.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }

        private ObservableCollection<StorageLocationItem> storageLocationItems =
            new ObservableCollection<StorageLocationItem>() { };

        private void UpdateStorageLocations() {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            var fixedDrives = new List<StorageLocationItem>() { };
            var integratedFolders = new List<StorageLocationItem>() { };
            storageLocationItems.Clear();
            foreach (var driveInfo in allDrives) {
                if (driveInfo.DriveType == DriveType.Fixed) {
                    if (driveInfo.Name.Contains("C") || !driveInfo.IsReady) continue;
                    fixedDrives.Add(new StorageLocationItem() {
                        Path = driveInfo.Name + "InkCanvasForClass",
                        Title = driveInfo.Name.Substring(0,1) + "盘 ",
                        Icon = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons-png/classic-icons/disk-drive.png")),
                        DriveType = driveInfo.DriveType,
                        IsDrive = true,
                        SelectItem = "d"+driveInfo.Name.Substring(0,1).ToLower()
                    });
                }
            }
            var docfolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            integratedFolders.Add(new StorageLocationItem() {
                Path = (docfolder.EndsWith("\\") ? docfolder.Substring(0, docfolder.Length - 1) : docfolder) + "\\InkCanvasForClass",
                Title = "“文档”文件夹 ",
                IsDrive = false,
                Icon = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons-png/classic-icons/documents-folder.png")),
                SelectItem = "fw"
            });
            var runfolder = AppDomain.CurrentDomain.BaseDirectory;
            integratedFolders.Add(new StorageLocationItem() {
                Path = (runfolder.EndsWith("\\") ? runfolder.Substring(0, runfolder.Length - 1) : runfolder) + "\\InkCanvasForClass",
                Title = "icc安装目录 ",
                IsDrive = false,
                Icon = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons-png/classic-icons/folder.png")),
                SelectItem = "fr"
            });
            var usrfolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            integratedFolders.Add(new StorageLocationItem() {
                Path = (usrfolder.EndsWith("\\") ? usrfolder.Substring(0, usrfolder.Length - 1) : usrfolder) + "\\InkCanvasForClass",
                Title = "当前用户目录 ",
                IsDrive = false,
                Icon = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons-png/classic-icons/user-folder.png")),
                SelectItem = "fu"
            });
            var dskfolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            integratedFolders.Add(new StorageLocationItem() {
                Path = (dskfolder.EndsWith("\\") ? dskfolder.Substring(0, dskfolder.Length - 1) : dskfolder) + "\\InkCanvasForClass",
                Title = "“桌面”文件夹 ",
                IsDrive = false,
                Icon = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons-png/classic-icons/desktop-folder.png")),
                SelectItem = "fd"
            });

            foreach (var i in fixedDrives) storageLocationItems.Add(i);
            foreach (var i in integratedFolders) storageLocationItems.Add(i);
            storageLocationItems.Add(new StorageLocationItem() {
                Path = "",
                Title = "自定义... ",
                IsDrive = false,
                Icon = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons-png/classic-icons/folder.png")),
                SelectItem = "c-"
            });
        }

        private bool isChangingUserStorageSelectionProgramically = false;

        private void UpdateUserStorageSelection() {
            // 先获取磁盘信息
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            var fixedDrives = new List<string>() { };
            foreach (var driveInfo in allDrives) {
                if (driveInfo.Name.Contains("C") || !driveInfo.IsReady) continue;
                fixedDrives.Add("d"+driveInfo.Name.Substring(0,1).ToLower());
            }

            var integratedFolders = new List<string>() {
                "fw", "fr", "fu", "fd" // fw - folder wendang ; fd - folder desktop ; fu - folder user ; fr - folder running
            };
            if (Settings.Storage.StorageLocation.Substring(0, 1) == "d") {
                if (fixedDrives.Count == 0) {
                    Settings.Storage.StorageLocation = "fw";
                    SaveSettingsToFile();
                    ComboBoxStoragePath.SelectedIndex = 0;
                } else if (fixedDrives.Contains(Settings.Storage.StorageLocation)) {
                    ComboBoxStoragePath.SelectedIndex = fixedDrives.IndexOf(Settings.Storage.StorageLocation);
                } else {
                    ComboBoxStoragePath.SelectedIndex = 0;
                    Settings.Storage.StorageLocation = fixedDrives[0];
                    SaveSettingsToFile();
                }
            } else if (Settings.Storage.StorageLocation.Substring(0, 1) == "f") {
                if (integratedFolders.Contains(Settings.Storage.StorageLocation)) {
                    ComboBoxStoragePath.SelectedIndex = fixedDrives.Count + integratedFolders.IndexOf(Settings.Storage.StorageLocation);
                } else {
                    ComboBoxStoragePath.SelectedIndex = fixedDrives.Count;
                    Settings.Storage.StorageLocation = "fw";
                    SaveSettingsToFile();
                }
            } else if (Settings.Storage.StorageLocation.Substring(0, 1) == "c") {
                ComboBoxStoragePath.SelectedIndex = storageLocationItems.Count - 1;
            }
        }

        private void ComboBoxStoragePath_DropDownOpened(object sender, EventArgs e) {
            isChangingUserStorageSelectionProgramically = true;
            UpdateStorageLocations();
            UpdateUserStorageSelection();
            isChangingUserStorageSelectionProgramically = false;
        }

        private async Task<int> GetDirectoryFilesCount(string path) {
            var count = await Task.Run(() => Directory.GetFiles(path, "*", SearchOption.AllDirectories).Length);
            return count;
        }

        private void InitStorageFoldersStructure(string path) {
            var basePath = new DirectoryInfo(path);
            var autoSavedInkPath = new DirectoryInfo(path+"\\AutoSavedInk");
            var autoSavedSnapshotPath = new DirectoryInfo(path+"\\AutoSavedSnapshot");
            var exportedInkPath = new DirectoryInfo(path+"\\ExportedInk");
            var quotedPhotosPath = new DirectoryInfo(path+"\\QuotedPhotos");
            var cachesPath = new DirectoryInfo(path+"\\caches");
            var paths = new DirectoryInfo[] {
                basePath, autoSavedInkPath, autoSavedSnapshotPath, exportedInkPath, quotedPhotosPath, cachesPath
            };
            foreach (var di in paths) {
                if (!di.Exists) di.Create();
            }
        }

        private bool isAnalyzingStorageInfo = false;

        private async void StartAnalyzeStorage(bool forceAnalyze = false) {
            if (isAnalyzingStorageInfo && !forceAnalyze) return;
            isAnalyzingStorageInfo = true;
            var item = storageLocationItems[ComboBoxStoragePath.SelectedIndex];
            StorageAnalazeWaitingGroup.Visibility = Visibility.Visible;
            StorageAnalazeGroup.Visibility = Visibility.Collapsed;
            StorageNowLocationTextBlock.Text = $"当前位置：{item.Path}";
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            var driveArr = allDrives.Where((info, i) => info.Name.Substring(0,1)==item.Path.Substring(0,1)).ToArray();
            if (driveArr.Length > 0) {
                StorageDiskUsageTextBlock.Visibility = Visibility.Visible;
                var freeSpace = driveArr[0].TotalFreeSpace;
                var usedSpace = driveArr[0].TotalSize - driveArr[0].TotalFreeSpace;
                StorageDiskUsageTextBlock.Text = $"磁盘使用情况：已用 {FormatBytes(usedSpace)}、剩余 {FormatBytes(freeSpace)}";
                var dirsize = await GetDirectorySizeAsync(new DirectoryInfo(item.Path));
                var formatedDirSize = FormatBytes(dirsize);
                var dirFilecount = await GetDirectoryFilesCount(item.Path);
                StorageDirectoryUsageTextBlock.Text = $"目录占用情况：已用 {formatedDirSize}，共 {dirFilecount} 个文件";
                var usedBorderWidth = Math.Round(388 * ((double)usedSpace / (double)(driveArr[0].TotalSize)), 1);
                var ICCUsedBorderWidth = Math.Round(usedBorderWidth * ((double)dirsize / (double)usedSpace), 1);
                usedBorderWidth = usedBorderWidth - ICCUsedBorderWidth;
                ICCDirectoryStorageAnalyzeGroup.Visibility = dirsize == 0 ? Visibility.Collapsed : Visibility.Visible;
                DiskUsageUsedSpaceBorder.Width = usedBorderWidth;
                DiskUsageICCSpaceBorder.Width = ICCUsedBorderWidth;
                var asiSize = await GetDirectorySizeAsync(new DirectoryInfo(item.Path + "\\AutoSavedInk"));
                var assSize = await GetDirectorySizeAsync(new DirectoryInfo(item.Path + "\\AutoSavedSnapshot"));
                var eiSize = await GetDirectorySizeAsync(new DirectoryInfo(item.Path + "\\ExportedInk"));
                var qpSize = await GetDirectorySizeAsync(new DirectoryInfo(item.Path + "\\QuotedPhotos"));
                var cachesSize = await GetDirectorySizeAsync(new DirectoryInfo(item.Path + "\\caches"));
                ClearCacheFilesButton.IsEnabled = cachesSize != 0;
                ClearAutoSavedSnapshotButton.IsEnabled = assSize != 0;
                StorageDirectoryAutoSavedInkUsageBorder.Width =
                    Math.Round(388 * ((double)asiSize / (double)dirsize), 1);
                StorageDirectoryAutoSavedSnapshotUsageBorder.Width =
                    Math.Round(388 * ((double)assSize / (double)dirsize), 1);
                StorageDirectoryExportedInkUsageBorder.Width = Math.Round(388 * ((double)eiSize / (double)dirsize), 1);
                StorageDirectoryQuotedPhotoUsageBorder.Width = Math.Round(388 * ((double)qpSize / (double)dirsize), 1);
                StorageDirectoryCachesUsageBorder.Width = Math.Round(388 * ((double)cachesSize / (double)dirsize), 1);
                StorageAutoSavedInkDescription.Text =
                    $"{Math.Round(100 * ((double)asiSize / (double)dirsize), 1)}% 、{FormatBytes(asiSize)}";
                StorageAutoSavedSnapshotDescription.Text =
                    $"{Math.Round(100 * ((double)assSize / (double)dirsize), 1)}% 、{FormatBytes(assSize)}";
                StorageExportedInkDescription.Text =
                    $"{Math.Round(100 * ((double)eiSize / (double)dirsize), 1)}% 、{FormatBytes(eiSize)}";
                StorageQuotedPhotosDescription.Text =
                    $"{Math.Round(100 * ((double)qpSize / (double)dirsize), 1)}% 、{FormatBytes(qpSize)}";
                StorageCachesDescription.Text =
                    $"{Math.Round(100 * ((double)cachesSize / (double)dirsize), 1)}% 、{FormatBytes(cachesSize)}";
                StorageAnalazeWaitingGroup.Visibility = Visibility.Collapsed;
                StorageAnalazeGroup.Visibility = Visibility.Visible;
                isAnalyzingStorageInfo = false;
            } else isAnalyzingStorageInfo = false;
        }

        private void ClearCacheFilesButton_Clicked(object sender, RoutedEventArgs e) {
            var di = new DirectoryInfo(storageLocationItems[ComboBoxStoragePath.SelectedIndex].Path+"\\caches");
            try {
                Directory.Delete(di.FullName, true);
                ShowNewToast("清理缓存成功！", MW_Toast.ToastType.Success, 2000);
            }
            catch (Exception ex) {
                ShowNewToast($"清理缓存失败！{ex.Message}", MW_Toast.ToastType.Error, 2000);
            }
            InitStorageFoldersStructure(storageLocationItems[ComboBoxStoragePath.SelectedIndex].Path);
            StartAnalyzeStorage();
        }

        private void ClearAutoSavedSnapshotButton_Clicked(object sender, RoutedEventArgs e) {
            var di = new DirectoryInfo(storageLocationItems[ComboBoxStoragePath.SelectedIndex].Path+"\\AutoSavedSnapshot");
            try {
                Directory.Delete(di.FullName, true);
                ShowNewToast("清理自动保存的截图成功！", MW_Toast.ToastType.Success, 2000);
            }
            catch (Exception ex) {
                ShowNewToast($"清理自动保存的截图失败！{ex.Message}", MW_Toast.ToastType.Error, 2000);
            }
            InitStorageFoldersStructure(storageLocationItems[ComboBoxStoragePath.SelectedIndex].Path);
            StartAnalyzeStorage();
        }

        private DirectoryInfo GetDirectoryInfoByIndex(int index) {
            var autoSavedInkPath = new DirectoryInfo(storageLocationItems[ComboBoxStoragePath.SelectedIndex].Path+"\\AutoSavedInk");
            var autoSavedSnapshotPath = new DirectoryInfo(storageLocationItems[ComboBoxStoragePath.SelectedIndex].Path+"\\AutoSavedSnapshot");
            var exportedInkPath = new DirectoryInfo(storageLocationItems[ComboBoxStoragePath.SelectedIndex].Path+"\\ExportedInk");
            var quotedPhotosPath = new DirectoryInfo(storageLocationItems[ComboBoxStoragePath.SelectedIndex].Path+"\\QuotedPhotos");
            var cachesPath = new DirectoryInfo(storageLocationItems[ComboBoxStoragePath.SelectedIndex].Path+"\\caches");
            return (new DirectoryInfo[]
                { autoSavedInkPath, quotedPhotosPath, exportedInkPath, cachesPath, autoSavedSnapshotPath })[index];
        }

        private Border lastStorageJumpToFolderBorderDown = null;

        private void StorageJumpToFolderBtn_MouseDown(object sender, MouseButtonEventArgs e) {
            if (lastStorageJumpToFolderBorderDown != null) return;
            lastStorageJumpToFolderBorderDown = (Border)sender;
            lastStorageJumpToFolderBorderDown.Background = new SolidColorBrush(Color.FromRgb(39, 39, 42));
        }

        private void StorageJumpToFolderBtn_MouseUp(object sender, MouseButtonEventArgs e) {
            if (lastStorageJumpToFolderBorderDown == null || lastStorageJumpToFolderBorderDown != (Border)sender) return;
            var bd = (Border)sender;
            StorageJumpToFolderBtn_MouseLeave(sender,null);
            var di = GetDirectoryInfoByIndex(Int32.Parse(bd.Name[bd.Name.Length - 1].ToString()));
            Process.Start("explorer.exe", di.FullName);
        }

        private void StorageJumpToFolderBtn_MouseLeave(object sender, MouseEventArgs e) {
            if (lastStorageJumpToFolderBorderDown == null || lastStorageJumpToFolderBorderDown != (Border)sender) return;
            lastStorageJumpToFolderBorderDown.Background = new SolidColorBrush(Colors.Transparent);
            lastStorageJumpToFolderBorderDown = null;
        }

        private void InitStorageManagementModule() {
            ComboBoxStoragePath.ItemsSource = storageLocationItems;
            ComboBoxStoragePath.DropDownOpened += ComboBoxStoragePath_DropDownOpened;
            ComboBoxStoragePath.SelectionChanged += ComboBoxStoragePath_OnSelectionChanged;
            isChangingUserStorageSelectionProgramically = true;
            UpdateStorageLocations();
            UpdateUserStorageSelection();
            isChangingUserStorageSelectionProgramically = false;
            InitStorageFoldersStructure(storageLocationItems[ComboBoxStoragePath.SelectedIndex].Path);
            StartAnalyzeStorage();
            var sb = new Border[] {
                StorageJumpToFolderBtn1, StorageJumpToFolderBtn2, StorageJumpToFolderBtn3, StorageJumpToFolderBtn4,
                StorageJumpToFolderBtn5,
            };
            foreach (var btn in sb) {
                btn.MouseUp += StorageJumpToFolderBtn_MouseUp;
                btn.MouseDown += StorageJumpToFolderBtn_MouseDown;
                btn.MouseLeave += StorageJumpToFolderBtn_MouseLeave;
            }
        }

        private void ComboBoxStoragePath_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (isChangingUserStorageSelectionProgramically) return;
            if (!isLoaded) return;
            Trace.WriteLine(ComboBoxStoragePath.SelectedIndex);
            Settings.Storage.StorageLocation = storageLocationItems[ComboBoxStoragePath.SelectedIndex].SelectItem;
            SaveSettingsToFile();
            InitStorageFoldersStructure(storageLocationItems[ComboBoxStoragePath.SelectedIndex].Path);
            StartAnalyzeStorage();
        }
    }
}
