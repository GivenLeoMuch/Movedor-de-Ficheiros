using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Movedor_de_Ficheiros.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movedor_de_Ficheiros.Service;

public sealed class FileMoverService
{
    private readonly MoveOptions _opt;
    private readonly ILogger<FileMoverService> _log;

    public FileMoverService(IOptions<MoveOptions> opt, ILogger<FileMoverService> log)
    {
        _opt = opt.Value;
        _log = log;
    }

    public Task RunAsync()
    {
        var source = Path.GetFullPath(_opt.Origem);
        var targetRoot = Path.GetFullPath(_opt.Destino);

        _log.LogInformation("Origem: {src}", source);
        _log.LogInformation("Destino: {dst}", targetRoot);

        if (!Directory.Exists(source))
        {
            _log.LogError("Diretório de origem inexistente: {src}", source);
            return Task.CompletedTask;
        }

        Directory.CreateDirectory(targetRoot);

        var search = _opt.IncludeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        int movedDirs = 0, movedFiles = 0, skipped = 0, errors = 0;

        // 1) Se for apenas 1º nível, podemos mover pastas inteiras com Directory.Move
        if (!_opt.IncludeSubdirectories)
        {
            foreach (var dir in Directory.EnumerateDirectories(source, "*", SearchOption.TopDirectoryOnly))
            {
                var name = Path.GetFileName(dir);
                var target = Path.Combine(targetRoot, name);

                try
                {
                    if (Directory.Exists(target))
                    {
                        if (_opt.Overwrite)
                        {
                            _log.LogInformation("Destino já existe, apagando pasta: {t}", target);
                            if (!_opt.DryRun) Directory.Delete(target, recursive: true);
                        }
                        else
                        {
                            _log.LogWarning("A saltar pasta '{name}' (destino existe). Ative Overwrite para substituir.", name);
                            skipped++;
                            continue;
                        }
                    }

                    if (_opt.DryRun)
                    {
                        _log.LogInformation("[DRY RUN] Moveria pasta: {name}", name);
                        movedDirs++;
                        continue;
                    }

                    Directory.Move(dir, target);
                    _log.LogInformation("Pasta movida: {name}", name);
                    movedDirs++;
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "Erro ao mover pasta '{name}'", name);
                    errors++;
                }
            }
        }

        // 2) Mover ficheiros (recursivo ou só 1º nível, conforme IncludeSubdirectories)
        var files = Directory.EnumerateFiles(source, "*.*", search);

        foreach (var file in files)
        {
            var relative = Path.GetRelativePath(source, file);
            var target = Path.Combine(targetRoot, relative);

            try
            {
                // Garante diretório de destino (preserva estrutura quando recursivo)
                var dirName = Path.GetDirectoryName(target);
                if (!string.IsNullOrEmpty(dirName))
                {
                    if (!_opt.DryRun) Directory.CreateDirectory(dirName);
                }

                if (File.Exists(target))
                {
                    if (_opt.Overwrite)
                    {
                        _log.LogInformation("Destino já existe, substituindo ficheiro: {t}", target);
                        if (!_opt.DryRun) File.Delete(target);
                    }
                    else
                    {
                        _log.LogWarning("A saltar '{rel}' (ficheiro já existe). Ative Overwrite para substituir.", relative);
                        skipped++;
                        continue;
                    }
                }

                if (_opt.DryRun)
                {
                    _log.LogInformation("[DRY RUN] Moveria ficheiro: {rel}", relative);
                    movedFiles++;
                    continue;
                }

                File.Move(file, target);
                _log.LogInformation("Ficheiro movido: {rel}", relative);
                movedFiles++;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Erro ao mover ficheiro '{rel}'", relative);
                errors++;
            }
        }

        _log.LogInformation("Concluído: {dirs} pastas e {files} ficheiros movidos. Skipped={skipped}, Errors={errors}",
            movedDirs, movedFiles, skipped, errors);

        return Task.CompletedTask;
    }
}

//public sealed class FileMoverService
//{
//    private readonly MoveOptions _opt;
//    private readonly ILogger<FileMoverService> _log;

//    public FileMoverService(IOptions<MoveOptions> opt, ILogger<FileMoverService> log)
//    {
//        _opt = opt.Value;
//        _log = log;
//    }

//    public Task RunAsync()
//    {
//        var source = Path.GetFullPath(_opt.Origem);
//        var targetRoot = Path.GetFullPath(_opt.Destino);

//        _log.LogInformation("Origem: {src}", source);
//        _log.LogInformation("Destino: {dst}", targetRoot);

//        if (!Directory.Exists(source))
//        {
//            _log.LogError("Diretório de origem inexistente: {src}", source);
//            return Task.CompletedTask;
//        }

//        Directory.CreateDirectory(targetRoot);

//        var search = _opt.IncludeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
//        var dirs = Directory.EnumerateDirectories(source, "*", search);

//        foreach (var dir in dirs)
//        {
//            // Se AllDirectories, preserva a estrutura relativa
//            var relative = Path.GetRelativePath(source, dir);
//            var target = Path.Combine(targetRoot, relative);

//            try
//            {
//                if (Directory.Exists(target))
//                {
//                    if (_opt.Overwrite)
//                    {
//                        _log.LogInformation("Destino já existe, apagando: {t}", target);
//                        if (!_opt.DryRun) Directory.Delete(target, recursive: true);
//                    }
//                    else
//                    {
//                        _log.LogWarning("Pulando '{rel}' (destino existe). Ative Overwrite para substituir.", relative);
//                        continue;
//                    }
//                }

//                if (_opt.DryRun)
//                {
//                    _log.LogInformation("[DRY RUN] Moveria: {rel}", relative);
//                    continue;
//                }

//                // Garante diretório pai quando preserva subpastas
//                Directory.CreateDirectory(Path.GetDirectoryName(target)!);

//                Directory.Move(dir, target);
//                _log.LogInformation("Movido: {rel}", relative);

//            }
//            catch (Exception ex)
//            {
//                _log.LogError(ex, "Erro ao mover '{rel}'", relative);
//            }
//        }

//        _log.LogInformation("Processo concluído.");
//        return Task.CompletedTask;
//    }
//}