using MeuPrimeiroMvc.Contexts;
using MeuPrimeiroMvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace MeuPrimeiroMvc.Controllers
{
    [Route("[controller]")]

    public class EquipeController : Controller

    {

        // Criar uma referência (insstância) sobre a comunicação do meu banco de dado

        ProjetoTesteContext context = new ProjetoTesteContext();
        public IActionResult Index()
        {
            // Forma de lista todos os itens da tabela de (Equipe)
            var listaEquipes = context.Equipes.ToList();

            // Passar  a tela (em forma de memoria) os dados das equipes cadastras
            ViewBag.listaEquipes = listaEquipes;

            return View();
        }

        [Route("Cadastrar")]
        public IActionResult CadastrarEquipe(IFormCollection formEquipe) // Recebendo dados no padrão FormData - para trabalhar com arquivos
        {
            // ALO ALO 
            if (formEquipe.Files.Count > 0)
            {
                /* Recebendo o arquivo anexado */
                var arquivoAnexado = formEquipe.Files[0]; // Dentro da possibilidade de receber vários arquivos, estamos recebendo apenas o primeiro (único)

                var pastaArmazenamento = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/equipe");
                // Directory.GetcurrentDirectory - Função para pegar a localização da pasta projeto
                /* Criar a pasta "wwwroot" - é o local configurado para acessar arquivos do navegador */

                // Verifiquei se a pasta de armazenamento não existe
                if (!Directory.Exists(pastaArmazenamento))
                {
                    // Caso não exista - o projeto fica responsável por criar essa pasta
                    Directory.CreateDirectory(pastaArmazenamento);
                }

                /* Passando a localização da pasta de armazenamento + o nome do arquivo a ser salvo*/
                var arquivoArmazenado = Path.Combine(pastaArmazenamento, arquivoAnexado.FileName);

                // Chamamos uma função do C# para a criação de arquivo - dentro da pasta de armazenamento
                using (var stream = new FileStream(arquivoArmazenado, FileMode.Create))
                {
                    // Para esse novo arquivo, copiamos o conteudo do arquivo anexado
                    arquivoAnexado.CopyTo(stream);
                }

                // Criando o objeto de equipe para cadastro
                Equipe equipe = new Equipe()
                {
                    Nome = formEquipe["Nome"],
                    Imagem = arquivoAnexado.FileName
                };

                // Armazenar a equipe no banco de dados
                context.Add(equipe);
            }

            else
            {
                // Criando o objeto de equipe para cadastro
                Equipe equipe = new Equipe()
                {
                    Nome = formEquipe["Nome"],
                    Imagem = "padrão.jpg"
                };
                // Armazenar a equipe no banco de dados
                context.Add(equipe);
            }

            //Registrar as alterações no banco de dados
            context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Na rota de excluir, vamos capturar o id que vem na url
        [Route("ExcluirEquipe/{idEquipe}")]
        public IActionResult ExcluirEquipe(int idEquipe)
        {
            //Verificar se existe jogadores que contenham o vinculo com a equipe
            List<Jogador>listaJogadores = context.Jogadors.Where(x => x.IdEquipe == idEquipe).ToList();

            if (listaJogadores.Count > 0)
            {
                //Remover todos os jogadores vinculados 
                foreach (Jogador jgd in listaJogadores)
                {
                    context.Remove(jgd);
                }

                //Salvando a remoção dos jogadores
                context.SaveChanges();
            }

            //Pegar o id de referência, e vou procurar a equipe no banco de dados
                Equipe equipe = context.Equipes.FirstOrDefault(x => x.Id == idEquipe); // como se fosse um select * from Equipe where id == (valor da equipe da tabela)

            context.Remove(equipe);

            context.SaveChanges();

            return RedirectToAction("Index");
        }

        [Route("Atualizar/{idEquipe}")]
        public IActionResult Atualizar(int idEquipe)
        {
            Equipe equipe = context.Equipes.FirstOrDefault(x => x.Id == idEquipe);

            ViewBag.Equipe = equipe;

            return View();
        }

        [Route("AtualizarEquipe")]
        public IActionResult AtualizarEquipe(Equipe equipe)
        {
            context.Equipes.Update(equipe);

            context.SaveChanges();

            return RedirectToAction("Index");
        }
    }  
}