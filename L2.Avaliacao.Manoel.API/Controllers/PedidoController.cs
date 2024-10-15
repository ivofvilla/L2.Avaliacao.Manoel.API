using L2.Avaliacao.Manoel.API.Domain.Commands.Pedido.Create;
using L2.Avaliacao.Manoel.API.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace L2.Avaliacao.Manoel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PedidoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("processar-pedidos")]
        public async Task<IActionResult> ProcessarPedidos([FromBody] CreatePedidoCommand pedidos, CancellationToken cancellation)
        {
            var result = await _mediator.Send(pedidos, cancellation);

            var jsonSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            return Ok(JsonConvert.SerializeObject(result, jsonSettings));
        }

    }
}
