using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Movedor_de_Ficheiros.Models;
using Movedor_de_Ficheiros.Service;
using System.IO;
using System.Xml.Linq;

var builder = Host.CreateApplicationBuilder(args);

// Config: appsettings.json + variáveis de ambiente + CLI
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables(prefix: "FILEMOVER_"); // ex.: FILEMOVER_FILEMOVER__ORIGEM
// Também aceita linha de comando: --FileMover:Origem=C:\...

// Options + validação
builder.Services
    .AddOptions<MoveOptions>()
    .Bind(builder.Configuration.GetSection("FileMover"))
    .Validate(o => !string.IsNullOrWhiteSpace(o.Origem), "Origem é obrigatória.")
    .Validate(o => !string.IsNullOrWhiteSpace(o.Destino), "Destino é obrigatório.")
    .ValidateOnStart();

builder.Services.AddSingleton<FileMoverService>();

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("FileMover");

try
{
    var svc = app.Services.GetRequiredService<FileMoverService>();
    await svc.RunAsync();
}
catch (OptionsValidationException ex)
{
    logger.LogError(ex, "Configuração inválida.");
}
catch (Exception ex)
{
    logger.LogError(ex, "Erro inesperado.");
}


//using System;
//using System.IO;
//using Movedor_de_Ficheiros;
//using Newtonsoft.Json;

//namespace FileMover
//{
//    class Programa
//    {
//        static void Main(string[] args)
//        {
//            string caminhoConfig = "C:\\Mr Robot\\Programming\\Movedor de Ficheiros\\Movedor de Ficheiros\\Movedor.json";

//            // Lê o ficheiro.
//            string json = File.ReadAllText(caminhoConfig);

//            // Linha de código que converte em objeto
//            Config config = JsonConvert.DeserializeObject<Config>(json);

//            // Usa os valores do JSON.
//            Console.WriteLine("Origem: " + config.Origem);
//            Console.WriteLine("Destino: " + config.Destino);

//            // Verifica a origem da origem da pasta.
//            if (!Directory.Exists(config.Origem))
//            {
//                Console.WriteLine("Infelizmente o diretório de origem não foi encontrado: " + config.Origem);
//                return;
//            }
//            else
//            {
//                Console.WriteLine("O diretório de origem foi encontrado encontrado!");
//            }

//            // Verifica se o destino existe.
//            if (!Directory.Exists(config.Destino))
//            {
//                Console.WriteLine("O diretório de destino não existe. Criarei o destino solicitado!");
//                Directory.CreateDirectory(config.Destino);
//                Console.WriteLine("O diretório de destino foi criado: " + config.Destino);
//            }
//            else
//            {
//                Console.WriteLine("Este diretório de destino já foi criado!");
//            }

//            // Parte responsável por mover os ficheiros.
//            foreach (var dir in Directory.GetDirectories(config.Origem))
//            {
//                string nome = Path.GetFileName(dir);
//                string destinoFinal = Path.Combine(config.Destino, nome);

//                try
//                {
//                    Directory.Move(dir, destinoFinal);
//                    Console.WriteLine($"Movido: {nome}");
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"Erro ao mover {nome}: {ex.Message}");
//                }
//            }

//            Console.WriteLine("Ficheiros movidos com sucesso!");
//        }
//    }
//}
