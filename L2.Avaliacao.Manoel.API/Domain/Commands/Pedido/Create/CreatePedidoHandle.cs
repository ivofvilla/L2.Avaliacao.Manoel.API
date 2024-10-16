using FluentValidation;
using L2.Avaliacao.Manoel.API.Models;
using MediatR;

namespace L2.Avaliacao.Manoel.API.Domain.Commands.Pedido.Create
{
    public class CreatePedidoHandle : IRequestHandler<CreatePedidoCommand, CreatePedidoResult>
    {
        private readonly IValidator<CreatePedidoCommand> _validator;
        private readonly List<DimensoesCaixa> CaixasDisponiveis = new List<DimensoesCaixa>
        {
            new DimensoesCaixa { Caixa_Id = "Caixa 1", Altura = 30, Largura = 40, Comprimento = 80 },  // 9600
            new DimensoesCaixa { Caixa_Id = "Caixa 2", Altura = 80, Largura = 50, Comprimento = 40 },  // 160k
            new DimensoesCaixa { Caixa_Id = "Caixa 3", Altura = 50, Largura = 80, Comprimento = 60 }   // 240k
        };
        //pedido 4 caixa 1 mouse e caixa 2 teclado
        //{"produto_id": "Mouse Gamer", "dimensoes": {"altura": 5, "largura": 8, "comprimento": 12}},
        //{"produto_id": "Teclado Mecânico", "dimensoes": {"altura": 4, "largura": 45, "comprimento": 15}}

        //pedido 4 caixa 1 notebook, microfot e web / gabarito
        //caixa 3 monitor
        //3
        //{"produto_id": "Webcam", "dimensoes": {"altura": 7, "largura": 10, "comprimento": 5}},
        //{"produto_id": "Microfone", "dimensoes": {"altura": 25, "largura": 10, "comprimento": 10}},
        //
        //1
        //{ "produto_id": "Monitor", "dimensoes": { "altura": 50, "largura": 60, "comprimento": 20} },
        //{ "produto_id": "Notebook", "dimensoes": { "altura": 2, "largura": 35, "comprimento": 25} }

        public CreatePedidoHandle(IValidator<CreatePedidoCommand> validator)
        {
            _validator = validator;
        }

        public async Task<CreatePedidoResult> Handle(CreatePedidoCommand command, CancellationToken cancellationToken = default)
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
                PedidoSaida pedidoSaida = new PedidoSaida { pedido_Id = pedido.Pedido_Id };
            
                var produtosOrdenados = pedido.Produtos.OrderByDescending(p => p.Dimensoes.Volume()).ToList();
            
                foreach (var produto in produtosOrdenados)
                {
                    bool produtoEmpacotado = false;
            
                    foreach (var caixa in pedidoSaida.Caixas)
                    {
                        var dimensoesCaixa = CaixasDisponiveis.First(c => c.Caixa_Id == caixa.Caixa_Id);
                        if (produto.Dimensoes.CabeNaCaixa(dimensoesCaixa))
                        {
                            caixa.Produtos.Add(produto.Produto_Id);
                            produtoEmpacotado = true;
                            break;
                        }
                    }
            
                    if (!produtoEmpacotado)
                    {
                        var novaCaixa = CaixasDisponiveis.FirstOrDefault(c => produto.Dimensoes.CabeNaCaixa(c));
                        if (novaCaixa != null)
                        {
                            pedidoSaida.Caixas.Add(new Caixa
                            {
                                Caixa_Id = novaCaixa.Caixa_Id,
                                Produtos = new List<string> { produto.Produto_Id }
                            });
                        }
                        else
                        {
                            pedidoSaida.Caixas.Add(new Caixa
                            {
                                Caixa_Id = null,
                                Produtos = new List<string> { produto.Produto_Id },
                                Observacao = "Produto não cabe em nenhuma caixa disponível."
                            });
                        }
                    }
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