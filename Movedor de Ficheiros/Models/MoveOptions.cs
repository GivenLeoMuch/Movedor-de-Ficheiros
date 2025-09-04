using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movedor_de_Ficheiros.Models;

public sealed class MoveOptions
{
    public string Origem { get; set; } = "";
    public string Destino { get; set; } = "";
    public bool IncludeSubdirectories { get; set; } = false; // move apenas pastas do 1º nível por padrão
    public bool DryRun { get; set; } = false;                // só simula (não move)
    public bool Overwrite { get; set; } = false;             // se true, apaga destino existente
}
