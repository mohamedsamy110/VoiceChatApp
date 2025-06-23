//using Microsoft.AspNetCore.Mvc.RazorPages;
//using VoiceChatApp.Models;

//namespace VoiceChatApp.Pages
//{
//    public class CallLogModel : PageModel
//    {
//        public List<CallRecord> CallHistory { get; set; }

//        public void OnGet()
//        {
//            // بيانات مؤقتة كمثال
//            CallHistory = new List<CallRecord>
//            {
//                new CallRecord { User = "Ahmed", StartTime = DateTime.Now.AddMinutes(-10), EndTime = DateTime.Now },
//                new CallRecord { User = "Fatma", StartTime = DateTime.Now.AddMinutes(-20), EndTime = DateTime.Now.AddMinutes(-10) }
//            };
//        }
//    }
//}



using Microsoft.AspNetCore.Mvc.RazorPages;

public class CallLogModel : PageModel
{
    public void OnGet()
    {
        // مفيش حاجة هنا لأن البيانات هتتجاب من JavaScript
    }
}
