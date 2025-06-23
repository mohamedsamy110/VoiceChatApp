using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using VoiceChatApp.Models;
using VoiceChatApp.Data;
using System;

namespace VoiceChatApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        private static readonly ConcurrentDictionary<string, string> Users = new();

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task JoinRoom(string username)
        {
            Users[Context.ConnectionId] = username;
            Console.WriteLine($"==> JoinRoom called by {username} at {DateTime.Now}");

            var call = new CallRecord
            {
                User = username,
                CallDate = DateTime.UtcNow.Date,
                StartTime = DateTime.UtcNow.TimeOfDay
            };

            _context.CallRecords.Add(call);
            await _context.SaveChangesAsync();

            await Clients.Others.SendAsync("UserJoined", Context.ConnectionId, username);
        }



        public async Task SendSignal(string targetConnectionId, string signal)
        {
            await Clients.Client(targetConnectionId).SendAsync("ReceiveSignal", Context.ConnectionId, signal);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (Users.TryRemove(Context.ConnectionId, out var username))
            {
                var call = _context.CallRecords
                            .Where(c => c.User == username && c.EndTime == null)
                            .OrderByDescending(c => c.StartTime)
                            .FirstOrDefault();

                if (call != null)
                {
                    call.EndTime = DateTime.UtcNow.TimeOfDay;
                    call.Duration = call.EndTime - call.StartTime;

                    await _context.SaveChangesAsync();
                }

                await Clients.Others.SendAsync("UserLeft", Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }


    }
}


