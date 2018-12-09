using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lottery.ViewModel
{
    public class ApostaViewModel
    {
        public string Name { get; set; }

        public string Combinations { get; set; }

        public int GameType { get; set; }

        public bool Automatic { get; set; }
    }
}
