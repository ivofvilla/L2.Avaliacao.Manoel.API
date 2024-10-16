using FluentValidation.Results;
using FluentValidation;
using L2.Avaliacao.Manoel.API.Domain.Commands.Pedido.Create;
using L2.Avaliacao.Manoel.API.Models;
using Moq;

namespace L2.Avaliacao.Manoel.Tests
{
    public class CreatePedidoHandleTests
    {
        private readonly Mock<IValidator<CreatePedidoCommand>> _validatorMock;
        private readonly CreatePedidoHandle _handler;

        public CreatePedidoHandleTests()
        {
            _validatorMock = new Mock<IValidator<CreatePedidoCommand>>();
            _handler = new CreatePedidoHandle(_validatorMock.Object);
        }

        [Fact]
        public async Task Handle_CommandInvalido_DeveRetornarErros()
        {
            // Arrange
            var invalidCommand = new CreatePedidoCommand();
            var validationResult = new ValidationResult(
                new List<ValidationFailure>
                {
                new ValidationFailure("Pedidos", "A lista de pedidos não pode estar vazia.")
                });

            _validatorMock.Setup(v => v.ValidateAsync(invalidCommand, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(validationResult);

            // Act
            var result = await _handler.Handle(invalidCommand, CancellationToken.None);

            // Assert
            Assert.False(result.Erros.Count == 0);
            Assert.Contains("A lista de pedidos não pode estar vazia.", result.Erros);
        }

        [Fact]
        public async Task Handle_CommandValido_EmpacotarCorretamente()
        {
            // Arrange
            var command = new CreatePedidoCommand
            {
                Pedidos = new List<PedidoEntrada>
            {
                new PedidoEntrada
                {
                    Pedido_Id = 1,
                    Produtos = new List<Produto>
                    {
                        new Produto { Produto_Id = "PS5", Dimensoes = new Dimensoes { Altura = 40, Largura = 10, Comprimento = 25 } },
                        new Produto { Produto_Id = "Volante", Dimensoes = new Dimensoes { Altura = 40, Largura = 30, Comprimento = 30 } }
                    }
                }
            }
            };

            var validationResult = new ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(validationResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Empty(result.Erros);
            Assert.Single(result.Pedidos);
            Assert.Equal("Caixa 2", result.Pedidos[0].Caixas[0].Caixa_Id);
            Assert.Contains("PS5", result.Pedidos[0].Caixas[0].Produtos);
            Assert.Contains("Volante", result.Pedidos[0].Caixas[0].Produtos);
        }

        [Fact]
        public async Task Handle_CommandValido_EmpacotarCorretamente_Pedido6()
        {
            // Arrange
            var command = new CreatePedidoCommand
            {
                Pedidos = new List<PedidoEntrada>
            {
                new PedidoEntrada
                {
                    Pedido_Id = 1,
                    Produtos = new List<Produto>
                    {
                        new Produto { Produto_Id = "Webcam", Dimensoes = new Dimensoes { Altura = 7, Largura = 10, Comprimento = 5 } },
                        new Produto { Produto_Id = "Microfone", Dimensoes = new Dimensoes { Altura = 25, Largura = 10, Comprimento = 10 } },
                        new Produto { Produto_Id = "Monitor", Dimensoes = new Dimensoes { Altura = 50, Largura = 60, Comprimento = 20 } },
                        new Produto { Produto_Id = "Notebook", Dimensoes = new Dimensoes { Altura = 2, Largura = 35, Comprimento = 25 } }
                    }
                }
            }
            };

            var validationResult = new ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(validationResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Empty(result.Erros);
            Assert.Single(result.Pedidos);
            Assert.Equal(2, result.Pedidos[0].Caixas.Count);
        }

        [Fact]
        public async Task Handle_ProductoNaoCabeNaCaixa_DevePreencherObservação()
        {
            // Arrange
            var command = new CreatePedidoCommand
            {
                Pedidos = new List<PedidoEntrada>
            {
                new PedidoEntrada
                {
                    Pedido_Id = 1,
                    Produtos = new List<Produto>
                    {
                        new Produto { Produto_Id = "Cadeira Gamer", Dimensoes = new Dimensoes { Altura = 120, Largura = 60, Comprimento = 70 } }
                    }
                }
            }
            };

            var validationResult = new ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(validationResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Empty(result.Erros);
            Assert.Single(result.Pedidos);
            Assert.Null(result.Pedidos[0].Caixas[0].Caixa_Id);
            Assert.Contains("Cadeira Gamer", result.Pedidos[0].Caixas[0].Produtos);
            Assert.Equal("Produto não cabe em nenhuma caixa disponível.", result.Pedidos[0].Caixas[0].Observacao);
        }
    }
}