using FTN.Common;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TelventDMS.Services.NetworkModelService.TestClient.Tests;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<long> allTypes;
        private const string noFilterMsg = "No filter";
        private TestGda tgda;

        public MainWindow()
        {
            IntializeTestGda();
            InitializeComponent();
        }

        private void IntializeTestGda()
        {
            allTypes = null;
            tgda = new TestGda();
        }


        

        private void comboBoxIdSelect_Initialized(object sender, EventArgs e)
        {
            GetAllTypes(sender);
        }

        private void listBoxProperties_Initialized(object sender, EventArgs e)
        {
            if (comboBoxIdSelect.Items.Count != 0)
                comboBoxIdSelect.SelectedItem = comboBoxIdSelect.Items[0];
        }

        private void comboBoxIdSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ModelResourcesDesc modelResources = new ModelResourcesDesc();
            ModelCode type = modelResources.GetModelCodeFromId((long)comboBoxIdSelect.SelectedItem);

            listBoxProperties.ItemsSource = modelResources.GetAllPropertyIds(type);
            listBoxProperties.UnselectAll();
        }

        private void btnGetValues_Click(object sender, RoutedEventArgs e)
        {
           
            listBoxProperties.ItemsSource = tgda.GetAllGids();
        }
        private void GetAllTypes(object sender)
        {
            if (allTypes != null)
            {
                ((ComboBox)sender).ItemsSource = allTypes;
            }
            else
            {
                try
                {
                    allTypes = tgda.GetAllGids();
                }
                catch
                {
                    allTypes = new List<long>();
                }
                ((ComboBox)sender).ItemsSource = allTypes;
            }
        }

    }
}
