using final_project_be_Application.Interface;
using final_project_be_Application.Ultils;
using final_project_be_Domain.DTOs.Message;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepository _messagesRepository;
        private readonly IHubContext<SignalRHub> _hubContext;
        public MessageController(IMessageRepository messagesRepository, IHubContext<SignalRHub> hubContext)
        {
            _messagesRepository = messagesRepository;
            _hubContext = hubContext;
        }
        [HttpGet("Room")]
        public async Task<IActionResult> GetConversation([FromQuery] Guid userId1, [FromQuery] Guid userId2)
        {
            if (userId1 == Guid.Empty || userId2 == Guid.Empty)
                return BadRequest(new { message = "Both userIds are required." });

            try
            {
                var messages = await _messagesRepository.GetConversationAsync(userId1, userId2);

                // Luôn trả về JSON, ngay cả khi không có tin nhắn
                if (messages == null)
                    return Ok(new List<MessageDto>());

                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // POST: api/messages/send
        [HttpPost("sendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] MessageDto messageDto)
        {
            if (string.IsNullOrWhiteSpace(messageDto.Content))
                return BadRequest("Content is required.");

            var isNewRoom = await _messagesRepository.IsNewChatRoomAsync(messageDto.SenderId, messageDto.ReceiverId);

            var result = await _messagesRepository.SendMessageAsync(
                messageDto.SenderId,
                messageDto.ReceiverId,
                messageDto.Content
            );

            var signalMessage = new MessageDto
            {
                SenderId = result.SenderId,
                ReceiverId = result.ReceiverId,
                Content = result.Content,
                SentAt = result.SentAt
            };

            // Gửi cho người nhận
            await _hubContext.Clients.User(signalMessage.ReceiverId.ToString())
                             .SendAsync("ReceiveMessage", signalMessage);

            if (isNewRoom)
            {
                await _hubContext.Clients.User(signalMessage.ReceiverId.ToString())
                                 .SendAsync("NewChatRoom", signalMessage.SenderId);
            }

            return Ok(result);
        }

        [HttpGet("chat-rooms")]
        public async Task<IActionResult> GetChatRooms([FromQuery] Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest(new { message = "UserId is required." });

            try
            {
                var chatPartners = await _messagesRepository.GetChatPartnersAsync(userId);

                // Luôn trả về JSON, ngay cả khi không có dữ liệu
                if (chatPartners == null || !chatPartners.Any())
                    return Ok(new List<ChatPartnerDto>()); // Trả về array rỗng thay vì NotFound

                return Ok(chatPartners);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }


    }
}
