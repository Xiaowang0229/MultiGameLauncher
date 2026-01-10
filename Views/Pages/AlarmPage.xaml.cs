using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MenuItem = System.Windows.Controls.MenuItem;
using TextBox = System.Windows.Controls.TextBox;

namespace MultiGameLauncher.Views.Pages
{
    /// <summary>
    /// AlarmPage.xaml 的交互逻辑
    /// </summary>
    public partial class AlarmPage : Page
    {
        private List<StackPanel> animationSP = new();
        public AlarmPage()
        {
            InitializeComponent();
            Loaded += (async (s, e) =>
            {
                try
                {
                    animationSP.Clear();
                    foreach (var sp in sp_ani.Children)
                    {
                        if (((StackPanel)sp).Tag != null)
                            if (((StackPanel)sp).Tag.ToString() == "ani")
                            {
                                animationSP.Add((StackPanel)sp);
                            }
                    }

                    foreach (var spp in animationSP)
                    {
                        spp.Margin = new Thickness(-2000, 0, 0, 10);
                    }

                    var animation = new ThicknessAnimation
                    {
                        To = new Thickness(0, 0, 0, 10),
                        Duration = TimeSpan.FromMilliseconds(500),
                        EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseOut }
                    };

                    foreach (var aniSP in animationSP)
                    {
                        aniSP.BeginAnimation(MarginProperty, animation);
                        await Task.Delay(100);
                    }
                }
                catch (InvalidOperationException) { }
                catch (Exception ex)
                {
                    
                }


            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            win.RootFrame.Navigate(new Launch());
        }

        private void GameTime_LostFocus(object sender, RoutedEventArgs e)
        {
            Variables.AlarmTime = TimeSpan.FromMinutes(int.Parse(GameTime.Text));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(Variables.UsingRealTimeAlarm != null)
            {
                if(Variables.UsingRealTimeAlarm == true)
                {
                    RealTimeRadio.IsChecked = true;
                    RealTimeStackPanel.Visibility = Visibility.Visible;
                    GameTimeStackPanel.Visibility = Visibility.Collapsed;
                    MinButton.Content = Variables.AlarmRealTime.Substring(3, 2);
                    HourButton.Content = Variables.AlarmRealTime.Substring(0, 2);
                }
                else if(Variables.UsingRealTimeAlarm == false)
                {
                    GameTimeRadio.IsChecked = true;
                    RealTimeStackPanel.Visibility = Visibility.Collapsed;
                    GameTimeStackPanel.Visibility = Visibility.Visible;
                    GameTime.Text = Variables.AlarmTime.TotalMinutes.ToString();
                }
            }
            

                for (int i = 0; i < 24; i++)
                {
                    if (i < 10)
                    {
                        var item = new MenuItem
                        {
                            Header = "0" + i.ToString()
                        };
                        item.Click += HourItemClick;
                        HourButton.Items.Add(item);
                    }
                    else
                    {
                        var item = new MenuItem
                        {
                            Header = i.ToString()
                        };
                        item.Click += HourItemClick;
                        HourButton.Items.Add(item);
                    }


                }
            for (int i = 0; i < 60; i++)
            {
                if(i < 10)
                {
                    var item = new MenuItem
                    {
                        Header = "0" + i.ToString()
                    };
                    item.Click += MinItemClick;
                    MinButton.Items.Add(item);
                }
                else
                {
                    var item = new MenuItem
                    {
                        Header = i.ToString()
                    };
                    item.Click += MinItemClick;
                    MinButton.Items.Add(item);
                }
            }
        }

        private void MinItemClick(object sender, RoutedEventArgs e)
        {
            MinButton.Content = ((MenuItem)sender).Header;
        }

        private void HourItemClick(object sender, RoutedEventArgs e)
        {
            HourButton.Content = ((MenuItem)sender).Header;
        }

        private void RealTimeRadio_Click(object sender, RoutedEventArgs e)
        {
            if(RealTimeRadio.IsChecked == true)
            {
                RealTimeStackPanel.Visibility = Visibility.Visible;
                GameTimeStackPanel.Visibility = Visibility.Collapsed;
                GameTimeRadio.IsChecked = false;
            }
        }

        private void GameTimeRadio_Click(object sender, RoutedEventArgs e)
        {
            if (GameTimeRadio.IsChecked == true)
            {
                GameTimeStackPanel.Visibility = Visibility.Visible;
                RealTimeStackPanel.Visibility = Visibility.Collapsed;
                RealTimeRadio.IsChecked = false;
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 只允许数字和小数点（如果需要小数）
            if (!Regex.IsMatch(e.Text, @"^[0-9\.]+$"))
            {
                e.Handled = true;  // 拦截非法输入
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // 允许 Backspace、Delete、Tab、Left/Right 等功能键
            if (e.Key == Key.Space || e.Key == Key.Back || e.Key == Key.Delete ||
                e.Key == Key.Tab || e.Key == Key.Left || e.Key == Key.Right)
            {
                return;
            }
            // 如果需要小数，只允许一个点
            if (e.Key == Key.Decimal || e.Key == Key.OemPeriod)
            {
                TextBox tb = sender as TextBox;
                if (tb.Text.Contains("."))
                    e.Handled = true;
            }
        }

        private void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            // 拦截粘贴非法内容
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!Regex.IsMatch(text, @"^[0-9\.]+$"))
                {
                    e.CancelCommand();
                    
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int MIN_VALUE = 1;
            int MAX_VALUE = 2147483647;
            TextBox tb = sender as TextBox;

            if (string.IsNullOrEmpty(tb.Text))
            {
                return;
            }

            // 移除非数字字符（双保险）
            string cleaned = Regex.Replace(tb.Text, @"[^\d]", "");

            if (long.TryParse(cleaned, out long value))
            {
                if (value < MIN_VALUE)
                {
                    tb.Text = MIN_VALUE.ToString();
                    tb.SelectionStart = tb.Text.Length;
                }
                else if (value > MAX_VALUE)
                {
                    tb.Text = MAX_VALUE.ToString();
                    tb.SelectionStart = tb.Text.Length;
                }
                else
                {
                    // 正常范围内，只更新去除非法字符后的内容
                    if (cleaned != tb.Text)
                    {
                        tb.Text = cleaned;
                        tb.SelectionStart = tb.Text.Length;
                    }
                }
            }
            else
            {
                // 解析失败（理论上不会发生）
                tb.Text = MIN_VALUE.ToString();
                tb.SelectionStart = tb.Text.Length;
            }
        }

        

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(RealTimeRadio.IsChecked == true)
            {
                if(!(string.IsNullOrEmpty(MinButton.Content.ToString())) && !(string.IsNullOrEmpty(HourButton.Content.ToString())))
                {
                    Variables.UsingRealTimeAlarm = true;
                    Variables.AlarmRealTime = $"{HourButton.Content.ToString()}:{MinButton.Content.ToString()}";
                    var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                    win.RootFrame.Navigate(new Launch());
                }
                else
                {
                    Tools.GetShowingWindow().ShowMessageAsync("错误", $"请输入分钟数后再点击此按钮！");
                }
            }
            else if(GameTimeRadio.IsChecked == true)
            {
                if(!string.IsNullOrEmpty(GameTime.Text))
                {
                    Variables.AlarmCTS.Cancel();
                    Variables.AlarmCTS = new CancellationTokenSource();
                    Variables.UsingRealTimeAlarm = false;
                    Variables.AlarmTime = TimeSpan.FromMinutes(int.Parse(GameTime.Text));
                    var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                    win.GametimeAlarm();
                    win.RootFrame.Navigate(new Launch());
                }
                else
                {
                    Tools.GetShowingWindow().ShowMessageAsync("错误", $"请输入分钟数后再点击此按钮！");
                }
            }
            else
            {
                Tools.GetShowingWindow().ShowMessageAsync("错误", $"请选择后再点击此按钮！");
            }
        }
    }
}
