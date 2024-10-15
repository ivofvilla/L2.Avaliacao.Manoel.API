using FluentValidation;
using L2.Avaliacao.Manoel.API.Models;
using MediatR;

namespace L2.Avaliacao.Manoel.API.Domain.Commands.Pedido.Create
{
    public class CreatePedidoHandle : IRequestHandler<CreatePedidoCommand, CreatePedidoResult>
    {
        private readonly IValidator<CreatePedidoCommand> _validator;

        public CreatePedidoHandle(IValidator<CreatePedidoCommand> validator)
        {
            _validator = validator;
        }

        public async Task<string> Handle(CreatePedidoCommand command, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return string.Empty;
            }

            var resultado = new CreatePedidoResult();

            foreach (var pedido in command.Pedidos) 
            {
                var pedidoSaida = new Models.PedidoSaida();
                pedidoSaida.pedido_Id = pedido.Pedido_Id;

                foreach (var caixa in pedido.Produtos.OrderBy(c => c.()))
                {
                    var produtosNaCaixa = new List<Produto>();

                    foreach (var produto in produtosRestantes.ToList())
                    {
                        if (produto.Dimensoes.CabeNaCaixa(caixa))
                        {
                            produtosNaCaixa.Add(produto);
                            produtosRestantes.Remove(produto);
                        }
                    }

                    if (produtosNaCaixa.Any())
                    {
                        resultadoPedido.CaixasUsadas.Add(new CaixaResultado
                        {
                            Caixa = caixa,
                            Produtos = produtosNaCaixa
                        });
                    }
                }
            }


            return string.Empty;
        }
    }
}
