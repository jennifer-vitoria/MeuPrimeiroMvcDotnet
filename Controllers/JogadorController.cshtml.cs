using MeuPrimeiroMvc.Contexts;
using MeuPrimeiroMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MeuPrimeiroMvc.Controllers
{
    [Route("[controller]")]
    public class JogadorController : Controller
    {
        ProjetoTesteContext context = new ProjetoTesteContext();


        // Construtor com injeção de dependência

        public IActionResult Index()
        {

            // .Include() - trago os dados das tabelas relacionadas
            var listaJogador = context.Jogadors.Include("IdEquipeNavigation").ToList();
            ViewBag.listaJogador = listaJogador;

            // Passando também a listas de equipes para montar o meu select
            var listaEquipes = context.Equipes.ToList();

            ViewBag.ListaEquipes = listaEquipes;

            return View();
        }

        [Route("Cadastrar")]
        public IActionResult CadastrarJogador(Jogador jogador)
        {
            context.Add(jogador);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Route("ExcluirJogador/{idJogador}")]
        public IActionResult ExcluirJogador(int idJogador)
        {
            //Pegar o id de referência, e vou procurar a equipe no banco de dados
            Jogador jogador = context.Jogadors.FirstOrDefault(x => x.Id == idJogador); // como se fosse um select * from Equipe where id == (valor da equipe da tabela)

            context.Remove(jogador);

            context.SaveChanges();

            return RedirectToAction("Index");
        }
        
         [Route("Atualizar/{idJogador}")]
        public IActionResult Atualizar(int idJogador)
        {
            Jogador jogador = context.Jogadors.FirstOrDefault(x => x.Id == idJogador);

            ViewBag.Jogador = jogador;

            return View();
        }

        [Route("AtualizarJogador")]
        public IActionResult AtualizarJogador(Jogador jogador)
        {
            context.Jogadors.Update(jogador);

            context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}