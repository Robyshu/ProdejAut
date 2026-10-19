using Microsoft.Win32;
using ProdejAutUkol.Models;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Xml.Linq;

namespace ProdejAutUkol
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnNacist_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "XML soubory (*.xml)|*.xml|Všechny soubory (*.*)|*.*",
                Title = "Vyberte XML soubor s prodejem aut"
            };

            string localXml = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "auta.xml");
            if (File.Exists(localXml))
            {
                openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                openFileDialog.FileName = "auta.xml";
            }

            if (openFileDialog.ShowDialog() == true)
                ZpracujXmlSoubor(openFileDialog.FileName);

        }
        private void ZpracujXmlSoubor(string cestaKSouboru)
        {
            try
            {
                XDocument doc = XDocument.Load(cestaKSouboru);
                List<AutoModel> seznamAut = new List<AutoModel>();

                foreach (XElement element in doc.Descendants("Auto"))
                {
                    string model = element.Element("Model")?.Value ?? string.Empty;
                    string datumStr = element.Element("DatumProdeje")?.Value ?? string.Empty;
                    string cenaStr = element.Element("Cena")?.Value ?? "0";
                    string dphStr = element.Element("DPH")?.Value ?? "0";

                    DateTime datum;
                    if (!DateTime.TryParseExact(datumStr, new[] { "yyyy-MM-dd" },
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out datum))
                    {
                        datum = DateTime.Parse(datumStr);
                    }

                    double cena = double.Parse(cenaStr, CultureInfo.InvariantCulture);
                    double dph = double.Parse(dphStr, CultureInfo.InvariantCulture);

                    seznamAut.Add(new AutoModel
                    {
                        Model = model,
                        DatumProdeje = datum,
                        Cena = cena,
                        DPH = dph
                    });
                }

                GridVsechnaAuta.ItemsSource = seznamAut;
                TxtCestaKSouboru.Text = Path.GetFileName(cestaKSouboru);

                var vikendovaAuta = seznamAut.Where(a =>
                    a.DatumProdeje.DayOfWeek == DayOfWeek.Saturday ||
                    a.DatumProdeje.DayOfWeek == DayOfWeek.Sunday);

                var vysledky = vikendovaAuta
                    .GroupBy(a => a.Model)
                    .Select(skupina => new SouhrnModel
                    {
                        NazevModelu = skupina.Key,
                        CenaBezDPH = skupina.Sum(a => a.Cena),
                        CenaSDPH = skupina.Sum(a => a.Cena * (1.0 + (a.DPH / 100.0)))
                    })
                    .OrderBy(r => r.NazevModelu)
                    .ToList();

                GridVysledky.ItemsSource = vysledky;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při načítání souboru: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}