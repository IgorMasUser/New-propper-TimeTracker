using AutoMapper;
using TimeTracker.Data;
using TimeTracker.Models;

namespace TimeTracker.GraphQL
{
    public class UserQuery
    {
        private readonly IMapper mapper;

        public UserQuery(IMapper mapper)
        {
            this.mapper = mapper;
        }
        public IQueryable<User> GetUsers([Service] ApplicationDbContext context)
        {
            //var userDTO = mapper.Map<UserReadDTO>(context.User.ToList());

            return context.User;
        }
    }
}
