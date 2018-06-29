using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace WpfApp4
{
    public partial class MainWindow : Window
    {
        int selectedImage;
        int iconWidth = 100;
        int iconHeight = 100;

        public List<DataFile> fileList = new List<DataFile>();
        public static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".BMP", ".PNG" };

        public MainWindow()
        {
            InitializeComponent();

            string filename = "C:/Program Files/Internet Explorer/iexplore.exe";
            System.Drawing.Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(filename);
            System.Drawing.Bitmap iconBitmap = icon.ToBitmap();



        }

        void Icon_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(string.Format("You clicked on the {0} icon.", (sender as Image).Name));

            selectedImage = Convert.ToInt32((sender as Image).Tag);
            ImageSource iconSource = fileList[selectedImage].image;
            Image_LargePreview.Source = iconSource;
            Console.WriteLine(fileList[selectedImage].image);
            //string imagePath = filePaths[selectedImage];
            LoadFileInfo(fileList[selectedImage].filePath);
        }

        void Icon_MouseEnter(object sender, RoutedEventArgs e)
        {
            (sender as Image).Opacity = 1.0;
        }

        void Icon_MouseLeave(object sender, RoutedEventArgs e)
        {
            (sender as Image).Opacity = 0.8;
        }

        private void Button_AddFiles_Click(object sender, RoutedEventArgs e)
        {
            
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Multiselect = true;

            string folderPath = "";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string []filePaths = dlg.FileNames;
                Label_FilePath.Content = folderPath;

                AddFiles(filePaths);
            }

            
        }

        private void AddFiles(string[] newFiles)
        {
            var filePaths = (newFiles.Where(name => !name.EndsWith(".ini"))).ToArray();

            //add new items to list
            int startIndex = fileList.Count;
            for (int i = 0; i < filePaths.Length; i++)
            {
                DataFile newDataFile = new DataFile();

                newDataFile.filePath = filePaths[i];
                newDataFile.name = System.IO.Path.GetFileNameWithoutExtension(filePaths[i]);
                newDataFile.index = startIndex + i;

                if (ImageExtensions.Contains(System.IO.Path.GetExtension(newDataFile.filePath).ToUpperInvariant()))
                {
                    newDataFile.image = new BitmapImage(new Uri(newDataFile.filePath, UriKind.Absolute));
                }
                else
                {
                    newDataFile.image = getIconSource(newDataFile.filePath);
                }

                

                fileList.Add(newDataFile);
            }

            setGridItems(fileList);
        }

        private void setGridItems(List<DataFile> itemList)
        {
            //clear grid
            clearGrid();

            //update grid
            foreach (DataFile f in itemList)
            {
                Image icon = new Image()
                {
                    Name = "image_" + f.index,
                    Tag = f.index,
                    Source = f.image,

                    Width = iconWidth,
                    Height = iconHeight,

                    Stretch = Stretch.Fill,
                    Opacity = 0.8

                };

                //add new events for each item
                icon.MouseLeftButtonDown += new MouseButtonEventHandler(Icon_Click);
                icon.MouseEnter += new MouseEventHandler(Icon_MouseEnter);
                icon.MouseLeave += new MouseEventHandler(Icon_MouseLeave);

                //add item to grid
                this.grid.Children.Add(icon);

                
            }
        }

        private void clearGrid()
        {
            this.grid.Children.RemoveRange(0, this.grid.Children.Count);
        }

        private ImageSource getIconSource(string iconPath)
        {
            ImageSource imageSource;

            System.Drawing.Icon testIcon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);

            using (System.Drawing.Bitmap bmp = testIcon.ToBitmap())
            {
                var stream = new MemoryStream();
                bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                imageSource = BitmapFrame.Create(stream);
            }

            return imageSource;
        }

        private void Slider_IconSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int iconSize = 50 + (int)(Slider_IconSize.Value * 25);

            iconWidth = iconSize;
            iconHeight = iconSize;

            double t = 400 / iconWidth;
            int iconRowCount = Convert.ToInt32(Math.Floor(t));

            int iconRowSpaceUsed = iconRowCount * iconWidth;
            int iconRowSpaceFree = 400 - iconRowSpaceUsed;

            Console.WriteLine(iconWidth);

            //grid.Margin. = iconRowSpaceUsed;
            //scrollviewer.Width = iconRowSpaceUsed;

            setGridItems(fileList);
        }

        private void LoadFileInfo(string filePath)
        {
            ListBox_FileInfo.Items.Clear();

            ListBoxItem FileName = new ListBoxItem();
            FileName.Content = "Name: " + System.IO.Path.GetFileName(filePath);
            ListBox_FileInfo.Items.Add(FileName);

            ListBoxItem FileSize = new ListBoxItem();
            long fileSizeKB = new System.IO.FileInfo(filePath).Length / (long)1024;
            FileSize.Content = "File Size: " + fileSizeKB + "kb";
            ListBox_FileInfo.Items.Add(FileSize);

            ListBoxItem ImageResolution = new ListBoxItem();
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(filePath);
            ImageResolution.Content = "Image Resolution: " + bmp.Width + "x" + bmp.Height;
            ListBox_FileInfo.Items.Add(ImageResolution);
        }


    }

}
