using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Lottery.Helpers
{
    /// <summary>
    /// Classe para gerar números aleatórios baseado na biblioteca RNGCryptoServiceProvider
    /// Inspirado em https://gist.github.com/1017834
    /// </summary>
    public class RandGenerator
    {
        readonly RNGCryptoServiceProvider CS;

        public RandGenerator()
        {
            CS = new RNGCryptoServiceProvider();
        }

        //Obter próximo inteiro
        public int Next(int minValue, int maxValue)
        {
            if (minValue >= maxValue)
                throw new ArgumentOutOfRangeException("Erro: 'minValue' deve ser menor que 'maxValue'");

            long diff = (long)maxValue - minValue;
            long upperBound = uint.MaxValue / diff * diff;

            uint ui;
            do
            {
                ui = GetRandomUInt();
            } while (ui >= upperBound);
            return (int)(minValue + (ui % diff));
        }

        //Gera um inteiro aleatório
        private uint GetRandomUInt()
        {
            var randomBytes = GenerateRandomBytes(sizeof(uint));
            return BitConverter.ToUInt32(randomBytes, 0);
        }

        //Gerar byte array aleatório
        private byte[] GenerateRandomBytes(int bytesNumber)
        {
            byte[] buffer = new byte[bytesNumber];
            CS.GetBytes(buffer);
            return buffer;
        }
    }
}