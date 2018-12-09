using Lottery.Helpers;
using Lottery.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lottery.Models
{
    /// <summary>
    /// Define as regras da Mega Sena
    /// </summary>
    public class MegaSena : ILotteryGame
    {
        //Constantes
        public int Size { get; } = 6; 
        public int MinValue { get; } = 1;
        public int MaxValue { get; } = 60;
        public IList<int> WinnerCombinations { get; } = new List<int> { 4, 5, 6 }; //quadras, quinas e senas

        /// <summary>
        /// Realiza o sorteio/escolha dos números e retorna como lista
        /// </summary>
        public List<int> GenerateNumbers()
        {
            var rng = new RandGenerator();
            var numbers = new List<int>(Size);

            //Gera bolas/números até a quantidade definida no Size
            while (numbers.Count < Size)
            {
                var ball = rng.Next(MinValue, MaxValue);

                //Verifica se a bola já não foi sorteada previamente
                if (!numbers.Contains(ball))
                    numbers.Add(ball);
            }

            return numbers.OrderBy(x => x).ToList();
        }

        /// <summary>
        /// Realiza o sorteio/escolha dos números e retorna como string
        /// </summary>
        public string GenerateNumbersString()
        {
            var result = GenerateNumbers();

            return string.Join('-', result);
        }
    }
}
