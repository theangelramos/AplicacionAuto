using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Maui.Storage;
using Microsoft.Maui.Devices;

using SkiaSharp;

using AplicacionAuto.Interfaces; 
using HTTPupt; 

namespace AplicacionAuto.Clases
{
    internal class PDF
    {
        private readonly IFileSystemService _fileSystemService;
        private string contenido { get; set; }
        PeticionHTTP peticion = new PeticionHTTP(Urls.UrlServer());
        string filePath;

        public PDF(string content)
        {
            this.contenido = content;
            if (Device.RuntimePlatform == Device.Android)
            {
                _fileSystemService = DependencyService.Get<IFileSystemService>();
                filePath = Path.Combine(_fileSystemService.GetExternalStorageDirectory()) + "/Download/Presupuesto_AutoUniverse_";
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                //Codigo para Almacenamiento externo de IOS
            }
        }

        private string GenerateRandomNumbers(int length)
        {
            Random random = new Random();
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                int randomNumber = random.Next(0, 10);
                stringBuilder.Append(randomNumber);
            }

            return stringBuilder.ToString();
        }

        public async Task<String> GenerarPDF(String Folio, String Marca, String Submarca, String Modelo, String Tipo, String Version, String Categoría, String Color, String Acabado, String TipoServicio, String Paquete, String Prioridad, String Tipo2, TallerDTO taller, String Fecha, String Hora, String NombreCompleto, String CorreoElectronico, String Telefono, String Estado, String Municipio, String Presupuesto, String opcionTodo, String tipoGolpe, List<GolpeMedida> datosGolpes, List<string> partesSeleccionadasPE)
        {
            var listaCadenas = contenido.Split('\n');
            int num = listaCadenas.Length;
            float pointsPerInch = 72;
            float width = 8.5f * pointsPerInch;
            float height = 11f * pointsPerInch;
            filePath += solicitudID + ".pdf";
            FileStream stream = File.OpenWrite(filePath);
            SKDocument document = SKDocument.CreatePdf(stream);
            using (var canvas = document.BeginPage(width, height))
            {
                canvas.Clear(SKColors.White);


                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "AplicacionAuto.Resources.Images.logo.png";
                using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (resourceStream != null)
                    {
                        using (var bitmap = SKBitmap.Decode(resourceStream))
                        {
                            var image = SKImage.FromBitmap(bitmap);

                            var pdfWidth = 400;


                            float imageWidth = pdfWidth / 4.00f;
                            float imageHeight = bitmap.Height * (imageWidth / bitmap.Width);

                            float imageX = (pdfWidth - imageWidth) / 10;
                            float imageY = 0;

                            canvas.DrawImage(image, new SKRect(imageX, imageY, imageX + imageWidth, imageY + imageHeight));
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Image resource not found.");
                    }

                    using (var paint = new SKPaint())
                    {
                        paint.TextSize = 14;
                        paint.Color = SKColors.Black;
                        SKColor celdaBackgroundColor = SKColors.Coral;
                        SKColor celdaBackgroundColorB = SKColors.White;
                        SKColor celdaBackgroundColorc = SKColors.Gold;
                        SKColor celdaBorderColor = SKColors.Black;
                        float x = 20;
                        float y = 20;

                        string[][] encabezadoTablaOrden = new string[][]
                        {
                        new string[] { "                                        Orden de Admisión de Presupuesto "},

                        };

                        float cellWidtho = 450;


                        foreach (var fila in encabezadoTablaOrden)
                        {
                            x = 148;

                            foreach (var valor in fila)
                            {
                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidtho, y + 30), paint);

                                paint.Color = celdaBackgroundColorc;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidtho - 1, y + 30 - 1), paint);

                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidtho + 10;
                            }
                            x = 20;

                        }

                        x = 20;
                        y += 35;

                        string[][] TablaFolio = new string[][]
                        {
                        new string[] { $"Folio: {solicitudID}"}

                        };


                        float cellWidthf = 170;



                        foreach (var fila in TablaFolio)
                        {
                            x = 428;

                            foreach (var valor in fila)
                            {
                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidthf, y + 30), paint);

                                paint.Color = celdaBackgroundColorB;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidthf - 1, y + 30 - 1), paint);




                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidthf + 10;
                            }
                            x = 80;
                            y += 25;
                        }


                        x = 20;
                        y += 15;


                        string[][] encabezadoTablaDatosVehiculos = new string[][]
                        {
                        new string[] { "                                                             Datos del vehículo     "},

                        };


                        float cellWidthe = 578;


                        foreach (var fila in encabezadoTablaDatosVehiculos)
                        {
                            x = 20;

                            foreach (var valor in fila)
                            {
                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidthe, y + 30), paint);

                                paint.Color = celdaBackgroundColor;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidthe - 1, y + 30 - 1), paint);

                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidthe + 10;
                            }
                            x = 20;
                            y += 25;
                        }




                        x = 20;
                        y += 15;


                        string[][] primeraTablaDatosVehiculos = new string[][]
                        {
                        new string[] { "Marca:", "Submarca:", "Modelo:", "Tipo:" },
                        new string[] { $"{Marca}", $"{Submarca}", $"{Modelo}", $"{Tipo}" },
                        };

                        float cellWidth1 = 137;


                        foreach (var fila in primeraTablaDatosVehiculos)
                        {
                            x = 20;
                            foreach (var valor in fila)
                            {



                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidth1, y + 30), paint);

                                paint.Color = celdaBackgroundColorB;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth1 - 1, y + 30 - 1), paint);




                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidth1 + 10;
                            }
                            x = 20;
                            y += 25;
                        }





                        x = 20;
                        y += 15;

                        string[][] segundaTablaDatosVehiculos = new string[][]
                        {
                        new string[] { "Versión:", "Categoría de color:", "Color:", "Acabado:" },
                        new string[] { $"{Version}", $"{Categoría}", $"{Color}", $"{Acabado}"},
                        };

                        float cellWidth2 = 137;

                        foreach (var fila in segundaTablaDatosVehiculos)
                        {
                            x = 20;
                            foreach (var valor in fila)
                            {

                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidth2, y + 30), paint);


                                paint.Color = celdaBackgroundColorB;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth2 - 1, y + 30 - 1), paint);


                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidth2 + 10;
                            }
                            x = 30;
                            y += 25;
                        }




                        x = 20;
                        y += 15;


                        string[][] encabezadoTablaServicio = new string[][]
                        {
                        new string[] { "                                                             Datos del Servicio      "},

                        };

                        float cellWidthes = 578;


                        foreach (var fila in encabezadoTablaServicio)
                        {
                            x = 20;

                            foreach (var valor in fila)
                            {

                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidthes, y + 30), paint);

                                paint.Color = celdaBackgroundColor;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidthes - 1, y + 30 - 1), paint);

                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidthes + 10;
                            }
                            x = 20;
                            y += 25;
                        }





                        float cellWidth3 = 186;


                        x = 20;
                        y += 15;
                        string[][] primeraTablaDatosServicio = new string[][]
                        {
                        new string[] { "Tipo Servicio:", "Paquete:" },
                        new string[] { $"{TipoServicio}", $"{Paquete}"},
                                            };


                        float cellWidth6 = 284;
                        foreach (var fila in primeraTablaDatosServicio)
                        {
                            x = 20;

                            foreach (var valor in fila)
                            {

                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidth6, y + 30), paint);


                                paint.Color = celdaBackgroundColorB;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth6 - 1, y + 30 - 1), paint);


                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidth6 + 10;
                            }
                            x = 20;
                            y += 25;
                        }



                        x = 20;
                        String titulo = "Error";
                        string contenido = "Error";

                        if (TipoServicio.ToLower() == "Pintura".ToLower())
                        {
                            titulo = "Tipo de pintado:";
                            if (opcionTodo == "Pintura")
                            {
                                contenido = "Todo el vehículo";
                            }
                            else if (opcionTodo == null)
                            {
                                contenido = "Por pieza";
                            }
                        }

                        if (TipoServicio.ToLower() == "Pulido y encerado".ToLower())
                        {
                            titulo = "Tipo de pulido y enserado:";
                            if (opcionTodo == "PulidoEncerado")
                            {
                                contenido = "Todo el vehículo";
                            }
                            else if (opcionTodo == null)
                            {
                                contenido = "Por pieza";
                            }

                        }

                        if (TipoServicio.ToLower() == "Hojalatería y Pintura".ToLower())
                        {
                            titulo = "Tipo de golpe:";
                            contenido = $"{tipoGolpe}";

                        }





                        float cellWidth5 = 186;

                        x = 20;
                        y += 15;
                        string[][] terceraTablaDatosServicio = new string[][]
                        {
                        new string[] { "Prioridad:", $"{titulo}" },
                        new string[] { $"{Prioridad}", $"{contenido}"},
                                            };


                        cellWidth6 = 284;

                        foreach (var fila in terceraTablaDatosServicio)
                        {
                            x = 20;

                            foreach (var valor in fila)
                            {

                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidth6, y + 30), paint);


                                paint.Color = celdaBackgroundColorB;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth6 - 1, y + 30 - 1), paint);


                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidth6 + 10;
                            }
                            x = 20;
                            y += 25;
                        }



                        x = 20;
                        y += 15;
                        string[][] encabezadoTablaServicio2 = new string[][]
                        {
                        new string[] { "                                                        Datos del Taller      "},

                        };


                        cellWidthes = 578;



                        foreach (var fila in encabezadoTablaServicio2)
                        {
                            x = 20;

                            foreach (var valor in fila)
                            {


                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidthes, y + 30), paint);


                                paint.Color = celdaBackgroundColor;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidthes - 1, y + 30 - 1), paint);


                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidthes + 10;
                            }
                            x = 20;
                            y += 25;
                        }





                        x = 20;
                        y += 15;


                        string[][] primeraTablaDatosServicio2 = new string[][]
                        {
                        new string[] { "Nombre del taller:", "Teléfono:", "Correo electrónico:"},
                        new string[] { $"{taller.Nombre}", $"{taller.Telefono}", $"{taller.CorreoElectronico}"},
                                            };


                        cellWidth3 = 186;
                        foreach (var fila in primeraTablaDatosServicio2)
                        {
                            x = 20;

                            foreach (var valor in fila)
                            {

                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidth3, y + 30), paint);


                                paint.Color = celdaBackgroundColorB;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth3 - 1, y + 30 - 1), paint);

                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidth3 + 10;
                            }
                            x = 20;
                            y += 25;
                        }

                        cellWidth3 = 284;

                        x = 20;
                        y += 15;
                        string[][] terceraTablaDatosServicio2 = new string[][]
                        {
                        new string[] { "Encargado:", "Fecha de la cita:"},
                        new string[] { $"{taller.Encargado}", $"{Fecha}"},
                                            };



                        foreach (var fila in terceraTablaDatosServicio2)
                        {
                            x = 20;

                            foreach (var valor in fila)
                            {

                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidth3, y + 30), paint);


                                paint.Color = celdaBackgroundColorB;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth3 - 1, y + 30 - 1), paint);


                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidth3 + 10;
                            }
                            x = 20;
                            y += 25;
                        }


                        x = 20;
                        y += 15;


                        string[][] segundaTablaDatosServicio = new string[][]
                        {
                        new string[] { $"Dirección del Taller:" },
                        new string[] { $"{taller.Direccion} \n"},
                        new string[] { $"Da click para redirigir al maps" },
                       new string[] {$"http://maps.google.com/maps?f=q&q={taller.Latitud}{taller.Longitud}&z=16" },
                        };


                        float cellWidth4 = 578;
                        float cellHeight = 75;


                        foreach (var fila in segundaTablaDatosServicio)
                        {
                            x = 20;

                            foreach (var valor in fila)
                            {

                                var lineas = valor.Split('\n');


                                float celdaHeight = cellHeight * lineas.Length;


                                paint.Color = celdaBorderColor;
                               
                                canvas.DrawRect(new SKRect(x, y, x + cellWidth4, y + celdaHeight), paint);
                                


                                paint.Color = celdaBackgroundColorB;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth4 - 1, y + celdaHeight - 1), paint);

                                paint.Color = SKColors.Black;
                                foreach (var linea in lineas)
                                {
                                    canvas.DrawText(linea, x + 10, y + 20, paint);
                                    y += cellHeight;
                                }

                                x += cellWidth4 + 10;
                                y -= cellHeight * lineas.Length;
                            }

                            x = 20;
                            y += 25;
                        }


                        x = 20;
                        y += 60;
                        string[][] tablaPresupuesto = new string[][]
                        {
                        new string[] { "Presupuesto:" },
                        new string[] { $"$ {Presupuesto} MX" }};


                        float cellWidth8 = 278;


                        foreach (var fila in tablaPresupuesto)
                        {
                            x = 320;

                            foreach (var valor in fila)
                            {

                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidth8, y + 30), paint);


                                paint.Color = celdaBackgroundColorB;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth8 - 1, y + 30 - 1), paint);


                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidth5 + 10;
                            }
                            x = 20;
                            y += 25;
                        }
                        x = 10;
                        y = 715;
                        string[][] tablaPresupuesto2 = new string[][]
                        {
                        new string[] {
                            "EL PRECIO PUEDE LLEGAR A CAMBIAR O PERMANECER EN LA \n" +
                            "CANTIDAD QUE SE MENCIONA, ESTO DEPENDIENDO DE LA \n" +
                            "REVISION FISICA DEL SERVICIO QUE SE SOLICITA." }};


                        cellWidth8 = 285;
                        cellHeight = 14;


                        foreach (var fila in tablaPresupuesto2)
                        {
                            paint.TextSize = 10;
                            x = 10;

                            foreach (var valor in fila)
                            {

                                var lineas = valor.Split('\n');


                                float celdaHeight = cellHeight * lineas.Length;


                                paint.Color = SKColors.Transparent;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidth8, y + celdaHeight), paint);


                                paint.Color = celdaBackgroundColorB;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth8 - 1, y + celdaHeight - 1), paint);


                                paint.Color = SKColors.Black;
                                foreach (var linea in lineas)
                                {
                                    canvas.DrawText(linea, x + 10, y + 20, paint);
                                    y += cellHeight;
                                }

                                x += cellWidth8 + 10;
                                y -= cellHeight * lineas.Length;
                            }

                            x = 20;
                            y += 25;
                        }
                    }


                    document.EndPage();

                    int dato = 0;
                    int tamanoGrupo = 3;

                    if (datosGolpes != null)
                    {


                        while (datosGolpes.Count > 0)
                        {

                            dato = 0;

                            if (datosGolpes.Count >= 3)
                            {
                                dato = 1;
                            }
                            using (var canvas2 = document.BeginPage(width, height))
                            {

                                canvas2.Clear(SKColors.White);

                                var assembly2 = Assembly.GetExecutingAssembly();


                                using (var paint = new SKPaint())
                                {
                                    paint.TextSize = 14;
                                    paint.Color = SKColors.Black;
                                    SKColor celdaBackgroundColor = SKColors.Coral;
                                    SKColor celdaBackgroundColorB = SKColors.White;
                                    SKColor celdaBackgroundColorc = SKColors.Gold;
                                    SKColor celdaBorderColor = SKColors.Black;
                                    float x = 20;
                                    float y = 15;


                                    String titulo = "Error";

                                    if (TipoServicio.ToLower() == "Pintura".ToLower())
                                    {
                                        titulo = "                                                     Piezas a Pintar del Vehículo  ";
                                    }

                                    if (TipoServicio.ToLower() == "Pulido y encerado".ToLower())
                                    {

                                        titulo = "                                            Piezas a Pulir y Enserar del Vehículo  ";
                                    }

                                    if (TipoServicio.ToLower() == "Hojalatería y Pintura".ToLower())
                                    {
                                        titulo = "                                                         Aréa Dañada del Vehículo  ";

                                    }
                                    string[][] encabezadoTablaOrden2 = new string[][]
                                    {
                                        new string[] { titulo},

                                    };


                                    float cellWidth14 = 578;



                                    foreach (var fila in encabezadoTablaOrden2)
                                    {
                                        x = 20;

                                        foreach (var valor in fila)
                                        {

                                            paint.Color = celdaBorderColor;
                                            canvas2.DrawRect(new SKRect(x, y, x + cellWidth14, y + 30), paint);


                                            paint.Color = celdaBackgroundColorc;
                                            canvas2.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth14 - 1, y + 30 - 1), paint);


                                            paint.Color = SKColors.Black;
                                            canvas2.DrawText(valor, x + 10, y + 20, paint);
                                            x += cellWidth14 + 10;
                                        }
                                        x = 20;

                                    }

                                    x = 50;
                                    y += 43;


                                    List<GolpeMedida> grupo = datosGolpes.GetRange(0, Math.Min(tamanoGrupo, datosGolpes.Count));
                                    foreach (var datoGolpe in grupo)
                                    {

                                        String datosTabla = "";
                                        String datosTabla1 = "";
                                        if (TipoServicio.ToLower() == "Pintura".ToLower())
                                        {
                                            datosTabla = "Datos de la pieza a pintar:";
                                            datosTabla1 = $"Diametro Horizontal: {datoGolpe.DiametroH}\n Diametro Vertical: {datoGolpe.DiametroV}\n";
                                        }



                                        if (TipoServicio.ToLower() == "Hojalatería y Pintura".ToLower())
                                        {
                                            datosTabla = "Datos del golpe:";
                                            datosTabla1 = $"Diametro Horizontal: {datoGolpe.DiametroH}\n Diametro Vertical: {datoGolpe.DiametroV}\n Profundidad: {datoGolpe.ProfundidadGolpe}";
                                        }

                                        string[][] tablaImagen = new string[][]
                                        {
                                            new string[] { $"{datoGolpe.PiezaNombre}", $"{datosTabla}" },
                                            new string[] { "prueba.jpeg",$"{datosTabla1}" },
                                        };

                                        float cellWidth4 = 284;
                                        float cellHeight = 57;

                                        foreach (var fila in tablaImagen)
                                        {
                                            x = 20;

                                            float maxCeldaHeight = 0;

                                            foreach (var valor in fila)
                                            {
                                                var lineas = valor.Split('\n');

                                                float celdaHeight = cellHeight * lineas.Length;

                                                maxCeldaHeight = Math.Max(maxCeldaHeight, celdaHeight);
                                            }

                                            foreach (var valor in fila)
                                            {
                                                var lineas = valor.Split('\n');

                                                paint.Color = celdaBorderColor;
                                                canvas2.DrawRect(new SKRect(x, y, x + cellWidth4, y + maxCeldaHeight), paint);

                                                paint.Color = celdaBackgroundColorB;
                                                canvas2.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth4 - 1, y + maxCeldaHeight - 1), paint);

                                                float currentY = y;

                                                paint.Color = SKColors.Black;
                                                foreach (var linea in lineas)
                                                {
                                                    if (linea.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) || linea.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || linea.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
                                                    {

                                                        if (datoGolpe.DatoImagen != null)
                                                        {
                                                            using (var imageStream = new MemoryStream(datoGolpe.DatoImagen))
                                                            {
                                                                using (var bitmap = SKBitmap.Decode(imageStream))
                                                                {
                                                                    var imageWidth = cellWidth4 - 20;
                                                                    var imageHeight = maxCeldaHeight - 10;

                                                                    canvas2.DrawBitmap(bitmap, new SKRect(x + 10, currentY + 5, x + 10 + imageWidth, currentY + 5 + imageHeight));
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        canvas2.DrawText(linea, x + 10, currentY + 20, paint);
                                                    }

                                                    currentY += cellHeight;
                                                }

                                                x += cellWidth4 + 10;
                                            }

                                            x = 20;
                                            y += maxCeldaHeight + 0;


                                        }
                                        x = 20;
                                        y += 17;


                                    }
                                    datosGolpes.RemoveRange(0, Math.Min(tamanoGrupo, datosGolpes.Count));

                                    if (dato == 0)
                                    {
                                        x += 20;
                                        y += 15;


                                        string[][] encabezadoTablaUsuario = new string[][]
                                        {
                            new string[] { "                                                               Datos del Cliente    "},

                                        };

                                        float cellWidtheu = 578;



                                        foreach (var fila in encabezadoTablaUsuario)
                                        {
                                            x = 20;

                                            foreach (var valor in fila)
                                            {

                                                paint.Color = celdaBorderColor;
                                                canvas2.DrawRect(new SKRect(x, y, x + cellWidtheu, y + 30), paint);

                                                paint.Color = celdaBackgroundColor;
                                                canvas2.DrawRect(new SKRect(x + 1, y + 1, x + cellWidtheu - 1, y + 30 - 1), paint);

                                                paint.Color = SKColors.Black;
                                                canvas2.DrawText(valor, x + 10, y + 20, paint);
                                                x += cellWidtheu + 10;
                                            }
                                            x = 20;
                                            y += 25;
                                        }

                                        x = 20;
                                        y += 15;

                                        string[][] primeraTablaDatosUsuarios = new string[][]
                                        {
                            new string[] { "Nombre completo:", "Correo electronico:" },
                            new string[] { $"{NombreCompleto}", $"{CorreoElectronico}"},
                                                            };

                                        float cellWidth6 = 284;

                                        foreach (var fila in primeraTablaDatosUsuarios)
                                        {
                                            x = 20;

                                            foreach (var valor in fila)
                                            {
                                                paint.Color = celdaBorderColor;
                                                canvas2.DrawRect(new SKRect(x, y, x + cellWidth6, y + 30), paint);

                                                paint.Color = celdaBackgroundColorB;
                                                canvas2.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth6 - 1, y + 30 - 1), paint);

                                                paint.Color = SKColors.Black;
                                                canvas2.DrawText(valor, x + 10, y + 20, paint);
                                                x += cellWidth6 + 10;
                                            }
                                            x = 20;
                                            y += 25;
                                        }

                                        // Reiniciar x y ajustar la posición vertical para la  tabla
                                        x = 20;
                                        y += 20; // Ajusta la posición vertical para la tabla

                                        // Datos de la  tabla con celdas más anchas
                                        string[][] segundaTablaDatosUsuarios = new string[][]
                                        {
                            new string[] { "Telefono:", "Estado:","Municipio:" },
                            new string[] { $"{Telefono}", $"{Estado}", $"{Municipio}"}};

                                        float cellWidth7 = 186;

                                        foreach (var fila in segundaTablaDatosUsuarios)
                                        {
                                            x = 20;

                                            foreach (var valor in fila)
                                            {
                                                paint.Color = celdaBorderColor;
                                                canvas2.DrawRect(new SKRect(x, y, x + cellWidth7, y + 30), paint);

                                                paint.Color = celdaBackgroundColorB;
                                                canvas2.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth7 - 1, y + 30 - 1), paint);

                                                paint.Color = SKColors.Black;
                                                canvas2.DrawText(valor, x + 10, y + 20, paint);
                                                x += cellWidth7 + 10;
                                            }
                                            x = 20;
                                            y += 25;
                                        }
                                    }

                                }







                            }

                            document.EndPage();

                        }



                    }
                    if (TipoServicio == "Pulido y encerado")
                    {
                        String piezasPE = null;
                        foreach (var pieza in partesSeleccionadasPE)
                        {
                            piezasPE += $"--> {pieza}\n";
                        }
                        using (var canvas2 = document.BeginPage(width, height))
                        {
                            canvas2.Clear(SKColors.White);

                            var assembly2 = Assembly.GetExecutingAssembly();

                            using (var paint = new SKPaint())
                            {
                                paint.TextSize = 14;
                                paint.Color = SKColors.Black;
                                SKColor celdaBackgroundColor = SKColors.Coral;
                                SKColor celdaBackgroundColorB = SKColors.White;
                                SKColor celdaBackgroundColorc = SKColors.Gold;
                                SKColor celdaBorderColor = SKColors.Black;

                                float x = 20;
                                float y = 15;


                                string[][] encabezadoTablaUsuario = new string[][]
                                {
                            new string[] { "                                                               Datos del Cliente    "},

                                };

                                float cellWidtheu = 578;


                                foreach (var fila in encabezadoTablaUsuario)
                                {
                                    x = 20;

                                    foreach (var valor in fila)
                                    {


                                        paint.Color = celdaBorderColor;
                                        canvas2.DrawRect(new SKRect(x, y, x + cellWidtheu, y + 30), paint);

                                        paint.Color = celdaBackgroundColor;
                                        canvas2.DrawRect(new SKRect(x + 1, y + 1, x + cellWidtheu - 1, y + 30 - 1), paint);

                                        paint.Color = SKColors.Black;
                                        canvas2.DrawText(valor, x + 10, y + 20, paint);
                                        x += cellWidtheu + 10;
                                    }
                                    x = 20;
                                    y += 25;
                                }

                                x = 20;
                                y += 15;

                                string[][] primeraTablaDatosUsuarios = new string[][]
                                {
                            new string[] { "Nombre completo:", "Correo electronico:" },
                            new string[] { $"{NombreCompleto}", $"{CorreoElectronico}"},
                                                    };

                                float cellWidth6 = 284;
                                foreach (var fila in primeraTablaDatosUsuarios)
                                {
                                    x = 20;
                                    foreach (var valor in fila)
                                    {

                                        paint.Color = celdaBorderColor;
                                        canvas2.DrawRect(new SKRect(x, y, x + cellWidth6, y + 30), paint);


                                        paint.Color = celdaBackgroundColorB;
                                        canvas2.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth6 - 1, y + 30 - 1), paint);


                                        paint.Color = SKColors.Black;
                                        canvas2.DrawText(valor, x + 10, y + 20, paint);
                                        x += cellWidth6 + 10;
                                    }
                                    x = 20;
                                    y += 25;
                                }


                                x = 20;
                                y += 20;
                                string[][] segundaTablaDatosUsuarios3 = new string[][]
                                {
                            new string[] { "Telefono:", "Estado:","Municipio:" },
                            new string[] { $"{Telefono}", $"{Estado}", $"{Municipio}"}};


                                float cellWidth7 = 186;


                                foreach (var fila in segundaTablaDatosUsuarios3)
                                {
                                    x = 20;

                                    foreach (var valor in fila)
                                    {

                                        paint.Color = celdaBorderColor;
                                        canvas2.DrawRect(new SKRect(x, y, x + cellWidth7, y + 30), paint);

                                        paint.Color = celdaBackgroundColorB;
                                        canvas2.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth7 - 1, y + 30 - 1), paint);

                                        paint.Color = SKColors.Black;
                                        canvas2.DrawText(valor, x + 10, y + 20, paint);
                                        x += cellWidth7 + 10;
                                    }
                                    x = 20;
                                    y += 25;
                                }



                                x += 20;
                                y += 25;
                                string[][] encabezadoTablaOrden2 = new string[][]
                                {
                                 new string[] { "                                                            Piezas Para Pulir y Encerar  "},

                                };


                                float cellWidth14 = 578;


                                foreach (var fila in encabezadoTablaOrden2)
                                {
                                    x = 20;
                                    foreach (var valor in fila)
                                    {

                                        paint.Color = celdaBorderColor;
                                        canvas2.DrawRect(new SKRect(x, y, x + cellWidth14, y + 30), paint);

                                        paint.Color = celdaBackgroundColorc;
                                        canvas2.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth14 - 1, y + 30 - 1), paint);

                                        paint.Color = SKColors.Black;
                                        canvas2.DrawText(valor, x + 10, y + 20, paint);
                                        x += cellWidth14 + 10;
                                    }
                                    x = 20;

                                }

                                x = 20;
                                y += 40;
                                string[][] segundaTablaDatosUsuarios = new string[][]
                                {
                        new string[] { $"{piezasPE}" }
                                                    };


                                float cellWidth4 = 578;
                                float cellHeight = 25;

                                foreach (var fila in segundaTablaDatosUsuarios)
                                {
                                    x = 20;

                                    foreach (var valor in fila)
                                    {

                                        var lineas = valor.Split('\n');

                                        float celdaHeight = cellHeight * lineas.Length;

                                        paint.Color = celdaBorderColor;
                                        canvas2.DrawRect(new SKRect(x, y, x + cellWidth4, y + celdaHeight), paint);

                                        paint.Color = celdaBackgroundColorB;
                                        canvas2.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth4 - 1, y + celdaHeight - 1), paint);

                                        paint.Color = SKColors.Black;
                                        foreach (var linea in lineas)
                                        {
                                            canvas2.DrawText(linea, x + 10, y + 20, paint);
                                            y += cellHeight;
                                        }

                                        x += cellWidth4 + 10;
                                        y -= cellHeight * lineas.Length;
                                    }

                                    x = 20;
                                    y += 25;
                                }









                            }




                            document.EndPage();
                        }

                    }


                    if (dato == 1)
                    {
                        using (var canvas23 = document.BeginPage(width, height))
                        {
                            canvas23.Clear(SKColors.White);

                            var assembly22 = Assembly.GetExecutingAssembly();

                            using (var paint = new SKPaint())
                            {
                                paint.TextSize = 14;
                                paint.Color = SKColors.Black;
                                SKColor celdaBackgroundColor = SKColors.Coral;
                                SKColor celdaBackgroundColorB = SKColors.White;
                                SKColor celdaBackgroundColorc = SKColors.Gold;
                                SKColor celdaBorderColor = SKColors.Black;


                                float x = 20;
                                float y = 15;
                                string[][] encabezadoTablaUsuario = new string[][]
                                {
        new string[] { "                                                               Datos del Cliente    "},

                                };


                                float cellWidtheu = 578;


                                foreach (var fila in encabezadoTablaUsuario)
                                {
                                    x = 20;

                                    foreach (var valor in fila)
                                    {

                                        paint.Color = celdaBorderColor;
                                        canvas23.DrawRect(new SKRect(x, y, x + cellWidtheu, y + 30), paint);

                                        paint.Color = celdaBackgroundColor;
                                        canvas23.DrawRect(new SKRect(x + 1, y + 1, x + cellWidtheu - 1, y + 30 - 1), paint);

                                        paint.Color = SKColors.Black;
                                        canvas23.DrawText(valor, x + 10, y + 20, paint);
                                        x += cellWidtheu + 10;
                                    }
                                    x = 20;
                                    y += 25;
                                }


                                x = 20;
                                y += 15;


                                string[][] primeraTablaDatosUsuarios = new string[][]
                                {
        new string[] { "Nombre completo:", "Correo electronico:" },
        new string[] { $"{NombreCompleto}", $"{CorreoElectronico}"},
                                                    };


                                float cellWidth6 = 284;
                                foreach (var fila in primeraTablaDatosUsuarios)
                                {
                                    x = 20;

                                    foreach (var valor in fila)
                                    {

                                        paint.Color = celdaBorderColor;
                                        canvas23.DrawRect(new SKRect(x, y, x + cellWidth6, y + 30), paint);


                                        paint.Color = celdaBackgroundColorB;
                                        canvas23.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth6 - 1, y + 30 - 1), paint);


                                        paint.Color = SKColors.Black;
                                        canvas23.DrawText(valor, x + 10, y + 20, paint);
                                        x += cellWidth6 + 10;
                                    }
                                    x = 20;
                                    y += 25;
                                }


                                x += 20;
                                y += 20;


                                string[][] segundaTablaDatosUsuarios = new string[][]
                                {
        new string[] { "Telefono:", "Estado:","Municipio:" },
        new string[] { $"{Telefono}", $"{Estado}", $"{Municipio}"}};


                                float cellWidth7 = 186;

                                foreach (var fila in segundaTablaDatosUsuarios)
                                {
                                    x = 20;

                                    foreach (var valor in fila)
                                    {

                                        paint.Color = celdaBorderColor;
                                        canvas23.DrawRect(new SKRect(x, y, x + cellWidth7, y + 30), paint);

                                        paint.Color = celdaBackgroundColorB;
                                        canvas23.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth7 - 1, y + 30 - 1), paint);

                                        paint.Color = SKColors.Black;
                                        canvas23.DrawText(valor, x + 10, y + 20, paint);
                                        x += cellWidth7 + 10;
                                    }
                                    x = 20;
                                    y += 25;
                                }






                            }






                        }
                        document.EndPage();

                    }



                }

                document.Close();
                stream.Close();
                return filePath;
            }
        }

        public async void GenerarPDFGFTV(String Folio, String Marca, String Submarca, String Modelo, String Tipo, String Version, String Categoría, String Color, String Acabado, String TipoServicio, String Prioridad, TallerDTO taller, String Fecha, String Hora, String NombreCompleto, String CorreoElectronico, String Telefono, String Estado, String Municipio, String tipoGolpe, List<Imagen> imagenesGolpeFuerte, String opcionTodo, String Paquete, String Presupuesto)
        {

            var listaCadenas = contenido.Split('\n');
            int num = listaCadenas.Length;
            float pointsPerInch = 72;
            float width = 8.5f * pointsPerInch;
            float height = 11f * pointsPerInch;

            using (var stream = new SKFileWStream(filePath))
            using (var document = SKDocument.CreatePdf(stream))
            using (var canvas = document.BeginPage(width, height))
            {
                canvas.Clear(SKColors.White);


                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "AplicacionAuto.Resources.Images.logo.png";
                using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (resourceStream != null)
                    {
                        using (var bitmap = SKBitmap.Decode(resourceStream))
                        {
                            var image = SKImage.FromBitmap(bitmap);

                            var pdfWidth = 400;


                            float imageWidth = pdfWidth / 4.00f;
                            float imageHeight = bitmap.Height * (imageWidth / bitmap.Width);

                            float imageX = (pdfWidth - imageWidth) / 10;
                            float imageY = 0;

                            canvas.DrawImage(image, new SKRect(imageX, imageY, imageX + imageWidth, imageY + imageHeight));
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Image resource not found.");
                    }

                    using (var paint = new SKPaint())
                    {
                        paint.TextSize = 14;
                        paint.Color = SKColors.Black;
                        SKColor celdaBackgroundColor = SKColors.Coral;
                        SKColor celdaBackgroundColorB = SKColors.White;
                        SKColor celdaBackgroundColorc = SKColors.Gold;
                        SKColor celdaBorderColor = SKColors.Black;

                        float x = 20;
                        float y = 20;
                        string[][] encabezadoTablaOrden = new string[][]
                        {
                        new string[] { "                                        Orden de Admisión de Presupuesto "},

                        };


                        float cellWidtho = 450;


                        foreach (var fila in encabezadoTablaOrden)
                        {
                            x = 148;

                            foreach (var valor in fila)
                            {
                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidtho, y + 30), paint);

                                paint.Color = celdaBackgroundColorc;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidtho - 1, y + 30 - 1), paint);

                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidtho + 10;
                            }
                            x = 20;

                        }

                        x = 20;
                        y += 35;


                        string[][] TablaFolio = new string[][]
                        {
                        new string[] { $"Folio: {solicitudID}"}

                        };



                        float cellWidthf = 170;



                        foreach (var fila in TablaFolio)
                        {
                            x = 428;

                            foreach (var valor in fila)
                            {

                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidthf, y + 30), paint);


                                paint.Color = celdaBackgroundColorB;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidthf - 1, y + 30 - 1), paint);




                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidthf + 10;
                            }
                            x = 80;
                            y += 25;
                        }


                        x = 20;
                        y += 15;


                        string[][] encabezadoTablaDatosVehiculos = new string[][]
                        {
                        new string[] { "                                                             Datos del vehículo     "},

                        };


                        float cellWidthe = 578;


                        foreach (var fila in encabezadoTablaDatosVehiculos)
                        {
                            x = 20;

                            foreach (var valor in fila)
                            {
                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidthe, y + 30), paint);

                                paint.Color = celdaBackgroundColor;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidthe - 1, y + 30 - 1), paint);

                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidthe + 10;
                            }
                            x = 20;
                            y += 25;
                        }




                        x = 20;
                        y += 15;


                        string[][] primeraTablaDatosVehiculos = new string[][]
                        {
                            new string[] { "Marca:", "Submarca:", "Modelo:", "Tipo:" },
                            new string[] { $"{Marca}", $"{Submarca}", $"{Modelo}", $"{Tipo}" },
                        };

                        float cellWidth1 = 137;


                        foreach (var fila in primeraTablaDatosVehiculos)
                        {
                            x = 20;
                            foreach (var valor in fila)
                            {


                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidth1, y + 30), paint);

                                paint.Color = celdaBackgroundColorB;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth1 - 1, y + 30 - 1), paint);




                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidth1 + 10;
                            }
                            x = 20;
                            y += 25;
                        }





                        x = 20;
                        y += 15;

                        string[][] segundaTablaDatosVehiculos = new string[][]
                        {
                            new string[] { "Versión:", "Categoría de color:", "Color:", "Acabado:" },
                            new string[] { $"{Version}", $"{Categoría}", $"{Color}", $"{Acabado}"},
                        };

                        float cellWidth2 = 137;

                        foreach (var fila in segundaTablaDatosVehiculos)
                        {
                            x = 20;
                            foreach (var valor in fila)
                            {
                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidth2, y + 30), paint);

                                paint.Color = celdaBackgroundColorB;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth2 - 1, y + 30 - 1), paint);

                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidth2 + 10;
                            }
                            x = 30;
                            y += 25;
                        }






                        if (opcionTodo != null)
                        {
                            x = 20;
                            y += 15;


                            string[][] encabezadoTablaServicio = new string[][]
                            {
                        new string[] { "                                                             Datos del Servicio      "},

                            };


                            float cellWidthes12 = 578;


                            foreach (var fila in encabezadoTablaServicio)
                            {
                                x = 20;

                                foreach (var valor in fila)
                                {

                                    paint.Color = celdaBorderColor;
                                    canvas.DrawRect(new SKRect(x, y, x + cellWidthes12, y + 30), paint);

                                    paint.Color = celdaBackgroundColor;
                                    canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidthes12 - 1, y + 30 - 1), paint);

                                    paint.Color = SKColors.Black;
                                    canvas.DrawText(valor, x + 10, y + 20, paint);
                                    x += cellWidthes12 + 10;
                                }
                                x = 20;
                                y += 25;
                            }





                            x = 20;
                            y += 15;
                            string[][] primeraTablaDatosServicio = new string[][]
                            {
                        new string[] { "Tipo Servicio:", "Paquete:" },
                        new string[] { $"{TipoServicio}", $"{Paquete}"},
                                                };


                            float cellWidth64 = 284;


                            foreach (var fila in primeraTablaDatosServicio)
                            {
                                x = 20;

                                foreach (var valor in fila)
                                {

                                    paint.Color = celdaBorderColor;
                                    canvas.DrawRect(new SKRect(x, y, x + cellWidth64, y + 30), paint);


                                    paint.Color = celdaBackgroundColorB;
                                    canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth64 - 1, y + 30 - 1), paint);


                                    paint.Color = SKColors.Black;
                                    canvas.DrawText(valor, x + 10, y + 20, paint);
                                    x += cellWidth64 + 10;
                                }
                                x = 20;
                                y += 25;
                            }




                            x = 20;
                            String titulo = "Error";
                            string contenido = "Error";

                            if (TipoServicio.ToLower() == "Pintura".ToLower())
                            {
                                titulo = "Tipo de pintado:";
                                if (opcionTodo == "Pintura")
                                {
                                    contenido = "Todo el vehículo";
                                }
                                else if (opcionTodo == null)
                                {
                                    contenido = "Por pieza";
                                }
                            }

                            if (TipoServicio.ToLower() == "Pulido y encerado".ToLower())
                            {
                                titulo = "Tipo de pulido y enserado:";
                                if (opcionTodo == "PulidoEncerado")
                                {
                                    contenido = "Todo el vehículo";
                                }
                                else if (opcionTodo == null)
                                {
                                    contenido = "Por pieza";
                                }

                            }

                            if (TipoServicio.ToLower() == "Hojalatería y Pintura".ToLower())
                            {
                                titulo = "Tipo de golpe:";
                                contenido = $"{tipoGolpe}";

                            }





                            float cellWidth5 = 186;
                            x = 20;
                            y += 15;
                            string[][] terceraTablaDatosServicio = new string[][]
                            {
                        new string[] { "Prioridad:", $"{titulo}" },
                        new string[] { $"{Prioridad}", $"{contenido}"},
                                                };


                            float cellWidth69 = 284;
                            foreach (var fila in terceraTablaDatosServicio)
                            {
                                x = 20;

                                foreach (var valor in fila)
                                {

                                    paint.Color = celdaBorderColor;
                                    canvas.DrawRect(new SKRect(x, y, x + cellWidth69, y + 30), paint);


                                    paint.Color = celdaBackgroundColorB;
                                    canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth69 - 1, y + 30 - 1), paint);


                                    paint.Color = SKColors.Black;
                                    canvas.DrawText(valor, x + 10, y + 20, paint);
                                    x += cellWidth69 + 10;
                                }
                                x = 20;
                                y += 25;
                            }




                        }
                        else
                        {
                            x = 20;
                            y += 15;


                            string[][] encabezadoTablaServicio = new string[][]
                            {
                                new string[] { "                                                        Datos del Servicio      "},
                            };


                            float cellWidthes11 = 578;
                            foreach (var fila in encabezadoTablaServicio)
                            {
                                x = 20;

                                foreach (var valor in fila)
                                {


                                    paint.Color = celdaBorderColor;
                                    canvas.DrawRect(new SKRect(x, y, x + cellWidthes11, y + 30), paint);

                                    paint.Color = celdaBackgroundColor;
                                    canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidthes11 - 1, y + 30 - 1), paint);

                                    paint.Color = SKColors.Black;
                                    canvas.DrawText(valor, x + 10, y + 20, paint);
                                    x += cellWidthes11 + 10;
                                }
                                x = 20;
                                y += 25;
                            }


                            x = 20;
                            y += 15;
                            string[][] primeraTablaDatosServicio = new string[][]
                            {
                                new string[] { "Tipo Servicio:", "Prioridad:", "Tipo golpe:"},
                                new string[] { $"{TipoServicio}", $"{Prioridad}", $"{tipoGolpe}"},
                                                };


                            float cellWidth38 = 186;
                            foreach (var fila in primeraTablaDatosServicio)
                            {
                                x = 20;
                                foreach (var valor in fila)
                                {

                                    paint.Color = celdaBorderColor;
                                    canvas.DrawRect(new SKRect(x, y, x + cellWidth38, y + 30), paint);

                                    paint.Color = celdaBackgroundColorB;
                                    canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth38 - 1, y + 30 - 1), paint);


                                    paint.Color = SKColors.Black;
                                    canvas.DrawText(valor, x + 10, y + 20, paint);
                                    x += cellWidth38 + 10;
                                }
                                x = 20;
                                y += 25;
                            }
                        }



                        x = 20;
                        y += 15;
                        string[][] encabezadoTablaServicio2 = new string[][]
                        {
                        new string[] { "                                                        Datos del Taller      "},

                        };


                        float cellWidthes = 578;
                        foreach (var fila in encabezadoTablaServicio2)
                        {
                            x = 20;

                            foreach (var valor in fila)
                            {


                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidthes, y + 30), paint);

                                paint.Color = celdaBackgroundColor;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidthes - 1, y + 30 - 1), paint);

                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidthes + 10;
                            }
                            x = 20;
                            y += 25;
                        }





                        x = 20;
                        y += 15;
                        string[][] primeraTablaDatosServicio2 = new string[][]
                        {
                        new string[] { "Nombre del taller:", "Teléfono:", "Correo electrónico:"},
                        new string[] { $"{taller.Nombre}", $"{taller.Telefono}", $"{taller.CorreoElectronico}"},
                                            };


                        float cellWidth3 = 186;
                        foreach (var fila in primeraTablaDatosServicio2)
                        {
                            x = 20;

                            foreach (var valor in fila)
                            {

                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidth3, y + 30), paint);


                                paint.Color = celdaBackgroundColorB;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth3 - 1, y + 30 - 1), paint);

                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidth3 + 10;
                            }
                            x = 20;
                            y += 25;
                        }

                        cellWidth3 = 284;

                        x = 20;
                        y += 15;
                        string[][] terceraTablaDatosServicio2 = new string[][]
                        {
                        new string[] { "Encargado:", "Fecha de la cita:"},
                        new string[] { $"{taller.Encargado}", $"{Fecha}"},
                                            };



                        foreach (var fila in terceraTablaDatosServicio2)
                        {
                            x = 20;

                            foreach (var valor in fila)
                            {

                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidth3, y + 30), paint);


                                paint.Color = celdaBackgroundColorB;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth3 - 1, y + 30 - 1), paint);


                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidth3 + 10;
                            }
                            x = 20;
                            y += 25;
                        }


                        x = 20;
                        y += 15;
                        string[][] segundaTablaDatosServicio = new string[][]
                        {
                        new string[] { $"Dirección del Taller:" },
                        new string[] { $"{taller.Direccion}" },
                        };


                        float cellWidth4 = 578;
                        float cellHeight = 75;

                        foreach (var fila in segundaTablaDatosServicio)
                        {
                            x = 20;

                            foreach (var valor in fila)
                            {

                                var lineas = valor.Split('\n');

                                float celdaHeight = cellHeight * lineas.Length;

                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidth4, y + celdaHeight), paint);

                                paint.Color = celdaBackgroundColorB;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth4 - 1, y + celdaHeight - 1), paint);

                                paint.Color = SKColors.Black;
                                foreach (var linea in lineas)
                                {
                                    canvas.DrawText(linea, x + 10, y + 20, paint);
                                    y += cellHeight;
                                }

                                x += cellWidth4 + 10;
                                y -= cellHeight * lineas.Length;
                            }

                            x = 20;
                            y += 25;
                        }

                        if (tipoGolpe != "Fuerte")
                        {
                            x = 20;
                            y += 60;


                            string[][] tablaPresupuesto = new string[][]
                            {
                        new string[] { "Presupuesto:" },
                        new string[] { $"$ {Presupuesto} MX" }};


                            float cellWidth8 = 278;

                            foreach (var fila in tablaPresupuesto)
                            {
                                x = 320;

                                foreach (var valor in fila)
                                {

                                    paint.Color = celdaBorderColor;
                                    canvas.DrawRect(new SKRect(x, y, x + cellWidth8, y + 30), paint);

                                    paint.Color = celdaBackgroundColorB;
                                    canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth8 - 1, y + 30 - 1), paint);

                                    paint.Color = SKColors.Black;
                                    canvas.DrawText(valor, x + 10, y + 20, paint);
                                    x += cellWidth8 + 10;
                                }
                                x = 20;
                                y += 25;
                            }
                            x = 10;
                            y = 715;
                            string[][] tablaPresupuesto2 = new string[][]
                            {
                        new string[] {
                            "EL PRECIO PUEDE LLEGAR A CAMBIAR O PERMANECER EN LA \n" +
                            "CANTIDAD QUE SE MENCIONA, ESTO DEPENDIENDO DE LA \n" +
                            "REVISION FISICA DEL SERVICIO QUE SE SOLICITA." }};


                            cellWidth8 = 285;
                            cellHeight = 14;


                            foreach (var fila in tablaPresupuesto2)
                            {
                                paint.TextSize = 10;
                                x = 10;

                                foreach (var valor in fila)
                                {

                                    var lineas = valor.Split('\n');


                                    float celdaHeight = cellHeight * lineas.Length;


                                    paint.Color = SKColors.Transparent;
                                    canvas.DrawRect(new SKRect(x, y, x + cellWidth8, y + celdaHeight), paint);


                                    paint.Color = celdaBackgroundColorB;
                                    canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth8 - 1, y + celdaHeight - 1), paint);


                                    paint.Color = SKColors.Black;
                                    foreach (var linea in lineas)
                                    {
                                        canvas.DrawText(linea, x + 10, y + 20, paint);
                                        y += cellHeight;
                                    }

                                    x += cellWidth8 + 10;
                                    y -= cellHeight * lineas.Length;
                                }

                                x = 20;
                                y += 25;
                            }
                        }
                        else if (tipoGolpe == "Fuerte")
                        {
                            x = 10;
                            y = 715;
                            string[][] tablaPresupuesto2 = new string[][]
                            {
                                new string[] {
                                    "SU VEICULO ES SELECCIONADO COMO GOLPE FUERTE, SE LE DARA MEJOR INFORMACION Y COSTEO AL ACUDIR A UNO DE \nNUESTROS TALLERES MAS CERCANOS O PUEDE COMUNICARSE CON NOSOTROS PARA MEJOR ASESORIA."}};


                            float cellWidth8 = 285;
                            cellHeight = 14;

                            foreach (var fila in tablaPresupuesto2)
                            {
                                paint.TextSize = 10;
                                x = 10;

                                foreach (var valor in fila)
                                {

                                    var lineas = valor.Split('\n');

                                    float celdaHeight = cellHeight * lineas.Length;

                                    paint.Color = SKColors.Transparent;
                                    canvas.DrawRect(new SKRect(x, y, x + cellWidth8, y + celdaHeight), paint);

                                    paint.Color = celdaBackgroundColorB;
                                    canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth8 - 1, y + celdaHeight - 1), paint);

                                    paint.Color = SKColors.Black;
                                    foreach (var linea in lineas)
                                    {
                                        canvas.DrawText(linea, x + 10, y + 20, paint);
                                        y += cellHeight;
                                    }

                                    x += cellWidth8 + 10;
                                    y -= cellHeight * lineas.Length;
                                }

                                x = 20;
                                y += 25;
                            }
                        }


                    }


                    document.EndPage();
                    int dato = 0;
                    int tamanoGrupo = 3;
                    if (imagenesGolpeFuerte != null)
                    {
                        while (imagenesGolpeFuerte.Count > 0)
                        {
                            dato = 0;

                            if (imagenesGolpeFuerte.Count >= 3)
                            {
                                dato = 1;
                            }
                            using (var canvas2 = document.BeginPage(width, height))
                            {

                                canvas.Clear(SKColors.White);

                                var assembly2 = Assembly.GetExecutingAssembly();


                                using (var paint = new SKPaint())
                                {
                                    paint.TextSize = 14;
                                    paint.Color = SKColors.Black;
                                    SKColor celdaBackgroundColor = SKColors.Coral;
                                    SKColor celdaBackgroundColorB = SKColors.White;
                                    SKColor celdaBackgroundColorc = SKColors.Gold;
                                    SKColor celdaBorderColor = SKColors.Black;

                                    float x = 20;
                                    float y = 15;
                                    string[][] encabezadoTablaOrden2 = new string[][]
                                    {
                                 new string[] { "                                                            Imagenes del Vehículo  "},

                                    };


                                    float cellWidth14 = 578;

                                    foreach (var fila in encabezadoTablaOrden2)
                                    {
                                        x = 20;

                                        foreach (var valor in fila)
                                        {

                                            paint.Color = celdaBorderColor;
                                            canvas.DrawRect(new SKRect(x, y, x + cellWidth14, y + 30), paint);

                                            paint.Color = celdaBackgroundColorc;
                                            canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth14 - 1, y + 30 - 1), paint);

                                            paint.Color = SKColors.Black;
                                            canvas.DrawText(valor, x + 10, y + 20, paint);
                                            x += cellWidth14 + 10;
                                        }
                                        x = 20;

                                    }

                                    x = 50;
                                    y += 43;

                                    List<Imagen> grupo = imagenesGolpeFuerte.GetRange(0, Math.Min(tamanoGrupo, imagenesGolpeFuerte.Count));

                                    foreach (var imagen in grupo)
                                    {

                                        string[][] tablaImagen = new string[][]
                                        {
new string[] { "prueba.jpeg"},
                                        };
                                        float cellWidth4 = 284;
                                        float cellHeight = 225;

                                        foreach (var fila in tablaImagen)
                                        {
                                            x = 160;

                                            float maxCeldaHeight = 0;

                                            foreach (var valor in fila)
                                            {

                                                var lineas = valor.Split('\n');


                                                float celdaHeight = cellHeight * lineas.Length;

                                                maxCeldaHeight = Math.Max(maxCeldaHeight, celdaHeight);
                                            }

                                            foreach (var valor in fila)
                                            {

                                                var lineas = valor.Split('\n');


                                                paint.Color = celdaBorderColor;
                                                canvas.DrawRect(new SKRect(x, y, x + cellWidth4, y + maxCeldaHeight), paint);


                                                paint.Color = celdaBackgroundColorB;
                                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth4 - 1, y + maxCeldaHeight - 1), paint);


                                                float currentY = y;

                                                paint.Color = SKColors.Black;
                                                foreach (var linea in lineas)
                                                {
                                                    if (linea.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) || linea.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || linea.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
                                                    {

                                                        if (imagen.DatoImagen != null)
                                                        {
                                                            using (var imageStream = new MemoryStream(imagen.DatoImagen))
                                                            {
                                                                using (var bitmap = SKBitmap.Decode(imageStream))
                                                                {
                                                                    var imageWidth = cellWidth4 - 20;
                                                                    var imageHeight = maxCeldaHeight - 10;

                                                                    canvas.DrawBitmap(bitmap, new SKRect(x + 10, currentY + 5, x + 10 + imageWidth, currentY + 5 + imageHeight));
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        canvas.DrawText(linea, x + 10, currentY + 20, paint);
                                                    }

                                                    currentY += cellHeight;
                                                }

                                                x += cellWidth4 + 10;
                                            }

                                            x = 20;
                                            y += maxCeldaHeight + 0;
                                        }
                                        x = 20;
                                        y += 17;
                                    }
                                    imagenesGolpeFuerte.RemoveRange(0, Math.Min(tamanoGrupo, imagenesGolpeFuerte.Count)); imagenesGolpeFuerte.RemoveRange(0, Math.Min(tamanoGrupo, imagenesGolpeFuerte.Count));

                                    if (dato == 0)
                                    {
                                        x += 20;
                                        y += 15;



                                        string[][] encabezadoTablaUsuario = new string[][]
                                        {
                                            new string[] { "                                                               Datos del Cliente    "},

                                        };


                                        float cellWidtheu = 578;



                                        foreach (var fila in encabezadoTablaUsuario)
                                        {
                                            x = 20;

                                            foreach (var valor in fila)
                                            {


                                                paint.Color = celdaBorderColor;
                                                canvas2.DrawRect(new SKRect(x, y, x + cellWidtheu, y + 30), paint);

                                                paint.Color = celdaBackgroundColor;
                                                canvas2.DrawRect(new SKRect(x + 1, y + 1, x + cellWidtheu - 1, y + 30 - 1), paint);

                                                paint.Color = SKColors.Black;
                                                canvas2.DrawText(valor, x + 10, y + 20, paint);
                                                x += cellWidtheu + 10;
                                            }
                                            x = 20;
                                            y += 25;
                                        }


                                        x = 20;
                                        y += 15;
                                        string[][] primeraTablaDatosUsuarios = new string[][]
                                        {
                                            new string[] { "Nombre completo:", "Correo electronico:" },
                                            new string[] { $"{NombreCompleto}", $"{CorreoElectronico}"},
                                        };


                                        float cellWidth6 = 284;
                                        foreach (var fila in primeraTablaDatosUsuarios)
                                        {
                                            x = 20;

                                            foreach (var valor in fila)
                                            {

                                                paint.Color = celdaBorderColor;
                                                canvas2.DrawRect(new SKRect(x, y, x + cellWidth6, y + 30), paint);


                                                paint.Color = celdaBackgroundColorB;
                                                canvas2.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth6 - 1, y + 30 - 1), paint);


                                                paint.Color = SKColors.Black;
                                                canvas2.DrawText(valor, x + 10, y + 20, paint);
                                                x += cellWidth6 + 10;
                                            }
                                            x = 20;
                                            y += 25;
                                        }


                                        x = 20;
                                        y += 20;
                                        string[][] segundaTablaDatosUsuarios = new string[][]
                                        {
new string[] { "Telefono:", "Estado:","Municipio:" },
new string[] { $"{Telefono}", $"{Estado}", $"{Municipio}"}};


                                        float cellWidth7 = 186;

                                        foreach (var fila in segundaTablaDatosUsuarios)
                                        {
                                            x = 20;

                                            foreach (var valor in fila)
                                            {

                                                paint.Color = celdaBorderColor;
                                                canvas2.DrawRect(new SKRect(x, y, x + cellWidth7, y + 30), paint);


                                                paint.Color = celdaBackgroundColorB;
                                                canvas2.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth7 - 1, y + 30 - 1), paint);


                                                paint.Color = SKColors.Black;
                                                canvas2.DrawText(valor, x + 10, y + 20, paint);
                                                x += cellWidth7 + 10;
                                            }
                                            x = 20;
                                            y += 25;
                                        }
                                    }






                                }






                            }
                            document.EndPage();
                        }
                    }

                    if (dato == 1)
                    {
                        using (var canvas23 = document.BeginPage(width, height))
                        {

                            canvas23.Clear(SKColors.White);

                            var assembly22 = Assembly.GetExecutingAssembly();


                            using (var paint = new SKPaint())
                            {
                                paint.TextSize = 14;
                                paint.Color = SKColors.Black;
                                SKColor celdaBackgroundColor = SKColors.Coral;
                                SKColor celdaBackgroundColorB = SKColors.White;
                                SKColor celdaBackgroundColorc = SKColors.Gold;
                                SKColor celdaBorderColor = SKColors.Black;


                                float x = 20;
                                float y = 15;


                                string[][] encabezadoTablaUsuario = new string[][]
                                {
        new string[] { "                                                               Datos del Cliente    "},

                                };


                                float cellWidtheu = 578;



                                foreach (var fila in encabezadoTablaUsuario)
                                {
                                    x = 20;

                                    foreach (var valor in fila)
                                    {


                                        paint.Color = celdaBorderColor;
                                        canvas23.DrawRect(new SKRect(x, y, x + cellWidtheu, y + 30), paint);


                                        paint.Color = celdaBackgroundColor;
                                        canvas23.DrawRect(new SKRect(x + 1, y + 1, x + cellWidtheu - 1, y + 30 - 1), paint);


                                        paint.Color = SKColors.Black;
                                        canvas23.DrawText(valor, x + 10, y + 20, paint);
                                        x += cellWidtheu + 10;
                                    }
                                    x = 20;
                                    y += 25;
                                }

                                x = 20;
                                y += 15;
                                string[][] primeraTablaDatosUsuarios = new string[][]
                                {
        new string[] { "Nombre completo:", "Correo electronico:" },
        new string[] { $"{NombreCompleto}", $"{CorreoElectronico}"},
                                                    };


                                float cellWidth6 = 284;


                                foreach (var fila in primeraTablaDatosUsuarios)
                                {
                                    x = 20;

                                    foreach (var valor in fila)
                                    {

                                        paint.Color = celdaBorderColor;
                                        canvas23.DrawRect(new SKRect(x, y, x + cellWidth6, y + 30), paint);


                                        paint.Color = celdaBackgroundColorB;
                                        canvas23.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth6 - 1, y + 30 - 1), paint);


                                        paint.Color = SKColors.Black;
                                        canvas23.DrawText(valor, x + 10, y + 20, paint);
                                        x += cellWidth6 + 10;
                                    }
                                    x = 20;
                                    y += 25;
                                }


                                x += 20;
                                y += 20;


                                string[][] segundaTablaDatosUsuarios = new string[][]
                                {
        new string[] { "Telefono:", "Estado:","Municipio:" },
        new string[] { $"{Telefono}", $"{Estado}", $"{Municipio}"}};


                                float cellWidth7 = 186;


                                foreach (var fila in segundaTablaDatosUsuarios)
                                {
                                    x = 20;

                                    foreach (var valor in fila)
                                    {

                                        paint.Color = celdaBorderColor;
                                        canvas23.DrawRect(new SKRect(x, y, x + cellWidth7, y + 30), paint);


                                        paint.Color = celdaBackgroundColorB;
                                        canvas23.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth7 - 1, y + 30 - 1), paint);


                                        paint.Color = SKColors.Black;
                                        canvas23.DrawText(valor, x + 10, y + 20, paint);
                                        x += cellWidth7 + 10;
                                    }
                                    x = 20;
                                    y += 25;
                                }






                            }





                        }
                        document.EndPage();

                    }


                    document.Close();
                }


                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });
            }
        }
        public async void GenerarPDFSinDatos(string Folio, TallerDTO taller, String Fecha, String Hora, String NombreCompleto, String CorreoElectronico, String Telefono, String Estado, String Municipio)
        {
            var listaCadenas = contenido.Split('\n');
            int num = listaCadenas.Length;
            float pointsPerInch = 72;
            float width = 8.5f * pointsPerInch;
            float height = 11f * pointsPerInch;

            using (var stream = new SKFileWStream(filePath))
            using (var document = SKDocument.CreatePdf(stream))
            using (var canvas = document.BeginPage(width, height))
            {
                canvas.Clear(SKColors.White);


                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "AplicacionAuto.Resources.Images.logo.png";
                using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (resourceStream != null)
                    {
                        using (var bitmap = SKBitmap.Decode(resourceStream))
                        {
                            var image = SKImage.FromBitmap(bitmap);

                            var pdfWidth = 400;


                            float imageWidth = pdfWidth / 4.00f;
                            float imageHeight = bitmap.Height * (imageWidth / bitmap.Width);

                            float imageX = (pdfWidth - imageWidth) / 10;
                            float imageY = 0;


                            canvas.DrawImage(image, new SKRect(imageX, imageY, imageX + imageWidth, imageY + imageHeight));
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Image resource not found.");
                    }


                    using (var paint = new SKPaint())
                    {
                        paint.TextSize = 14;
                        paint.Color = SKColors.Black;
                        SKColor celdaBackgroundColor = SKColors.Coral;
                        SKColor celdaBackgroundColorB = SKColors.White;
                        SKColor celdaBackgroundColorc = SKColors.Gold;
                        SKColor celdaBorderColor = SKColors.Black;

                        float x = 20;
                        float y = 20;
                        string[][] encabezadoTablaOrden = new string[][]
                        {
                        new string[] { "                                        Orden de Admisión de Cotización "},

                        };


                        float cellWidtho = 450;
                        foreach (var fila in encabezadoTablaOrden)
                        {
                            x = 148;

                            foreach (var valor in fila)
                            {

                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidtho, y + 30), paint);

                                paint.Color = celdaBackgroundColorc;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidtho - 1, y + 30 - 1), paint);

                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidtho + 10;
                            }
                            x = 20;

                        }

                        x = 20;
                        y += 35;


                        string[][] TablaFolio = new string[][]
                        {
                        new string[] { $"Folio: {solicitudID}"}

                        };



                        float cellWidthf = 170;



                        foreach (var fila in TablaFolio)
                        {
                            x = 428;
                            foreach (var valor in fila)
                            {

                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidthf, y + 30), paint);


                                paint.Color = celdaBackgroundColorB;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidthf - 1, y + 30 - 1), paint);





                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidthf + 10;
                            }
                            x = 80;
                            y += 25;
                        }


                        x = 20;
                        y += 15;




                        string[][] encabezadoTablaServicio = new string[][]
                        {
                        new string[] { "                                                        Datos del Taller      "},

                        };

                        float cellWidthes = 578;


                        foreach (var fila in encabezadoTablaServicio)
                        {
                            x = 20;

                            foreach (var valor in fila)
                            {

                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidthes, y + 30), paint);

                                paint.Color = celdaBackgroundColor;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidthes - 1, y + 30 - 1), paint);

                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidthes + 10;
                            }
                            x = 20;
                            y += 25;
                        }





                        x = 20;
                        y += 15;
                        string[][] primeraTablaDatosServicio = new string[][]
                        {
                        new string[] { "Nombre del taller:", "Teléfono:", "Correo electrónico:"},
                        new string[] { $"{taller.Nombre}", $"{taller.Telefono}", $"{taller.CorreoElectronico}"},
                                            };


                        float cellWidth3 = 186;
                        foreach (var fila in primeraTablaDatosServicio)
                        {
                            x = 20;

                            foreach (var valor in fila)
                            {

                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidth3, y + 30), paint);


                                paint.Color = celdaBackgroundColorB;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth3 - 1, y + 30 - 1), paint);


                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidth3 + 10;
                            }
                            x = 20;
                            y += 25;
                        }


                        x = 20;
                        y += 15;
                        string[][] terceraTablaDatosServicio = new string[][]
                        {
                        new string[] { "Encargado:", "Fecha de la cita:"},
                        new string[] { $"{taller.Encargado}", $"{Fecha}"},
                                            };

                        cellWidth3 = 284;

                        foreach (var fila in terceraTablaDatosServicio)
                        {
                            x = 20;

                            foreach (var valor in fila)
                            {

                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidth3, y + 30), paint);


                                paint.Color = celdaBackgroundColorB;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth3 - 1, y + 30 - 1), paint);


                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidth3 + 10;
                            }
                            x = 20;
                            y += 25;
                        }


                        x = 20;
                        y += 15;
                        string[][] segundaTablaDatosServicio = new string[][]
                        {
                        new string[] { $"Dirección del Taller:" },
                        new string[] { $"{taller.Direccion}" },
                        };


                        float cellWidth4 = 578;
                        float cellHeight = 75;

                        foreach (var fila in segundaTablaDatosServicio)
                        {
                            x = 20;

                            foreach (var valor in fila)
                            {

                                var lineas = valor.Split('\n');


                                float celdaHeight = cellHeight * lineas.Length;


                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidth4, y + celdaHeight), paint);

                                paint.Color = celdaBackgroundColorB;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth4 - 1, y + celdaHeight - 1), paint);

                                paint.Color = SKColors.Black;
                                foreach (var linea in lineas)
                                {
                                    canvas.DrawText(linea, x + 10, y + 20, paint);
                                    y += cellHeight;
                                }

                                x += cellWidth4 + 10;
                                y -= cellHeight * lineas.Length;
                            }

                            x = 20;
                            y += 25;
                        }

                        x = 20;
                        y += 65;
                        string[][] encabezadoTablaUsuario = new string[][]
                        {
                        new string[] { "                                                           Datos del Cliente    "},

                        };


                        float cellWidtheu = 578;


                        foreach (var fila in encabezadoTablaUsuario)
                        {
                            x = 20;

                            foreach (var valor in fila)
                            {


                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidtheu, y + 30), paint);


                                paint.Color = celdaBackgroundColor;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidtheu - 1, y + 30 - 1), paint);


                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidtheu + 10;
                            }
                            x = 20;
                            y += 25;
                        }


                        x = 20;
                        y += 15;


                        string[][] primeraTablaDatosUsuarios = new string[][]
                        {
                        new string[] { "Nombre completo:", "Correo electronico" },
                        new string[] { $"{NombreCompleto}", $"{CorreoElectronico}"},
                                            };


                        float cellWidth6 = 284;
                        foreach (var fila in primeraTablaDatosUsuarios)
                        {
                            x = 20;

                            foreach (var valor in fila)
                            {

                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidth6, y + 30), paint);


                                paint.Color = celdaBackgroundColorB;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth6 - 1, y + 30 - 1), paint);


                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidth6 + 10;
                            }
                            x = 20;
                            y += 25;
                        }


                        x = 20;
                        y += 20;
                        string[][] segundaTablaDatosUsuarios = new string[][]
                        {
                        new string[] { "Telefono:", "Estado","Municipio" },
                        new string[] { $"{Telefono}", $"{Estado}",$"{Municipio}"},
                                            };


                        float cellWidth7 = 186;
                        foreach (var fila in segundaTablaDatosUsuarios)
                        {
                            x = 20;

                            foreach (var valor in fila)
                            {

                                paint.Color = celdaBorderColor;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidth7, y + 30), paint);


                                paint.Color = celdaBackgroundColorB;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth7 - 1, y + 30 - 1), paint);


                                paint.Color = SKColors.Black;
                                canvas.DrawText(valor, x + 10, y + 20, paint);
                                x += cellWidth7 + 10;
                            }
                            x = 20;
                            y += 25;
                        }

                        x = 10;
                        y = 715;
                        string[][] tablaPresupuesto2 = new string[][]
                        {
                       new string[] {
                                    "SE LE DARA MEJOR INFORMACION Y COSTEO AL ACUDIR A UNO DE NUESTROS TALLERES MAS CERCANOS O PUEDE\n COMUNICARSE CON NOSOTROS PARA MEJOR ASESORIA."}};


                        var cellWidth8 = 285;
                        cellHeight = 14;


                        foreach (var fila in tablaPresupuesto2)
                        {
                            paint.TextSize = 10;
                            x = 10;

                            foreach (var valor in fila)
                            {

                                var lineas = valor.Split('\n');


                                float celdaHeight = cellHeight * lineas.Length;


                                paint.Color = SKColors.Transparent;
                                canvas.DrawRect(new SKRect(x, y, x + cellWidth8, y + celdaHeight), paint);


                                paint.Color = celdaBackgroundColorB;
                                canvas.DrawRect(new SKRect(x + 1, y + 1, x + cellWidth8 - 1, y + celdaHeight - 1), paint);


                                paint.Color = SKColors.Black;
                                foreach (var linea in lineas)
                                {
                                    canvas.DrawText(linea, x + 10, y + 20, paint);
                                    y += cellHeight;
                                }

                                x += cellWidth8 + 10;
                                y -= cellHeight * lineas.Length;
                            }

                            x = 20;
                            y += 25;
                        }






                    }


                    document.EndPage();

                    document.Close();
                }


                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)

                });
            }
        }

        int? usuarioID;
        int? cocheID;
        int? acabadoID;
        int? colorID;
        int? tipoServicioPaqueteID;
        int? prioridadID;
        int? tipoGolpeID;
        public string fechaEmision;
        public String fechaCita;
        public String horaCita;
        public Double presupuesto;
        List<SolicitudDTO> solicituds;
        int? solicitudID;
        List<int> DatosPiezaIDs;
        List<DatosPiezaDTO> datosPiezas;
        int? datosPiezaID;
        List<SolicitudDatosPiezaDTO> solicitudDatosPiezas;
        public int crearSolicitud(List<UsuarioDTO> listaUsuarioDTO, List<CocheDTO> listaCocheDTO, List<Acabado> listaAcabado, List<ColorDTO> listaColorDTO, List<TipoServicioPaqueteDTO> listaTipoServicioPaqueteDTO, int? tallerID, List<PrioridadDTO> listaPrioridadDTO, List<TipoGolpeDTO> listaTipoGolpeDTO, String fechaCita, String horaCita, Double presupuesto, List<MedidadDTO> listaMedidadDTO)
        {

            foreach (var usuario in listaUsuarioDTO)
            {
                usuarioID = usuario.ID;
            }
            foreach (var coche in listaCocheDTO)
            {
                cocheID = coche.ID;
            }
            foreach (var acabado in listaAcabado)
            {
                acabadoID = acabado.ID;
            }

            foreach (var color in listaColorDTO)
            {
                colorID = color.ID;
            }

            foreach (var tipoServicioPaquete in listaTipoServicioPaqueteDTO)
            {
                tipoServicioPaqueteID = tipoServicioPaquete.ID;
            }

            foreach (var prioridad in listaPrioridadDTO)
            {
                prioridadID = prioridad.ID;
            }
            foreach (var tipoGolpe in listaTipoGolpeDTO)
            {
                tipoGolpeID = tipoGolpe.ID;
            }


            DateTime ahora = DateTime.Now;


            string nombreMes = ahora.ToString("MMMM", new System.Globalization.CultureInfo("es-ES"));


            string formatoPersonalizado = $"{ahora.Day}/{nombreMes}/{ahora.Year} a las {ahora.ToString("hh:mm tt")}";
            fechaEmision = formatoPersonalizado;


            if (usuarioID == 0)
            {
                usuarioID = null;
            }
            if (cocheID == 0)
            {
                cocheID = null;
            }
            if (acabadoID == 0)
            {
                acabadoID = null;
            }
            if (colorID == 0)
            {
                colorID = null;
            }
            if (tipoServicioPaqueteID == 0)
            {
                tipoServicioPaqueteID = null;
            }
            if (tallerID == 0)
            {
                tallerID = null;
            }
            if (prioridadID == 0)
            {
                prioridadID = null;
            }
            if (tipoGolpeID == 0)
            {
                tipoGolpeID = null;
            }

            SolicitudDTO solicitud = new SolicitudDTO()
            {
                UsuarioID = usuarioID,
                CocheID = cocheID,
                AcabadoID = acabadoID,
                ColorID = colorID,
                TipoServicioPaqueteID = tipoServicioPaqueteID,
                TallerID = tallerID,
                PrioridadID = prioridadID,
                TipoGolpeID = tipoGolpeID,
                FechaEmision = formatoPersonalizado,
                FechaCita = fechaCita,
                HoraCita = horaCita,
                Presupuesto = presupuesto



            };

            String jsonRecibir = null;
            String jsonEnviar = JsonConvertidor.Objeto_Json(solicitud);
            peticion.PedirComunicacion("Solicitud/Agregar", MetodoHTTP.POST, TipoContenido.JSON, Preferences.Get("token", ""));
            peticion.enviarDatos(jsonEnviar);
            jsonRecibir = peticion.ObtenerJson();

            peticion.PedirComunicacion("Solicitud/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Get("token", ""));
            String jsonRecibir3 = peticion.ObtenerJson();
            solicituds = JsonConvertidor.Json_ListaObjeto<SolicitudDTO>(jsonRecibir3);
            if (tipoServicioPaqueteID != null && horaCita != null)
            {

            }
            var objetoObtenidoSolicitud = solicituds.FindAll(x => x.UsuarioID == solicitud.UsuarioID && x.CocheID == solicitud.CocheID && x.AcabadoID == solicitud.AcabadoID && x.ColorID == solicitud.ColorID && x.TipoServicioPaqueteID == solicitud.TipoServicioPaqueteID && x.TallerID == solicitud.TallerID && x.PrioridadID == solicitud.PrioridadID && x.TipoGolpeID == solicitud.TipoGolpeID && x.FechaEmision == solicitud.FechaEmision && x.FechaCita == solicitud.FechaCita && x.Presupuesto == solicitud.Presupuesto);


            List<SolicitudDTO> ListaSolicituds = objetoObtenidoSolicitud.Select(u => new SolicitudDTO
            {
                ID = u.ID,
                UsuarioID = u.UsuarioID,
                CocheID = u.CocheID,
                AcabadoID = u.AcabadoID,
                ColorID = u.ColorID,
                TipoServicioPaqueteID = u.TipoServicioPaqueteID,
                TallerID = u.TallerID,
                PrioridadID = u.PrioridadID,
                TipoGolpeID = u.TipoGolpeID,
                FechaEmision = u.FechaEmision,
                FechaCita = u.FechaCita,
                HoraCita = u.HoraCita,
                Presupuesto = u.Presupuesto
            }).ToList();

            foreach (var soli in ListaSolicituds)
            {
                solicitudID = soli.ID;
            }

            foreach (var datosPieza in listaMedidadDTO)
            {

                DatosPiezaDTO datosPiezaDTO = new DatosPiezaDTO
                {
                    PiezaID = datosPieza.PiezaID,
                    ImagenID = datosPieza.ImagenID,
                    DiametroH = datosPieza.MedidaH,
                    DiametroV = datosPieza.MedidaV,
                    Profundidad = datosPieza.profundidad


                };
                String jsonRecibir2 = null;
                String jsonEnviar2 = JsonConvertidor.Objeto_Json(datosPiezaDTO);
                peticion.PedirComunicacion("DatosPieza/Agregar", MetodoHTTP.POST, TipoContenido.JSON, Preferences.Get("token", ""));
                peticion.enviarDatos(jsonEnviar2);
                jsonRecibir2 = peticion.ObtenerJson();

                peticion.PedirComunicacion("DatosPieza/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Get("token", ""));
                String jsonRecibir31 = peticion.ObtenerJson();
                datosPiezas = JsonConvertidor.Json_ListaObjeto<DatosPiezaDTO>(jsonRecibir31);

                var objetoObtenidoDatoPieza = datosPiezas.FindAll(x => x.PiezaID == datosPieza.PiezaID && x.ImagenID == datosPieza.ImagenID && x.DiametroH == datosPieza.MedidaH && x.DiametroV == datosPieza.MedidaV && x.Profundidad == datosPieza.profundidad);

                List<DatosPiezaDTO> listaDatosPieza = objetoObtenidoDatoPieza.Select(u => new DatosPiezaDTO
                {
                    ID = u.ID,
                    PiezaID = u.PiezaID,
                    ImagenID = u.ImagenID,
                    DiametroH = u.DiametroH,
                    DiametroV = u.DiametroV,
                    Profundidad = u.Profundidad
                }).ToList();

                foreach (var item in listaDatosPieza)
                {
                    datosPiezaID = item.ID;
                }

                SolicitudDatosPiezaDTO solicitudDatosPieza = new SolicitudDatosPiezaDTO
                {
                    DatosPiezaID = datosPiezaID,
                    SolicitudID = solicitudID
                };

                String jsonRecibir4 = null;
                String jsonEnviar3 = JsonConvertidor.Objeto_Json(solicitudDatosPieza);
                peticion.PedirComunicacion("SolicitudDatosPieza/Agregar", MetodoHTTP.POST, TipoContenido.JSON, Preferences.Get("token", ""));
                peticion.enviarDatos(jsonEnviar3);
                jsonRecibir4 = peticion.ObtenerJson();


                peticion.PedirComunicacion("SolicitudDatosPieza/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Get("token", ""));
                String jsonRecibir5 = peticion.ObtenerJson();
                solicitudDatosPiezas = JsonConvertidor.Json_ListaObjeto<SolicitudDatosPiezaDTO>(jsonRecibir5);

                var objetoObtenidoSolicitudDatoPieza = solicitudDatosPiezas.FindAll(x => x.DatosPiezaID == datosPiezaID && x.SolicitudID == solicitudID);

                return (int)solicitudID;

            }


            return 0;

        }
    }
}
