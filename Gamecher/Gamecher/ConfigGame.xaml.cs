using Microsoft.Win32;
using SharpConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Gamecher
{
    /// <summary>
    /// Lógica de interacción para ConfigTest.xaml
    /// </summary>
    public partial class ConfigGame : Window
    {
        Configuration config = null;
        List<Setting> arrayOfSettings = null;
        List<TextBox> arrayOfTextBox = null;
        string treatedPath = "";

        public ConfigGame(string configPath)
        {
            //TODO Guardar el path en el juego una vez hecho, y comprobar si esta guardado antes de ejecutar esta lógica.
            this.SizeToContent = SizeToContent.Height;
            InitializeComponent();

            treatedPath = configPath;
            treatedPath = treatedPath.Replace(@"$main_disk$", Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)));
            treatedPath = treatedPath.Replace(@"$user_name$", Environment.UserName);
            try{
                if (treatedPath.EndsWith(".cfg") || treatedPath.EndsWith(".ini"))
                {
                    config = Configuration.LoadFromFile(treatedPath);
                    arrayOfSettings = new List<Setting>();
                    arrayOfTextBox = new List<TextBox>();

                    foreach (var section in config)
                    {
                        foreach (var setting in section)
                        {
                            arrayOfSettings.Add(setting);
                            var nameOfSetting = new TextBlock()
                            {
                                Text = setting.Name,
                                Margin = new Thickness(50, 0, 5, 0),
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                TextAlignment = TextAlignment.Left,
                                FontWeight = FontWeights.Bold,
                                Foreground = new SolidColorBrush(Color.FromRgb(255, 248, 248))
                            };
                            var leftBorder = new Border()
                            {
                                Margin = new Thickness(0, 0, 0, 5),
                                BorderThickness = new Thickness(0.5),
                                Height = 30,
                                CornerRadius = new CornerRadius(5),
                                Background = new SolidColorBrush(Color.FromRgb(72, 75, 81)),
                                HorizontalAlignment = HorizontalAlignment.Stretch
                            };
                            leftBorder.Child = nameOfSetting;
                            leftStackPanel.Children.Add(leftBorder);

                            var valueOfSetting = new TextBox
                            {
                                Margin = new Thickness(10, 0, 10, 0),
                                Background = new SolidColorBrush(Colors.Transparent),
                                Foreground = new SolidColorBrush(Color.FromRgb(126, 126, 127)),
                                BorderBrush = Brushes.Transparent,
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center
                            };

                            //TODO In a future, make the appropiate checks, to show the user proper types of values, instead of reading everything as a string.
                            //var typeOfConfigParam = GetTypeOfConfigParam(setting);

                            //if (typeOfConfigParam == "".GetType())
                            //{
                            var splittedValue = Regex.Split(setting.RawValue, @"""");
                            foreach (string s in splittedValue)
                            {
                                Console.Write(" " + s);
                            }
                            if (splittedValue.Length > 1)
                            {
                                valueOfSetting.Text = splittedValue[1];
                                Console.WriteLine(">1");
                                valueOfSetting.Tag = "hasQuotes";
                            }
                            else
                            {
                                valueOfSetting.Text = splittedValue[0];
                                Console.WriteLine("<1");
                                valueOfSetting.Tag = "hasNoQuotes";
                            }
                            //}
                            //else
                            //{
                            //    valueOfSetting.Text = setting.GetValue(typeOfConfigParam).ToString();
                            //    valueOfSetting.Tag = "hasNoQuotes";
                            //}
                            arrayOfTextBox.Add(valueOfSetting);

                            var rightBorder = new Border()
                            {
                                Margin = new Thickness(0, 0, 0, 5),
                                BorderThickness = new Thickness(0.5),
                                Height = 30,
                                CornerRadius = new CornerRadius(5),
                                Background = new SolidColorBrush(Color.FromRgb(72, 75, 81)),
                                HorizontalAlignment = HorizontalAlignment.Stretch
                            };
                            rightBorder.Child = valueOfSetting;
                            rightStackPanel.Children.Add(rightBorder);
                        }
                        leftStackPanel.Children.Add(new Separator
                        {
                            Background = Brushes.Transparent,
                            Height = 15
                        });
                        rightStackPanel.Children.Add(new Separator
                        {
                            Background = Brushes.Transparent,
                            Height = 15
                        });

                    }

                }
                //TODO Also develop compatibility for .xml files.
                //else if (treatedPath.EndsWith(".xml"))
                //{
                //
                //}
                else
                {
                    (Application.Current.MainWindow as MainWindow).Opacity = 0.9;
                    (Application.Current.MainWindow as MainWindow).Effect = new BlurEffect();
                    MessageBox.Show("There was an error loading the configuration file.", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    (Application.Current.MainWindow as MainWindow).Opacity = 1;
                    (Application.Current.MainWindow as MainWindow).Effect = null;
                }
            }catch
            {
                (Application.Current.MainWindow as MainWindow).Opacity = 0.9;
                (Application.Current.MainWindow as MainWindow).Effect = new BlurEffect();
                MessageBox.Show("There was an error loading the configuration file.", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                (Application.Current.MainWindow as MainWindow).Opacity = 1;
                (Application.Current.MainWindow as MainWindow).Effect = null;
            }
        }

        private static Type GetTypeOfConfigParam(Setting setting)
        {
            var type = "".GetType();
            try
            {
                int check = setting.IntValue;
                type = check.GetType();
                Console.WriteLine("int");
                goto FinishedChecking;
            }
            catch { }
            try
            {
                decimal check = setting.DecimalValue;
                type = check.GetType();
                Console.WriteLine("decimal");
                goto FinishedChecking;
            }
            catch { }
            try
            {
                float check = setting.FloatValue;
                type = check.GetType();
                Console.WriteLine("float");
                goto FinishedChecking;
            }
            catch { }
            try
            {
                bool check = setting.BoolValue;
                type = check.GetType();
                Console.WriteLine("bool");
                goto FinishedChecking;
            }
            catch { }

            Console.WriteLine("string");
            FinishedChecking:
            return type;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Effect = null;
            Application.Current.MainWindow.Opacity = 1;
            Close();
        }

        private void WindowTopBarClicked(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void DeclinePressed(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Effect = null;
            Application.Current.MainWindow.Opacity = 1;
            Close();
        }

        private void AcceptPressed(object sender, MouseButtonEventArgs e)
        {

            for (int i = 0; i < arrayOfSettings.Count; i++)
            {
                if (arrayOfTextBox[i].Tag.Equals("hasQuotes"))
                {
                    arrayOfSettings[i].SetValue(@"""" + arrayOfTextBox[i].Text + @"""");
                }
                else
                {
                    arrayOfSettings[i].SetValue(arrayOfTextBox[i].Text.ToString());
                }
            }
            config.SaveToFile(treatedPath);

            Application.Current.MainWindow.Effect = null;
            Application.Current.MainWindow.Opacity = 1;
            Close();
        }
    }
}
