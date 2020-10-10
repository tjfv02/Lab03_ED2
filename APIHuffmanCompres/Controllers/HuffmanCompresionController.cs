using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CompresionHuffman;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIHuffmanCompres.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HuffmanCompresionController : ControllerBase
    {
        
        public static IWebHostEnvironment _environment;
        public HuffmanCompresionController(IWebHostEnvironment env)
        {
            _environment = env;
        }

       
        //-------- Metodos Huffman --------------------
        Compresion ComprimirHuffman = new Compresion();

        public void DescomprimirArchivos(IFormFile objFile)
        {
            string[] FileName1 = objFile.FileName.Split(".");
            ComprimirHuffman.Descomprimir(_environment.ContentRootPath + "\\ArchivosCompresos\\" + objFile.FileName, _environment.ContentRootPath + "\\ArchivosCompresos\\" + FileName1[0] + ".txt");

        }

        public void ComprimirArchivos(IFormFile objFile, string id)
        {
            string[] FileName1 = objFile.FileName.Split(".");
            ComprimirHuffman.Comprimir(_environment.ContentRootPath + "\\ArchivosCompresos\\" + objFile.FileName, _environment.ContentRootPath + "\\ArchivosCompresos\\" + id + ".huff", FileName1[0], _environment.ContentRootPath + "\\ArchivosCompresos\\" + "Compresiones.txt");

        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------

        [Route("/api/compressions")]
        [HttpGet]
        public  IEnumerable<MisCompresiones> DescargarCompresiones()
        {

            var memory = new MemoryStream();
            MisCompresiones xd = new MisCompresiones();
            List<MisCompresiones> ListaChida = new List<MisCompresiones>();
            string[] split;
            try
            {
            using (var stream = new StreamReader(_environment.ContentRootPath + "\\ArchivosCompresos\\" + "Compresiones.txt"))
            {
                
                while (!stream.EndOfStream)
                {
                    
                   split = stream.ReadLine().Split(",");
                   xd.nombreOriginal = split[0];
                   xd.razonDeCompresion = Convert.ToDouble(split[1]);
                   xd.factorDeCompresion = Convert.ToDouble(split[2]);
                   xd.porcentajeDeCompresion = Convert.ToDouble(split[3]);
                   xd.RutaO = (split[4]);

                    ListaChida.Add(xd);
                    

                    
                }
            }

                return ListaChida;

            }
            catch (Exception)
            {

                return ListaChida;
            }



           
        }


        [Route("/api/compress/{id}")]
        [HttpPost]
        public async Task<IActionResult> SubirFileTxt([FromForm] IFormFile objFile, string id)
        {
            try
            {
                if (objFile.Length > 0)
                {
                    if (!Directory.Exists(_environment.ContentRootPath + "\\ArchivosCompresos\\")) Directory.CreateDirectory(_environment.ContentRootPath + "\\ArchivosCompresos\\");
                    using var _fileStream = System.IO.File.Create(_environment.ContentRootPath + "\\ArchivosCompresos\\" + objFile.FileName);
                    objFile.CopyTo(_fileStream);
                    _fileStream.Flush();
                    _fileStream.Close();

                    ComprimirArchivos(objFile, id);
                    var memory = new MemoryStream();

                    using (var stream = new FileStream(_environment.ContentRootPath + "\\ArchivosCompresos\\" + id + ".huff", FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }

                    memory.Position = 0;
                    return File(memory, System.Net.Mime.MediaTypeNames.Application.Octet, id + ".huff");
                }

                return StatusCode(404, "Archivo Vacio");

            }
            catch
            {

                return StatusCode(404, "Error");
            }
        }

        //
        [Route("/api/decompress")]
        [HttpPost]
        public async Task<IActionResult> SubirFileHuff([FromForm] IFormFile objFile)
        {
            try
            {
                if (objFile.Length > 0)
                {
                    if (!Directory.Exists(_environment.ContentRootPath + "\\ArchivosCompresos\\")) Directory.CreateDirectory(_environment.ContentRootPath + "\\ArchivosCompresos\\");
                    using var _fileStream = System.IO.File.Create(_environment.ContentRootPath + "\\ArchivosCompresos\\" + objFile.FileName);
                    objFile.CopyTo(_fileStream);
                    _fileStream.Flush();
                    _fileStream.Close();
                    DescomprimirArchivos(objFile);

                    var memory = new MemoryStream();
                    var name = objFile.FileName;
                    using (var stream = new FileStream(_environment.ContentRootPath + "\\ArchivosCompresos\\" + objFile.FileName , FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }

                    memory.Position = 0;
                    return File(memory, System.Net.Mime.MediaTypeNames.Application.Octet, objFile.FileName /*+ ".txt"*/);
                }
                else
                {
                    return StatusCode(404, "Archivo Vacio");
                }
            }
            catch
            {
                return StatusCode(404, "Error");
            }
        }



       
    }
}
