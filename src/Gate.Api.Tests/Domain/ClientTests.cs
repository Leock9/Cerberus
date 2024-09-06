using Bogus;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Gate.Api.Domain;

namespace Gate.Api.Tests.Domain;

public class ClientTests
{
    [Fact]
    public void CreateClient()
    {
        var faker = new Faker("pt_BR");
        var cpfWithFormat = faker.Person.Cpf();
        var name = faker.Person.FullName;
        var email = faker.Person.Email;
        var address = faker.Address.ToString();

        var client = new Client(name, cpfWithFormat!, email, address!);

        client.Should()
                  .Match<Client>(c => c.Name == name)
              .And.Match<Client>(c => c.Document == cpfWithFormat)
              .And.NotBeNull();
    }

    [Fact]
    public void CreateClientWhenNameIsEmpty()
    {
        var faker = new Faker("pt_BR");
        var cpfWithFormat = faker.Person.Cpf();
        var name = string.Empty;
        var email = faker.Person.Email;
        var address = faker.Address.ToString();

        Action action = () =>
        {
            new Client(name, cpfWithFormat!, email, address!);
        };

        action.Should()
              .Throw<DomainException>()
              .WithMessage("Name is required");
    }

    [Fact]
    public void CreateClientWhenDocumentIsInvalid()
    {
        var faker = new Faker("pt_BR");
        var cpfWithFormat = faker.Person.DateOfBirth.ToString();
        var name = faker.Person.FullName;
        var email = faker.Person.Email;
        var address = faker.Address.ToString();

        Action action = () =>
        {
            new Client(name, cpfWithFormat!, email, address!);
        };

        action.Should()
              .Throw<DomainException>()
              .WithMessage("Document is invalid");
    }

    [Fact]
    public void CreateClientWhenEmailIsEmpty()
    {
        var faker = new Faker("pt_BR");
        var cpfWithFormat = faker.Person.Cpf();
        var name = faker.Person.FullName;
        var email = string.Empty;
        var address = faker.Address.ToString();

        Action action = () =>
        {
            new Client(name, cpfWithFormat!, email, address!);
        };

        action.Should()
              .Throw<DomainException>()
              .WithMessage("Email is invalid");
    }

    [Fact]
    public void CreateClientWhenEmailIsInvalid()
    {
        var faker = new Faker("pt_BR");
        var cpfWithFormat = faker.Person.Cpf();
        var name = faker.Person.FullName;
        var email = faker.Person.FullName;
        var address = faker.Address.ToString();

        Action action = () =>
        {
            new Client(name, cpfWithFormat!, email, address, address);
        };

        action.Should()
              .Throw<DomainException>()
              .WithMessage("Email is invalid");
    }

    [Fact]
    public void CreateClientWhenAddressIsEmpty()
    {
        var faker = new Faker("pt_BR");
        var cpfWithFormat = faker.Person.Cpf();
        var name = faker.Person.FullName;
        var email = faker.Person.Email;
        var address = string.Empty;

        Action action = () =>
        {
            new Client(name, cpfWithFormat!, email, address);
        };

        action.Should()
              .Throw<DomainException>()
              .WithMessage("Address is required");
    }
}
