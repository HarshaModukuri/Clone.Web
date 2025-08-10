using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using OktaClone.Web.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using OktaClone.Web.Models;
using System.Collections.Generic;

namespace OktaClone.Web.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatHub(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public override async Task OnConnectedAsync()
        {
            var user = await _userManager.GetUserAsync(Context.User);
            if (user != null)
            {
                await Clients.Caller.SendAsync("SetUserName", user.UserName);
            }
            await base.OnConnectedAsync();
        }

        public async Task<string> SendMessage(string senderId, string receiverId, string message)
        {
            Console.WriteLine($"SendMessage received: SenderId={senderId}, ReceiverId={receiverId}, Message={message}");
            var sender = await _userManager.FindByIdAsync(senderId);
            var receiver = await _userManager.FindByIdAsync(receiverId);

            if (sender == null)
            {
                Console.WriteLine($"Error: Sender not found for senderId: {senderId}");
                return "Error: Sender not found.";
            }
            if (receiver == null)
            {
                Console.WriteLine($"Error: Receiver not found for receiverId: {receiverId}");
                return "Error: Receiver not found.";
            }

            var chatMessage = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message,
                Timestamp = System.DateTime.UtcNow,
                IsRead = false // Mark as unread by default
            };

            _context.ChatMessages.Add(chatMessage);
            try
            {
                await _context.SaveChangesAsync();

                // Send message to sender
                if (sender?.UserName != null && receiver?.UserName != null)
                {
                    await Clients.User(senderId).SendAsync("ReceiveMessage", sender.UserName, receiver.UserName, message, chatMessage.Timestamp);
                }
                // Send message to receiver
                if (sender?.UserName != null && receiver?.UserName != null)
                {
                    await Clients.User(receiverId).SendAsync("ReceiveMessage", sender.UserName, receiver.UserName, message, chatMessage.Timestamp);
                }
                return "Success";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving message or sending via SignalR: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return $"Error: {ex.Message}";
            }
        }

        public async Task<List<object>> GetUsers()
        {
            var currentUserId = Context.UserIdentifier;
            var users = await _userManager.Users
                                        .Where(u => u.Id != currentUserId)
                                        .Select(u => new { u.Id, u.UserName, u.FullName })
                                        .ToListAsync();
            return users.Cast<object>().ToList();
        }

        public async Task<List<ChatMessage>> GetChatHistory(string user1Id, string user2Id)
        {
            var messages = await _context.ChatMessages
                .Where(m => (m.SenderId == user1Id && m.ReceiverId == user2Id) || (m.SenderId == user2Id && m.ReceiverId == user1Id))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
            return messages;
        }

        public async Task MarkMessagesAsRead(string senderId, string receiverId)
        {
            var messages = await _context.ChatMessages
                .Where(m => m.SenderId == senderId && m.ReceiverId == receiverId && !m.IsRead)
                .ToListAsync();

            foreach (var message in messages)
            {
                message.IsRead = true;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUnreadMessageCount(string userId)
        {
            return await _context.ChatMessages
                .CountAsync(m => m.ReceiverId == userId && !m.IsRead);
        }
    }
}