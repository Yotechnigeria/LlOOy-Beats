using System;
using System.IO;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using BeatsAppClient;

namespace BeatsAppClient
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new Login());
        }
    }
}