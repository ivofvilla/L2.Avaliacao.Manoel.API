using FluentValidation;
using L2.Avaliacao.Manoel.API.Models;
using MediatR;

namespace L2.Avaliacao.Manoel.API.Domain.Commands.Pedido.Create
{
    public class CreatePedidoHandle : IRequestHandler<CreatePedidoCommand, CreatePedidoResult?>
    {
        private readonly IValidator<CreatePedidoCommand> _validator;
        private readonly List<DimensoesCaixa> CaixasDisponiveis = new List<DimensoesCaixa>
        {
            new DimensoesCaixa { Caixa_Id = "Caixa 1", Altura = 30, Largura = 40, Comprimento = 80 },  // 9600
            new DimensoesCaixa { Caixa_Id = "Caixa 2", Altura = 80, Largura = 50, Comprimento = 40 },  // 160k
            new DimensoesCaixa { Caixa_Id = "Caixa 3", Altura = 50, Largura = 80, Comprimento = 60 }   // 240k
        };
        public CreatePedidoHandle(IValidator<CreatePedidoCommand> validator)
        {
            _validator = validator;
        }

        public async Task<CreatePedidoResult?> Handle(CreatePedidoCommand command, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            var resultado = new CreatePedidoResult();

            if (!validationResult.IsValid)
            {
                resultado.Erros.AddRange(validationResult.Errors.Select(s => s.ErrorMessage));
                return resultado;
            }

            var pedidosSaida = new List<PedidoSaida>();


            foreach (var pedido in command.Pedidos)
            {
                var pedidoSaida = new PedidoSaida { pedido_Id = pedido.Pedido_Id };
                var produtosRestantes = new List<Produto>(pedido.Produtos);

                foreach (var caixa in CaixasDisponiveis)
                {
                    var produtosNaCaixa = new List<string>();
                    for (int i = produtosRestantes.Count - 1; i >= 0; i--)
                    {
                        var produto = produtosRestantes[i];
                        if (produto.Dimensoes.CabeNaCaixa(caixa))
                        {
                            produtosNaCaixa.Add(produto.Produto_Id);
                            produtosRestantes.RemoveAt(i);
                        }
                    }

                    if (produtosNaCaixa.Any())
                    {
                        pedidoSaida.Caixas.Add(new Caixa
                        {
                            Caixa_Id = caixa.Caixa_Id,
                            Produtos = produtosNaCaixa
                        });
                    }
                }

                foreach (var produto in produtosRestantes)
                {
                    pedidoSaida.Caixas.Add(new Caixa
                    {
                        Caixa_Id = null,
                        Produtos = new List<string> { produto.Produto_Id },
                        Observacao = "Produto não cabe em nenhuma caixa disponível."
                    });
                }

                resultado.Pedidos.Add(pedidoSaida);
            }

            return resultado;
        }

        static bool ProdutoCabeNaCaixa(Produto produto, Dimensoes caixa)
        {
            return produto.Dimensoes.Altura <= caixa.Altura && produto.Dimensoes.Largura <= caixa.Largura && produto.Dimensoes.Comprimento <= caixa.Comprimento;
        }
    }
}