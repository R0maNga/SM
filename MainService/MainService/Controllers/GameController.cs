﻿using AutoMapper;
using BLL.Models.Input.GameInput;
using BLL.Models.Output.GameOutput;
using BLL.Services.Interfaces;
using MainService.Models.Request.GemeRequest;
using MainService.Models.Response.GameResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IGameService _gameService;
        private readonly IMessageProducer _messageProducer;

        public GameController(IMapper mapper, IGameService gameService, IMessageProducer messageProducer)
        {
            _mapper = mapper;
            _gameService = gameService;
            _messageProducer = messageProducer;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame (CreateGameRequest game, CancellationToken token)
        {
            try
            {
                var mappedGame = _mapper.Map<CreateGameInput>(game);
                await _gameService.CreateGame(mappedGame, token);
                var gameByName = await _gameService.GetGameByName(mappedGame.Name, token);
                var mappedModel = _mapper.Map<GameOutputForGameService>(gameByName);
                _messageProducer.SendMessage(mappedModel, "games");

                return Ok("Game Created");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateGame(UpdateGameRequest game, CancellationToken token)
        {
            try
            {
                var foundGame = await _gameService.GetGameById(game.Id, token);
                if (foundGame is null)

                    return BadRequest();

                var mappedGame = _mapper.Map<UpdateGameInput>(game);
                await _gameService.UpdateGame(mappedGame, token);

                return Ok("Game Updated");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteGame(DeleteGameRequest game, CancellationToken token)
        {
            try
            {
                var foundGane = await _gameService.GetGameById(game.Id, token);
                if (foundGane is null)

                    return BadRequest();

                var mappedGame = _mapper.Map<DeleteGameInput>(foundGane);
                await _gameService.DeleteGame(mappedGame, token);

                return Ok("Game Deleted");

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGames(CancellationToken token)
        {
            try
            {
                var foundGames = await _gameService.GetGames(token);
                var mappedGames = _mapper.Map<IEnumerable<GetGameResponse>>(foundGames);

                return Ok(mappedGames);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGameById(int id, CancellationToken token)
        {
            try
            {
              var game =  await _gameService.GetGameById(id, token);

              return Ok(game);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("get-games-by-name/{name}")]
        public async Task<IActionResult> GetGameByName(string name, CancellationToken token)
        {
            try
            {
                var game = await _gameService.GetGameByName(name, token);

                return Ok(game);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
