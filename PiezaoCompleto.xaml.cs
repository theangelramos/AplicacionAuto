using AplicacionAuto.Clases;
using Microsoft.Maui.Controls;
using System;

namespace AplicacionAuto
{
    public partial class PiezaoCompleto : ContentPage
    {
        DatosAuto datosAuto1;
        String prioridad1;
        String servicio1;
        String opcionTodo;

        public PiezaoCompleto(DatosAuto datosAuto, String prioridad, String servicio)
        {
            InitializeComponent();
            datosAuto1 = datosAuto;
            prioridad1 = prioridad;
            servicio1 = servicio;

            // Ocultar la barra de navegación programáticamente
            Microsoft.Maui.Controls.NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            todoVehiculoBtn.IsEnabled = true;
            piezasBtn.IsEnabled = true;
        }

        private async void ButtonPieza(object sender, EventArgs e)
        {
            piezasBtn.IsEnabled = false;
            await Navigation.PushAsync(new Piezas(datosAuto1, prioridad1, servicio1));
        }

        private async void ButtonCompleto(object sender, EventArgs e)
        {
            opcionTodo = "Pintura";
            todoVehiculoBtn.IsEnabled = false;
            await Navigation.PushAsync(new TodoVehiculo(datosAuto1, prioridad1, servicio1, opcionTodo));
        }
    }
}