

//using Microsoft.AspNetCore.SignalR;
//using System.Collections.Concurrent;

//namespace VoiceChatApp.Hubs
//{
//    public class ChatHub : Hub
//    {
//        private static readonly ConcurrentDictionary<string, string> Users = new();

//        public async Task JoinRoom(string username)
//        {
//            Users[Context.ConnectionId] = username;
//            await Clients.Others.SendAsync("UserJoined", Context.ConnectionId, username);
//        }

//        public async Task SendSignal(string targetConnectionId, string signal)
//        {
//            await Clients.Client(targetConnectionId).SendAsync("ReceiveSignal", Context.ConnectionId, signal);
//        }

//        public override async Task OnDisconnectedAsync(Exception exception)
//        {
//            Users.TryRemove(Context.ConnectionId, out _);
//            await Clients.Others.SendAsync("UserLeft", Context.ConnectionId);
//        }
//    }




//}


using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using VoiceChatApp.Models; // لازم تتأكد إن namespace بتاع الموديل CallRecord ده صح
using VoiceChatApp.Data;   // ده اللي فيه ApplicationDbContext
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

        //public async Task JoinRoom(string username)
        //{
        //    Users[Context.ConnectionId] = username;
        //    Console.WriteLine($"==> JoinRoom called by {username} at {DateTime.Now}");

        //    // حفظ البيانات في قاعدة البيانات
        //    var call = new CallRecord
        //    {
        //        User = username,
        //        StartTime = DateTime.Now
        //    };

        //    _context.CallRecords.Add(call);
        //    await _context.SaveChangesAsync();

        //    await Clients.Others.SendAsync("UserJoined", Context.ConnectionId, username);
        //}

        public async Task JoinRoom(string username)
        {
            Users[Context.ConnectionId] = username;
            Console.WriteLine($"==> JoinRoom called by {username} at {DateTime.Now}");

            // ✅ تحديث لحفظ التاريخ والوقت
            var call = new CallRecord
            {
                User = username,
                CallDate = DateTime.UtcNow.Date, // ✅ حفظ التاريخ فقط
                StartTime = DateTime.UtcNow.TimeOfDay // ✅ حفظ الوقت فقط
            };

            _context.CallRecords.Add(call);
            await _context.SaveChangesAsync();

            await Clients.Others.SendAsync("UserJoined", Context.ConnectionId, username);
        }



        public async Task SendSignal(string targetConnectionId, string signal)
        {
            await Clients.Client(targetConnectionId).SendAsync("ReceiveSignal", Context.ConnectionId, signal);
        }

        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
        //    Users.TryRemove(Context.ConnectionId, out _);
        //    await Clients.Others.SendAsync("UserLeft", Context.ConnectionId);
        //}

        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
        //    if (Users.TryRemove(Context.ConnectionId, out var username))
        //    {
        //        var call = _context.CallRecords
        //                    .Where(c => c.User == username && c.EndTime == null)
        //                    .OrderByDescending(c => c.StartTime)
        //                    .FirstOrDefault();

        //        if (call != null)
        //        {
        //            call.EndTime = DateTime.Now;
        //            call.Duration = call.EndTime - call.StartTime;

        //            await _context.SaveChangesAsync();
        //        }

        //        await Clients.Others.SendAsync("UserLeft", Context.ConnectionId);
        //    }

        //    await base.OnDisconnectedAsync(exception);
        //}

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
                    call.EndTime = DateTime.UtcNow.TimeOfDay; // ✅ حفظ الوقت فقط
                    call.Duration = call.EndTime - call.StartTime;

                    await _context.SaveChangesAsync();
                }

                await Clients.Others.SendAsync("UserLeft", Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }


    }
}


