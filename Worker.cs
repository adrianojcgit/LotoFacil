using Lotofacil._2_Core;
using Lotofacil.Servico;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Collections.Generic;

namespace Lotofacil
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        string fileName = "";
        string sourcePath = "";
        string targetPath = "";
        string sourceFile = "";
        string destFile = "";

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //while (!stoppingToken.IsCancellationRequested)
            //{
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                Console.WriteLine("Iniciando a leitura de dados da planilha excel!");
                
                Thread t2 = new(new ThreadStart(AdicionarFilaRabbitMQ));
                t2.Start();
                
                //await Task.Delay(90000, stoppingToken);
            //}

        }

        private void AdicionarFilaRabbitMQ()
        {

            try
            {
                Console.WriteLine("Lendo parâmetros do arquivo!");
                fileName = _configuration.GetSection("Arquivos").GetSection("Arquivo").Value;
                sourcePath = _configuration.GetSection("Arquivos").GetSection("ArquivoOrigem").Value;
                targetPath = _configuration.GetSection("Arquivos").GetSection("ArquivoDestino").Value;
                sourceFile = Path.Combine(sourcePath, fileName);
                destFile = Path.Combine(targetPath, fileName);
                Console.WriteLine("Fim da leitura de parâmetros do arquivo!");
                if (Directory.Exists(sourcePath))
                {
                    if (File.Exists(sourceFile))
                    {
                        var loto = LotofacilServico.ReadXls(fileName, sourcePath);
                        var resultLot = SetarParametros(loto);
                        CriarPlanilhaExcel(resultLot, destFile);
                        AbrePlanilhaExcel(destFile);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void CriarPlanilhaExcel(List<LotoFacilDto> loto, string arqDestino)
        {
            Console.WriteLine("Iniciando a criação da Planilha Excel!");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage excelPackage = new ExcelPackage();
            var workSheet = excelPackage.Workbook.Worksheets.Add("LotoFacil_14");
            workSheet.TabColor = Color.Black;
            workSheet.DefaultRowHeight = 12;
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;
            workSheet.Cells[1, 1].Value = "Concurso";
            workSheet.Cells[1, 2].Value = "Data";
            workSheet.Cells[1, 3].Value = "Bola01";
            workSheet.Cells[1, 4].Value = "Bola02";
            workSheet.Cells[1, 5].Value = "Bola03";
            workSheet.Cells[1, 6].Value = "Bola04";
            workSheet.Cells[1, 7].Value = "Bola05";
            workSheet.Cells[1, 8].Value = "Bola06";
            workSheet.Cells[1, 9].Value = "Bola07";
            workSheet.Cells[1, 10].Value = "Bola08";
            workSheet.Cells[1, 11].Value = "Bola09";
            workSheet.Cells[1, 12].Value = "Bola10";
            workSheet.Cells[1, 13].Value = "Bola11";
            workSheet.Cells[1, 14].Value = "Bola12";
            workSheet.Cells[1, 15].Value = "Bola13";
            workSheet.Cells[1, 16].Value = "Bola14";
            workSheet.Cells[1, 17].Value = "Bola15";
            workSheet.Cells[1, 18].Value = "Concurso Recorrente";
            workSheet.Cells[1, 19].Value = "Acerto";

            var indice = 2;
            foreach(var item in loto)
            {
                workSheet.Cells[indice, 1].Value = item.Concurso;
                workSheet.Cells[indice, 2].Value = item.Data;
                workSheet.Cells[indice, 3].Value = item.Bola01;
                workSheet.Cells[indice, 4].Value = item.Bola02;
                workSheet.Cells[indice, 5].Value = item.Bola03;
                workSheet.Cells[indice, 6].Value = item.Bola04;
                workSheet.Cells[indice, 7].Value = item.Bola05;
                workSheet.Cells[indice, 8].Value = item.Bola06;
                workSheet.Cells[indice, 9].Value = item.Bola07;
                workSheet.Cells[indice, 10].Value = item.Bola08;
                workSheet.Cells[indice, 11].Value = item.Bola09;
                workSheet.Cells[indice, 12].Value = item.Bola10;
                workSheet.Cells[indice, 13].Value = item.Bola11;
                workSheet.Cells[indice, 14].Value = item.Bola12;
                workSheet.Cells[indice, 15].Value = item.Bola13;
                workSheet.Cells[indice, 16].Value = item.Bola14;
                workSheet.Cells[indice, 17].Value = item.Bola15;
                workSheet.Cells[indice, 18].Value = item.ConcursoItem;
                workSheet.Cells[indice, 19].Value = item.Acerto;
                indice++;
            }

            for(int i = 1; i <= 19; i++)
                workSheet.Column(i).AutoFit();

            if(File.Exists(arqDestino))
                File.Delete(arqDestino);

            FileStream fileStream = File.Create(arqDestino); ;
            fileStream.Close();

            File.WriteAllBytes(arqDestino, excelPackage.GetAsByteArray());
            excelPackage.Dispose();
            Console.WriteLine("Fim da criação da Planilha Excel!");

        }

        private static void AbrePlanilhaExcel(string arqDestino)
        {
            Console.WriteLine("Iniciando a abertura da Planilha Excel!");
            var arquivoExcel = new ExcelPackage(new FileInfo(arqDestino));
            ExcelWorksheet planilha = arquivoExcel.Workbook.Worksheets.FirstOrDefault();
            int rows = planilha.Dimension.Rows;
            int cols = planilha.Dimension.Columns;
            for(int i = 1;i <= rows;i++)
            {
                for(int j = 1; j <= cols; j++)
                {
                    string conteudo = planilha.Cells[i,j].Value.ToString();

                }
            }
            Console.WriteLine("Fim da abertura da Planilha Excel!");
        }

        private List<LotoFacilDto> SetarParametros(LotofacilBaseDto lotofacil)
        {
            try
            {
                List<LotoFacilDto> listLoto = new List<LotoFacilDto>();
                int totalLinhas = lotofacil.LotoFacilDtos.Count;
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                for (int item = 0; item <= lotofacil.LotoFacilDtos.Count -1; item++)
                {
                    Console.WriteLine($"Lendo concurso {lotofacil.LotoFacilDtos[item].Concurso} - {item} de {totalLinhas} apostas");
                    //Console.WriteLine("Aguare...");
                    //Thread.Sleep(500);
                    var contaAcertos = CompararNumerosSorteados(lotofacil.LotoFacilDtos[item], lotofacil);

                    LotoFacilDto lotoFacilDto = new LotoFacilDto();

                    foreach (var item3 in contaAcertos)
                    {
                        if (item3.Concurso == lotofacil.LotoFacilDtos[item].Concurso)
                        {                           
                            lotoFacilDto.Bola01 = item3.Bola01;
                            lotoFacilDto.Bola02 = item3.Bola02;
                            lotoFacilDto.Bola03 = item3.Bola03;
                            lotoFacilDto.Bola04 = item3.Bola04;
                            lotoFacilDto.Bola05 = item3.Bola05;
                            lotoFacilDto.Bola06 = item3.Bola06;
                            lotoFacilDto.Bola07 = item3.Bola07;
                            lotoFacilDto.Bola08 = item3.Bola08;
                            lotoFacilDto.Bola09 = item3.Bola09;
                            lotoFacilDto.Bola10 = item3.Bola10;
                            lotoFacilDto.Bola11 = item3.Bola11;
                            lotoFacilDto.Bola12 = item3.Bola12;
                            lotoFacilDto.Bola13 = item3.Bola13;
                            lotoFacilDto.Bola14 = item3.Bola14;
                            lotoFacilDto.Bola15 = item3.Bola15;
                            lotoFacilDto.Concurso = item3.Concurso;
                            lotoFacilDto.ConcursoItem = item3.ConcursoItem;
                            lotoFacilDto.Data = item3.Data;
                            lotoFacilDto.Acerto = item3.Acerto;
                            listLoto.Add(lotoFacilDto);
                        }
                    }
                }

                stopwatch.Stop();
                Console.WriteLine($"Tempo decorrido para leitura: {stopwatch.Elapsed}");

                return listLoto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<LotoFacilDto> CompararNumerosSorteados(LotoFacilDto loto, LotofacilBaseDto lotoItem)
        {
            List<LotoFacilDto> listLoto = new List<LotoFacilDto>();
            int contaAcertoBola = 0;
            int totalLinhas = lotoItem.LotoFacilDtos.Count;

            for (int i = 0; i <= lotoItem.LotoFacilDtos.Count - 1; i++)
            {
                //Console.WriteLine($"Comparando concurso número {loto.Concurso} com {lotoItem.LotoFacilDtos[i].Concurso}");
                //Thread.Sleep( 10 );
                LotoFacilDto lotoFacilDto = new LotoFacilDto();

                if (lotoItem.LotoFacilDtos[i].Concurso != loto.Concurso)
                {
                    contaAcertoBola = 0;
                    for (int col = 2; col <= 16; col++)
                    {
                        

                        //Bola 01
                        if (col == 2)
                        {
                            if (VerificarCompararBola(loto.Bola01, lotoItem.LotoFacilDtos[i].Bola01) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola01, lotoItem.LotoFacilDtos[i].Bola02) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola01, lotoItem.LotoFacilDtos[i].Bola03) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola01, lotoItem.LotoFacilDtos[i].Bola04) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola01, lotoItem.LotoFacilDtos[i].Bola05) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola01, lotoItem.LotoFacilDtos[i].Bola06) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola01, lotoItem.LotoFacilDtos[i].Bola07) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola01, lotoItem.LotoFacilDtos[i].Bola08) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola01, lotoItem.LotoFacilDtos[i].Bola09) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola01, lotoItem.LotoFacilDtos[i].Bola10) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola01, lotoItem.LotoFacilDtos[i].Bola11) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola01, lotoItem.LotoFacilDtos[i].Bola12) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola01, lotoItem.LotoFacilDtos[i].Bola13) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola01, lotoItem.LotoFacilDtos[i].Bola14) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola01, lotoItem.LotoFacilDtos[i].Bola15) > 0)
                                contaAcertoBola++;

                            lotoItem.LotoFacilDtos[i].Acerto = contaAcertoBola;
                            lotoItem.LotoFacilDtos[i].ConcursoItem = lotoItem.LotoFacilDtos[i].Concurso;

                        }
                        else if (col == 3)
                        {
                            //Bola 02
                            if (VerificarCompararBola(loto.Bola02, lotoItem.LotoFacilDtos[i].Bola01) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola02, lotoItem.LotoFacilDtos[i].Bola02) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola02, lotoItem.LotoFacilDtos[i].Bola03) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola02, lotoItem.LotoFacilDtos[i].Bola04) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola02, lotoItem.LotoFacilDtos[i].Bola05) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola02, lotoItem.LotoFacilDtos[i].Bola06) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola02, lotoItem.LotoFacilDtos[i].Bola07) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola02, lotoItem.LotoFacilDtos[i].Bola08) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola02, lotoItem.LotoFacilDtos[i].Bola09) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola02, lotoItem.LotoFacilDtos[i].Bola10) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola02, lotoItem.LotoFacilDtos[i].Bola11) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola02, lotoItem.LotoFacilDtos[i].Bola12) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola02, lotoItem.LotoFacilDtos[i].Bola13) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola02, lotoItem.LotoFacilDtos[i].Bola14) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola02, lotoItem.LotoFacilDtos[i].Bola15) > 0)
                                contaAcertoBola++;

                            lotoItem.LotoFacilDtos[i].Acerto = contaAcertoBola;
                            lotoItem.LotoFacilDtos[i].ConcursoItem = lotoItem.LotoFacilDtos[i].Concurso;
                        }
                        else if (col == 4)
                        {
                            //Bola 03
                            if (VerificarCompararBola(loto.Bola03, lotoItem.LotoFacilDtos[i].Bola01) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola03, lotoItem.LotoFacilDtos[i].Bola02) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola03, lotoItem.LotoFacilDtos[i].Bola03) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola03, lotoItem.LotoFacilDtos[i].Bola04) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola03, lotoItem.LotoFacilDtos[i].Bola05) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola03, lotoItem.LotoFacilDtos[i].Bola06) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola03, lotoItem.LotoFacilDtos[i].Bola07) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola03, lotoItem.LotoFacilDtos[i].Bola08) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola03, lotoItem.LotoFacilDtos[i].Bola09) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola03, lotoItem.LotoFacilDtos[i].Bola10) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola03, lotoItem.LotoFacilDtos[i].Bola11) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola03, lotoItem.LotoFacilDtos[i].Bola12) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola03, lotoItem.LotoFacilDtos[i].Bola13) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola03, lotoItem.LotoFacilDtos[i].Bola14) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola03, lotoItem.LotoFacilDtos[i].Bola15) > 0)
                                contaAcertoBola++;

                            lotoItem.LotoFacilDtos[i].Acerto = contaAcertoBola;
                            lotoItem.LotoFacilDtos[i].ConcursoItem = lotoItem.LotoFacilDtos[i].Concurso;
                        }
                        else if (col == 5)
                        {
                            //Bola 04
                            if (VerificarCompararBola(loto.Bola04, lotoItem.LotoFacilDtos[i].Bola01) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola04, lotoItem.LotoFacilDtos[i].Bola02) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola04, lotoItem.LotoFacilDtos[i].Bola03) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola04, lotoItem.LotoFacilDtos[i].Bola04) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola04, lotoItem.LotoFacilDtos[i].Bola05) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola04, lotoItem.LotoFacilDtos[i].Bola06) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola04, lotoItem.LotoFacilDtos[i].Bola07) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola04, lotoItem.LotoFacilDtos[i].Bola08) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola04, lotoItem.LotoFacilDtos[i].Bola09) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola04, lotoItem.LotoFacilDtos[i].Bola10) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola04, lotoItem.LotoFacilDtos[i].Bola11) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola04, lotoItem.LotoFacilDtos[i].Bola12) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola04, lotoItem.LotoFacilDtos[i].Bola13) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola04, lotoItem.LotoFacilDtos[i].Bola14) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola04, lotoItem.LotoFacilDtos[i].Bola15) > 0)
                                contaAcertoBola++;

                            lotoItem.LotoFacilDtos[i].Acerto = contaAcertoBola;
                            lotoItem.LotoFacilDtos[i].ConcursoItem = lotoItem.LotoFacilDtos[i].Concurso;
                        }
                        else if (col == 6)
                        {
                            //Bola 05
                            if (VerificarCompararBola(loto.Bola05, lotoItem.LotoFacilDtos[i].Bola01) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola05, lotoItem.LotoFacilDtos[i].Bola02) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola05, lotoItem.LotoFacilDtos[i].Bola03) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola05, lotoItem.LotoFacilDtos[i].Bola04) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola05, lotoItem.LotoFacilDtos[i].Bola05) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola05, lotoItem.LotoFacilDtos[i].Bola06) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola05, lotoItem.LotoFacilDtos[i].Bola07) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola05, lotoItem.LotoFacilDtos[i].Bola08) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola05, lotoItem.LotoFacilDtos[i].Bola09) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola05, lotoItem.LotoFacilDtos[i].Bola10) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola05, lotoItem.LotoFacilDtos[i].Bola11) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola05, lotoItem.LotoFacilDtos[i].Bola12) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola05, lotoItem.LotoFacilDtos[i].Bola13) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola05, lotoItem.LotoFacilDtos[i].Bola14) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola05, lotoItem.LotoFacilDtos[i].Bola15) > 0)
                                contaAcertoBola++;

                            lotoItem.LotoFacilDtos[i].Acerto = contaAcertoBola;
                            lotoItem.LotoFacilDtos[i].ConcursoItem = lotoItem.LotoFacilDtos[i].Concurso;
                        }
                        else if (col == 7)
                        {
                            //Bola 06
                            if (VerificarCompararBola(loto.Bola06, lotoItem.LotoFacilDtos[i].Bola01) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola06, lotoItem.LotoFacilDtos[i].Bola02) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola06, lotoItem.LotoFacilDtos[i].Bola03) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola06, lotoItem.LotoFacilDtos[i].Bola04) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola06, lotoItem.LotoFacilDtos[i].Bola05) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola06, lotoItem.LotoFacilDtos[i].Bola06) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola06, lotoItem.LotoFacilDtos[i].Bola07) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola06, lotoItem.LotoFacilDtos[i].Bola08) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola06, lotoItem.LotoFacilDtos[i].Bola09) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola06, lotoItem.LotoFacilDtos[i].Bola10) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola06, lotoItem.LotoFacilDtos[i].Bola11) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola06, lotoItem.LotoFacilDtos[i].Bola12) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola06, lotoItem.LotoFacilDtos[i].Bola13) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola06, lotoItem.LotoFacilDtos[i].Bola14) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola06, lotoItem.LotoFacilDtos[i].Bola15) > 0)
                                contaAcertoBola++;

                            lotoItem.LotoFacilDtos[i].Acerto = contaAcertoBola;
                            lotoItem.LotoFacilDtos[i].ConcursoItem = lotoItem.LotoFacilDtos[i].Concurso;
                        }
                        else if (col == 8)
                        {
                            //Bola 07
                            if (VerificarCompararBola(loto.Bola07, lotoItem.LotoFacilDtos[i].Bola01) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola07, lotoItem.LotoFacilDtos[i].Bola02) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola07, lotoItem.LotoFacilDtos[i].Bola03) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola07, lotoItem.LotoFacilDtos[i].Bola04) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola07, lotoItem.LotoFacilDtos[i].Bola05) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola07, lotoItem.LotoFacilDtos[i].Bola06) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola07, lotoItem.LotoFacilDtos[i].Bola07) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola07, lotoItem.LotoFacilDtos[i].Bola08) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola07, lotoItem.LotoFacilDtos[i].Bola09) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola07, lotoItem.LotoFacilDtos[i].Bola10) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola07, lotoItem.LotoFacilDtos[i].Bola11) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola07, lotoItem.LotoFacilDtos[i].Bola12) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola07, lotoItem.LotoFacilDtos[i].Bola13) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola07, lotoItem.LotoFacilDtos[i].Bola14) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola07, lotoItem.LotoFacilDtos[i].Bola15) > 0)
                                contaAcertoBola++;

                            lotoItem.LotoFacilDtos[i].Acerto = contaAcertoBola;
                            lotoItem.LotoFacilDtos[i].ConcursoItem = lotoItem.LotoFacilDtos[i].Concurso;
                        }
                        else if (col == 9)
                        {
                            //Bola 08
                            if (VerificarCompararBola(loto.Bola08, lotoItem.LotoFacilDtos[i].Bola01) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola08, lotoItem.LotoFacilDtos[i].Bola02) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola08, lotoItem.LotoFacilDtos[i].Bola03) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola08, lotoItem.LotoFacilDtos[i].Bola04) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola08, lotoItem.LotoFacilDtos[i].Bola05) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola08, lotoItem.LotoFacilDtos[i].Bola06) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola08, lotoItem.LotoFacilDtos[i].Bola07) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola08, lotoItem.LotoFacilDtos[i].Bola08) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola08, lotoItem.LotoFacilDtos[i].Bola09) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola08, lotoItem.LotoFacilDtos[i].Bola10) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola08, lotoItem.LotoFacilDtos[i].Bola11) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola08, lotoItem.LotoFacilDtos[i].Bola12) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola08, lotoItem.LotoFacilDtos[i].Bola13) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola08, lotoItem.LotoFacilDtos[i].Bola14) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola08, lotoItem.LotoFacilDtos[i].Bola15) > 0)
                                contaAcertoBola++;

                            lotoItem.LotoFacilDtos[i].Acerto = contaAcertoBola;
                            lotoItem.LotoFacilDtos[i].ConcursoItem = lotoItem.LotoFacilDtos[i].Concurso;
                        }
                        else if (col == 10)
                        {
                            //Bola 09
                            if (VerificarCompararBola(loto.Bola09, lotoItem.LotoFacilDtos[i].Bola01) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola09, lotoItem.LotoFacilDtos[i].Bola02) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola09, lotoItem.LotoFacilDtos[i].Bola03) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola09, lotoItem.LotoFacilDtos[i].Bola04) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola09, lotoItem.LotoFacilDtos[i].Bola05) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola09, lotoItem.LotoFacilDtos[i].Bola06) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola09, lotoItem.LotoFacilDtos[i].Bola07) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola09, lotoItem.LotoFacilDtos[i].Bola08) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola09, lotoItem.LotoFacilDtos[i].Bola09) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola09, lotoItem.LotoFacilDtos[i].Bola10) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola09, lotoItem.LotoFacilDtos[i].Bola11) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola09, lotoItem.LotoFacilDtos[i].Bola12) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola09, lotoItem.LotoFacilDtos[i].Bola13) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola09, lotoItem.LotoFacilDtos[i].Bola14) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola09, lotoItem.LotoFacilDtos[i].Bola15) > 0)
                                contaAcertoBola++;

                            lotoItem.LotoFacilDtos[i].Acerto = contaAcertoBola;
                            lotoItem.LotoFacilDtos[i].ConcursoItem = lotoItem.LotoFacilDtos[i].Concurso;
                        }
                        else if (col == 11)
                        {
                            //Bola 010
                            if (VerificarCompararBola(loto.Bola10, lotoItem.LotoFacilDtos[i].Bola01) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola10, lotoItem.LotoFacilDtos[i].Bola02) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola10, lotoItem.LotoFacilDtos[i].Bola03) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola10, lotoItem.LotoFacilDtos[i].Bola04) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola10, lotoItem.LotoFacilDtos[i].Bola05) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola10, lotoItem.LotoFacilDtos[i].Bola06) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola10, lotoItem.LotoFacilDtos[i].Bola07) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola10, lotoItem.LotoFacilDtos[i].Bola08) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola10, lotoItem.LotoFacilDtos[i].Bola09) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola10, lotoItem.LotoFacilDtos[i].Bola10) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola10, lotoItem.LotoFacilDtos[i].Bola11) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola10, lotoItem.LotoFacilDtos[i].Bola12) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola10, lotoItem.LotoFacilDtos[i].Bola13) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola10, lotoItem.LotoFacilDtos[i].Bola14) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola10, lotoItem.LotoFacilDtos[i].Bola15) > 0)
                                contaAcertoBola++;

                            lotoItem.LotoFacilDtos[i].Acerto = contaAcertoBola;
                            lotoItem.LotoFacilDtos[i].ConcursoItem = lotoItem.LotoFacilDtos[i].Concurso;
                        }
                        else if (col == 12)
                        {
                            //Bola 11
                            if (VerificarCompararBola(loto.Bola11, lotoItem.LotoFacilDtos[i].Bola01) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola11, lotoItem.LotoFacilDtos[i].Bola02) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola11, lotoItem.LotoFacilDtos[i].Bola03) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola11, lotoItem.LotoFacilDtos[i].Bola04) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola11, lotoItem.LotoFacilDtos[i].Bola05) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola11, lotoItem.LotoFacilDtos[i].Bola06) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola11, lotoItem.LotoFacilDtos[i].Bola07) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola11, lotoItem.LotoFacilDtos[i].Bola08) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola11, lotoItem.LotoFacilDtos[i].Bola09) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola11, lotoItem.LotoFacilDtos[i].Bola10) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola11, lotoItem.LotoFacilDtos[i].Bola11) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola11, lotoItem.LotoFacilDtos[i].Bola12) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola11, lotoItem.LotoFacilDtos[i].Bola13) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola11, lotoItem.LotoFacilDtos[i].Bola14) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola11, lotoItem.LotoFacilDtos[i].Bola15) > 0)
                                contaAcertoBola++;

                            lotoItem.LotoFacilDtos[i].Acerto = contaAcertoBola;
                            lotoItem.LotoFacilDtos[i].ConcursoItem = lotoItem.LotoFacilDtos[i].Concurso;
                        }
                        else if (col == 13)
                        {
                            //Bola 12
                            if (VerificarCompararBola(loto.Bola12, lotoItem.LotoFacilDtos[i].Bola01) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola12, lotoItem.LotoFacilDtos[i].Bola02) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola12, lotoItem.LotoFacilDtos[i].Bola03) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola12, lotoItem.LotoFacilDtos[i].Bola04) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola12, lotoItem.LotoFacilDtos[i].Bola05) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola12, lotoItem.LotoFacilDtos[i].Bola06) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola12, lotoItem.LotoFacilDtos[i].Bola07) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola12, lotoItem.LotoFacilDtos[i].Bola08) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola12, lotoItem.LotoFacilDtos[i].Bola09) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola12, lotoItem.LotoFacilDtos[i].Bola10) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola12, lotoItem.LotoFacilDtos[i].Bola11) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola12, lotoItem.LotoFacilDtos[i].Bola12) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola12, lotoItem.LotoFacilDtos[i].Bola13) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola12, lotoItem.LotoFacilDtos[i].Bola14) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola12, lotoItem.LotoFacilDtos[i].Bola15) > 0)
                                contaAcertoBola++;

                            lotoItem.LotoFacilDtos[i].Acerto = contaAcertoBola;
                            lotoItem.LotoFacilDtos[i].ConcursoItem = lotoItem.LotoFacilDtos[i].Concurso;
                        }
                        else if (col == 14)
                        {
                            //Bola 13
                            if (VerificarCompararBola(loto.Bola13, lotoItem.LotoFacilDtos[i].Bola01) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola13, lotoItem.LotoFacilDtos[i].Bola02) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola13, lotoItem.LotoFacilDtos[i].Bola03) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola13, lotoItem.LotoFacilDtos[i].Bola04) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola13, lotoItem.LotoFacilDtos[i].Bola05) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola13, lotoItem.LotoFacilDtos[i].Bola06) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola13, lotoItem.LotoFacilDtos[i].Bola07) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola13, lotoItem.LotoFacilDtos[i].Bola08) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola13, lotoItem.LotoFacilDtos[i].Bola09) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola13, lotoItem.LotoFacilDtos[i].Bola10) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola13, lotoItem.LotoFacilDtos[i].Bola11) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola13, lotoItem.LotoFacilDtos[i].Bola12) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola13, lotoItem.LotoFacilDtos[i].Bola13) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola13, lotoItem.LotoFacilDtos[i].Bola14) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola13, lotoItem.LotoFacilDtos[i].Bola15) > 0)
                                contaAcertoBola++;

                            lotoItem.LotoFacilDtos[i].Acerto = contaAcertoBola;
                            lotoItem.LotoFacilDtos[i].ConcursoItem = lotoItem.LotoFacilDtos[i].Concurso;
                        }
                        else if (col == 15)
                        {
                            //Bola 14
                            if (VerificarCompararBola(loto.Bola14, lotoItem.LotoFacilDtos[i].Bola01) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola14, lotoItem.LotoFacilDtos[i].Bola02) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola14, lotoItem.LotoFacilDtos[i].Bola03) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola14, lotoItem.LotoFacilDtos[i].Bola04) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola14, lotoItem.LotoFacilDtos[i].Bola05) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola14, lotoItem.LotoFacilDtos[i].Bola06) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola14, lotoItem.LotoFacilDtos[i].Bola07) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola14, lotoItem.LotoFacilDtos[i].Bola08) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola14, lotoItem.LotoFacilDtos[i].Bola09) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola14, lotoItem.LotoFacilDtos[i].Bola10) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola14, lotoItem.LotoFacilDtos[i].Bola11) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola14, lotoItem.LotoFacilDtos[i].Bola12) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola14, lotoItem.LotoFacilDtos[i].Bola13) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola14, lotoItem.LotoFacilDtos[i].Bola14) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola14, lotoItem.LotoFacilDtos[i].Bola15) > 0)
                                contaAcertoBola++;

                            lotoItem.LotoFacilDtos[i].Acerto = contaAcertoBola;
                            lotoItem.LotoFacilDtos[i].ConcursoItem = lotoItem.LotoFacilDtos[i].Concurso;
                        }
                        else if (col == 16)
                        {
                            //Bola 15
                            if (VerificarCompararBola(loto.Bola15, lotoItem.LotoFacilDtos[i].Bola01) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola15, lotoItem.LotoFacilDtos[i].Bola02) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola15, lotoItem.LotoFacilDtos[i].Bola03) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola15, lotoItem.LotoFacilDtos[i].Bola04) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola15, lotoItem.LotoFacilDtos[i].Bola05) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola15, lotoItem.LotoFacilDtos[i].Bola06) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola15, lotoItem.LotoFacilDtos[i].Bola07) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola15, lotoItem.LotoFacilDtos[i].Bola08) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola15, lotoItem.LotoFacilDtos[i].Bola09) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola15, lotoItem.LotoFacilDtos[i].Bola10) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola15, lotoItem.LotoFacilDtos[i].Bola11) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola15, lotoItem.LotoFacilDtos[i].Bola12) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola15, lotoItem.LotoFacilDtos[i].Bola13) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola15, lotoItem.LotoFacilDtos[i].Bola14) > 0)
                                contaAcertoBola++;
                            else if (VerificarCompararBola(loto.Bola15, lotoItem.LotoFacilDtos[i].Bola15) > 0)
                                contaAcertoBola++;

                            lotoItem.LotoFacilDtos[i].Acerto = contaAcertoBola;
                            lotoItem.LotoFacilDtos[i].ConcursoItem = lotoItem.LotoFacilDtos[i].Concurso;
                        }

                        lotoFacilDto.Concurso = loto.Concurso;
                        lotoFacilDto.ConcursoItem = lotoItem.LotoFacilDtos[i].ConcursoItem;
                        lotoFacilDto.Data = loto.Data;
                        lotoFacilDto.Bola01 = loto.Bola01;
                        lotoFacilDto.Bola02 = loto.Bola02;
                        lotoFacilDto.Bola03 = loto.Bola03;
                        lotoFacilDto.Bola04 = loto.Bola04;
                        lotoFacilDto.Bola05 = loto.Bola05;
                        lotoFacilDto.Bola06 = loto.Bola06;
                        lotoFacilDto.Bola07 = loto.Bola07;
                        lotoFacilDto.Bola08 = loto.Bola08;
                        lotoFacilDto.Bola09 = loto.Bola09;
                        lotoFacilDto.Bola10 = loto.Bola10;
                        lotoFacilDto.Bola11 = loto.Bola11;
                        lotoFacilDto.Bola12 = loto.Bola12;
                        lotoFacilDto.Bola13 = loto.Bola13;
                        lotoFacilDto.Bola14 = loto.Bola14;
                        lotoFacilDto.Bola15 = loto.Bola15;
                        lotoFacilDto.Acerto = lotoItem.LotoFacilDtos[i].Acerto;
                    }

                    if (lotoFacilDto.Acerto >= 14)
                    {
                        listLoto.Add(lotoFacilDto);
                        //Console.WriteLine("");
                        //Thread.Sleep( 10 );
                    }
                }

            }
            return listLoto;
        }

        private int SomaPontos(int bola01, int bolaN)
        {
            int conta = 0;
            if (bola01 == bolaN)
                conta++;

            return conta;
        }

        private int VerificarCompararBola(int bola, int bolaN)
        {
            int conta = 0;
            conta = SomaPontos(bola, bolaN);
            return conta;
        }

        private int VerificaBola01(int indice, int bola01, int bolaN)
        {
            int conta = 0;
            conta = SomaPontos(bola01, bolaN);
            return conta;
        }



        private LotoFacilDto MontarMatriz(LotoFacilDto lotoFacilDto, int totalLinhas, int linha)
        {
            LotoFacilDto list = new LotoFacilDto();
            string[,] matriz = new string[2978, 17];
   
            matriz[linha, 0] = lotoFacilDto.Concurso.ToString();
            matriz[linha, 1] = lotoFacilDto.Data.ToString();

            matriz[linha, 2] = lotoFacilDto.Bola01.ToString();
            matriz[linha, 3] = lotoFacilDto.Bola02.ToString();
            matriz[linha, 4] = lotoFacilDto.Bola03.ToString();
            matriz[linha, 5] = lotoFacilDto.Bola04.ToString();
            matriz[linha, 6] = lotoFacilDto.Bola05.ToString();
            matriz[linha, 7] = lotoFacilDto.Bola06.ToString();
            matriz[linha, 8] = lotoFacilDto.Bola07.ToString();
            matriz[linha, 9] = lotoFacilDto.Bola08.ToString();
            matriz[linha, 10] = lotoFacilDto.Bola09.ToString();
            matriz[linha, 11] = lotoFacilDto.Bola10.ToString();
            matriz[linha, 12] = lotoFacilDto.Bola11.ToString();
            matriz[linha, 13] = lotoFacilDto.Bola12.ToString();
            matriz[linha, 14] = lotoFacilDto.Bola13.ToString();
            matriz[linha, 15] = lotoFacilDto.Bola14.ToString();
            matriz[linha, 16] = lotoFacilDto.Bola15.ToString();

            return list;
            //}
        }
    }
}
