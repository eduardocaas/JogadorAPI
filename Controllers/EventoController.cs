﻿using JogadorAPI.InputModels;
using JogadorAPI.Models;
using JogadorAPI.Services;
using JogadorAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Opw.HttpExceptions;
using System.Net.Mime;

namespace JogadorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventoController : ControllerBase
    {
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Create(
            [FromBody] EventoInputModel inputModel,
            [FromServices] MySqlConnection connection)
        {
            try
            {
                Evento evento = new Evento(
                                inputModel.IdUsuario,
                                inputModel.Cidade,
                                inputModel.Bairro,
                                inputModel.Horario,
                                inputModel.DuracaoMinutos,
                                inputModel.Descricao,
                                inputModel.Posicao);

                EventoCreateViewModel model = EventoService.Create(evento, connection);
                return Ok(model);
            }
            catch (HttpException hException)
            {
                return StatusCode((int)hException.StatusCode, new
                {
                    message = hException.Message,
                    date = DateTime.Now.ToString("dd/MM/yyyy - H:mm")
                });
            }
            catch (Exception exception)
            {
                if (exception.Message.Contains("foreign"))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = "Id de usuário inválido",
                        date = DateTime.Now.ToString("dd/MM/yyyy - H:mm")
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = exception.Message,
                    date = DateTime.Now.ToString("dd/MM/yyyy - H:mm")
                });
            }
        }
    }
}
