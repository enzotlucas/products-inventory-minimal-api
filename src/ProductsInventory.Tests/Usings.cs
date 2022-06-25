global using FluentAssertions;
global using FluentValidation;
global using FluentValidation.Results;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.Logging;
global using NSubstitute;
global using ProductsInventory.API.Application.Commands;
global using ProductsInventory.API.Application.Extensions;
global using ProductsInventory.API.Core.Entities;
global using ProductsInventory.API.Core.Exceptions;
global using ProductsInventory.API.Core.Repositories;
global using ProductsInventory.API.Core.ValueObjects;
global using ProductsInventory.Tests.Mocks;
global using System.Net;
global using System.Security.Claims;
global using Xunit;