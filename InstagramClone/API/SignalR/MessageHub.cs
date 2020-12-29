using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public MessageHub(IMessageRepository messageRepository, IMapper mapper, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"].ToString();
            var groupName = GetGroupName(Context.User.FindFirst(ClaimTypes.Name)?.Value, otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var messages = await _unitOfWork.MessageRepository.GetMessageThread(Context.User.FindFirst(ClaimTypes.Name)?.Value, otherUser);

            await Clients.Group(groupName).SendAsync("RecieveMessageThread", messages);
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.Compare(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception); 
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context.User.FindFirst(ClaimTypes.Name)?.Value;
            if (username == createMessageDto.RecipientUsername || createMessageDto.Content.Length == 0)
            {
                throw new HubException("You cannot send a message to yourself");
            }

            AppUser sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            AppUser recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);
            if (recipient == null)
            {
                throw new HubException("Recipien cannot be found");
            }

            Message message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            _unitOfWork.MessageRepository.AddMessage(message);
            
            if (await _unitOfWork.Complete())
            {
                var group = GetGroupName(sender.UserName, recipient.UserName);
                var messageDto = _mapper.Map<Message, MessageDto>(message);
                await Clients.Group(group).SendAsync("NewMessage", messageDto);
            }
            
        }

    }
}
