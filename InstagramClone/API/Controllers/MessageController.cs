using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class MessageController : BaseApiController
    {
      
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MessageController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = HttpContext.GetUsername();
            if (username == createMessageDto.RecipientUsername || createMessageDto.Content.Length == 0)
            {
                return BadRequest("Cannot send email to yourself");
            }

            AppUser sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            AppUser recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);
            if (recipient == null)
            {
                return NotFound("Recipien cannot be found");
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
                var messageDto = _mapper.Map<Message, MessageDto>(message);
                return Ok(messageDto);
            }
            return BadRequest("Failed to send message");
        }
        // {{url}}/messages and in params tab in Postman add parameters from MessageParams object
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            // fetch the username form the token
            messageParams.Username = HttpContext.GetUsername();
            var messages = await _unitOfWork.MessageRepository.GetMessagesForUser(messageParams);
            // set the respons epagination header
            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);
            if (messages.ListOfItems != null)
            {
                return Ok(messages.ListOfItems);
            }
            return NotFound("No messages found for the user");
        }
        // {{url}}/message/message-thread/2/13
        [HttpGet("message-thread/{senderId:int}/{recieverId:int}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> MessageThread(int senderId, int recieverId)
        {
            int userId = HttpContext.GetUserId();
            if (userId != senderId) return BadRequest("Bad user");
            if (senderId != recieverId)
            {
                IEnumerable<MessageDto> messageThreadDtos = await _unitOfWork.MessageRepository.GetMessageThread(senderId, recieverId);
                if (messageThreadDtos.Count() > 0)
                {
                    return Ok(messageThreadDtos);
                }
                return NotFound("No message thred for the users");
            }

            return BadRequest("Error in messageThread");
        }
        // we intenionally skiped adding Pagination for this request, you can do it if you want
        //{{url}}/message/thread/bruce
        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> MessageThread(string username)
        {
            string logedUsername = HttpContext.GetUsername();

            IEnumerable<MessageDto> messageThreadDtos = await _unitOfWork.MessageRepository.GetMessageThread(logedUsername, username);

            return Ok(messageThreadDtos);
            // dont throw error response if there are not messages with the user
        }

        // {{url}}/message/11
        [HttpDelete("{messageId:int}")]
        public async Task<ActionResult> DeleteMessage(int messageId)
        {
            string username = HttpContext.GetUsername();
            int Id = messageId;
            Message messageToDelete = await _unitOfWork.MessageRepository.GetMessageById(messageId);
            // run some checks
            if (messageToDelete.SenderUsername != username && messageToDelete.RecipientUsername != username)
            {
                return Unauthorized("You're unautjorized to delete this message");
            }

            if (messageToDelete.SenderUsername == username)
            {
                messageToDelete.SenderDeletedMessage = true;
            }

            if (messageToDelete.RecipientUsername == username)
            {
                messageToDelete.RecipientDeletedMessage = true;
            }

            // if both deleted the message, then we can delete it from the server
            if (messageToDelete.SenderDeletedMessage && messageToDelete.RecipientDeletedMessage)
            {
                _unitOfWork.MessageRepository.DeleteMessage(messageToDelete);
            }

            if (await _unitOfWork.Complete())
            {
                return Ok(new { message = true});
            }

            return BadRequest("Problem deleting message");
           
        }

        
    }
}
