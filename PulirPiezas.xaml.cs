using AplicacionAuto.Clases;
using HTTPupt;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using Microsoft.Maui.Storage;

namespace AplicacionAuto
{
    public partial class PulirPiezas : ContentPage
    {
        private readonly PeticionHTTP _peticion = new PeticionHTTP(Urls.UrlServer());
        private static Boolean _alerta = false;
        private readonly List<string> _partesSeleccionadasPE;

        private readonly DatosAuto _datosAuto;
        private readonly String _prioridad;
        private readonly String _servicio;
        private String _tipoGolpe;
        private String _opcionTodo;
        private List<Imagen> _imagenesTodoElVehiculo;

        public PulirPiezas(DatosAuto datosAuto, String prioridad, String servicio)
        {
            InitializeComponent();

            // Asignar valores a propiedades
            _datosAuto = datosAuto;
            _prioridad = prioridad;
            _servicio = servicio;
            _partesSeleccionadasPE = new List<string>();

            CargarPiezas();
        }

        private void CargarPiezas()
        {
            // Verificar conexión a internet
            bool tieneConexionInternet = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

            if (!tieneConexionInternet)
            {
                CerrarApp("Sin conexión", "Conéctate a una red para obtener los datos");
                return;
            }

            try
            {
                // Realizar petición al servidor
                _peticion.PedirComunicacion(
                    "Pieza/Obtener",
                    MetodoHTTP.GET,
                    TipoContenido.JSON,
                    Preferences.Default.Get("token", "")
                );

                string jsonRecibir = _peticion.ObtenerJson();
                List<PiezaDTO> piezas = JsonConvertidor.Json_ListaObjeto<PiezaDTO>(jsonRecibir);

                // Filtrar piezas para "Pulido y encerado"
                var piezasFiltradas = piezas.FindAll(x =>
                    x.TipoPiezaNombre.ToLower() == "Pulido y encerado".ToLower());

                piezas = piezasFiltradas;

                // Remover "Todo el vehículo" de las opciones
                piezas.RemoveAll(p => p.Nombre == "Todo el vehículo");

                // Asignar como contexto de datos
                BindingContext = new { piezas };

                // Generar controles dinámicamente para cada pieza
                piezasContainer.Children.Clear();
                foreach (var pieza in piezas)
                {
                    var horizontalStackLayout = new HorizontalStackLayout
                    {
                        Spacing = 10,
                        Margin = new Thickness(0, 5)
                    };

                    var label = new Label
                    {
                        Text = pieza.Nombre,
                        VerticalOptions = LayoutOptions.Center,
                        WidthRequest = 200
                    };

                    var checkBox = new CheckBox();
                    checkBox.CheckedChanged += CheckBox_CheckedChanged;

                    horizontalStackLayout.Children.Add(label);
                    horizontalStackLayout.Children.Add(checkBox);

                    piezasContainer.Children.Add(horizontalStackLayout);
                }
            }
            catch (Exception)
            {
                CerrarApp(
                    "Error en la petición al servidor",
                    "\n\nAsegúrate de tener una conexión estable a internet."
                );
            }
        }

        public async void CerrarApp(String titulo, String mensaje)
        {
            if (_alerta == false)
            {
                await DisplayAlert(titulo, mensaje, "Aceptar");
                _alerta = true;
            }
        }

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is not CheckBox checkBox) return;

            string parteCarro = GetCheckBoxText(checkBox);
            if (string.IsNullOrEmpty(parteCarro)) return;

            if (e.Value)
            {
                if (!_partesSeleccionadasPE.Contains(parteCarro))
                {
                    _partesSeleccionadasPE.Add(parteCarro);
                }
            }
            else
            {
                if (_partesSeleccionadasPE.Contains(parteCarro))
                {
                    _partesSeleccionadasPE.Remove(parteCarro);
                }
            }
        }

        private string GetCheckBoxText(CheckBox checkBox)
        {
            // Obtener el HorizontalStackLayout padre
            if (checkBox.Parent is not HorizontalStackLayout horizontalStackLayout)
                return string.Empty;

            // El Label es el primer elemento del HorizontalStackLayout
            if (horizontalStackLayout.Children[0] is not Label label)
                return string.Empty;

            return label.Text;
        }

        private void ButtonGuardar_Clicked(object sender, EventArgs e)
        {
            // Actualizar la fuente de datos del ListView
            lstPartesSeleccionadasListView.ItemsSource = null;
            lstPartesSeleccionadasListView.ItemsSource = _partesSeleccionadasPE;

            // Mostrar botón siguiente si hay elementos seleccionados
            btnSiguiente.IsVisible = _partesSeleccionadasPE.Count > 0;
        }

        private async void ButtonSiguiente_Clicked(object sender, EventArgs e)
        {
            btnSiguiente.IsEnabled = false;

            await Navigation.PushAsync(new PaquetePulidoEncerado(
                _datosAuto,
                _prioridad,
                _servicio,
                _tipoGolpe,
                _partesSeleccionadasPE,
                _opcionTodo,
                _imagenesTodoElVehiculo
            ));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            btnSiguiente.IsEnabled = true;
        }
    }
}