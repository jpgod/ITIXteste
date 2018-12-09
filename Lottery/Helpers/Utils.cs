using Lottery.Models.Base;
using System;
using System.Collections.Generic;
using Lottery.Models;
using Lottery.ViewModel;

namespace Lottery.Helpers
{
    public static class Utils
    {
         /// <summary>
        /// Retornar o tipo de jogo
        /// </summary>
        public static ILotteryGame GetGameType(int gameType)
        {
            switch (gameType)
            {
                case 1:
                    return new MegaSena();
                default:
                    throw new ArgumentException("Tipo de jogo inválido!");
            }
        }

        /// <summary>
        /// Verificar que números foram acertados numa aposta.
        /// </summary>
        /// <param name="aposta"></param>
        /// <param name="sorteio"></param>
        /// <returns></returns>
        public static AcertosVM ValidarSorteio(Aposta aposta, List<int> sorteio)
        {
            List<int> acertos = new List<int>(); List<int> erros = new List<int>();

            foreach (var item in aposta.Combinations)
            {
                if (sorteio.Contains(item))
                    acertos.Add(item);
                else
                    erros.Add(item);
            }

            if (aposta.LotteryType.WinnerCombinations.Contains(acertos.Count))
            {
                return new AcertosVM()
                {
                    Id = aposta.Id,
                    Name = aposta.Name,
                    Acertos = string.Join('-', acertos),
                    Erros = string.Join('-', erros),
                    TotalAcertos = acertos.Count
                };
            }
            else
                return null;
        }

        public static List<Aposta> GetApostas()
        {
            List<Aposta> apostas = new List<Aposta>(10);

            apostas.Add(new Aposta { Id = 1, Name = "Apostador", Combinations = new List<int>() { 1, 2, 3, 4, 5, 6 }, LotteryType = new MegaSena(), TimeStamp = new DateTime(2018, 01, 01) });
            apostas.Add(new Aposta { Id = 2, Name = "Apostador", Combinations = new List<int>() { 15, 25, 23, 34, 45, 36 }, LotteryType = new MegaSena(), TimeStamp = new DateTime(2018, 01, 01) });
            apostas.Add(new Aposta { Id = 3, Name = "Apostador", Combinations = new List<int>() { 8, 23, 31, 14, 35, 22 }, LotteryType = new MegaSena(), TimeStamp = new DateTime(2018, 01, 01) });
            apostas.Add(new Aposta { Id = 4, Name = "Apostador", Combinations = new List<int>() { 9, 21, 13, 14, 52, 44 }, LotteryType = new MegaSena(), TimeStamp = new DateTime(2018, 01, 01) });

            apostas.Add(new Aposta { Id = 5, Name = "Apostador", Combinations = new List<int>() { 11, 25, 34, 36, 35, 60 }, LotteryType = new MegaSena(), TimeStamp = new DateTime(2018, 01, 01) });
            apostas.Add(new Aposta { Id = 6, Name = "Apostador", Combinations = new List<int>() { 12, 21, 31, 14, 51, 26 }, LotteryType = new MegaSena(), TimeStamp = new DateTime(2018, 01, 01) });
            apostas.Add(new Aposta { Id = 7, Name = "Apostador", Combinations = new List<int>() { 13, 22, 32, 24, 52, 26 }, LotteryType = new MegaSena(), TimeStamp = new DateTime(2018, 01, 01) });
            apostas.Add(new Aposta { Id = 8, Name = "Apostador", Combinations = new List<int>() { 14, 22, 33, 34, 52, 26 }, LotteryType = new MegaSena(), TimeStamp = new DateTime(2018, 01, 01) });

            apostas.Add(new Aposta { Id = 9, Name = "Apostador", Combinations = new List<int>() { 10, 22, 13, 43, 52, 46 }, LotteryType = new MegaSena(), TimeStamp = new DateTime(2018, 01, 01) });
            apostas.Add(new Aposta { Id = 10, Name = "Apostador", Combinations = new List<int>() { 10, 42, 35, 43, 25, 36 }, LotteryType = new MegaSena(), TimeStamp = new DateTime(2018, 01, 01) });
            apostas.Add(new Aposta { Id = 11, Name = "Apostador", Combinations = new List<int>() { 10, 12, 34, 14, 45, 16 }, LotteryType = new MegaSena(), TimeStamp = new DateTime(2018, 01, 01) });
            apostas.Add(new Aposta { Id = 12, Name = "Apostador", Combinations = new List<int>() { 10, 32, 36, 54, 55, 60 }, LotteryType = new MegaSena(), TimeStamp = new DateTime(2018, 01, 01) });

            return apostas;
        }
    }
}
