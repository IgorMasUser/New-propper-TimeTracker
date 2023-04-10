﻿using AutoMapper;
using TimeTracker.Data;
using TimeTracker.DTOs;
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

        //[UseFiltering]
        //[UseSorting]
        //public IQueryable<UserReadDTO> GetUsers([Service] ApplicationDbContext context)
        //{
        //    IQueryable<UserReadDTO> userDTO = (context.User.Select(u => mapper.Map<User, UserReadDTO>(u)));

        //    return userDTO;
        //}

        [UseFiltering]
        [UseSorting]
        public IQueryable<User> GetUsers([Service] ApplicationDbContext context) //works
        {
            return context.User;
        }
    }
}