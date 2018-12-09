using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lottery.ViewModel
{
    public class ResultViewModel
    {
        public string Error { get; set; }
        public string Sorteio { get; set; }
        public List<AcertosVM> Vencedores { get; set; }
    }

    public class AcertosVM
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Acertos { get; set; }
        public string Erros { get; set; }

        public int TotalAcertos { get; set; }
    }
}
