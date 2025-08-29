using KnightDesk.Core.Application.DTOs;
using KnightDesk.Core.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace KnightDesk.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IAccountService accountService, ILogger<AccountsController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        /// <summary>
        /// Get all accounts
        /// </summary>
        /// <returns>List of accounts</returns>
        [HttpGet]
        public async Task<GeneralResponseDTO<IEnumerable<AccountDTO>>> GetAllAccounts()
        {
            try
            {
                var accounts = await _accountService.GetAllAccountsAsync();
                return accounts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all accounts");
                return new GeneralResponseDTO<IEnumerable<AccountDTO>>
                {
                    Code = 500,
                    Message = "Internal server error",
                    Errors = new List<ResponseError>
                    {
                        new ResponseError { Code = 500, Message = "An error occurred while processing your request." }
                    }
                };
            }
        }

        /// <summary>
        /// Get account by ID
        /// </summary>
        /// <param name="id">Account ID</param>
        /// <returns>Account details</returns>
        [HttpGet("{id}")]
        public async Task<GeneralResponseDTO<AccountDTO>> GetAccount(int id)
        {
            try
            {
                var account = await _accountService.GetAccountByIdAsync(id);
                HttpContext.Response.StatusCode = account.Code;
                return account!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting account {AccountId}", id);
                return new GeneralResponseDTO<AccountDTO>
                {
                    Code = 500,
                    Message = "Internal server error",
                    Errors = new List<ResponseError>
                    {
                        new ResponseError { Code = 500, Message = "An error occurred while processing your request." }
                    }
                };
            }
        }
        //add account 
        [HttpPost("add-account")]
        public async Task<GeneralResponseDTO<AccountDTO>> AddAccount([FromBody] CreateAccountDTO accountDto)
        {
            try
            {
                var account = await _accountService.CreateAccountAsync(accountDto);
                HttpContext.Response.StatusCode = account.Code;
                return account!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding account");
                return new GeneralResponseDTO<AccountDTO>
                {
                    Code = 500,
                    Message = "Internal server error",
                    Errors = new List<ResponseError>
                    {
                        new ResponseError { Code = 500, Message = "An error occurred while processing your request." }
                    }
                };
            }
        }
        //update account
        [HttpPut("update-account")]
        public async Task<GeneralResponseDTO<AccountDTO>> UpdateAccount([FromBody] UpdateAccountDTO accountDto)
        {
            try
            {
                var account = await _accountService.UpdateAccountAsync(accountDto);
                HttpContext.Response.StatusCode = account.Code;
                return account!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating account");
                return new GeneralResponseDTO<AccountDTO>
                {
                    Code = 500,
                    Message = "Internal server error",
                    Errors = new List<ResponseError>
                    {
                        new ResponseError { Code = 500, Message = "An error occurred while processing your request." }
                    }
                };
            }
        }
        //delete account
        [HttpDelete("{id}")]
        public async Task<GeneralResponseDTO<bool>> DeleteAccount(int id)
        {
            try
            {
                var account = await _accountService.DeleteAccountAsync(id);
                HttpContext.Response.StatusCode = account.Code;
                return account!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting account {AccountId}", id);
                return new GeneralResponseDTO<bool>
                {
                    Code = 500,
                    Message = "Internal server error",
                    Errors = new List<ResponseError>
                    {
                        new ResponseError { Code = 500, Message = "An error occurred while processing your request." }
                    }
                };
            }
        }

        /// <summary>
        /// Search accounts by text
        /// </summary>
        /// <param name="searchText">Search term</param>
        /// <returns>List of matching accounts</returns>
        [HttpGet("search")]
        public async Task<GeneralResponseDTO<IEnumerable<AccountDTO>>> SearchAccounts([FromQuery] string searchText = "")
        {
            try
            {
                var accounts = await _accountService.SearchAccountsAsync(searchText);
                return accounts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching accounts with term '{SearchTerm}'", searchText);
                return new GeneralResponseDTO<IEnumerable<AccountDTO>>
                {
                    Code = 500,
                    Message = "Internal server error",
                    Errors = new List<ResponseError>
                    {
                        new ResponseError { Code = 500, Message = "An error occurred while processing your request." }
                    }
                };

            }
        }

        /// <summary>
        /// Toggle favorite status for an account
        /// </summary>
        /// <param name="id">Account ID</param>
        /// <returns>Updated account</returns>
        [HttpPut("{id}/toggle-favorite")]
        public async Task<GeneralResponseDTO<bool>> ToggleFavorite(int id)
        {
            try
            {
                var account = await _accountService.ToggleFavoriteAsync(id);
                HttpContext.Response.StatusCode = account.Code;
                return account!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling favorite for account {AccountId}", id);
                return new GeneralResponseDTO<bool>
                {
                    Code = 500,
                    Message = "Internal server error",
                    Errors = new List<ResponseError>
                    {
                        new ResponseError { Code = 500, Message = "An error occurred while processing your request." }
                    }
                };
            }
        }

        /// <summary>
        /// Get favorite accounts only
        /// </summary>
        /// <returns>List of favorite accounts</returns>
        [HttpGet("favorites")]
        public async Task<GeneralResponseDTO<IEnumerable<AccountDTO>>> GetFavoriteAccounts()
        {
            try
            {
                var accounts = await _accountService.GetFavoriteAccountsAsync();
                return accounts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting favorite accounts");
                return new GeneralResponseDTO<IEnumerable<AccountDTO>>
                {
                    Code = 500,
                    Message = "Internal server error",
                    Errors = new List<ResponseError>
                    {
                        new ResponseError { Code = 500, Message = "An error occurred while processing your request." }
                    }
                };
            }
        }
        //get list account by user id
        /// <summary>
        /// Get list of accounts by user ID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List of accounts by user ID</returns>
        [HttpGet("user/{userId}")]
        public async Task<GeneralResponseDTO<IEnumerable<AccountDTO>>> GetListAccountsByUserId(int userId)
        {
            try
            {
                var accounts = await _accountService.GetListAccountsByUserId(userId);
                return accounts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting accounts for user {UserId}", userId);
                return new GeneralResponseDTO<IEnumerable<AccountDTO>>
                {
                    Code = 500,
                    Message = "Internal server error",
                    Errors = new List<ResponseError>
                    {
                        new ResponseError { Code = 500, Message = "An error occurred while processing your request." }
                    }
                };
            }
        }
    }
}
