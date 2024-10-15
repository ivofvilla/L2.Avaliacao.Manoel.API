using MediatR;

namespace L2.Avaliacao.Manoel.API.Domain.Commands.Pedido.Create
{
    public class CreatePedidoCommand : IRequest<CreatePedidoResult>
    {
        public List<Models.PedidoEntrada> Pedidos { get; set; } = new List<Models.PedidoEntrada>();
    }
}
