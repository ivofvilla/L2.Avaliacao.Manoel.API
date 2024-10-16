namespace L2.Avaliacao.Manoel.API.Models
{
    public class Dimensoes
    {
        public int Altura { get; set; }
        public int Largura { get; set; }
        public int Comprimento { get; set; }

        public int Volume() => this.Altura * this.Largura * this.Comprimento;

        public bool CabeNaCaixa(Dimensoes caixa)
        {
            return Altura <= caixa.Altura && Largura <= caixa.Largura && Comprimento <= caixa.Comprimento;
        }
    }
}
