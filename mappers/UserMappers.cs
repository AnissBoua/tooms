using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tooms.dtos.user;
using tooms.models;

namespace tooms.mappers
{
    public static class UserMappers
    {
        public static UserDto ToUserDto(this User user) {
            return new UserDto {
                Id = user.Id,
                Nickname = user.Nickname,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public static User ToUser(this UserCreateDto request) {
            return new User {
                Nickname = request.Nickname,
                Email = request.Email,
                Password = request.Password,
            };
        }

        public static UserConversationDto ToUserConversationDto(this User user) {
            return new UserConversationDto {
                Id = user.Id,
                Nickname = user.Nickname,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}