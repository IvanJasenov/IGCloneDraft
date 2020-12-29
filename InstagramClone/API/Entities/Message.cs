using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Message
    {
        public int Id { get; set; }

        #region Sender
        // will make a fk based on convention
        public int SenderId { get; set; }

        public string SenderUsername { get; set; }
        // navigation property
        public AppUser Sender { get; set; }
        #endregion

        #region Recipient
        // will make a fk based on convention
        public int RecipientId { get; set; }

        public string RecipientUsername { get; set; }
        // navigation property
        public AppUser Recipient { get; set; }
        #endregion

        public string Content { get; set; }

        public DateTime? DateRead { get; set; }

        public DateTime DateSend { get; set; } = DateTime.Now;

        public bool SenderDeletedMessage { get; set; }

        public bool RecipientDeletedMessage { get; set; }
    }
}
