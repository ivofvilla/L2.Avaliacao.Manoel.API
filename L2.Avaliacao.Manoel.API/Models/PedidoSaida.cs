namespace L2.Avaliacao.Manoel.API.Models
{
    public class PedidoSaida
    {
        public int pedido_Id { get; set; }
        public List<Caixas> Caixas = new List<Caixas>();

    }
}
