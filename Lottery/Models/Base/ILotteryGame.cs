using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lottery.Models.Base
{
    /// <summary>
    /// Inteface que define o que é comum à um jogo de loteria
    /// </summary>
    public interface ILotteryGame
    {
        /// <summary>
        /// Quantidade de números numa aposta
        /// </summary>
        int Size { get; }           
        
        /// <summary>
        /// O menor número que se pode apostar
        /// </summary>
        int MinValue { get; }

        /// <summary>
        /// O maior número que se pode apostar
        /// </summary>
        int MaxValue { get; }

        /// <summary>
        /// Lista de combinações que podem ganhar
        /// </summary>
        IList<int> WinnerCombinations {get;}

        List<int> GenerateNumbers();

        string GenerateNumbersString();
    }
}
