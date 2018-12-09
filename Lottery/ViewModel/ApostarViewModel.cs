using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lottery.ViewModel
{
    public class ApostarViewModel
    {
        public string Error { get; set; }

        public List<ApostaVM> ListApostas { get; set; }
    }

    public class ApostaVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Combinations { get; set; }

        public string TimeStamp { get; set; }
    }
}
