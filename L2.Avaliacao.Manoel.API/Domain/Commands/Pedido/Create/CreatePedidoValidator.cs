using FluentValidation;
using L2.Avaliacao.Manoel.API.Models;

namespace L2.Avaliacao.Manoel.API.Domain.Commands.Pedido.Create
{
    public class CreatePedidoValidator : AbstractValidator<CreatePedidoCommand>
    {
        public CreatePedidoValidator()
        {
            RuleFor(x => x.Pedidos)
               .NotEmpty().WithMessage("A lista de pedidos não pode estar vazia.")
               .ForEach(pedido => pedido.SetValidator(new PedidoValidator()));
        }
    }

    public class PedidoValidator : AbstractValidator<Models.PedidoEntrada>
    {
        public PedidoValidator()
        {
            RuleFor(x => x.Pedido_Id).GreaterThan(0).WithMessage("O ID do pedido deve ser maior que 0.");
            RuleFor(x => x.Produtos)
                .NotEmpty().WithMessage("A lista de produtos não pode estar vazia.")
                .ForEach(produto => produto.SetValidator(new ProdutoValidator()));
        }
    }


    public class ProdutoValidator : AbstractValidator<Produto>
    {
        public ProdutoValidator()
        {
            RuleFor(x => x.Produto_Id).NotEmpty().WithMessage("O ID do produto não pode ser nulo.");
            RuleFor(x => x.Dimensoes)
                .NotNull().WithMessage("As dimensões devem ser fornecidas.")
                .SetValidator(new DimensoesValidator());
        }
    }


    public class DimensoesValidator : AbstractValidator<Dimensoes>
    {
        public DimensoesValidator()
        {
            RuleFor(x => x.Altura).GreaterThan(0).WithMessage("A altura deve ser maior que 0.");
            RuleFor(x => x.Largura).GreaterThan(0).WithMessage("A largura deve ser maior que 0.");
            RuleFor(x => x.Comprimento).GreaterThan(0).WithMessage("O comprimento deve ser maior que 0.");
        }
    }
}
