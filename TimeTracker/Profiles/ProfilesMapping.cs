using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TimeTracker.DTOs;
using TimeTracker.Models;

namespace TimeTracker.Profiles
{
    public class ProfilesMapping : Profile
    {
        public ProfilesMapping()
        {
           CreateMap<User, UserReadDTO>();
           CreateMap<UserCreateDTO, User>();
        }
    }
}
