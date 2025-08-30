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

            // Lê o ficheiro
            string json = File.ReadAllText(caminhoConfig);

            // Converte em objeto
            Config config = JsonConvert.DeserializeObject<Config>(json);

            // Usa os valores do JSON
            Console.WriteLine("Origem: " + config.Origem);
            Console.WriteLine("Destino: " + config.Destino);


                //Parte responsável por mover os ficheiros.
                foreach (var dir in Directory.GetDirectories(config.Origem))
                {
                    string nome = Path.GetFileName(dir);
                    string destinoFinal = Path.Combine(config.Destino, nome);

                    Directory.Move(dir, destinoFinal);
                    Console.WriteLine("Ficheiros movidos com sucesso!");
                }
            }
        }
    }
