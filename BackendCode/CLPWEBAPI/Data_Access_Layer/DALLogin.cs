﻿using Data_Access_Layer.Repository;
using Data_Access_Layer.Repository.Entities;
using Data_Access_Layer.Repository.Model;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Data_Access_Layer
{
    public class DALLogin
    {
        private readonly AppDbContext _cIDbContext;
        public DALLogin(AppDbContext cIDbContext)
        {
            _cIDbContext = cIDbContext;
        }

        public User LoginUser(User user)
        {
            User userObj = new User();
            try
            {
                var query = from u in _cIDbContext.User
                            where u.EmailAddress == user.EmailAddress && u.IsDeleted == false
                            select new
                            {
                                u.Id,
                                u.FirstName,
                                u.LastName,
                                u.PhoneNumber,
                                u.EmailAddress,
                                u.UserType,
                                u.Password,
                                UserImage = u.UserImage
                            };

                var userData = query.FirstOrDefault();

                if (userData != null)
                {
                    if (userData.Password == user.Password)
                    {
                        userObj.Id = userData.Id;
                        userObj.FirstName = userData.FirstName;
                        userObj.LastName = userData.LastName;
                        userObj.PhoneNumber = userData.PhoneNumber;
                        userObj.EmailAddress = userData.EmailAddress;
                        userObj.UserType = userData.UserType;
                        userObj.UserImage = userData.UserImage;
                        userObj.Message = "Login Successfully";
                    }
                    else
                    {
                        userObj.Message = "Incorrect Password.";
                    }
                }
                else
                {
                    userObj.Message = "Email Address Not Found.";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return userObj;
        }

        public string Register(User user)
        {
            string result = string.Empty;
            try
            {
                bool emailExists = _cIDbContext.User.Any(u => u.EmailAddress == user.EmailAddress && !u.IsDeleted);
                if (!emailExists)
                {
                    string maxEmployeeIdStr = _cIDbContext.UserDetail.Max(ud => ud.EmployeeId);
                    int maxEmployeeId = 0;
                    if (!string.IsNullOrEmpty(maxEmployeeIdStr))
                    {
                        if (int.TryParse(maxEmployeeIdStr, out int parsedEmployeeId))
                        {
                            maxEmployeeId = parsedEmployeeId;
                        }
                        else
                        {
                            throw new Exception("Error while converting string to int.");
                        }
                    }
                    int newEmployeeId = maxEmployeeId + 1;

                    var newUser = new User
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        PhoneNumber = user.PhoneNumber,
                        EmailAddress = user.EmailAddress,
                        Password = user.Password,
                        UserType = user.UserType,
                        CreatedDate = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    _cIDbContext.User.Add(newUser);
                    _cIDbContext.SaveChanges();
                    var newUserDetail = new UserDetail
                    {
                        UserId = newUser.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        PhoneNumber = user.PhoneNumber,
                        EmailAddress = user.EmailAddress,
                        UserType = user.UserType,
                        Name = user.FirstName,
                        Surname = user.LastName,
                        EmployeeId = newEmployeeId.ToString(),
                        Department = "IT",
                        Status = true
                    };
                    _cIDbContext.UserDetail.Add(newUserDetail);
                    _cIDbContext.SaveChanges();
                    result = "User Register Successfully";
                }
                else
                {
                    throw new Exception("Email is Already Exist.");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }
        public async Task<User> GetUserByIdAsync(int id)
        {
            try
            {
                User user = await _cIDbContext.User.FirstAsync(x => x.Id == id && !x.IsDeleted);
                if (user == null)
                {
                    throw new Exception("User not found");
                }
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }
        }

        public async Task<string> UpdateUserAsync(UpdateUserModel user)
        {
            try
            {
                string result = "";

                using (var transaction = await _cIDbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var updatedUserDetail = await _cIDbContext.UserDetail.FirstOrDefaultAsync(x => x.UserId == user.Id);
                        if (updatedUserDetail != null)
                        {
                            updatedUserDetail.Name = user.FirstName;
                            updatedUserDetail.Surname = user.LastName;
                            updatedUserDetail.ModifiedDate = DateTime.UtcNow;

                            // idk if we need to update notMapped columns, will see
                            updatedUserDetail.FirstName = user.FirstName;
                            updatedUserDetail.LastName = user.LastName;
                            updatedUserDetail.EmailAddress = user.EmailAddress;
                            updatedUserDetail.PhoneNumber = user.PhoneNumber;

                        }
                        var updatedUser = await _cIDbContext.User.FirstOrDefaultAsync(x => x.Id == user.Id);
                        if (updatedUser != null)
                        {
                            updatedUser.FirstName = user.FirstName;
                            updatedUser.LastName = user.LastName;
                            updatedUser.PhoneNumber = user.PhoneNumber;
                            updatedUser.EmailAddress = user.EmailAddress;
                            updatedUser.Password = user.Password;
                            updatedUser.ModifiedDate = DateTime.UtcNow;
                        }
                        await _cIDbContext.SaveChangesAsync();

                        await transaction.CommitAsync();

                        result = "User Updated Successfully!";
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw ex;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in updating User", ex);
            }

        }

    }
}
