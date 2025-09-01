using System;
using System.IO;
using Movedor_de_Ficheiros;
using Newtonsoft.Json;

namespace FileMover
{
    class Programa
    {
        static void Main(string[] args)
        {
            string caminhoConfig = "C:\\Mr Robot\\Programming\\Movedor de Ficheiros\\Movedor de Ficheiros\\Movedor.json";

            // Lê o ficheiro.
            string json = File.ReadAllText(caminhoConfig);

            // Linha de código que converte em objeto
            Config config = JsonConvert.DeserializeObject<Config>(json);

            // Usa os valores do JSON.
            Console.WriteLine("Origem: " + config.Origem);
            Console.WriteLine("Destino: " + config.Destino);

            // Verifica a origem da origem da pasta.
            if (!Directory.Exists(config.Origem))
            {
                Console.WriteLine("Infelizmente o diretório de origem não foi encontrado: " + config.Origem);
                return;
            }
            else
            {
                Console.WriteLine("O diretório de origem foi encontrado encontrado!");
            }

            // Verifica se o destino existe.
            if (!Directory.Exists(config.Destino))
            {
                Console.WriteLine("O diretório de destino não existe. Criarei o destino solicitado!");
                Directory.CreateDirectory(config.Destino);
                Console.WriteLine("O diretório de destino foi criado: " + config.Destino);
            }
            else
            {
                Console.WriteLine("Este diretório de destino já foi criado!");
            }

            // Parte responsável por mover os ficheiros.
            foreach (var dir in Directory.GetDirectories(config.Origem))
            {
                string nome = Path.GetFileName(dir);
                string destinoFinal = Path.Combine(config.Destino, nome);

                try
                {
                    Directory.Move(dir, destinoFinal);
                    Console.WriteLine($"Movido: {nome}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao mover {nome}: {ex.Message}");
                }
            }

            Console.WriteLine("Ficheiro movido com sucesso!");
        }
    }
}
