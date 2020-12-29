using API.DTOs;
using API.Entities;
using API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);

        void DeleteMessage(Message message);

        Task<Message> GetMessageById(int id);

        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
            
        Task<IEnumerable<MessageDto>> GetMessageThread(int currentUserId, int recipientId);

        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername);

        //Task<bool> SaveAllAsync();
    }
}
