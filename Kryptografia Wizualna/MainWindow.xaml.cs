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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Path = System.IO.Path;
using System.Drawing;
using Color = System.Drawing.Color;

namespace Kryptografia_Wizualna
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Bitmap Udzial1;
        Bitmap Udzial2;
        Bitmap Obraz;
        Bitmap Zdeszyfrowany;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void WybierzObraz(TextBox Box)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Obrazy bitmap (*.bmp)|*.bmp|Tagged Image File Format (*.tif)|*.tif|Joint Photographic Experts Group (*.jpg)|*.jpg|Graphic Interchange Format (*.gif)|*.gif|Portable Network Graphics (*.png)|*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                Box.Text = openFileDialog.FileName;
            }
        }
        //wczytanie obrazu do szyfrowania i zaproponowanie przykładowych nazw do zapisu
        private void BWczytObraz_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Obrazy bitmap (*.bmp)|*.bmp|Tagged Image File Format (*.tif)|*.tif|Joint Photographic Experts Group (*.jpg)|*.jpg|Graphic Interchange Format (*.gif)|*.gif|Portable Network Graphics (*.png)|*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                BoxWczytPocz.Text = openFileDialog.FileName;
                BoxZapisUdzial1.Text = BoxWczytPocz.Text.Insert(BoxWczytPocz.Text.Length - 4, "Udzial1");
                BoxZapisUdzial2.Text = BoxWczytPocz.Text.Insert(BoxWczytPocz.Text.Length - 4, "Udzial2");
                Obraz = new Bitmap(openFileDialog.FileName);
                CzarnoBialy(Obraz);
                ImageSzyfrowany.Source = BitmapToImageSource(Obraz);
                BoxLogi.Text += "Wczytano obraz do zaszyfrowania: " + BoxWczytPocz.Text + "\n";
            }
        }
        //wczytywanie udziałów do rozszyfrowania
        private void BWczytUdzial1_Click(object sender, RoutedEventArgs e)
        {
            WybierzObraz(BoxWczytUdzial1);
            Udzial1 = new Bitmap(BoxWczytUdzial1.Text);
            ImageUdzial1.Source = BitmapToImageSource(Udzial1);
            BoxLogi.Text += "Wczytano pierwszy udział do odszyfrowania obrazu: " + BoxWczytUdzial1.Text + "\n";
        }

        private void BWybierzUdzial2_Click(object sender, RoutedEventArgs e)
        {
            WybierzObraz(BoxWczytUdzial2);
            Udzial2 = new Bitmap(BoxWczytUdzial2.Text);
            ImageUdzial2.Source = BitmapToImageSource(Udzial2);
            BoxLogi.Text += "Wczytano drugi udział do odszyfrowania obrazu: " + BoxWczytUdzial2.Text + "\n";
        }

        private void BoxWczytPocz_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BoxWczytPocz.Text.Length > 0) BKoduj.IsEnabled = true;
            else BKoduj.IsEnabled = false;
        }

        private void BoxWczytUdzial1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(BoxWczytUdzial1.Text.Length>0 && BoxWczytUdzial2.Text.Length>0)
            {
                BoxKoncowy.Text = Path.GetDirectoryName(BoxWczytUdzial2.Text) + "\\Koncowy.jpg";
                BDekoduj.IsEnabled = true;
            }
            else
            {
                BDekoduj.IsEnabled = false;
            }
        }

        private void BoxWczytUdzial2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BoxWczytUdzial1.Text.Length > 0 && BoxWczytUdzial2.Text.Length > 0)
            {
                BoxKoncowy.Text = Path.GetDirectoryName(BoxWczytUdzial2.Text) + "\\Koncowy.jpg";
                BDekoduj.IsEnabled = true;
            }
            else
            {
                BDekoduj.IsEnabled = false;
            }
        }
        //metoda do przekształcania obrazu na czarno-biały
        public void CzarnoBialy(Bitmap Bmp)
        {
            int rgb;
            Color c;

            for (int y = 0; y < Bmp.Height; y++)
                for (int x = 0; x < Bmp.Width; x++)
                {
                    c = Bmp.GetPixel(x, y);
                    rgb = (int)Math.Round(.299 * c.R + .587 * c.G + .114 * c.B);
                    if (rgb < 128) rgb = 0;
                    else rgb = 255;
                    Bmp.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
                }
        }
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
        private void BKoduj_Click(object sender, RoutedEventArgs e)
        {
            BKoduj.IsEnabled = false;
            int width = Obraz.Width;
            int height = Obraz.Height;
            Random rnd = new Random();
            int numer = 0;
            Udzial1 = new Bitmap(width*2, height*2);
            Udzial2 = new Bitmap(width*2, height*2);
            Color czarny = Color.FromArgb(0, 0, 0);
            Color bialy = Color.FromArgb(255, 255, 255);
            for(int i=0; i<width;i++)
            {
                for(int j=0;j<height;j++)
                {
                    //losowanie opcji kodowanie spomiędzy dwóch
                    numer = rnd.Next(1000);
                    if (numer < 500)
                    {
                        //nadanie różnych wartości pikseli jeśli piksel oryginału jest czarny
                        if (Obraz.GetPixel(i, j).R == 0)
                        {
                            Udzial1.SetPixel(i * 2, j * 2, czarny);
                            Udzial1.SetPixel(i * 2, j * 2 + 1, bialy);
                            Udzial1.SetPixel(i * 2 + 1, j * 2, bialy);
                            Udzial1.SetPixel(i * 2 + 1, j * 2 + 1, czarny);

                            Udzial2.SetPixel(i * 2, j * 2, bialy);
                            Udzial2.SetPixel(i * 2, j * 2 + 1, czarny);
                            Udzial2.SetPixel(i * 2 + 1, j * 2, czarny);
                            Udzial2.SetPixel(i * 2 + 1, j * 2 + 1, bialy);
                        }
                        //nadanie tych samych wartości jeśli jest biały
                        else
                        {
                            Udzial1.SetPixel(i * 2, j * 2, czarny);
                            Udzial1.SetPixel(i * 2, j * 2 + 1, bialy);
                            Udzial1.SetPixel(i * 2 + 1, j * 2, bialy);
                            Udzial1.SetPixel(i * 2 + 1, j * 2 + 1, czarny);

                            Udzial2.SetPixel(i * 2, j * 2, czarny);
                            Udzial2.SetPixel(i * 2, j * 2 + 1, bialy);
                            Udzial2.SetPixel(i * 2 + 1, j * 2, bialy);
                            Udzial2.SetPixel(i * 2 + 1, j * 2 + 1, czarny);
                        }
                    }
                    else
                    {
                        if (Obraz.GetPixel(i, j).R == 0)
                        {
                            Udzial1.SetPixel(i * 2, j * 2, bialy);
                            Udzial1.SetPixel(i * 2, j * 2 + 1, czarny);
                            Udzial1.SetPixel(i * 2 + 1, j * 2, czarny);
                            Udzial1.SetPixel(i * 2 + 1, j * 2 + 1, bialy);

                            Udzial2.SetPixel(i * 2, j * 2, czarny);
                            Udzial2.SetPixel(i * 2, j * 2 + 1, bialy);
                            Udzial2.SetPixel(i * 2 + 1, j * 2, bialy);
                            Udzial2.SetPixel(i * 2 + 1, j * 2 + 1, czarny);
                        }
                        else
                        {
                            Udzial1.SetPixel(i * 2, j * 2, bialy);
                            Udzial1.SetPixel(i * 2, j * 2 + 1, czarny);
                            Udzial1.SetPixel(i * 2 + 1, j * 2, czarny);
                            Udzial1.SetPixel(i * 2 + 1, j * 2 + 1, bialy);

                            Udzial2.SetPixel(i * 2, j * 2, bialy);
                            Udzial2.SetPixel(i * 2, j * 2 + 1, czarny);
                            Udzial2.SetPixel(i * 2 + 1, j * 2, czarny);
                            Udzial2.SetPixel(i * 2 + 1, j * 2 + 1, bialy);
                        }
                    }
                }
            }
            ImageUdzial1.Source = BitmapToImageSource(Udzial1);
            ImageUdzial2.Source = BitmapToImageSource(Udzial2);
            BoxLogi.Text += "Kodowanie obrazu pomiędzy udziały zakończone.\n";
            BDekoduj.IsEnabled = true;
            BZapisUdzial1.IsEnabled = true;
            BZapisUdzial2.IsEnabled = true;
            BKoduj.IsEnabled = true;
        }

        private void BDekoduj_Click(object sender, RoutedEventArgs e)
        {
            BDekoduj.IsEnabled = false;
            if (Udzial1.Width != Udzial2.Width && Udzial1.Height != Udzial2.Height)
            {
                MessageBox.Show("Udziały nie mają tych samych wymiarów!");
            }
            else
            {
                int width = Udzial1.Width / 2;
                int height = Udzial1.Height / 2;
                Zdeszyfrowany = new Bitmap(width, height);
                Color czarny = Color.FromArgb(0, 0, 0);
                Color bialy = Color.FromArgb(255, 255, 255);
                for (int i=0;i<width;i++)
                {
                    for(int j=0;j<height;j++)
                    {
                        //ustawienie piksela jako biały jeśli w obu udziałach piksele o tych samych współrzędnych mają te same wartości
                        if(Udzial1.GetPixel(i * 2 ,j * 2).Equals(Udzial2.GetPixel(i * 2,j * 2)) && 
                            Udzial1.GetPixel(i * 2, j * 2+1).Equals(Udzial2.GetPixel(i * 2, j * 2+1)) && 
                            Udzial1.GetPixel(i * 2+1, j * 2).Equals(Udzial2.GetPixel(i * 2+1, j * 2)) && 
                            Udzial1.GetPixel(i * 2+1, j * 2+1).Equals(Udzial2.GetPixel(i * 2+1, j * 2+1)))
                        {
                            Zdeszyfrowany.SetPixel(i, j, bialy);
                        }
                        else
                        {
                            Zdeszyfrowany.SetPixel(i, j, czarny);
                        }
                    }
                }
            }
            ImageZdeszyfrowany.Source = BitmapToImageSource(Zdeszyfrowany);
            BoxLogi.Text += "Dekodowanie obrazu z udziałów zakończone.\n";
            BDekoduj.IsEnabled = true;
        }

        private void BZapisUdzial1_Click(object sender, RoutedEventArgs e)
        {
            Udzial1.Save(BoxZapisUdzial1.Text);
            BoxLogi.Text += "Zapisano pierwszy udział do pliku "+ BoxZapisUdzial1.Text + "\n";
        }

        private void BZapisUdzial2_Click(object sender, RoutedEventArgs e)
        {
            Udzial2.Save(BoxZapisUdzial2.Text);
            BoxLogi.Text += "Zapisano drugi udział do pliku " + BoxZapisUdzial2.Text + "\n";
        }

        private void BKoncowy_Click(object sender, RoutedEventArgs e)
        {
            Zdeszyfrowany.Save(BoxKoncowy.Text);
            BoxLogi.Text += "Zapisano zdeszyfrowany obraz do pliku " + BoxKoncowy.Text + "\n";
        }

        private void BHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Program koduje obraz pomiędzy dwa udziały, których wymiary są dwukrotnie większe od obrazu oryginalnego.\n" +
                "Każdy piksel obrazu oryginalnego jest kodowany na czterech pikselach każdego z udziałów.\n" +
                "Jeśli piksel oryginału jest czarny to udziały mają różne wartości pikseli o tych samych współrzędnych.\n" +
                "W przypadku kiedy piksel oryginału jest biały to udziały mają te same wartości pikseli o tych samych współrzędnych");
        }
    }
}
