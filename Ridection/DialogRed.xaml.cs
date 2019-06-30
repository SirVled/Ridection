using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace Ridection
{
    /// <summary>
    /// Interaction logic for DialogRed.xaml
    /// </summary>
    public partial class DialogRed : Window
    {
        private Rectangle recs; // Объект который отправляем после редактирования
        private Ellipse eles;  // Объект который отправляем после редактирования

        private Ellipse el; // Круг который редактируем
        private Rectangle rec; // Прямоугольник который редактируем

        private Color rgbFormat; // Цвет объекта который редактируется

        private double wight = 0; // Ширина
        private double height = 0; // Высота
        private double opacity = 0; // Прозрачность

        public DialogRed(Ellipse eles)
        {
            InitializeComponent();
            this.eles = eles;
            wight = eles.Width;
            height = eles.Height;

            opacity = eles.Opacity;
        }

        public DialogRed(Rectangle recs)
        {
            InitializeComponent();
            this.recs = recs;

            wight = recs.Width;
            height = recs.Height;

            opacity = recs.Opacity;
        }

        /// <summary>
        /// Загрузка окна
        /// </summary>
        /// <param name="sender">Окно</param>
        /// <param name="e">Loaded</param>
        private void StartRedect(object sender, RoutedEventArgs e)
        {         
            for (int i = 0; i < 11; i++)
                StackP.Children.Add(SetColor(i));

           
            //wWigh.Text = ((recs != null) ? recs.Width : eles.Width).ToString();
          //  hHeig.Text = ((recs != null) ? recs.Height : eles.Height).ToString();

            wWigh.Text = wight.ToString();
            hHeig.Text = height.ToString();

            SetObj();

            valOP.Text = opacity.ToString();
            Slid.Value = opacity;
           // valOP.Text = ((recs != null) ? recs.Opacity : eles.Opacity).ToString();
           // Slid.Value = ((recs != null) ? recs.Opacity : eles.Opacity);

            LoadImg.Click += LoadImges;
            
            if(recs != null)           
                rgbFormat = ((SolidColorBrush)(recs.Fill)).Color;     
            else
                rgbFormat = ((SolidColorBrush)(eles.Fill)).Color;  

            //Set Sllider and TextBox\\

            Trgb1.Text = ((int)rgbFormat.R).ToString();
            Trgb2.Text = ((int)rgbFormat.G).ToString();
            Trgb3.Text = ((int)rgbFormat.B).ToString();
            
        }

        /// <summary>
        /// Загрузка изображения на объект редактирования
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Click</param>
        private void LoadImges(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.FileName = "";

            Nullable<bool> result = dlg.ShowDialog();

            if(result == true)            
            {
                try
                {
                    if (recs != null)
                    {
                        rec.Fill = new ImageBrush(new BitmapImage(new Uri(dlg.FileName.ToString(), UriKind.RelativeOrAbsolute)));

                        MainWindow.recs.Fill = new ImageBrush(new BitmapImage(new Uri(dlg.FileName.ToString())));
                    }
                    else
                    {
                        el.Fill = new ImageBrush(new BitmapImage(new Uri(dlg.FileName.ToString(), UriKind.RelativeOrAbsolute)));

                        MainWindow.eles.Fill = new ImageBrush(new BitmapImage(new Uri(dlg.FileName.ToString())));
                    }
                }
                catch (NotSupportedException) { }
            }
        }

        /// <summary>
        /// Установка цвета на лейбл
        /// </summary>
        /// <param name="i">Номер цвета</param>
        /// <returns>Лабел с Foreground выбранного цвета</returns>
        private Label SetColor(int i)
        {
            Label lb = new Label();
            lb.FontFamily = new FontFamily("Times new Roman");
            lb.BorderThickness = new Thickness(1.5);
            lb.FontSize = 16;

            switch(i)
            {
                case 0:
                    lb.Content = "Red";
                    lb.BorderBrush = Brushes.Red;
                    lb.Foreground = Brushes.Red;
                    break;
                case 1:
                    lb.Content = "Blue";
                    lb.BorderBrush = Brushes.Blue;
                    lb.Foreground = Brushes.Blue;
                    break;
                case 2:
                    lb.Content = "Green";
                    lb.BorderBrush = Brushes.Green;
                    lb.Foreground = Brushes.Green;
                    break;
                case 3:
                    lb.Content = "Yellow";
                    lb.BorderBrush = Brushes.Yellow;
                    lb.Foreground = Brushes.Yellow;
                    break;
                case 4:
                    lb.Content = "Black";
                    lb.BorderBrush = Brushes.Black;
                    lb.Foreground = Brushes.Black;
                    break;
                case 5:
                    lb.Content = "White";
                    lb.BorderBrush = Brushes.White;
                    lb.Foreground = Brushes.Black;
                    break;
                case 6:
                    lb.Content = "Gray";
                    lb.BorderBrush = Brushes.Gray;
                    lb.Foreground = Brushes.Gray;
                    break;
                case 7:
                    lb.Content = "Purple";
                    lb.BorderBrush = Brushes.Purple;
                    lb.Foreground = Brushes.Purple;
                    break;
                case 8:
                    lb.Content = "Brown";
                    lb.BorderBrush = Brushes.Brown;
                    lb.Foreground = Brushes.Brown;
                    break;
                case 9:
                    lb.Content = "Orange";
                    lb.BorderBrush = Brushes.Orange;
                    lb.Foreground = Brushes.Orange;
                    break;
                case 10:
                    lb.Content = "Pink";
                    lb.BorderBrush = Brushes.Pink;
                    lb.Foreground = Brushes.Pink;
                    break;
            }

            lb.MouseLeftButtonDown += SetColorObj;
            return lb;
            
        }

        /// <summary>
        /// Установка цвета для объекта редактирования
        /// </summary>
        /// <param name="sender">Slider</param>
        /// <param name="e">Передвижение ползунка слийдера</param>
        private void SetColorObj(object sender, MouseButtonEventArgs e)
        {
            if (recs != null)
            {
                rec.Fill = (sender as Label).BorderBrush;
                MainWindow.recs.Fill = (sender as Label).BorderBrush;

                rgbFormat = ((SolidColorBrush)(rec.Fill)).Color;           
            }
            else
            {
                el.Fill = (sender as Label).BorderBrush;
                MainWindow.eles.Fill = (sender as Label).BorderBrush;

                rgbFormat = ((SolidColorBrush)(el.Fill)).Color;

            }

            Trgb1.Text = ((int)rgbFormat.R).ToString();
            Trgb2.Text = ((int)rgbFormat.G).ToString();
            Trgb3.Text = ((int)rgbFormat.B).ToString();
        }
        
        private void SetObj()
        {
            if (recs != null)
            {
                rec = new Rectangle { 
                   VerticalAlignment = VerticalAlignment.Top,
                   HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                   Width = Del_Lab.Width,
                   Height = Del_Lab.Height,
                   Margin = Del_Lab.Margin,
                   Opacity = recs.Opacity,
                   Fill = recs.Fill
                };
            
                sObj.Children.Add(rec);
            }
            else if(eles != null)
            {
                el = new Ellipse
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    Width = Del_Lab.Width,
                    Height = Del_Lab.Height,
                    Margin = Del_Lab.Margin,
                    Opacity = eles.Opacity,
                    Fill = eles.Fill
                };

                sObj.Children.Add(el);
            }

            sObj.Children.Remove(Del_Lab);
        }
        /// <summary>
        /// ++ ширина объекта
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Click</param>
        private void Click_to_P_Wight(object sender, RoutedEventArgs e)
        {
            try
            {
                wWigh.Text = (Int32.Parse(wWigh.Text.ToString()) + 1).ToString();
                if(recs != null)
                {
                    MainWindow.recs.Width = Double.Parse(wWigh.Text);
                }
                else
                {
                    MainWindow.eles.Width = Double.Parse(wWigh.Text);
                }
            }
            catch (FormatException) { wWigh.Text = 1.ToString(); }
        }

        /// <summary>
        /// -- ширина объекта
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Click</param>
        private void Click_to_M_Wight(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Int32.Parse(wWigh.Text.ToString()) > 1)
                
                    wWigh.Text = (Int32.Parse(wWigh.Text.ToString()) - 1).ToString();

                if (recs != null)
                {
                    MainWindow.recs.Width = Double.Parse(wWigh.Text);
                }
                else
                {
                    MainWindow.eles.Width = Double.Parse(wWigh.Text);
                }              
            }
            catch (FormatException) { }
        }

        /// <summary>
        /// ++ высота объекта
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Click</param>
        private void Click_to_P_Hight(object sender, RoutedEventArgs e)
        {
            try
            {
                hHeig.Text = (Int32.Parse(hHeig.Text.ToString()) + 1).ToString();

                if (recs != null)
                {
                    MainWindow.recs.Height = Double.Parse(hHeig.Text);
                }
                else
                {
                    MainWindow.eles.Height = Double.Parse(hHeig.Text);
                }
            }
            catch (FormatException) { hHeig.Text = 1.ToString(); }
        }

        /// <summary>
        /// -- высота объекта
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Click</param>
        private void Click_to_M_Hight(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Int32.Parse(hHeig.Text.ToString()) > 1)  
                    hHeig.Text = (Int32.Parse(hHeig.Text.ToString()) - 1).ToString();

                if (recs != null)
                {
                    MainWindow.recs.Height = Double.Parse(hHeig.Text);
                }
                else
                {
                    MainWindow.eles.Height = Double.Parse(hHeig.Text);
                }
            }
            catch (FormatException) { }
        }


        private void ValueRef(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            valOP.Text = ((e.NewValue.ToString().Length >= 5) ? e.NewValue.ToString().Remove(4) : e.NewValue.ToString());

            if (recs != null)
            {
                rec.Opacity = e.NewValue;
                MainWindow.recs.Opacity = e.NewValue;
            }
            else
            {
                el.Opacity = e.NewValue;
                MainWindow.eles.Opacity = e.NewValue;
            }
        }

        private void Valid_Text(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0) || (sender as TextBox).Text.Length >= 3)
                e.Handled = true;

            
        }

        private void Save(object sender, RoutedEventArgs e)
        {

            if(recs != null)
            {
                MainWindow.recs.Fill = rec.Fill;
                MainWindow.recs.Opacity = Slid.Value;
                MainWindow.recs.Width = (!wWigh.Text.Equals("")) ? Double.Parse(wWigh.Text) : MainWindow.recs.Width;
                MainWindow.recs.Height = (!hHeig.Text.Equals("")) ? Double.Parse(hHeig.Text) : MainWindow.recs.Height;
            }
            else
            {
                MainWindow.eles.Fill = el.Fill;
                MainWindow.eles.Opacity = Slid.Value;
                MainWindow.eles.Width = (!wWigh.Text.Equals("")) ? Double.Parse(wWigh.Text) : MainWindow.eles.Width;
                MainWindow.eles.Height = (!hHeig.Text.Equals("")) ? Double.Parse(hHeig.Text) : MainWindow.eles.Height;
            }

            Close();
        }

        private void SetSizeObj(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (recs != null)
                {
                    MainWindow.recs.Width = (!wWigh.Text.Equals("")) ? Double.Parse(wWigh.Text) : MainWindow.recs.Width;
                    MainWindow.recs.Height = (!hHeig.Text.Equals("")) ? Double.Parse(hHeig.Text) : MainWindow.recs.Height;
                }
                else if (eles != null)
                {
                    MainWindow.eles.Width = (!wWigh.Text.Equals("")) ? Double.Parse(wWigh.Text) : MainWindow.eles.Width;
                    MainWindow.eles.Height = (!hHeig.Text.Equals("")) ? Double.Parse(hHeig.Text) : MainWindow.eles.Height;
                }
            }
            catch (Exception) { }
        }

        private void Valid_RGB(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0) || (sender as TextBox).Text.Length >= 3 )
                e.Handled = true;

        }

        private void CheckRgbFormat(object sender, TextChangedEventArgs e)
        {
            try
            {               
                if (Int32.Parse((sender as TextBox).Text) > 255)
                    (sender as TextBox).Text = "255";

                if ((sender as TextBox).Text[0].Equals('0'))
                {
                    (sender as TextBox).Text = "0";
                }

                checkToRgb((sender as TextBox).Name);
            }
            catch (Exception) { (sender as TextBox).Text = ""; }
        }

        private void checkToRgb(string p)
        {
            switch(p)
            {
                case "Trgb1":
                    Srgb1.Value = Int32.Parse(Trgb1.Text);
                    break;
                case "Trgb2":
                    Srgb2.Value = Int32.Parse(Trgb2.Text);
                    break;
                case "Trgb3":
                    Srgb3.Value = Int32.Parse(Trgb3.Text);
                    break;
            }
        }

        private void SlRgb(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                СheckToSlider((sender as Slider).Name, e.NewValue);
           
                SetRgbColor();
            }
            catch (Exception) { }
        }

        private void SetRgbColor()
        {
            if (recs != null)
            {
                rec.Fill = new SolidColorBrush(Color.FromRgb((byte)Srgb1.Value, (byte)Srgb2.Value, (byte)Srgb3.Value));
                MainWindow.recs.Fill = rec.Fill;
            }
            else
            {
                el.Fill = new SolidColorBrush(Color.FromRgb((byte)Srgb1.Value, (byte)Srgb2.Value, (byte)Srgb3.Value));
                MainWindow.eles.Fill = el.Fill;
            }
        }

        private void СheckToSlider(string p1, double p2)
        {
            switch(p1)
            {
                case "Srgb1" :
                    Trgb1.Text = ((int)p2).ToString();
                    break;
                case "Srgb2":
                    Trgb2.Text = ((int)p2).ToString();
                    break;
                case "Srgb3":
                    Trgb3.Text = ((int)p2).ToString();
                    break;
            }
        }         
    }
}
