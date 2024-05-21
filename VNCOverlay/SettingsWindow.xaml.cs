﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VNCOverlay
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private SettingsViewModel viewModel;    
        private readonly PortsHelper _portsHelper;
        private readonly OverlayHelper _overlayHelper;

        public SettingsWindow(PortsHelper portsHelper, OverlayHelper overlayHelper)
        {
            InitializeComponent();
            viewModel = new SettingsViewModel();
            this.DataContext = viewModel;
            _portsHelper = portsHelper;
            _overlayHelper = overlayHelper;
        }

        private void PortNumberTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Enable or disable the Save button based on validation state
            if (Validation.GetHasError(PortNumberTxt))
            {
                SaveBtn.IsEnabled = false;
            }
            else
            {
                SaveBtn.IsEnabled = true;
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            // Save the port number to the configuration file
            _portsHelper.UpdatePorts(viewModel.PortNumber);
            this.Close();
        }

        private void BasicBtn_Click(object sender, RoutedEventArgs e)
        {
            _overlayHelper.UpdateOverlayType("basic");
        }

        private void CenterBtn_Click(object sender, RoutedEventArgs e)
        {
            _overlayHelper.UpdateOverlayType("center");
        }

        private void LoudBtn_Click(object sender, RoutedEventArgs e)
        {
            _overlayHelper.UpdateOverlayType("loud");
        }
    }
}
