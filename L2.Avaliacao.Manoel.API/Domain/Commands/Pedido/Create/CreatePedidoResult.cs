﻿namespace L2.Avaliacao.Manoel.API.Domain.Commands.Pedido.Create
{
    public class CreatePedidoResult
    {
        public List<Models.PedidoSaida> Pedidos { get; set; } = new List<Models.PedidoSaida>();

        public List<string> Erros { get; set; } = new List<string>();

    }
}
