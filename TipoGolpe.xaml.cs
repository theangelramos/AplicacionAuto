using AplicacionAuto.Clases;
using Microsoft.Maui.Controls;
using System;

namespace AplicacionAuto
{
    public partial class TipoGolpe : ContentPage
    {
        DatosAuto datosAuto1;
        String prioridad1;
        String servicio1;
        String tipoGolpe;

        public TipoGolpe(DatosAuto datosAuto, String prioridad, String servicio)
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
            medioBtn.IsEnabled = true;
            leveBtn.IsEnabled = true;
            fuerteBtn.IsEnabled = true;
        }

        private async void ButtonLeve(object sender, EventArgs e)
        {
            tipoGolpe = "Leve";
            leveBtn.IsEnabled = false;
            await Navigation.PushAsync(new AjenaCarroceria(datosAuto1, prioridad1, servicio1, tipoGolpe));
        }

        private async void ButtonMedio(object sender, EventArgs e)
        {
            tipoGolpe = "Mediano";
            medioBtn.IsEnabled = false;
            await Navigation.PushAsync(new AjenaCarroceria(datosAuto1, prioridad1, servicio1, tipoGolpe));
        }

        private async void ButtonFuerte(object sender, EventArgs e)
        {
            tipoGolpe = "Fuerte";
            fuerteBtn.IsEnabled = false;
            await Navigation.PushAsync(new GolpeFuerte(datosAuto1, prioridad1, servicio1, tipoGolpe));
        }

        private async void OnImageTapped(object sender, EventArgs e)
        {
            string descripcion = "1. Golpe Leve:\r\nUn impacto suave en la superficie de un objeto, como un vehículo, que resulta en marcas menores, abolladuras superficiales y raspaduras en la pintura. Suele ser causado por colisiones de baja energía con objetos pequeños y puede abordarse con reparaciones sencillas.\r\n\r\n" +
                "2. Golpe Medio:\r\nUn impacto más notable en la carrocería u objeto, que puede resultar en abolladuras más profundas, raspaduras extensas y posibles daños a componentes internos. Suele ser el resultado de colisiones a velocidades moderadas o contacto con objetos de tamaño moderado.\r\n\r\n" +
                "3. Golpe Fuerte:\r\nUn impacto significativo y posiblemente destructivo que causa daños extensos en la carrocería, incluidas abolladuras profundas, deformaciones estructurales y daños a componentes internos. Suele ser el resultado de colisiones a alta velocidad o impactos con objetos grandes. La reparación de golpes fuertes puede requerir trabajos intensivos de hojalatería, reemplazo de partes y pintura.";
            await DisplayAlert("Ayuda", descripcion, "Aceptar");
        }
    }
}