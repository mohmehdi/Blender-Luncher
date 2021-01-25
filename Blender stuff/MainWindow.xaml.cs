using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Collections.Generic;
using System.Diagnostics;

namespace Blender_stuff
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    struct Version
    {
        public Button V;
        public Button D;
        public string Path;

        public Version(Button v, Button d,string path)
        {
            V = v;
            D = d;
            Path = path;
        }
    }
    public partial class MainWindow : Window
    {
        Dictionary<string, Version> VersionDic;
        private string versionButtonStr;
        private string deleteButtonStr;

        public MainWindow()
        {
            InitializeComponent();

            versionButtonStr = XamlWriter.Save(VersionBTN);
            deleteButtonStr = XamlWriter.Save(deleteBTN);

            VersionDic = new Dictionary<string, Version>();
            LoadData();

        }
        private void SaveData()
        {
            StreamWriter sw = new StreamWriter("Versions.txt");
           
            foreach (var item in VersionDic)
            {
                sw.WriteLine(item.Key + "#" + item.Value.Path);
            }

            sw.Close();
        }
        private void LoadData()
        {
            if (!File.Exists("Versions.txt"))
                return;

            StreamReader sr = new StreamReader("Versions.txt");
            string line = sr.ReadLine();
            //Continue to read until you reach end of file
            while (line != null)
            {
                string[] data = line.Split('#');
                string[] pathArr = data[1].Split('\\');
                string version = pathArr[pathArr.Length - 2];
                AddNewVersion(data[0], version, data[1]);

                //getting next line
                line = sr.ReadLine();
            }

            sr.Close();
        }
        private void OpenBlender(object sender, RoutedEventArgs e)
        {
            Process.Start(VersionDic[(sender as Button).Name.Remove(0, 1)].Path);
        }
        private void AddNewVersion(string key,string version,string path)
        {
            var v = (Button)XamlReader.Parse(versionButtonStr);
            v.Height = 50;
            v.IsEnabled = true;
            v.Content = version;
            v.Name = "v" + key;
            v.Click += OpenBlender;
            VersionPanel.Children.Add(v);

            var d = (Button)XamlReader.Parse(deleteButtonStr);
            d.Height = 50;
            d.IsEnabled = true;
            d.Name = "d" + key;
            d.Click += DeleteVersion;
            DeletePanel.Children.Add(d);

            VersionDic.Add(key, new Version(v, d,path));
        }
        private void DeleteVersion(object sender , RoutedEventArgs e)
        {
            //TODO: Remove event and make it null for memory crap
            string ver= (sender as Button).Name.Remove(0,1);

            VersionPanel.Children.Remove(VersionDic[ver].V);
            DeletePanel.Children.Remove(VersionDic[ver].D);
            VersionDic.Remove(ver);
            SaveData();
        }
        private void Add_Version(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string path = openFileDialog.FileName;
                string[] pathArr = path.Split('\\');
                if (path.Contains("blender.exe"))
                {
                    string version = pathArr[pathArr.Length - 2];
                    string key = version.Replace(" ", "").Replace(".", "");
                    if (VersionDic.ContainsKey(key))
                    {
                        return;
                    }

                    AddNewVersion(key, version, path);
                    SaveData();
                }
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
            Application.Current.Shutdown();
        }

        private void Drag(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
