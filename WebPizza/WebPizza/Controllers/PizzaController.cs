﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using WebPizza.Data;
using Microsoft.EntityFrameworkCore;
using WebPizza.ViewModels.Pizza;
using WebPizza.Services.ControllerServices.Interfaces;
using WebPizza.Services.Interfaces;
using FluentValidation;
using WebPizza.ViewModels.Category;
using WebPizza.ViewModels.Ingredient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebPizza.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class PizzaController(IMapper mapper,
    IValidator<PizzaCreateVm> createValidator,
    IPizzaControllerService service,
    IPaginationService<PizzaVm, PizzaFilterVm> pagination,
    PizzaDbContext pizzaContext
    ) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var list = await pizzaContext.Pizzas
               .ProjectTo<PizzaVm>(mapper.ConfigurationProvider)
               .ToArrayAsync();

            return Ok(list);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] PizzaSearchVm search)
    {
        try
        {
            var query = pizzaContext.Pizzas.AsQueryable();

            if (search.ValuesId != null && search.ValuesId.Length > 0)
            {
                query = query.Where(p => search.ValuesId.All(id => p.Filters.Any(f => f.FilterValueId == id)));
            }

            var list = await query
                .ProjectTo<PizzaVm>(mapper.ConfigurationProvider)
                .ToArrayAsync();

            return Ok(list);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetPage([FromQuery] PizzaFilterVm vm)
    {
        try
        {
            return Ok(await pagination.GetPageAsync(vm));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var pizza = await pizzaContext.Pizzas
            .ProjectTo<PizzaVm>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (pizza is null)
            return NotFound();

        return Ok(pizza);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] PizzaCreateVm vm)
    {

        var validationResult = await createValidator.ValidateAsync(vm);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        try
        {
            await service.CreateAsync(vm);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


    [HttpPatch]
    public async Task<IActionResult> Update([FromForm] PizzaEditVm vm)
    {
        //var validationResult = await editValidator.ValidateAsync(vm);

        //if (!validationResult.IsValid)
        //{
        //    return BadRequest(validationResult.Errors);
        //}

        try
        {
            await service.UpdateAsync(vm);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await service.DeleteIfExistsAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

}
