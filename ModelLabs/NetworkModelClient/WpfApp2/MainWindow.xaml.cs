using FTN.Common;
using NetworkModelClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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


namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<long> gidovi = new List<long>();
        private List<string> codovi = new List<string>();
        private List<ModelCode> tipovi = new List<ModelCode>();
        public List<long> GIDOVI
        {
            get
            {
                return gidovi;
            }
            set
            {
                gidovi = value;
               
            }

        }

        public List<string> ModelKodovi
        {
            get
            {
                return codovi;
            }
            set
            {
                codovi = value;

            }

        }

        public List<ModelCode> TIPOVI
        {
            get
            {
                return tipovi;
            }
            set
            {
                tipovi = value;

            }

        }

        ClientGDA client = new ClientGDA();
        public MainWindow()
        {
            InitializeComponent();
            GIDOVI=client.GetAllGids();
            ModelKodovi=NadjiModelKodove(GIDOVI);
            DataContext = this;
        }

        private List<string> NadjiModelKodove(List<long> listaGids)
        {
            List<string> codovi = new List<string>();

            foreach (var item in listaGids)
            {
                ModelCode code = client.GetModelCodeFromId(item);
                codovi.Add(code.ToString());
            }

            return codovi;
        }

        private void listaGids_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listaGids.SelectedItem != null)
            {
                
                var selectedValue = (long)listaGids.SelectedItem;

                

                List<ModelCode> listaatributa = client.GetAllPropertyIdsForEntityId(selectedValue);
                listaAttr.ItemsSource = listaatributa;
                listaAttr.SelectedItems.Clear();
                text_box.Text = "";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var selectedValue = (long)listaGids.SelectedItem;

            List<ModelCode> listaAtributa = new List<ModelCode>();
            foreach (var v in listaAttr.SelectedItems)
            {
                listaAtributa.Add((ModelCode)v);
            }

            
            text_box.Text=client.GetValues(selectedValue, listaAtributa);

        }

       

        private void listaModelCodovi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listaModelCodovi.SelectedItem != null)
            {

                string selectedString = (string)listaModelCodovi.SelectedItem;
                ModelCode selectedEnumValue = (ModelCode)Enum.Parse(typeof(ModelCode), selectedString);


                List<ModelCode> listaatributa = client.NadjiAtributeMC(selectedEnumValue);
                listaProp.ItemsSource = listaatributa;
                listaProp.SelectedItems.Clear();
                text_extended.Text = "";
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string selectedString = (string)listaModelCodovi.SelectedItem;
            ModelCode selectedEnumValue = (ModelCode)Enum.Parse(typeof(ModelCode), selectedString);
            DMSType dms = new DMSType();

            List<ModelCode> listaAtributa = new List<ModelCode>();
            foreach (var v in listaProp.SelectedItems)
            {
                listaAtributa.Add((ModelCode)v);
            }


            dms=client.ConvertModelCodeToDMSType(selectedEnumValue);


            text_extended.Text = client.GetExtentValues(dms, listaAtributa);


        }

        private void listaGidoviRelated_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listaGidoviRelated.SelectedItem != null)
            {

                var selectedValue = (long)listaGidoviRelated.SelectedItem;



                List<ModelCode> listaatributa = client.NadjiAtrIDs(selectedValue);
                comboPropRelated.ItemsSource = listaatributa;
                tipovi = client.GetDMSTypes();
                comboTypes.ItemsSource = tipovi;

               
              
            }



        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            var selectedValue = (long)listaGidoviRelated.SelectedItem;

            List<ModelCode> listaAtributa = new List<ModelCode>();
            foreach (var v in listaPropRelated.SelectedItems)
            {
                listaAtributa.Add((ModelCode)v);
            }

            Association association = new Association();
            association.PropertyId = (ModelCode)comboPropRelated.SelectedItem;
            association.Type = (ModelCode)comboTypes.SelectedItem;


            text_realated.Text = client.GetRelatedValues(selectedValue,association,listaAtributa);


        }

        private void comboTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //string selectedString = (string)comboTypes.SelectedItem;
            //ModelCode selectedEnumValue = (ModelCode)Enum.Parse(typeof(ModelCode), selectedString);

            listaPropRelated.ItemsSource = client.NadjiAtributeMC((ModelCode)comboTypes.SelectedItem);
        }
    }
}
