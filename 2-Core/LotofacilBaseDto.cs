using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lotofacil._2_Core
{
    public class LotofacilBaseDto
    {
        public string Id { get; set; }
        public List<LotoFacilDto> LotoFacilDtos { get; set; }
        public LotofacilBaseDto(List<LotoFacilDto> loto)
        {
            LotoFacilDtos = loto;
        }
    }
}
