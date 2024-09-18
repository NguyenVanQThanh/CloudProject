using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data.Repository;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class MessageHub(IUnitOfWork unitOfWork, IMapper _mapper
    , IHubContext <PresenceHub> presenceHub) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext?.Request.Query["user"];

            if (Context.User == null || string.IsNullOrEmpty(otherUser)){
                throw new Exception("Cannot join group");
            }
            var groupName = GetGroupName(Context.User.GetUserName(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddToGroup(groupName);
            await Clients.Group(groupName).SendAsync("UpdateGroup", group);

            var messages = await unitOfWork.MessageRepository.GetMessagesThread(Context.User.GetUserName(), otherUser!);
            await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
        }
        public async Task SendMessage(CreateMessageDto createMessageDto){
            var username = Context.User?.GetUserName() ?? throw new Exception("could not get user");
            if (username == createMessageDto.RecipientUsername.ToLower()){
                throw new Exception("You cannot send message yourself");
            }
            var sender = await unitOfWork.UserRepository.GetUserByUserName(username);
            var recipient = await unitOfWork.UserRepository.GetUserByUserName(createMessageDto.RecipientUsername);

            if (recipient == null || sender == null || sender.UserName == null) 
                throw new Exception("Cannot not send message this time");
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
            var groupName = GetGroupName(sender.UserName,recipient.UserName);
            var group = await unitOfWork.MessageRepository.GetMessageGroup(groupName);

            if (group != null && group.Connections.Any(x=>x.UserName == recipient.UserName)){
                message.DateRead = DateTime.UtcNow;
            }
            else{
                var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
                if (connections != null && connections?.Count != null){
                    await presenceHub.Clients.Clients(connections).SendAsync("NewMEssageReceived", 
                    new {userName = sender.UserName, knownAs = sender.KnownAs});
                }
            }
            unitOfWork.MessageRepository.AddMessage(message);
            if (await unitOfWork.Complete()){
                await Clients.Group(groupName).SendAsync("NewMessage",  _mapper.Map<MessageDto>(message));
            } 
            
        }

        public override async Task OnDisconnectedAsync(Exception? exception){
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
            await base.OnDisconnectedAsync(exception);
        }
        private async Task<Group> AddToGroup(string groupName){
            var username = Context.User?.GetUserName() ?? throw new Exception("Cannot get username");
            var group = await unitOfWork.MessageRepository.GetMessageGroup(groupName);
            var connection = new Connection{
                ConnectionId = Context.ConnectionId,
                UserName = username
            };
            if (group == null){
                group = new Group{Name=groupName};
                unitOfWork.MessageRepository.AddGroup(group);
            }
            group.Connections.Add(connection);
            if (await unitOfWork.Complete()) return group;
            throw new HubException("Failed to join group");
        }
        private async Task<Group> RemoveFromMessageGroup(){
            var group = await unitOfWork.MessageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group?.Connections.FirstOrDefault(x=>x.ConnectionId == Context.ConnectionId);
            if (connection!= null && group != null){
                unitOfWork.MessageRepository.RemoveConnection(connection);
                if (await unitOfWork.Complete()) return group;
            }
            throw new Exception("Failed to remove from group");
        }
        private string GetGroupName(string caller, string? other){
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}