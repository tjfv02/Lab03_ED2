using System;
using CompresionHuffman;


namespace ConsolaDePruebasHuff
{
    class Program
    {
        static void Main(string[] args)
        {
            Compresion huffman = new Compresion();

            huffman.Comprimir("cuento.txt", "compresion.txt","cuento", "factorescompresion.txt");
            huffman.Descomprimir("compresion.txt", "descompresion.txt");
        



        }
    }
}
