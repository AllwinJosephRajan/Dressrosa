using Dapper;
using Dressrosa.Api.Dto;
using Dressrosa.Api.Model;
using Dressrosa.API.Core.Model;

namespace Dressrosa.Api.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly IDBUnitOfWork _dBManager;
        public UserRepository(IDBUnitOfWork dBManager)
        {
            _dBManager = dBManager;
        }

        public async Task<User> Login(TokenRequest tokenRequest)
        {
            try
            {
                using (_dBManager)
                {
                    var query = @"SELECT * FROM users where EmailAddress = @EmailAddressOrUserName and Password = @Password limit 1";
                    object param = new
                    {
                        EmailAddress = tokenRequest.EmailAddressOrUserName,
                        Password = tokenRequest.Password
                    };
                    var result = await _dBManager.Connection.QueryFirstAsync<User>(query, param);
                    if (result == null)
                    {
                        return null;
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
       
        public async Task<bool> CheckUserNameExistsAsync(string userName)
        {
            using (_dBManager)
            {
                string query = @"SELECT COUNT(*) FROM users WHERE UserName = @UserName";
                int count = await _dBManager.Connection.ExecuteScalarAsync<int>(query, new { UserName = userName });

                return count > 0;
            }
        }
        public async Task<bool> CheckEmailExistsAsync(string emailAddress)
        {
            using (_dBManager)
            {
                string CheckEmailExists = @"SELECT COUNT(*) FROM users WHERE EmailAddress = @EmailAddress";
                int count = await _dBManager.Connection.ExecuteScalarAsync<int>(CheckEmailExists, new { EmailAddress = emailAddress });

                return count > 0;
            }
        }

        public async Task<User> GetUserByEmailAsync(string emailAddress)
        {
            try
            {
                using (_dBManager)
                {
                    var query = @"
SELECT 
    u.Id, u.FirstName, u.LastName, u.UserName, u.EmailAddress, u.Password,
    u.PhoneNumber, u.CreatedOn, u.UpdateOn, u.CreatedBy, u.DeleteBit, u.TenantId,
    rm.Id as RoleMappingId, rm.RoleId
FROM users u
INNER JOIN user_rolemapping rm ON u.Id = rm.UserId
WHERE u.EmailAddress = @EmailAddress 
  AND u.DeleteBit = 0";

                    var userDict = new Dictionary<string, User>();

                    var result = await _dBManager.Connection.QueryAsync<User, UserRoleMappingDto, User>(
                        query,
                        (user, role) =>
                        {
                            if (!userDict.TryGetValue(user.Id, out var currentUser))
                            {
                                currentUser = user;
                                currentUser.UserRoleMapping = new List<UserRoleMappingDto>();
                                userDict.Add(user.Id, currentUser);
                            }

                            currentUser.UserRoleMapping.Add(new UserRoleMappingDto
                            {
                                Id = role.Id,
                                UserId = user.Id,
                                RoleId = role.RoleId
                            });

                            return currentUser;
                        },
                        new { EmailAddress = emailAddress },
                        splitOn: "RoleMappingId"
                    );

                    return result.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            try
            {
                using (_dBManager)
                {
                    var query = @"
SELECT 
    u.Id, u.FirstName, u.LastName, u.UserName, u.EmailAddress, u.Password,
    u.PhoneNumber, u.CreatedOn, u.UpdateOn, u.CreatedBy, u.DeleteBit, u.TenantId,
    rm.Id as RoleMappingId, rm.RoleId
FROM users u
INNER JOIN user_rolemapping rm ON u.Id = rm.UserId
WHERE u.UserName = @UserName 
  AND u.DeleteBit = 0";

                    var userDict = new Dictionary<string, User>();

                    var result = await _dBManager.Connection.QueryAsync<User, UserRoleMappingDto, User>(
                        query,
                        (user, role) =>
                        {
                            if (!userDict.TryGetValue(user.Id, out var currentUser))
                            {
                                currentUser = user;
                                currentUser.UserRoleMapping = new List<UserRoleMappingDto>();
                                userDict.Add(user.Id, currentUser);
                            }

                            currentUser.UserRoleMapping.Add(new UserRoleMappingDto
                            {
                                Id = role.Id,
                                UserId = user.Id,
                                RoleId = role.RoleId
                            });

                            return currentUser;
                        },
                        new { UserName = userName },
                        splitOn: "RoleMappingId"
                    );

                    return result.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<User> AddUserAsync(User user)
        {
            const string userInsertQuery = @"
            INSERT INTO users (Id, FirstName, LastName, UserName, EmailAddress, Password, PhoneNumber, TenantId,CreatedOn, DeleteBit)
            VALUES (@Id, @FirstName, @LastName, @UserName, @EmailAddress, @Password, @PhoneNumber, @TenantId,@CreatedOn, @DeleteBit);";

            // SQL query for inserting into the userrolemapping table
            const string roleMappingInsertQuery = @"
            INSERT INTO user_rolemapping (Id, UserId, RoleId)
            VALUES (@Id, @UserId, @RoleId);";

            try
            {
                var currentDateTime = DateTime.UtcNow;

                user.Id = Guid.NewGuid().ToString();
                user.CreatedOn = currentDateTime;
                user.DeleteBit = false;
                using (_dBManager)
                {
                    var userParameters = new DynamicParameters();
                    userParameters.Add("@id", user.Id);
                    userParameters.Add("@FirstName", user.FirstName);
                    userParameters.Add("@LastName", user.LastName);
                    userParameters.Add("@UserName", user.UserName);
                    userParameters.Add("@EmailAddress", user.EmailAddress);
                    userParameters.Add("@Password", user.Password);
                    userParameters.Add("@PhoneNumber", user.PhoneNumber);
                    userParameters.Add("@TenantId", user.TenantId);
                    userParameters.Add("@CreatedOn", user.CreatedOn);
                    userParameters.Add("@DeleteBit", user.DeleteBit);

                    // Execute user insert query
                    var userResult = await _dBManager.Connection.ExecuteAsync(userInsertQuery, userParameters);
                    if (userResult <= 0)
                    {
                        throw new Exception("User insertion failed.");
                    }

                    // Insert into user_userrolemapping table
                    foreach (var mapping in user.UserRoleMapping)
                    {

                        mapping.Id = Guid.NewGuid().ToString();
                        var mappingParameters = new DynamicParameters();
                        mappingParameters.Add("@Id", mapping.Id);
                        mappingParameters.Add("@RoleId", mapping.RoleId);
                        mappingParameters.Add("@UserId", user.Id);
                        await _dBManager.Connection.ExecuteAsync(roleMappingInsertQuery, mappingParameters);
                    }
                }
                return user;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
