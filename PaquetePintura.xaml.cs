using AplicacionAuto.Clases;
using HTTPupt;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace AplicacionAuto
{
    public partial class PaquetePintura : ContentPage
    {
        // HTTP Request handler
        private readonly PeticionHTTP peticion;

        // Data fields
        private DatosAuto datosAuto;
        private string prioridad;
        private string servicio;
        private string tipoGolpe;
        private List<GolpeMedida> datosGolpes;
        private string paquete;
        private string opcionTodo;
        private List<Imagen> imagenesGolpeFuerte;
        private List<string> partesSeleccionadasPE;
        private List<Imagen> imagenesTodoElVehiculo;

        public PaquetePintura(DatosAuto datosAuto, String prioridad, String servicio,
                           ObservableCollection<GolpeMedida> datosGolpes, String opcionTodo,
                           List<Imagen> imagenesTodoElVehiculo)
        {
            InitializeComponent();

            // Ocultar barra de navegación
            NavigationPage.SetHasNavigationBar(this, false);

            // Inicializar la clase de petición HTTP
            peticion = new PeticionHTTP(Urls.UrlServer());

            // Almacenar los parámetros pasados al constructor
            this.datosAuto = datosAuto;
            this.prioridad = prioridad;
            this.servicio = servicio;
            this.opcionTodo = opcionTodo;

            if (datosGolpes != null)
            {
                this.datosGolpes = new List<GolpeMedida>(datosGolpes);
            }

            this.imagenesTodoElVehiculo = imagenesTodoElVehiculo;

            // Inicializar listas vacías
            this.imagenesGolpeFuerte = new List<Imagen>();
            this.partesSeleccionadasPE = new List<string>();

            // Cargar descripciones de paquetes
            LoadPackageData();
        }

        private void LoadPackageData()
        {
            try
            {
                // Solicitar datos de tipos de servicio
                peticion.PedirComunicacion(
                    "tiposerviciopaquete/Obtener",
                    MetodoHTTP.GET,
                    TipoContenido.JSON,
                    Preferences.Get("token", "")
                );

                String json = peticion.ObtenerJson();

                // Convertir JSON a lista de objetos
                var tipoServicioPaquete = JsonConvertidor.Json_ListaObjeto<TipoServicioPaqueteDTO>(json);

                // Filtrar descripciones para el tipo de servicio 1 (Pintura)
                var descripciones = tipoServicioPaquete.Where(c => c.TipoServicioID == 1).ToList();

                // Configurar etiquetas con las descripciones
                if (descripciones.Count >= 4)
                {
                    lblEconomico.Text = descripciones[0].Descripcion;
                    lblIntermedio.Text = descripciones[1].Descripcion;
                    lblPro.Text = descripciones[2].Descripcion;
                    lblPlus.Text = descripciones[3].Descripcion;
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores básico
                DisplayAlert("Error", "No se pudieron cargar los datos de paquetes: " + ex.Message, "Aceptar");
            }
        }

        // Resetear estados de botones cuando la página aparece
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Habilitar todos los botones
            economicoBtn.IsEnabled = true;
            intermedioBtn.IsEnabled = true;
            proBtn.IsEnabled = true;
            plusBtn.IsEnabled = true;
        }

        // Manejadores de eventos para los botones de paquetes
        private async void ButtonPinEconomico(object sender, EventArgs e)
        {
            economicoBtn.IsEnabled = false;
            paquete = "Económico";
            await NavigateToMap();
        }

        private async void ButtonPinIntermedio(object sender, EventArgs e)
        {
            intermedioBtn.IsEnabled = false;
            paquete = "Intermedio";
            await NavigateToMap();
        }

        private async void ButtonPinPro(object sender, EventArgs e)
        {
            proBtn.IsEnabled = false;
            paquete = "Profesional";
            await NavigateToMap();
        }

        private async void ButtonPinPlus(object sender, EventArgs e)
        {
            plusBtn.IsEnabled = false;
            paquete = "Plus";
            await NavigateToMap();
        }

        // Botón de Siguiente
        private async void ButtonSiguiente(object sender, EventArgs e)
        {
            // Verificar si se ha seleccionado un paquete
            if (string.IsNullOrEmpty(paquete))
            {
                await DisplayAlert("Atención", "Por favor, selecciona un paquete antes de continuar", "Aceptar");
                return;
            }

            await NavigateToMap();
        }

        // Método auxiliar para la navegación
        private async Task NavigateToMap()
        {
            await Navigation.PushAsync(new Mapa(
                datosAuto,
                prioridad,
                servicio,
                opcionTodo,
                tipoGolpe,
                datosGolpes,
                paquete,
                imagenesGolpeFuerte,
                partesSeleccionadasPE,
                imagenesTodoElVehiculo
            ));
        }

        // Manejador para el botón de ayuda
        private async void OnImageTapped(object sender, EventArgs e)
        {
            string descripcion = "Selecciona un paquete de pintura de acuerdo a tus necesidades. " +
                                "Cada paquete tiene diferentes características y precios. " +
                                "¡Estamos aquí para brindarte el mejor servicio!";

            await DisplayAlert("Ayuda", descripcion, "Aceptar");
        }
    }
}