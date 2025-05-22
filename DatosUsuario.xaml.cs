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
using System.Text.RegularExpressions;

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

            // Configurar validación inicial del botón
            ValidarFormulario();

            // Asignar evento TextChanged a cada Entry
            txtCorreoElectronico.TextChanged += OnTextChanged;
            txtNombreCompleto.TextChanged += OnTextChanged;
            txtTelefono.TextChanged += OnTextChanged;
            txtEstado.TextChanged += OnTextChanged;
            txtMunicipio.TextChanged += OnTextChanged;
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
            ValidarFormulario();
        }

        public byte[] DatoImagen { get; set; }

        // Método para validar el correo electrónico
        private bool EsCorreoValido(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
                return false;

            try
            {
                // Expresión regular para validar formato de correo
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(correo);
            }
            catch
            {
                return false;
            }
        }

        // Método para validar número de teléfono
        private bool EsTelefonoValido(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono))
                return false;

            // Verificar que tenga 10 dígitos y solo sean números
            return telefono.Length == 10 && telefono.All(char.IsDigit);
        }

        // Evento cuando cambia el texto de cualquier entrada
        public void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ValidarFormulario();
        }

        // Método para validar todos los campos del formulario
        private void ValidarFormulario()
        {
            // Validar correo
            bool correoValido = EsCorreoValido(txtCorreoElectronico.Text);
            lblErrorCorreo.IsVisible = !string.IsNullOrEmpty(txtCorreoElectronico.Text) && !correoValido;

            // Validar nombre
            bool nombreValido = !string.IsNullOrWhiteSpace(txtNombreCompleto.Text);
            lblErrorNombre.IsVisible = !string.IsNullOrEmpty(txtNombreCompleto.Text) && !nombreValido;

            // Validar teléfono
            bool telefonoValido = EsTelefonoValido(txtTelefono.Text);
            lblErrorTelefono.IsVisible = !string.IsNullOrEmpty(txtTelefono.Text) && !telefonoValido;

            // Validar estado
            bool estadoValido = !string.IsNullOrWhiteSpace(txtEstado.Text);
            lblErrorEstado.IsVisible = !string.IsNullOrEmpty(txtEstado.Text) && !estadoValido;

            // Validar municipio
            bool municipioValido = !string.IsNullOrWhiteSpace(txtMunicipio.Text);
            lblErrorMunicipio.IsVisible = !string.IsNullOrEmpty(txtMunicipio.Text) && !municipioValido;

            // Habilitar o deshabilitar botón según validaciones
            siguienteBtn.IsEnabled = correoValido && nombreValido && telefonoValido && estadoValido && municipioValido;
        }

        async private void Button_Clicked(object sender, EventArgs e)
        {
            // Validar nuevamente antes de procesar
            if (!ValidarCampos())
            {
                await DisplayAlert("Campos incompletos", "Todos los campos son obligatorios y deben estar correctamente completados.", "Aceptar");
                return;
            }

            siguienteBtn.IsEnabled = false;

            try
            {
                // Usar el indicador de actividad de MAUI en lugar de UserDialogs
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

                await Navigation.PushModalAsync(loadingPage);

                // Crear una instancia de la clase PDF
                PDF pdfGenerator = new PDF("contenido del pdf"); // Ajusta esto según tus necesidades

                try
                {
                    // Aquí llama al método correspondiente de generación de PDF según tus datos
                    if (!string.IsNullOrEmpty(tipoGolpe1) && imagenesGolpeFuerte1 != null && imagenesGolpeFuerte1.Count > 0)
                    {
                        // Genera PDF para golpe fuerte con imágenes
                        ruta = await pdfGenerator.GenerarPDFGFTV(
                            "Folio", datosAuto1.Marca, datosAuto1.Submarca, datosAuto1.Modelo,
                            datosAuto1.Tipo, datosAuto1.Version, categoriaColor, datosAuto1.Color,
                            datosAuto1.Acabado, servicio1, prioridad1, taller1,
                            fecha1, hora1, txtNombreCompleto.Text, txtCorreoElectronico.Text,
                            txtTelefono.Text, txtEstado.Text, txtMunicipio.Text,
                            tipoGolpe1, imagenesGolpeFuerte1, opcionTodo1, paquete1, presupuesto);
                    }
                    else if (datosGolpes1 != null && datosGolpes1.Count > 0)
                    {
                        // Genera PDF para servicio con datos de golpes
                        ruta = await pdfGenerator.GenerarPDF(
                            "Folio", datosAuto1.Marca, datosAuto1.Submarca, datosAuto1.Modelo,
                            datosAuto1.Tipo, datosAuto1.Version, categoriaColor, datosAuto1.Color,
                            datosAuto1.Acabado, servicio1, paquete1, prioridad1,
                            datosAuto1.Tipo, taller1, fecha1, hora1,
                            txtNombreCompleto.Text, txtCorreoElectronico.Text, txtTelefono.Text,
                            txtEstado.Text, txtMunicipio.Text, presupuesto,
                            opcionTodo1, tipoGolpe1, datosGolpes1, partesSeleccionadasPE1);
                    }
                    else
                    {
                        // Genera PDF básico sin datos específicos
                        ruta = await pdfGenerator.GenerarPDFSinDatos(
                            "Folio", taller1, fecha1, hora1,
                            txtNombreCompleto.Text, txtCorreoElectronico.Text, txtTelefono.Text,
                            txtEstado.Text, txtMunicipio.Text);
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error al generar PDF", $"Detalles: {ex.Message}", "Aceptar");
                    Console.WriteLine($"Error detallado: {ex}");
                }

                await Navigation.PopModalAsync();

                // Comprobar que ruta no sea nula antes de subir archivo
                if (!string.IsNullOrEmpty(ruta) && File.Exists(ruta))
                {
                    await UploadFileAsync(ruta);
                    CerrarApp("Proceso Completado", "La aplicación se cerrará automáticamente");
                }
                else
                {
                    await DisplayAlert("Aviso", "No se generó archivo para subir o no se pudo acceder al archivo", "Aceptar");
                    Console.WriteLine($"Ruta del archivo: {ruta}, ¿Existe? {(ruta != null ? File.Exists(ruta).ToString() : "La ruta es nula")}");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un problema: {ex.Message}", "Aceptar");
                Console.WriteLine($"Error completo: {ex}");
            }
            finally
            {
                siguienteBtn.IsEnabled = true;
            }
        }

        private bool ValidarCampos()
        {
            // Validación más completa que verifica formato
            return !string.IsNullOrWhiteSpace(txtNombreCompleto.Text) &&
                   EsCorreoValido(txtCorreoElectronico.Text) &&
                   EsTelefonoValido(txtTelefono.Text) &&
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

            // Asegurar que la aplicación se cierre después de mostrar el mensaje
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                // En Android
                System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            else
            {
                // Para otras plataformas
                Application.Current.Quit();
            }
        }

        private async Task UploadFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    await DisplayAlert("Error", "La ruta del archivo es inválida", "OK");
                    return;
                }

                if (!File.Exists(filePath))
                {
                    await DisplayAlert("Error", "El archivo no existe en la ruta especificada", "OK");
                    return;
                }

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
                            await DisplayAlert("Operación Exitosa", "Archivo almacenado en la nube", "OK");
                        }
                        else
                        {
                            await DisplayAlert("Error en el servidor", $"Código: {response.StatusCode}", "OK");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error al subir archivo", ex.Message, "OK");
            }
        }
    }
}