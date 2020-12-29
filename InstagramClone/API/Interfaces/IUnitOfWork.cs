using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        // readonly properties, has gets only
        IUserRepository UserRepository { get; }

        IMessageRepository MessageRepository { get; }

        ILikesRepository LikesRepository { get; }

        IPhotoRepository PhotoRepository { get; }

        IPhotoCommentsRepository PhotoCommentsRepository { get; }

        IPhotoLikesRepository PhotoLikesRepository { get; }


        Task<bool> Complete();

        bool HasChanges();
    }
}
