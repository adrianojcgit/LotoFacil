using Lotofacil._2_Core;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using System.ComponentModel;

namespace Lotofacil.Servico
{
    public class LotofacilServico
    {
        private readonly IConfiguration _configuration;
        public LotofacilServico(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static LotofacilBaseDto ReadXls(string NomeArquivo, string DirArqClienteOrigem)
        {

            var response = new List<LotoFacilDto>();
            Guid id = Guid.NewGuid();
            
            var arquivo = DirArqClienteOrigem + NomeArquivo;

            FileInfo existingFile = new FileInfo(fileName: arquivo);

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int colCount = worksheet.Dimension.End.Column;
                int rowCount = worksheet.Dimension.End.Row;

                for (int row = 2; row <= rowCount; row++)
                {
                    var l = new LotoFacilDto
                    {

                        Concurso = int.Parse(worksheet.Cells[row, 1].Value.ToString()),
                        Data = Convert.ToDateTime(worksheet.Cells[row, 2].Value.ToString()),
                        Bola01 = int.Parse(worksheet.Cells[row, 3].Value.ToString()),
                        Bola02 = int.Parse(worksheet.Cells[row, 4].Value.ToString()),
                        Bola03 = int.Parse(worksheet.Cells[row, 5].Value.ToString()),
                        Bola04 = int.Parse(worksheet.Cells[row, 6].Value.ToString()),
                        Bola05 = int.Parse(worksheet.Cells[row, 7].Value.ToString()),
                        Bola06 = int.Parse(worksheet.Cells[row, 8].Value.ToString()),
                        Bola07 = int.Parse(worksheet.Cells[row, 9].Value.ToString()),
                        Bola08 = int.Parse(worksheet.Cells[row, 10].Value.ToString()),
                        Bola09 = int.Parse(worksheet.Cells[row, 11].Value.ToString()),
                        Bola10 = int.Parse(worksheet.Cells[row, 12].Value.ToString()),
                        Bola11 = int.Parse(worksheet.Cells[row, 13].Value.ToString()),
                        Bola12 = int.Parse(worksheet.Cells[row, 14].Value.ToString()),
                        Bola13 = int.Parse(worksheet.Cells[row, 15].Value.ToString()),
                        Bola14 = int.Parse(worksheet.Cells[row, 16].Value.ToString()),
                        Bola15 = int.Parse(worksheet.Cells[row, 17].Value.ToString())
                    }; 
                    
                    response.Add(l);
                    
                }
            }
            var loto = new LotofacilBaseDto(response);
            loto.Id = id.ToString();
            return loto;
        }
    }
}
