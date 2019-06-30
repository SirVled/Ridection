using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
using System.Windows.Threading;

namespace Ridection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private List<Rectangle> reces = new List<Rectangle>(); // Массив прямоугольников на форме
        private List<char> Zitem = new List<char>(); // Массив символов которые служат для определения последовательности Z - координат

        private List<Ellipse> elleps = new List<Ellipse>(); // Массив кружков на форме

        private Rectangle rec;
        private Ellipse el; 

        public int CountGG = 0; // Количество объектов на форме


        public static Ellipse eles { get; set; }// Кружок который выполняет перемещение
        public static Rectangle recs { get; set; } // Прямоугольник который выполняет перемещение

        private UIElement elem { get; set; } // Элемент по которому производится перемещение мыши
       
        private Point mouse { get; set; } // Позиция курсора мыши

        private Boolean trueMouseLeftButt = false; // Определяем зажата ли левая клавиша мыши

        private Thread sd; // Поток в котором выполняется постройка
        

        private DialogRed red; // Класс редактирования объекта

        /// <summary>
        /// Загрузка окна
        /// </summary>
        /// <param name="sender">Window</param>
        /// <param name="e">Loaded</param>
        private void Load(object sender, RoutedEventArgs e)
        {
          //  AddRec();           

            Button bt = new Button();

            bt.VerticalAlignment = VerticalAlignment.Bottom;
            bt.HorizontalAlignment = HorizontalAlignment.Right;
            bt.Height = 30;
            bt.Content = "Сохранить";
            bt.Click += SaveFile;
            gg.Children.Add(bt);

            bt = new Button();
            bt.VerticalAlignment = VerticalAlignment.Bottom;
            bt.HorizontalAlignment = HorizontalAlignment.Left;
            bt.Height = 30;
            bt.Margin = new Thickness(20, 0, 0, 0);
            bt.Content = "Загрузить";
            bt.Click += LoadFile;        
            gg.Children.Add(bt);

            bt = new Button();
            bt.VerticalAlignment = VerticalAlignment.Bottom;
            bt.HorizontalAlignment = HorizontalAlignment.Left;
            bt.Height = 30;
            bt.Content = "☻";
            bt.Click += Clear_all;
            gg.Children.Add(bt);

            bt = new Button();
            bt.VerticalAlignment = VerticalAlignment.Bottom;
            bt.HorizontalAlignment = HorizontalAlignment.Right;
            bt.Height = 30;
            bt.Content = "Загрузить img";
            bt.Click += ImageAdd;
            bt.Margin = new Thickness(0, 0, 70, 0);
            gg.Children.Add(bt);

            Application ap = Application.Current;
            ap.Exit += AppExit;
            MouseMove += MouseMoveL;

            this.MouseLeftButtonUp += MfB;

            this.KeyDown += CreateObj;

            CountGG = gg.Children.Count;
        }

        /// <summary>
        /// Добавление картинки
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Click</param>
        private void ImageAdd(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.FileName = "";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                sd = new Thread((ThreadImg));
                sd.SetApartmentState(ApartmentState.STA);
               
                sd.Start(dlg.FileName);
            }
        }

        /// <summary>
        /// Создание картинки
        /// </summary>
        /// <param name="obj">Путь картинки</param>
        private void ThreadImg(object obj)
        {
            int xQu = 2;

            Boolean tr = true;
            BitmapImage bitmap = new BitmapImage();
            try
            {

                //загрузка картинки
                bitmap.BeginInit();
                bitmap.UriSource = new Uri((string)obj);

                bitmap.EndInit();
            }
            catch (NotSupportedException) { tr = false; }

            Boolean recOrEllips = true;
           if(MessageBox.Show("Отобразить Rec объектами или Ellispse?", "Отображение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                recOrEllips = false;
            }          
            try
            {
                for (int i = 0; i < bitmap.PixelHeight / xQu; i++)
                {
                    for (int j = 0; j < bitmap.PixelWidth / xQu; j++)
                    {
                        if (j * xQu < (bitmap.PixelWidth) && i * xQu < (bitmap.PixelHeight))
                        {

                            CroppedBitmap cb = new CroppedBitmap(bitmap, new Int32Rect(j * xQu, i * xQu, 1, 1));
                            var pixels = new byte[4];
                            cb.CopyPixels(pixels, 4, 0);
                            if ((byte)pixels[3] != 0)
                            {
                                
                                //  MessageBox.Show((byte)pixels[0] + " " + (byte)pixels[1] + " " + (byte)pixels[2] + " ////" + (byte)pixels[3]);
                                gg.Dispatcher.Invoke(/*DispatcherPriority.Normal,*/ new ThreadStart(() =>
                                {
                                    if (recOrEllips)
                                    {
                                        Zitem.Add('r');
                                        AddImgRec(xQu, i, j, pixels);
                                    }
                                    else
                                    {
                                        Zitem.Add('e');
                                        AddImgEllipse(xQu, i, j, pixels);
                                    }
                                }));
                            }
                        }
                    }
                }
            }
            catch (ArgumentException) { }
            finally
            {
                if (tr)
                    MessageBox.Show("Усё");
                else
                    MessageBox.Show("Ошибка формата картинки!", "Ошибка!!!", MessageBoxButton.OK, MessageBoxImage.Warning);
                sd.Abort();
            }
        }

        /// <summary>
        /// Постройка картинки кружками
        /// </summary>
        /// <param name="xQu">Качество картинки</param>
        /// <param name="i">Высона</param>
        /// <param name="j">Ширина</param>
        /// <param name="pixels">Массив цветов</param>
        private void AddImgEllipse(int xQu, int i, int j, byte[] pixels)
        {
            Ellipse reccc = new Ellipse
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                Width = xQu * 2,
                Height = xQu * 2,
                Fill = new SolidColorBrush(Color.FromRgb(pixels[2], pixels[1], pixels[0])),
                Margin = new Thickness(95 + j * xQu * 2, i * xQu * 2, 0, 0),

            };
            
            elleps.Add(reccc);


            reccc.MouseLeftButtonDown += TrCl;
            reccc.MouseRightButtonDown += DeleteObj;

            gg.Children.Add(reccc);
        }

        /// <summary>
        /// Постройка картинки прямоугольниками
        /// </summary>
        /// <param name="xQu">Качество картинки</param>
        /// <param name="i">Высона</param>
        /// <param name="j">Ширина</param>
        /// <param name="pixels">Массив цветов</param>
        private void AddImgRec(int xQu, int i, int j, byte[] pixels)
        {
            Rectangle reccc = new Rectangle
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                Width = xQu * 2,
                Height = xQu * 2,
                Fill = new SolidColorBrush(Color.FromRgb(pixels[2], pixels[1], pixels[0])),
                Margin = new Thickness(95 + j * xQu * 2, i * xQu * 2, 0, 0),

            };

            reces.Add(reccc);

            reccc.MouseLeftButtonDown += TrCl;
            reccc.MouseRightButtonDown += DeleteObj;

            gg.Children.Add(reccc);
        }
        

        /// <summary>
        /// Очистка всех объектов из формы
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Click</param>
        private void Clear_all(object sender, RoutedEventArgs e)
        {
            reces.Clear();
            elleps.Clear();
            Zitem.Clear();

            if (sd != null)
               sd.Abort();

            gg.Children.RemoveRange(CountGG, gg.Children.Count);
        }

        /// <summary>
        /// Загрузка картинки (через блокнот)
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Click</param>
        private void LoadFile(object sender, RoutedEventArgs e)
        {
            
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.FileName = "";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";
  
            Nullable<bool> result = dlg.ShowDialog();          
            if (result == true)
            {
                sd = new Thread((ThreadFileRead));
                sd.SetApartmentState(ApartmentState.STA);

                sd.Start(dlg.FileName);
                
            }
            
        }

        /// <summary>
        /// Постройка формы через блокнот
        /// </summary>
        /// <param name="obj">Путь до файла</param>
        private void ThreadFileRead(object obj)
        {
            StreamReader file = null;
            try
            {
                file = new StreamReader((string)obj);
                String[] ss;
                do
                {
                    ss = new String[6];

                    ss = file.ReadLine().Replace(" :", "").Split(' ');

                    if (ss[0].Equals("Rec"))
                    {
                        gg.Dispatcher.Invoke(new ThreadStart(() => { 
                        Rectangle cek = new Rectangle();
                        var converter = new System.Windows.Media.BrushConverter();
                        var brush = (Brush)converter.ConvertFromString(ss[9]);

                        cek.Fill = brush;
                        cek.Opacity = Double.Parse(ss[7]);
                        cek.Margin = new Thickness(Double.Parse(ss[1]), Double.Parse(ss[2]), 0, 0);

                        cek.VerticalAlignment = VerticalAlignment.Top;
                        cek.HorizontalAlignment = HorizontalAlignment.Left;

                        cek.Width = Double.Parse(ss[4]);
                        cek.Height = Double.Parse(ss[5]);

                        cek.MouseLeftButtonDown += TrCl;
                        cek.MouseRightButtonDown += DeleteObj;


                        gg.Children.Add(cek);

                        reces.Add(cek);

                        Zitem.Add('r');
                        }));
                    }
                    else
                    {
                        
                        gg.Dispatcher.Invoke(new ThreadStart(() => {
                            try
                            {
                                Ellipse cek = new Ellipse();

                                var converter = new System.Windows.Media.BrushConverter();
                                var brush = (Brush)converter.ConvertFromString(ss[9]);

                                cek.Fill = brush;
                                cek.Opacity = Double.Parse(ss[7]);
                                cek.Margin = new Thickness(Double.Parse(ss[1]), Double.Parse(ss[2]), 0, 0);

                                cek.VerticalAlignment = VerticalAlignment.Top;
                                cek.HorizontalAlignment = HorizontalAlignment.Left;
                                cek.Width = Double.Parse(ss[4]);
                                cek.Height = Double.Parse(ss[5]);

                                cek.MouseLeftButtonDown += TrCl;
                                cek.MouseRightButtonDown += DeleteObj;


                                gg.Children.Add(cek);


                                elleps.Add(cek);

                                Zitem.Add('e');
                            }
                            catch (Exception) { }
                           
                        }));
                    
                    }
                   
                    for (int i = 0; i < ss.Length; i++)
                    {
                        Console.WriteLine(ss[i]);
                    }
                } while (ss[1] != null);
            }
            catch (NullReferenceException) { }
            catch (FormatException) { }
            catch (IndexOutOfRangeException) { }

            finally
            {
                if (file != null)
                    file.Close();
            }
        }

        /// <summary>
        /// Сохранение всех объектов в файл
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Click</param>
        private void SaveFile(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.FileName = "Cor";

            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";
            int tempR = 0;
            int tempE = 0;
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                StreamWriter file = new StreamWriter(dlg.FileName);
                try
                {

                    for (int i = 0; i < Zitem.Count; i++)
                    {
                        if (Zitem[i].Equals('r'))
                        {                         
                            file.WriteLine("Rec : " + reces[tempR].Margin.Left + " " + reces[tempR].Margin.Top + " Size : "
                                + reces[tempR].ActualWidth + " " + reces[tempR].ActualHeight + " Opacity : " + reces[tempR].Opacity + " Fill : " + reces[tempR].Fill);
                            tempR++;                        
                        }
                        else
                        {
                            file.WriteLine("Ellips : " + elleps[tempE].Margin.Left + " " + elleps[tempE].Margin.Top + " Size : "
                                    + elleps[tempE].ActualWidth + " " + elleps[tempE].ActualHeight + " Opacity : " + elleps[tempE].Opacity + " Fill : " + elleps[tempE].Fill);
                            tempE++;
                        }
                    }
                }
                finally
                {
                    file.Close();
                }
                MessageBox.Show("Кол-во объектов сохраненно : " + (gg.Children.Count - CountGG).ToString());     
            }                            
        }

        /// <summary>
        /// Создание объектов по нажатию клавиш (NumPad1,NumPad2,NumPad3,NumPad4)
        /// </summary>
        /// <param name="sender">Window</param>
        /// <param name="e">KetDown</param>
        private void CreateObj(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.NumPad1: 
                    case Key.D1:
                        AddRec(75,75);
                        break;
                case Key.NumPad2:
                    case Key.D2:
                        AddElip(75,75);
                        break;
                case Key.NumPad3:
                    case Key.D3:
                        AddRec(95, 50);
                        break;
                case Key.NumPad4:
                    case Key.D4:
                        AddElip(95, 50);
                        break;                      
            }
        }

        /// <summary>
        /// Удаление объекта
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">MouseRightDown</param>
        private void DeleteObj(object sender, MouseButtonEventArgs e)
        {
            var re = sender as Rectangle;
            var elap = sender as Ellipse;

            if(re != null)
            {
                reces.Remove(re);
                Zitem.Remove('r');
                gg.Children.Remove(re);
            }
            else
            {
                elleps.Remove(elap);
                Zitem.Remove('e');               
                gg.Children.Remove(elap);
            }

        }

        /// <summary>
        /// Добавление кружка
        /// </summary>
        /// <param name="Width">Ширина</param>
        /// <param name="Height">Высота</param>
        private void AddElip(int Width, int Height)
        {
            el = new Ellipse();

            Zitem.Add('e');
            elleps.Add(el);

            el.Width = Width;
            el.Height = Height;

            el.Fill = Brushes.Black;
            el.Opacity = 0.66;

            el.VerticalAlignment = VerticalAlignment.Top;
            el.HorizontalAlignment = HorizontalAlignment.Left;

            gg.Children.Add(el);


            el.MouseRightButtonDown += DeleteObj;
            el.MouseLeftButtonDown += TrCl;
        }
   
        /// <summary>
        /// Закрытие приложение
        /// </summary>
        /// <param name="sender">Application</param>
        /// <param name="e">Exit</param>
        private void AppExit(object sender, ExitEventArgs e)
        {
            if (sd != null)
                sd.Abort();
        }
        /// <summary>
        /// Добавление прямоугольника
        /// </summary>
        /// <param name="Width">Ширина</param>
        /// <param name="Height">Высота</param>
        private void AddRec(int Width, int Height)
        {
            rec = new Rectangle();

            Zitem.Add('r');
            reces.Add(rec);

            rec.Width = Width;
            rec.Height = Height;

            rec.Fill = Brushes.Black;
            rec.Opacity = 0.66;
         
            rec.VerticalAlignment = VerticalAlignment.Top;
            rec.HorizontalAlignment = HorizontalAlignment.Left;

            gg.Children.Add(rec);

            rec.MouseRightButtonDown += DeleteObj;
            rec.MouseLeftButtonDown += TrCl;
        }         
      
        /// <summary>
        /// Отмена перетягивания объекта
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">MouseLeftUp</param>
        private void MfB(object sender, MouseButtonEventArgs e)
        {
            trueMouseLeftButt = false;
        }

        /// <summary>
        /// Перемещение объекта
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">МouseLeftDown</param>
        private void TrCl(object sender, MouseButtonEventArgs e)
        {
           
            // if ((sender as Rectangle).Equals(recs))
            //{
              //  timers.Stop();
            //}
            //else

            recs = sender as Rectangle;
            eles = sender as Ellipse;
            
            if (e.ClickCount == 1)
            {
                trueMouseLeftButt = true;
            }
            else if(e.ClickCount == 2)
            {

                if(recs != null)
                     red = new DialogRed(recs);
                else
                     red = new DialogRed(eles);


                red.ShowDialog();
            }
        }

        /// <summary>
        /// Перемещение объекта
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">МouseMove</param>
        private void MouseMoveL(object sender, MouseEventArgs e)
        {
            if (trueMouseLeftButt && e.LeftButton.ToString().Equals("Pressed"))
            {
                elem = (VisualTreeHelper.GetParent(gg) as UIElement);
                mouse = e.GetPosition(elem);
                if(recs != null)
                    recs.Margin = new Thickness((mouse.X - recs.ActualWidth / 2 > 1 /*& mouse.X + recs.ActualWidth / 2 + 20 < this.ActualWidth*/) ? mouse.X - recs.ActualWidth / 2 : 0, 
                        (mouse.Y - recs.ActualHeight / 2 > 1 /*& mouse.Y + recs.ActualHeight + 5 < this.ActualHeight*/) ? mouse.Y - recs.ActualHeight / 2 : 0, 0, 0);
                else if(!eles.Equals(null))
                    eles.Margin = new Thickness((mouse.X - eles.ActualWidth / 2 > 1 /*& mouse.X + eles.ActualWidth / 2 + 20 < this.ActualWidth*/) ? mouse.X - eles.ActualWidth / 2 : 0,
                        (mouse.Y - eles.ActualHeight / 2 > 1 /*& mouse.Y + eles.ActualHeight + 5 < this.ActualHeight*/) ? mouse.Y - eles.ActualHeight / 2 :0, 0, 0);
            }
        }

        /// <summary>
        /// DragDrop картинки на форму
        /// </summary>
        /// <param name="sender">Окно</param>
        /// <param name="e">DragDrop</param>
        private void DropImgOrFile(object sender, DragEventArgs e)
        {
            string[] file = (string[])e.Data.GetData(DataFormats.FileDrop);
            sd = new Thread((ThreadImg));
            sd.SetApartmentState(ApartmentState.STA);

            sd.Start(file[0]);           
        }
    }
}
