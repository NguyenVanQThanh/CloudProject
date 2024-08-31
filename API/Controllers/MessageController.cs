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
    public class MessageController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;

        public MessageController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper){
            _mapper = mapper;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto){
            var username = User.GetUserName();
            if (username == createMessageDto.RecipientUsername.ToLower()){
                return BadRequest("You cannot send a message to yourself");
            }
            var sender = await _userRepository.GetUserByUserName(username);
            var recipient = await _userRepository.GetUserByUserName(createMessageDto.RecipientUsername);

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
            _messageRepository.AddMessage(message);
            if (await _messageRepository.SaveChangesAsync()) return Ok(_mapper.Map<MessageDto>(message));
            return BadRequest("Failed to send message");
        }
        [HttpGet]
        public async Task<ActionResult<List<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams){
            messageParams.Username = User.GetUserName();
            var messages = await _messageRepository.GetMessageForUser(messageParams);
            Response.AddPaginationHeader( new PaginationHeader(messages.CurrentPage, messages.PageSize
            , messages.TotalCount, messages.TotalPages));
            return Ok(messages);
        }
        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username){
            var currentUsername = User.GetUserName();
            return Ok(await _messageRepository.GetMessagesThread(currentUsername,username));
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id){
            var username = User.GetUserName();
            var message = await _messageRepository.GetMessage(id);
            if (message.SenderUsername != username && message.RecipientUsername != username){
                return Unauthorized();
            }
            if (message.SenderUsername == username) message.SenderDeleted = true;
            if (message.RecipientUsername == username) message.RecipientDeleted = true;
            if (message.SenderDeleted && message.RecipientDeleted){
                _messageRepository.DeleteMessage(message);
            }
            if (await _messageRepository.SaveChangesAsync()) return Ok();
            return BadRequest("Fail to delete message");
        }
    }
}