using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;

using Lottery.Models;
using Lottery.ViewModel;
using Lottery.Models.Base;
using Lottery.Helpers;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace Lottery.Controllers
{
    [Route("api/[controller]/[action]")]
    public class LotteryController : Controller
    {
        private readonly IMemoryCache cache;
        private readonly IMapper _mapper;

        public LotteryController(IMemoryCache cache, IMapper mapper)
        {
            this.cache = cache;
            this._mapper = mapper;
        }

        // API para gerar N quantidades de apostas para testar
        // GET: api/Lottery/GenerateApostas/{quantity}
        [HttpGet]
        public ActionResult GenerateApostas(int quantity)
        {
            var gameType = new MegaSena();
            var Jogos = new List<Aposta>(quantity);

            for (int i = 1; i <= quantity; i++)
            {
                var aposta = new Aposta()
                {
                    Id = i,
                    Combinations = gameType.GenerateNumbers(),
                    Name = "Teste",
                    LotteryType = gameType,
                    TimeStamp = DateTime.Now
                };

                Jogos.Add(aposta);
            }

            cache.Set("apostas", Jogos);

            return NoContent();
        }

        // Listar apostas já feitas, padrão limita ás ultimas 50 apostas
        // GET: api/Lottery/GetGamesList
        [HttpGet]
        public ActionResult<IEnumerable<Aposta>> GetGamesList(int limit = 50)
        {
            var apostas = cache.Get<List<Aposta>>("apostas");
            if(apostas != null)
                apostas = apostas.OrderByDescending(x => x.TimeStamp).Take(limit).ToList();

            var result = _mapper.Map<List<ApostaVM>>(apostas);

            return Ok(result);
        }

        // Apagar lista de apostas para realizar novo sorteio
        // GET: api/Lottery/Reset
        [HttpGet]
        public ActionResult Reset()
        {
            cache.Set("apostas", new List<Aposta>(10));

            var apostas = cache.Get<List<Aposta>>("apostas");

            return NoContent();
        }

        // Realiza o sorteio. Retorna os números sorteados e a lista de jogos com 4, 5 ou 6 acertos.
        // POST api/Lottery/Sortear/{type}
        [HttpPost]
        public ActionResult<ResultViewModel> Sortear([FromBody] SorteioViewModel vm)
        {
            try
            {
                ILotteryGame gameType = Utils.GetGameType(vm.GameType);

                var Jogos = cache.Get<List<Aposta>>("apostas");
                if (Jogos == null || Jogos.Count == 0)
                    return Ok(new ResultViewModel() { Error = "Não existe apostas cadastradas", Sorteio = string.Empty, Vencedores = null });

                //Gerar a combinação do sorteio
                var sorteio = gameType.GenerateNumbers();

                //Processar resultados
                List<AcertosVM> resultados = new List<AcertosVM>();
                foreach (var item in Jogos)
                {
                    var resultado = Utils.ValidarSorteio(item, sorteio);
                    if (resultado != null)
                        resultados.Add(resultado);
                }

                //Ordernar pela quantidade de acertos
                resultados = resultados.OrderByDescending(x => x.TotalAcertos).ToList();

                return Ok(new ResultViewModel() { Error = string.Empty, Sorteio = string.Join('-', sorteio), Vencedores = resultados });
            }
            catch (Exception ex)
            {
                return Ok(new ResultViewModel() { Error = ex.Message, Sorteio = string.Empty, Vencedores = null });
            }
   
        }

        // Registrar uma aposta, retorna o ID da aposta, os números caso seja uma aposta automática ou lista de erros.
        // POST api/Lottery/Apostar
        [HttpPost]
        public ActionResult<dynamic> Apostar([FromBody] ApostaViewModel vm)
        {
            try
            {
                //Recuperar os jogos anteriores e o ultimo ID
                var apostas = cache.Get<List<Aposta>>("apostas");
                if (apostas == null)
                    apostas = new List<Aposta>(10);

                int lastID = apostas.Count == 0 ? 0 : apostas.Last().Id;

                ILotteryGame gameType = Utils.GetGameType(vm.GameType);

                //Se o array viar vazio, trata-se de requisição para gerar uma aposta automática
                List<int> combinations; List<string> errors;
                if (vm.Automatic)
                    vm.Combinations = gameType.GenerateNumbersString();

                //Validar se a aposta é válida
                (errors,combinations) = ValidateAposta(vm, gameType);

                if(errors.Any())
                    return Ok(new ApostarViewModel() { Error = string.Join(',', errors), ListApostas = null });

                //Registra a aposta e devolve o ID e os números (para o usuário saber eles quando é automático)
                var aposta = new Aposta()
                {
                    Id = lastID + 1,
                    Name = vm.Name,
                    Combinations = combinations,
                    LotteryType = gameType,
                    TimeStamp = DateTime.Now
                };

                //Gravar a aposta em memória
                apostas.Add(aposta);
                cache.Set("apostas", apostas);

                if (apostas != null)
                    apostas = apostas.OrderByDescending(x => x.TimeStamp).Take(50).ToList();

                var result = _mapper.Map<List<ApostaVM>>(apostas);
                return Ok(new ApostarViewModel() { Error = string.Empty, ListApostas = result });
            }
            catch (Exception ex)
            {
                return Ok(new ApostarViewModel() { Error = ex.Message, ListApostas = null });
            }
        }

        //Validar se a aposta é válida
        private (List<string>,List<int>) ValidateAposta(ApostaViewModel aposta, ILotteryGame gameType)
        {
            List<string> errors = new List<string>(); List<int> combinations = new List<int>();

            try
            {
                if(string.IsNullOrEmpty(aposta.Name))
                    errors.Add("'Nome' não deve ser nulo!");
                else if (aposta.Name.Length > 20)
                    errors.Add("'Nome' deve ter até 20 caracteres");

                if (string.IsNullOrEmpty(aposta.Combinations))
                    errors.Add("Aposta vazia!");
                else
                {
                    //Fazer o parse para array de inteiros
                    combinations = aposta.Combinations.Split('-').Select(int.Parse).ToList();

                    //Validar se os números são todos diferentes
                    if (combinations.Any(x => combinations.Count(y => x == y) > 1))
                        errors.Add("A aposta tem que ter números diferentes!");

                    //Validar se os números estão no intervalo válido
                    if (combinations.Any(x => x < gameType.MinValue || x > gameType.MaxValue))
                        errors.Add(string.Format("Os números da aposta tem que estar entre {0} e {1}!", gameType.MinValue, gameType.MaxValue));

                    //Validar a quantidade de números
                    if (combinations.Count != gameType.Size)
                        errors.Add(string.Format("A aposta deve conter apenas {0} números!", gameType.Size));
                }
            }
            catch (FormatException)
            {
                errors.Add("Apenas números separados por traços!");
            }

            return (errors, combinations);
        }
    }
}
