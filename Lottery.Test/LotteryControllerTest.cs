using Lottery.Controllers;
using System;
using Xunit;
using Lottery.Models.Base;
using Lottery.Models;
using Microsoft.Extensions.Caching.Memory;
using Lottery.ViewModel;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Lottery.Test
{
    public class LotteryControllerTest
    {
        LotteryController Controller;
        private readonly IMemoryCache cache;

        public LotteryControllerTest()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            var mapper = config.CreateMapper();

            cache = new MemoryCache(new MemoryCacheOptions());
            Controller = new LotteryController(cache, mapper);
        }

        /// <summary>
        /// Verificar se retorna mensagem de erro se realizar um sorteio sem apostas
        /// </summary>
        [Fact]
        public void Sorteio_ShouldShowErrorNoData()
        {
            //Arrange
            var vm = new SorteioViewModel
            {
                GameType = 1
            };

            //Act
            var response = Controller.Sortear(vm).Result as OkObjectResult;

            //Assert
            Assert.NotNull(response);
            var sorteioResponde = response.Value as ResultViewModel;
            Assert.NotEmpty(sorteioResponde.Error);
            Assert.Contains("Não existe apostas cadastradas", sorteioResponde.Error);
            Assert.Null(sorteioResponde.Vencedores);
            Assert.Empty(sorteioResponde.Sorteio);
        }

        /// <summary>
        /// Verifica se realizou o sorteio e valida os numeros
        /// </summary>
        [Fact]
        public void Sorteio_ShouldGenerateValid()
        {
            //Arrange
            var vm = new SorteioViewModel
            {
                GameType = 1
            };

            //Act
            var okResult1 = Controller.GenerateApostas(1);
            var response = Controller.Sortear(vm).Result as OkObjectResult;

            //Assert
            Assert.NotNull(response);
            var sorteioResponde = response.Value as ResultViewModel;
            Assert.Empty(sorteioResponde.Error);
            Assert.NotEmpty(sorteioResponde.Sorteio);
            Assert.IsType<List<AcertosVM>>(sorteioResponde.Vencedores);
            Assert.Matches(@"^\d{1,2}-\d{1,2}-\d{1,2}-\d{1,2}-\d{1,2}-\d{1,2}$", sorteioResponde.Sorteio);
        }

        /// <summary>
        /// Verifica se realizou uma aposta automática com sucesso
        /// </summary>
        [Fact]
        public void Aposta_Automatic()
        {
            //Arrange
            var vm = new ApostaViewModel()
            {
                Automatic = true,
                GameType = 1,
                Name = "testes"
            };

            //Act
            var response = Controller.Apostar(vm).Result as OkObjectResult;

            //Assert
            Assert.NotNull(response);
            var apostaResponde = response.Value as ApostarViewModel;
            var item = Assert.IsType<List<ApostaVM>>(apostaResponde.ListApostas);
            Assert.Matches(@"^\d{1,2}-\d{1,2}-\d{1,2}-\d{1,2}-\d{1,2}-\d{1,2}$", item[0].Combinations);
        }

        /// <summary>
        /// Realiza uma aposta manual e a valida
        /// </summary>
        [Fact]
        public void Aposta_IsValidResponse()
        {
            //Arrange
            var vm = new ApostaViewModel()
            {
                Automatic = false,
                Combinations = "4-8-15-16-23-42",
                GameType = 1,
                Name  = "testes"
            };

            //Act
            var response = Controller.Apostar(vm).Result as OkObjectResult;

            //Assert
            Assert.NotNull(response);
            var apostaResponde = response.Value as ApostarViewModel;
            var item = Assert.IsType<List<ApostaVM>>(apostaResponde.ListApostas);
            Assert.Equal("4-8-15-16-23-42", item[0].Combinations);
        }

        /// <summary>
        /// Valida se gera erro para apostas inválidas
        /// </summary>
        [Fact]
        public void Aposta_ShouldShowInvalidAposta()
        {
            //Arrange
            var incomplete = new ApostaViewModel()
            {
                Automatic = false,
                Combinations = "4-8-15-16-23",
                GameType = 1,
                Name = "testes"
            };

            //Act
            var response = Controller.Apostar(incomplete).Result as OkObjectResult;

            //Assert
            Assert.NotNull(response);
            var apostaResponde = response.Value as ApostarViewModel;
            Assert.Contains("A aposta deve conter apenas 6 números!", apostaResponde.Error);
        }

        /// <summary>
        /// Valida se gera erro para usuário em branco
        /// </summary>
        [Fact]
        public void Aposta_ShouldShowInvalidName()
        {
            //Arrange
            var incomplete = new ApostaViewModel()
            {
                Automatic = false,
                Combinations = "4-8-15-16-23-42",
                GameType = 1,
                Name = string.Empty
            };

            //Act
            var response = Controller.Apostar(incomplete).Result as OkObjectResult;

            //Assert
            Assert.NotNull(response);
            var apostaResponde = response.Value as ApostarViewModel;
            Assert.Contains("'Nome' não deve ser nulo!", apostaResponde.Error);
        }

        /// <summary>
        /// Valida se gera erro para números repetidos
        /// </summary>
        [Fact]
        public void Aposta_ShouldShowRepeatedNumbers()
        {
            //Arrange
            var incomplete = new ApostaViewModel()
            {
                Automatic = false,
                Combinations = "1-1-1-1-1-1",
                GameType = 1,
                Name = "Test"
            };

            //Act
            var response = Controller.Apostar(incomplete).Result as OkObjectResult;

            //Assert
            Assert.NotNull(response);
            var apostaResponde = response.Value as ApostarViewModel;
            Assert.Contains("A aposta tem que ter números diferentes!", apostaResponde.Error);
        }

        /// <summary>
        /// Valida se gera erro para numeros fora do intervalo 1-60
        /// </summary>
        [Fact]
        public void Aposta_ShouldShowInvalidNumbers()
        {
            //Arrange
            var incomplete = new ApostaViewModel()
            {
                Automatic = false,
                Combinations = "1-2-3-4-5-61",
                GameType = 1,
                Name = "Test"
            };

            //Act
            var response = Controller.Apostar(incomplete).Result as OkObjectResult;

            //Assert
            Assert.NotNull(response);
            var apostaResponde = response.Value as ApostarViewModel;
            Assert.Contains("Os números da aposta tem que estar entre 1 e 60!", apostaResponde.Error);
        }

        /// <summary>
        /// Valida se gera erro para entrada não numérica
        /// </summary>
        [Fact]
        public void Aposta_ShouldShowInvalidInput()
        {
            //Arrange
            var incomplete = new ApostaViewModel()
            {
                Automatic = false,
                Combinations = "X",
                GameType = 1,
                Name = "Test"
            };

            //Act
            var response = Controller.Apostar(incomplete).Result as OkObjectResult;

            //Assert
            Assert.NotNull(response);
            var apostaResponde = response.Value as ApostarViewModel;
            Assert.Contains("Apenas números separados por traços!", apostaResponde.Error);
        }

        /// <summary>
        /// Testar retorno de 25 apostas automáticas
        /// </summary>
        [Fact]
        public void Aposta_GetAllApostas()
        {
            //Arrange
            int qtdApostas = 25;

            //Act
            var okResult1 = Controller.GenerateApostas(qtdApostas);
            var okResult2 = Controller.GetGamesList(qtdApostas).Result as OkObjectResult;

            //Assert
            Assert.IsType<NoContentResult>(okResult1);
            var items = Assert.IsType<List<ApostaVM>>(okResult2.Value);
            Assert.Equal(25, items.Count);
        }

        /// <summary>
        /// Testar Reset dos jogos
        /// </summary>
        [Fact]
        public void Aposta_ShouldResetApostas()
        {
            //Arrange
            int qtdApostas = 25;

            //Act
            var okResult1 = Controller.GenerateApostas(qtdApostas);
            var okResult2 = Controller.Reset();
            var okResult3 = Controller.GetGamesList().Result as OkObjectResult;

            //Assert
            Assert.IsType<NoContentResult>(okResult1);
            Assert.IsType<NoContentResult>(okResult2);
            var items = Assert.IsType<List<ApostaVM>>(okResult3.Value);
            Assert.Empty(items);
        }
    }
}
