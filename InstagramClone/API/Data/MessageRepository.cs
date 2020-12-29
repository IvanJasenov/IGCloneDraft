using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async void AddMessage(Message message)
        {
            await _context.AddAsync(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessageById(int id)
        {
            return await _context.Messages
                                .Include(el => el.Recipient)
                                .Include(el => el.Sender)
                                .SingleOrDefaultAsync(el => el.Id == id) ;
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            // order by most recently send first
            var query = _context.Messages
                                .OrderByDescending(m => m.DateSend)
                                .AsQueryable();

            // read on unread messages
            query = messageParams.Container switch
            {
                "Inbox" => query.Where(el => el.RecipientUsername == messageParams.Username && el.RecipientDeletedMessage == false),
                "Outbox" => query.Where(el => el.SenderUsername == messageParams.Username && el.SenderDeletedMessage == false),
                _ => query.Where(el => el.RecipientUsername == messageParams.Username && el.DateRead == null && el.RecipientDeletedMessage == false)
            };

            // projection from one type to another from Queryable types
            var messagesDtoQuery = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            var pagedListDtos = await PagedList<MessageDto>.CreateAsync(messagesDtoQuery, messageParams.PageNumber, messageParams.ItemsPerPage);

            return pagedListDtos;
        }
        // this with Id was initial version
        public async Task<IEnumerable<MessageDto>> GetMessageThread(int currentUserId, int recipientId)
        {
            var query = _context.Messages
                                .Where(el => el.SenderId == currentUserId && el.RecipientId == recipientId && el.RecipientDeletedMessage == false
                                        ||
                                        el.SenderId == recipientId && el.RecipientId == currentUserId && el.SenderDeletedMessage == false)
                                .OrderByDescending(m => m.DateSend)
                                .AsQueryable();
            // I added this line for changing the DateRead
            await query.ForEachAsync(el => el.DateRead = DateTime.Now);
            await _context.SaveChangesAsync();

            var messagesDtoQueary = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            IEnumerable<MessageDto> messagesDtos = await messagesDtoQueary.ToListAsync();
            
            return messagesDtos;
        }
        // this is from the lecture-like approach, lecture #186
        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
        {
            var query = _context.Messages
                               .Where(el => el.SenderUsername == currentUserName && el.RecipientUsername == recipientUserName && el.RecipientDeletedMessage == false 
                                            || 
                                            el.SenderUsername == recipientUserName && el.RecipientUsername == currentUserName && el.SenderDeletedMessage == false)
                               .OrderByDescending(m => m.DateSend)
                               .AsQueryable();
            // check for unred messages
            await query.ForEachAsync(el => el.DateRead = DateTime.Now);
            await _context.SaveChangesAsync();

            var messagesDtoQuery = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            IEnumerable<MessageDto> messagesDtos = await messagesDtoQuery.ToListAsync();

            return messagesDtos;
        }

        //public async Task<bool> SaveAllAsync()
        //{
        //    return await _context.SaveChangesAsync() > 0 ? true : false;
        //}
    }
}
