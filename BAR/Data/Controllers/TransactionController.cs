using BAR.Data.Interfaces;
using BAR.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BAR.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransaction _ITransaction;
        public TransactionController(ITransaction iTransaction)
        {
            _ITransaction = iTransaction;
        }
        [HttpGet]
        public async Task<List<UserTransaction>> Get()
        {
            return await Task.FromResult(_ITransaction.GetTransactions());
        }
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            UserTransaction transaction = _ITransaction.GetTransaction(id);
            if (transaction != null)
            {
                return Ok(transaction);
            }
            return NotFound();
        }
        [HttpPost]
        {
            public void
        }
    }
}
