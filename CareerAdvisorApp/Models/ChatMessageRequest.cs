namespace CareerAdvisorApp.Models;
public class ChatMessageRequest
    {
        public string Message { get; set; } = string.Empty;
        public string ConnectionId { get; set; } = string.Empty;
    }