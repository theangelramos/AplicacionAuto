using AplicacionAuto.Clases;
using HTTPupt;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace AplicacionAuto
{
    public partial class PaqueteHojalateriaPintura : ContentPage
    {
        PeticionHTTP peticion = new PeticionHTTP(Urls.UrlServer());

        DatosAuto datosAuto1;
        String prioridad1;
        String servicio1;
        String tipoGolpe1;
        List<GolpeMedida> datosGolpes1;
        String paquete;
        String opcionTodo1;
        List<Imagen> imagenesGolpeFuerte = new List<Imagen>();
        private List<string> partesSeleccionadasPE;
        List<Imagen> imagenesTodoElVehiculo;

        public PaqueteHojalateriaPintura(DatosAuto datosAuto, String prioridad, String servicio, String tipoGolpe, ObservableCollection<GolpeMedida> datosGolpes)
        {
            InitializeComponent();

            peticion.PedirComunicacion("tiposerviciopaquete/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Get("token", ""));
            String json = peticion.ObtenerJson();

            var tipoServicioPaquete = JsonConvertidor.Json_ListaObjeto<TipoServicioPaqueteDTO>(json);

            var descripciones = tipoServicioPaquete.Where(c => c.TipoServicioID == 1).ToList();

            lblEconomico.Text = descripciones[0].Descripcion;
            lblIntermedio.Text = descripciones[1].Descripcion;
            lblPro.Text = descripciones[2].Descripcion;
            lblPlus.Text = descripciones[3].Descripcion;
            datosAuto1 = datosAuto;
            prioridad1 = prioridad;
            servicio1 = servicio;
            tipoGolpe1 = tipoGolpe;
            datosGolpes1 = new List<GolpeMedida>(datosGolpes);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            economicoBtn.IsEnabled = true;
            intermedioBtn.IsEnabled = true;
            proBtn.IsEnabled = true;
            plusBtn.IsEnabled = true;
        }

        private async void ButtonEconomico(object sender, EventArgs e)
        {
            economicoBtn.IsEnabled = false;
            paquete = "Económico";
            await Navigation.PushAsync(new Mapa(datosAuto1, prioridad1, servicio1, opcionTodo1, tipoGolpe1, datosGolpes1, paquete, imagenesGolpeFuerte, partesSeleccionadasPE, imagenesTodoElVehiculo));
        }

        private async void ButtonIntermedio(object sender, EventArgs e)
        {
            intermedioBtn.IsEnabled = false;
            paquete = "Intermedio";
            await Navigation.PushAsync(new Mapa(datosAuto1, prioridad1, servicio1, opcionTodo1, tipoGolpe1, datosGolpes1, paquete, imagenesGolpeFuerte, partesSeleccionadasPE, imagenesTodoElVehiculo));
        }

        private async void ButtonPro(object sender, EventArgs e)
        {
            proBtn.IsEnabled = false;
            paquete = "Profesional";
            await Navigation.PushAsync(new Mapa(datosAuto1, prioridad1, servicio1, opcionTodo1, tipoGolpe1, datosGolpes1, paquete, imagenesGolpeFuerte, partesSeleccionadasPE, imagenesTodoElVehiculo));
        }

        private async void ButtonPlus(object sender, EventArgs e)
        {
            plusBtn.IsEnabled = false;
            paquete = "Plus";
            await Navigation.PushAsync(new Mapa(datosAuto1, prioridad1, servicio1, opcionTodo1, tipoGolpe1, datosGolpes1, paquete, imagenesGolpeFuerte, partesSeleccionadasPE, imagenesTodoElVehiculo));
        }

        private async void OnImageTapped(object sender, EventArgs e)
        {
            string descripcion = "Selecciona un paquete de hojalatería y pintura de acuerdo a tus necesidades. Cada paquete tiene diferentes características y precios. " +
                "¡Estamos aquí para brindarte el mejor servicio!";
            await DisplayAlert("Ayuda", descripcion, "Aceptar");
        }
    }
}