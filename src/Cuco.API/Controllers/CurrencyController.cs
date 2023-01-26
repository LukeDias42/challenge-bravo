using Cuco.Application.Contracts.Requests;
using Cuco.Application.Contracts.Responses;
using Cuco.Application.Services;
using Cuco.Domain.Currencies.Extensions;
using Cuco.Domain.Currencies.Models.DTOs;
using Cuco.Domain.Currencies.Services.Repositories;
using Cuco.Domain.Roles.Models.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cuco.API.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/currency")]
public class CurrencyController : ControllerBase
{
    private readonly ICurrencyRepository _currencyRepository;

    public CurrencyController(ICurrencyRepository currencyRepository)
    {
        _currencyRepository = currencyRepository;
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<CurrencyDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAllAsync()
    {
        try
        {
            var currencies = await _currencyRepository.GetAllDtoAsync();
            if (currencies is null || !currencies.Any())
                return StatusCode(StatusCodes.Status500InternalServerError);
            return Ok(currencies);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("{symbol}")]
    [ProducesResponseType(typeof(CurrencyDto), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetBySymbolValueAsync(string symbol)
    {
        try
        {
            var currency = (await _currencyRepository.GetBySymbolAsNoTrackingAsync(symbol))?.ToDto();
            if (currency is null)
                return StatusCode(StatusCodes.Status500InternalServerError);
            return Ok(currency);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(typeof(SaveCurrencyResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult> AddAsync(
        [FromServices] IAddCurrencyService service,
        [FromBody] SaveCurrencyRequest request)
    {
        try
        {
            var response = await service.AddCurrency(request);
            if (response is null)
                return StatusCode(StatusCodes.Status500InternalServerError);
            return Ok(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(typeof(SaveCurrencyResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult> UpdateAsync(
        [FromServices] IUpdateCurrencyService service,
        [FromBody] SaveCurrencyRequest request)
    {
        try
        {
            var response = await service.UpdateCurrency(request);
            if (response is null)
                return StatusCode(StatusCodes.Status500InternalServerError);
            return Ok(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete("{symbol}")]
    [Authorize(Roles = $"{RoleNames.Admin}")]
    [ProducesResponseType(typeof(DeleteCurrencyResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteAsync(
        [FromServices] IDeleteCurrencyService service,
        string symbol)
    {
        try
        {
            var response = await service.DeleteCurrency(symbol);
            if (response is null)
                return StatusCode(StatusCodes.Status500InternalServerError);
            return Ok(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("sync")]
    [ProducesResponseType(typeof(SyncCurrenciesResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult> SyncCurrenciesAsync(
        [FromServices] ISyncCurrenciesService service)
    {
        try
        {
            var result = await service.SyncCurrencies();
            return Ok(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}