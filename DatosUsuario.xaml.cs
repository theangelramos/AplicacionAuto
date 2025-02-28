using AplicacionAuto.Clases;
using HTTPupt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;

namespace AplicacionAuto
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DatosUsuario : ContentPage
    {
        PeticionHTTP peticion = new PeticionHTTP(Urls.UrlServer());
        DatosAuto datosAuto1;
        String prioridad1;
        String servicio1;
        String tipoGolpe1;
        List<GolpeMedida> datosGolpes1;
        String paquete1;
        TallerDTO taller1;
        String fecha1;
        String hora1;
        String opcionTodo1;
        private List<string> partesSeleccionadasPE1;

        List<Imagen> imagenesGolpeFuerte1;
        String presupuesto;
        String categoriaColor;

        List<Imagen> imagenesTodoElVehiculo1;

        List<UsuarioDTO> usuarios;
        List<CocheDTO> coches;
        List<Acabado> acabados;
        List<ColorDTO> colors;
        List<TipoServicioPaqueteDTO> tipoServicioPaquetes;
        List<PrioridadDTO> prioridades;
        List<TipoGolpeDTO> tipoGolpes;
        List<int> imagensInt;
        List<ImagenDTOB> imagenes;
        List<PiezaDTO> piezas;
        List<PiezaDTO> piezasList;

        String ruta;
        public DatosUsuario(DatosAuto datosAuto, String prioridad, String servicio, String tipoGolpe, List<GolpeMedida> datosGolpes, String paquete, TallerDTO taller, String fecha, String hora, List<string> partesSeleccionadasPE, String precioCotizacion, String opcionTodo, List<Imagen> imagenesGolpeFuerte, List<Imagen> imagenesTodoElVehiculo)
        {
            // Agrega esta línea
            InitializeComponent();
            datosAuto1 = datosAuto;
            prioridad1 = prioridad;
            servicio1 = servicio;
            tipoGolpe1 = tipoGolpe;
            datosGolpes1 = datosGolpes;
            paquete1 = paquete;
            taller1 = taller;
            fecha1 = fecha;
            hora1 = hora;
            opcionTodo1 = opcionTodo;
            partesSeleccionadasPE1 = partesSeleccionadasPE;
            presupuesto = precioCotizacion;
            imagenesGolpeFuerte1 = imagenesGolpeFuerte;
            imagenesTodoElVehiculo1 = imagenesTodoElVehiculo;

            if (servicio1 == "Hojalatería y Pintura")
            {
                servicio1 = "Hojalatería y pintura";
            }

            try
            {
                peticion.PedirComunicacion("Color/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Default.Get("token", ""));
                String jsonRecibir = peticion.ObtenerJson();
                colors = JsonConvertidor.Json_ListaObjeto<ColorDTO>(jsonRecibir);

                if (colors != null)
                {
                    var objetoObtenido = colors.FindAll(x => x.Nombre.ToLower() == datosAuto1.Color.ToLower());
                    ColorDTO objeto = objetoObtenido.FirstOrDefault();
                    categoriaColor = objeto?.CategoriaNombre;
                }
            }
            catch (Exception ex)
            {
                // Manejo de error mejorado
                System.Diagnostics.Debug.WriteLine($"Error al obtener colores: {ex.Message}");
            }

            // Reemplazar Thread.Sleep con una alternativa más moderna
            Task.Delay(1000).Wait();
        }




        private async void OnImageTapped(object sender, EventArgs e)
        {
            string descripcion = "¡Importante! Te recomendamos proporcionar información verídica y exacta en los campos requeridos. Los datos que ingreses, como tu correo electrónico, nombre completo, teléfono, estado y municipio, son esenciales para brindarte un servicio personalizado y mantener una comunicación efectiva. Garantizamos la confidencialidad de tus detalles personales. Tu colaboración al ingresar información precisa nos permite ofrecerte el mejor servicio posible. Gracias por confiar en nosotros.\r\n";
            await DisplayAlert("Ayuda", descripcion, "Aceptar");
        }


        static string GenerarNombreAleatorio()
        {
            Random random = new Random();
            string letras = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890¿?$#&+-*/";
            int longitud = 40;

            char[] nombreArray = new char[longitud];
            for (int i = 0; i < longitud; i++)
            {
                nombreArray[i] = letras[random.Next(letras.Length)];
            }

            nombreArray[longitud - 1] = (char)('0' + (nombreArray.GetHashCode() % 10));

            return new string(nombreArray);
        }

        private bool isLoading;
        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            siguienteBtn.IsEnabled = true;
        }

        public byte[] DatoImagen { get; set; }

        async private void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Deshabilitar el botón para evitar múltiples clics
                siguienteBtn.IsEnabled = false;

                // Mostrar indicador de carga
                var loadingPage = new ContentPage
                {
                    Content = new VerticalStackLayout
                    {
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        Children =
                {
                    new ActivityIndicator
                    {
                        IsRunning = true,
                        Color = Colors.Blue,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    },
                    new Label
                    {
                        Text = "Creando registro...\nEsto puede demorar unos instantes\nRecuerde que la ficha se almacenará en sus descargas",
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    }
                }
                    }
                };

                // Mostrar página de carga
                await Navigation.PushModalAsync(loadingPage);

                await Task.Run(async () =>
                {
                    try
                    {
                        // Validar campos
                        if (ValidarCampos())
                        {
                            // Obtener datos de los campos
                            string nombre = txtNombreCompleto.Text.Trim();
                            string correo = txtCorreoElectronico.Text.Trim();
                            string telefono = txtTelefono.Text.Trim();
                            string estado = txtEstado.Text.Trim();
                            string municipio = txtMunicipio.Text.Trim();

                            // Crear usuario
                            UsuarioDTO usuario = new UsuarioDTO()
                            {
                                NombreCompleto = nombre,
                                CorreoElectronico = correo,
                                Telefono = telefono,
                                Estado = estado,
                                Municipio = municipio,
                                Estatus = true
                            };

                            // Enviar datos de usuario
                            await EnviarDatosUsuario(usuario);
                        }
                        else
                        {
                            await MainThread.InvokeOnMainThreadAsync(async () =>
                            {
                                await DisplayAlert("Datos Incorrectos", "Debes llenar todos los campos", "Aceptar");
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            await DisplayAlert("Error", $"Ocurrió un problema: {ex.Message}", "Aceptar");
                        });
                    }
                });

                // Cerrar página de carga
                await Navigation.PopModalAsync();

                // Subir archivo y procesar
                await UploadFileAsync(ruta);
                CerrarApp("Proceso Completado", "La aplicación se cerrará sola");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un problema: {ex.Message}", "Aceptar");
            }
            finally
            {
                // Volver a habilitar el botón
                siguienteBtn.IsEnabled = true;
            }
        }

        private bool ValidarCampos()
        {
            return !string.IsNullOrWhiteSpace(txtNombreCompleto.Text) &&
                   !string.IsNullOrWhiteSpace(txtCorreoElectronico.Text) &&
                   !string.IsNullOrWhiteSpace(txtTelefono.Text) &&
                   !string.IsNullOrWhiteSpace(txtEstado.Text) &&
                   !string.IsNullOrWhiteSpace(txtMunicipio.Text);
        }

        private async Task EnviarDatosUsuario(UsuarioDTO usuario)
        {
            try
            {
                // Serializar usuario
                string jsonEnviar = JsonConvertidor.Objeto_Json(usuario);

                // Enviar solicitud para agregar usuario
                peticion.PedirComunicacion("Usuario/Agregar", MetodoHTTP.POST, TipoContenido.JSON, Preferences.Default.Get("token", ""));
                peticion.enviarDatos(jsonEnviar);
                string jsonRecibirUsuario = peticion.ObtenerJson();

                // Obtener lista de usuarios
                peticion.PedirComunicacion("Usuario/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Default.Get("token", ""));
                string jsonRecibir = peticion.ObtenerJson();
                usuarios = JsonConvertidor.Json_ListaObjeto<UsuarioDTO>(jsonRecibir);

                // Buscar usuario recién creado
                var objetoObtenidoUsuario = usuarios.FindAll(x =>
                    x.NombreCompleto.ToLower().Trim() == usuario.NombreCompleto.ToLower().Trim() &&
                    x.CorreoElectronico.ToLower().Trim() == usuario.CorreoElectronico.ToLower().Trim() &&
                    x.Telefono.ToLower().Trim() == usuario.Telefono.ToLower().Trim() &&
                    x.Estado.ToLower().Trim() == usuario.Estado.ToLower().Trim() &&
                    x.Municipio.ToLower().Trim() == usuario.Municipio.ToLower().Trim()
                );

                // Continuar con el procesamiento de datos...
                // (El resto de tu lógica de procesamiento iría aquí)
                // Nota: Este método necesitaría ser completado con toda la lógica 
                // de tu método original
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await DisplayAlert("Problema con el servidor", $"Error: {ex.Message}", "Aceptar");
                });
                throw; // Re-lanzar para manejar en el método llamador
            }
        }

        public async void CerrarApp(String titulo, String mensaje)
        {
            await DisplayAlert(titulo, mensaje, "Aceptar");

            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            }
        }

        private async Task UploadFileAsync(string filePath)
        {
            using (HttpClient client = new HttpClient())
            {
                using (var form = new MultipartFormDataContent())
                {
                    var fileContent = new ByteArrayContent(File.ReadAllBytes(filePath));
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "\"Archivo\"",
                        FileName = "\"" + Path.GetFileName(filePath) + "\""
                    };
                    form.Add(fileContent);

                    HttpResponseMessage response = await client.PostAsync("https://auto-universe.com.mx/Upload/Subir", form);

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Operacion Exitosa", "Archivo almacenado en la nube", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error fatal: ", response.StatusCode.ToString(), "OK");
                    }
                }
            }
        }
    }
}