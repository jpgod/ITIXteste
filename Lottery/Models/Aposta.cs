using Lottery.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lottery.Models
{
    /// <summary>
    /// Modelo que registra uma aposta de um jogo.
    /// </summary>
    public class Aposta
    {
        public int Id { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Name { get; set; }

        public List<int> Combinations { get; set; }

        public virtual ILotteryGame LotteryType { get; set; }
    }
}
