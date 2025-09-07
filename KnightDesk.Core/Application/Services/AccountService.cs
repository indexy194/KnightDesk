using AutoMapper;
using KnightDesk.Core.Application.DTOs;
using KnightDesk.Core.Domain.Entities;
using KnightDesk.Core.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace KnightDesk.Core.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountService> _logger;

        public AccountService(IUnitOfWork unitOfWork,
                              IMapper mapper,
                              ILogger<AccountService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GeneralResponseDTO<IEnumerable<AccountDTO>>> GetAllAccountsAsync()
        {
            try
            {
                var entry = await _unitOfWork.Accounts.GetAllAsync();
                // use inumrable bc this is read-only collection, less memory than list
                return new GeneralResponseDTO<IEnumerable<AccountDTO>>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = "Accounts retrieved successfully",
                    Data = _mapper.Map<IEnumerable<AccountDTO>>(entry)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all accounts");
                return new GeneralResponseDTO<IEnumerable<AccountDTO>>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while retrieving all accounts"
                };
            }
        }

        public async Task<GeneralResponseDTO<AccountDTO?>> GetAccountByIdAsync(int id)
        {
            try
            {
                var account = await _unitOfWork.Accounts.GetByIdAsync(id);
                if (account == null)
                {
                    return new GeneralResponseDTO<AccountDTO?>
                    {
                        Code = (int)RESPONSE_CODE.NotFound,
                        Message = $"Account with ID {id} not found",
                        Data = null
                    };
                }
                return new GeneralResponseDTO<AccountDTO?>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = "Account retrieved successfully",
                    Data = _mapper.Map<AccountDTO>(account)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting account by id {AccountId}", id);
                return new GeneralResponseDTO<AccountDTO?>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while retrieving the account"
                };
            }
        }

        public async Task<GeneralResponseDTO<IEnumerable<AccountDTO>>> GetAccountsByServerAsync(int serverNo)
        {
            try
            {
                var accounts = await _unitOfWork.Accounts.GetAccountsByServerAsync(serverNo);
                return new GeneralResponseDTO<IEnumerable<AccountDTO>>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = "Accounts retrieved successfully",
                    Data = _mapper.Map<IEnumerable<AccountDTO>>(accounts)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting accounts by server {ServerNo}", serverNo);
                return new GeneralResponseDTO<IEnumerable<AccountDTO>>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while retrieving accounts by server"
                };
            }
        }

        public async Task<GeneralResponseDTO<IEnumerable<AccountDTO>>> GetFavoriteAccountsAsync()
        {
            try
            {
                var accounts = await _unitOfWork.Accounts.GetFavoriteAccountsAsync();
                return new GeneralResponseDTO<IEnumerable<AccountDTO>>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = "Accounts favorite retrieved successfully",
                    Data = _mapper.Map<IEnumerable<AccountDTO>>(accounts)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting favorite accounts");
                return new GeneralResponseDTO<IEnumerable<AccountDTO>>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while retrieving favorite accounts"
                };
            }
        }

        public async Task<GeneralResponseDTO<IEnumerable<AccountDTO>>> SearchAccountsAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return new GeneralResponseDTO<IEnumerable<AccountDTO>>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = "Search term cannot be empty",
                        Data = new List<AccountDTO>()
                    };
                }

                var accounts = await _unitOfWork.Accounts.SearchAccountsAsync(searchTerm);
                return new GeneralResponseDTO<IEnumerable<AccountDTO>>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = "Accounts searched successfully",
                    Data = _mapper.Map<IEnumerable<AccountDTO>>(accounts)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching accounts with term {SearchTerm}", searchTerm);
                return new GeneralResponseDTO<IEnumerable<AccountDTO>>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while searching accounts"
                };
            }
        }

        public async Task<GeneralResponseDTO<AccountDTO>> CreateAccountAsync(CreateAccountDTO account)
        {
            try
            {
                // Validate account
                var validateAccount = new BaseAccountDTO
                {
                    Username = account.Username,
                    Password = account.Password,
                    IndexCharacter = account.IndexCharacter,
                    ServerInfoId = account.ServerInfoId
                };
                var validationResult = ValidateAccount(validateAccount);
                if (!validationResult)
                {
                    return new GeneralResponseDTO<AccountDTO>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = "Invalid account data"
                    };
                }
                //Check if user exists
                var checkUser = await _unitOfWork.Users.GetByIdAsync(account.UserId);
                if (checkUser == null)
                {
                    return new GeneralResponseDTO<AccountDTO>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = $"User with ID {account.UserId} does not exist"
                    };
                }
                //Check if server exists
                var checkServer = await _unitOfWork.ServerInfos.GetByIdAsync(account.ServerInfoId);
                if (checkServer == null)
                {
                    return new GeneralResponseDTO<AccountDTO>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = $"Server with ID {account.ServerInfoId} does not exist"
                    };
                }
                // Check if username already exists for this server
                var usernameExistsResult = await IsUsernameExistsAsync(account.Username!);
                if (usernameExistsResult.Data)
                {
                    return new GeneralResponseDTO<AccountDTO>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = $"Username '{account.Username}' already exists"
                    };
                }
                var entry = _mapper.Map<Account>(account);
                var createdAccount = await _unitOfWork.Accounts.AddAsync(entry);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Account created successfully: {Username}", account.Username);
                return new GeneralResponseDTO<AccountDTO>
                {
                    Code = (int)RESPONSE_CODE.Created,
                    Message = "Account created successfully",
                    Data = _mapper.Map<AccountDTO>(createdAccount)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating account {Username}", account.Username);
                return new GeneralResponseDTO<AccountDTO>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while creating the account"
                };
            }
        }

        public async Task<GeneralResponseDTO<AccountDTO>> UpdateAccountAsync(UpdateAccountDTO account)
        {
            try
            {
                // Validate account
                var validateAccount = new BaseAccountDTO
                {
                    Username = account.Username,
                    Password = account.Password,
                    IndexCharacter = account.IndexCharacter,
                    ServerInfoId = account.ServerInfoId
                };
                var validationResult = ValidateAccount(validateAccount);
                if (!validationResult)
                {
                    return new GeneralResponseDTO<AccountDTO>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = "Invalid account data"
                    };
                }
                //check if user exists
                var checkUser = await _unitOfWork.Users.GetByIdAsync(account.UserId);
                if (checkUser == null)
                {
                    return new GeneralResponseDTO<AccountDTO>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = $"User with ID {account.UserId} does not exist"
                    };
                }
                //check if server exists
                var checkServer = await _unitOfWork.ServerInfos.GetByIdAsync(account.ServerInfoId);
                if (checkServer == null)
                {
                    return new GeneralResponseDTO<AccountDTO>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = $"Server with ID {account.ServerInfoId} does not exist"
                    };
                } 
                // Check if account exists
                var existingAccount = await _unitOfWork.Accounts.GetByIdAsync(account.Id);
                if (existingAccount == null)
                {
                    return new GeneralResponseDTO<AccountDTO>
                    {
                        Code = (int)RESPONSE_CODE.NotFound,
                        Message = $"Account with ID {account.Id} not found"
                    };
                }
                
                // Approach 1: Map DTO to existing tracked entity (Best for large entities)
                // This preserves EF tracking and Id while updating all other properties
                _mapper.Map(account, existingAccount);

                await _unitOfWork.Accounts.UpdateAsync(existingAccount);
                await _unitOfWork.SaveChangesAsync();

                // Get the updated account with ServerInfo included
                
                _logger.LogInformation("Account updated successfully: {AccountId}", account.Id);
                var updatedAccount = await _unitOfWork.Accounts.GetByIdAsync(account.Id);
                return new GeneralResponseDTO<AccountDTO>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = "Account updated successfully",
                    Data = _mapper.Map<AccountDTO>(updatedAccount)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating account {AccountId}", account.Id);
                return new GeneralResponseDTO<AccountDTO>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while updating the account"
                };
            }
        }

        public async Task<GeneralResponseDTO<bool>> DeleteAccountAsync(int id)
        {
            try
            {
                var account = await _unitOfWork.Accounts.GetByIdAsync(id);
                if (account == null)
                {
                    _logger.LogWarning("Account not found for deletion: {AccountId}", id);
                    return new GeneralResponseDTO<bool>
                    {
                        Code = (int)RESPONSE_CODE.NotFound,
                        Message = $"Account with ID {id} not found",
                        Data = false
                    };
                }

                // Soft delete
                account.IsDeleted = true;
                await _unitOfWork.Accounts.UpdateAsync(account);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Account soft deleted successfully: {AccountId}", id);
                return new GeneralResponseDTO<bool>
                {
                    Message = "Account deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting account {AccountId}", id);
                return new GeneralResponseDTO<bool>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while deleting the account",
                    Data = false
                };
            }
        }

        public async Task<GeneralResponseDTO<bool>> ToggleFavoriteAsync(int accountId)
        {
            try
            {
                // Check if account exists
                var account = await _unitOfWork.Accounts.GetByIdAsync(accountId);
                if (account == null)
                {
                    return new GeneralResponseDTO<bool>
                    {
                        Code = (int)RESPONSE_CODE.NotFound,
                        Message = $"Account with ID {accountId} not found",
                        Data = false
                    };
                }

                await _unitOfWork.Accounts.ToggleFavoriteAsync(accountId);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Account favorite toggled: {AccountId}", accountId);
                return new GeneralResponseDTO<bool>
                {
                    Message = "Account favorite status toggled successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling favorite for account {AccountId}", accountId);
                return new GeneralResponseDTO<bool>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while toggling favorite status",
                    Data = false
                };
            }
        }
        private bool ValidateAccount(BaseAccountDTO account)
        {
            if (account == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(account.Username))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(account.Password))
            {
                return false;
            }

            if (account.ServerInfoId < 0)
            {
                return false;
            }

            if (account.IndexCharacter < 0)
            {
                return false;
            }

            return true;
        }

        public async Task<GeneralResponseDTO<bool>> IsUsernameExistsAsync(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return new GeneralResponseDTO<bool>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = "Username cannot be empty",
                        Data = false
                    };
                }

                var exists = await _unitOfWork.Accounts.IsUsernameExistsAsync(username);
                return new GeneralResponseDTO<bool>
                {
                    Message = exists ? "Username already exists" : "Username is available",
                    Data = exists
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking username existence {Username}", username);
                return new GeneralResponseDTO<bool>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while checking username availability",
                    Data = false
                };
            }
        }

        public async Task<GeneralResponseDTO<IEnumerable<AccountDTO>>> GetListAccountsByUserId(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return new GeneralResponseDTO<IEnumerable<AccountDTO>>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = "Invalid user ID",
                        Data = new List<AccountDTO>()
                    };
                }

                var accounts = await _unitOfWork.Accounts.GetListAccountsByUserId(userId);
                return new GeneralResponseDTO<IEnumerable<AccountDTO>>(_mapper.Map<IEnumerable<AccountDTO>>(accounts));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting accounts by userId {UserId}", userId);
                return new GeneralResponseDTO<IEnumerable<AccountDTO>>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while retrieving user accounts"
                };
            }
        }

        public async Task<GeneralResponseDTO<IEnumerable<AccountDTO>>> GetFavoriteAccountsByUserId(int userId)
        {
            try
            {
                if(userId < 0) { 
                    return new GeneralResponseDTO<IEnumerable<AccountDTO>>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = "Invalid user ID",
                        Data = new List<AccountDTO>()
                    };
                }
                var accounts = await _unitOfWork.Accounts.GetListAccountsByUserId(userId);
                //filter only favorite accounts
                var result = accounts.Where(a => a.IsFavorite).ToList();
                return new GeneralResponseDTO<IEnumerable<AccountDTO>>(_mapper.Map<IEnumerable<AccountDTO>>(result));

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error getting favorite accounts by userId {UserId}", userId);
                return new GeneralResponseDTO<IEnumerable<AccountDTO>>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while retrieving favorite user accounts"
                };
            }
        }
    }
}
