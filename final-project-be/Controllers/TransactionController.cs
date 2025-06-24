using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Answer;
using final_project_be_Domain.DTOs.Transaction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        public TransactionController(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        // POST api/<TransactionController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TransactionDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var transaction = await _transactionRepository.CreateTransaction(dto);
                return Ok(transaction);
            }
            catch (Exception e)
            {
                return Ok("Error" + e);
            }
        }
    }
}
