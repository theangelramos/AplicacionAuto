using AplicacionAuto.Clases;
using HTTPupt;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace AplicacionAuto
{
    public partial class PaquetePulidoEncerado : ContentPage
    {
        PeticionHTTP peticion = new PeticionHTTP(Urls.UrlServer());
        DatosAuto datosAuto1;
        String prioridad1;
        String servicio1;
        String tipoGolpe1;
        String paquete;
        String opcionTodo1;
        List<GolpeMedida> datosGolpes1;

        List<Imagen> imagenesGolpeFuerte = new List<Imagen>();
        List<Imagen> imagenesTodoElVehiculo1;

        private List<string> partesSeleccionadasPE1;

        public PaquetePulidoEncerado(DatosAuto datosAuto, String prioridad, String servicio, String tipoGolpe, List<string> partesSeleccionadasPE, String opcionTodo, List<Imagen> imagenesTodoElVehiculo)
        {
            InitializeComponent();

            // Ocultar barra de navegación
            NavigationPage.SetHasNavigationBar(this, false);

            datosAuto1 = datosAuto;
            prioridad1 = prioridad;
            servicio1 = servicio;
            tipoGolpe1 = tipoGolpe;
            opcionTodo1 = opcionTodo;
            partesSeleccionadasPE1 = partesSeleccionadasPE;
            imagenesTodoElVehiculo1 = imagenesTodoElVehiculo;
        }

        public static async Task<PaquetePulidoEncerado> CreateAsync(DatosAuto datosAuto, String prioridad, String servicio, String tipoGolpe, List<string> partesSeleccionadasPE, String opcionTodo, List<Imagen> imagenesTodoElVehiculo)
        {
            var page = new PaquetePulidoEncerado(datosAuto, prioridad, servicio, tipoGolpe, partesSeleccionadasPE, opcionTodo, imagenesTodoElVehiculo);

            page.peticion.PedirComunicacion("tiposerviciopaquete/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Get("token", ""));
            String json = await page.peticion.ObtenerJson();

            var tipoServicioPaquete = JsonConvertidor.Json_ListaObjeto<TipoServicioPaqueteDTO>(json);

            var descripciones = tipoServicioPaquete.Where(c => c.TipoServicioID == 1).ToList();

            page.lblEconomico.Text = descripciones[0].Descripcion;
            page.lblIntermedio.Text = descripciones[1].Descripcion;
            page.lblPro.Text = descripciones[2].Descripcion;
            page.lblPlus.Text = descripciones[3].Descripcion;

            return page;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            economicoBtn.IsEnabled = true;
            intermedioBtn.IsEnabled = true;
            proBtn.IsEnabled = true;
            plusBtn.IsEnabled = true;
        }

        private async void ButtonPulEconomico(object sender, EventArgs e)
        {
            economicoBtn.IsEnabled = false;
            paquete = "Económico";
            await Navigation.PushAsync(new Mapa(datosAuto1, prioridad1, servicio1, opcionTodo1, tipoGolpe1, datosGolpes1, paquete, imagenesGolpeFuerte, partesSeleccionadasPE1, imagenesTodoElVehiculo1));
        }

        private async void ButtonPulIntermedio(object sender, EventArgs e)
        {
            intermedioBtn.IsEnabled = false;
            paquete = "Intermedio";
            await Navigation.PushAsync(new Mapa(datosAuto1, prioridad1, servicio1, opcionTodo1, tipoGolpe1, datosGolpes1, paquete, imagenesGolpeFuerte, partesSeleccionadasPE1, imagenesTodoElVehiculo1));
        }

        private async void ButtonPulPro(object sender, EventArgs e)
        {
            proBtn.IsEnabled = false;
            paquete = "Profesional";
            await Navigation.PushAsync(new Mapa(datosAuto1, prioridad1, servicio1, opcionTodo1, tipoGolpe1, datosGolpes1, paquete, imagenesGolpeFuerte, partesSeleccionadasPE1, imagenesTodoElVehiculo1));
        }

        private async void ButtonPulPlus(object sender, EventArgs e)
        {
            plusBtn.IsEnabled = false;
            paquete = "Plus";
            await Navigation.PushAsync(new Mapa(datosAuto1, prioridad1, servicio1, opcionTodo1, tipoGolpe1, datosGolpes1, paquete, imagenesGolpeFuerte, partesSeleccionadasPE1, imagenesTodoElVehiculo1));
        }

        private async void OnImageTapped(object sender, EventArgs e)
        {
            string descripcion = "Selecciona un paquete de pulido y encerado de acuerdo a tus necesidades. Cada paquete tiene diferentes características y precios. " +
                "¡Estamos aquí para brindarte el mejor servicio!";
            await DisplayAlert("Ayuda", descripcion, "Aceptar");
        }
    }
}