using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessageController(IUnitOfWork unitOfWork, IMapper _mapper) : BaseApiController
    {
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto){
            var username = User.GetUserName();
            if (username == createMessageDto.RecipientUsername.ToLower()){
                return BadRequest("You cannot send a message to yourself");
            }
            var sender = await unitOfWork.UserRepository.GetUserByUserName(username);
            var recipient = await unitOfWork.UserRepository.GetUserByUserName(createMessageDto.RecipientUsername);

            if (recipient == null) return NotFound("Couldn't find");
            var message = new Message{
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content,
                SenderDeleted = false,
                RecipientDeleted = false,
                DateRead = null
            };
            unitOfWork.MessageRepository.AddMessage(message);
            if (await unitOfWork.Complete()) return Ok(_mapper.Map<MessageDto>(message));
            return BadRequest("Failed to send message");
        }
        [HttpGet]
        public async Task<ActionResult<List<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams){
            messageParams.Username = User.GetUserName();
            var messages = await unitOfWork.MessageRepository.GetMessageForUser(messageParams);
            Response.AddPaginationHeader( new PaginationHeader(messages.CurrentPage, messages.PageSize
            , messages.TotalCount, messages.TotalPages));
            return Ok(messages);
        }
        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username){
            var currentUsername = User.GetUserName();
            return Ok(await unitOfWork.MessageRepository.GetMessagesThread(currentUsername,username));
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id){
            var username = User.GetUserName();
            var message = await unitOfWork.MessageRepository.GetMessage(id);
            if (message.SenderUsername != username && message.RecipientUsername != username){
                return Unauthorized();
            }
            if (message.SenderUsername == username) message.SenderDeleted = true;
            if (message.RecipientUsername == username) message.RecipientDeleted = true;
            if (message.SenderDeleted && message.RecipientDeleted){
                unitOfWork.MessageRepository.DeleteMessage(message);
            }
            if (await unitOfWork.Complete()) return Ok();
            return BadRequest("Fail to delete message");
        }
    }
}