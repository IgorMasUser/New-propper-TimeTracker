using AutoMapper;
using TimeTracker.DTOs;
using TimeTracker.Models;

namespace TimeTracker.Profiles
{
    public class ProfilesMapping : Profile
    {
        public ProfilesMapping()
        {
           CreateMap<User, UserDTO>();
        }

    }
}
