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

            // Converte em objeto
            Config config = JsonConvert.DeserializeObject<Config>(json);

            // Usa os valores do JSON.
            Console.WriteLine("Origem: " + config.Origem);
            Console.WriteLine("Destino: " + config.Destino);

            // Verificação da origem.
            if (!Directory.Exists(config.Origem))
            {
                Console.WriteLine("Diretório de origem não encontrado: " + config.Origem);
                return;
            }
            else
            {
                Console.WriteLine("Diretório de origem encontrado!");
            }

            // Verificação do destino.
            if (!Directory.Exists(config.Destino))
            {
                Console.WriteLine("Diretório de destino não existe. Criando...");
                Directory.CreateDirectory(config.Destino);
                Console.WriteLine("Diretório de destino criado: " + config.Destino);
            }
            else
            {
                Console.WriteLine("Diretório de destino já existe!");
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

            Console.WriteLine("Processo concluído!");
        }
    }
}
