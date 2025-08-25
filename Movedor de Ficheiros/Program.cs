using System;
using System.IO;
using Newtonsoft.Json;

namespace NomeQualquer
{
    namespace NomeQualquer
    {
        class Programa
        {
            static void Main(string[] args)
            {
                //Definição dos diretorios de origem e destino.
                string origem = @"D:\Origem";
                string destino = @"D:\Destino";

                //Parte responsável por mover os ficheiros.
                foreach (var dir in Directory.GetDirectories(origem))
                {
                    string nome = Path.GetFileName(dir);
                    string destinoFinal = Path.Combine(destino, nome);

                    Directory.Move(dir, destinoFinal);
                    Console.WriteLine("Ficheiros movidos com sucesso!");
                }
            }
        }
    }
}