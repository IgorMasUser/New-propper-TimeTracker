using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using TimeTracker.Data;
using TimeTracker.DTOs;

public class PDFController : Controller
{
    private readonly IUserRepo repository;
    private readonly IMapper mapper;

    public PDFController(IUserRepo userRepo, IMapper mapper)
    {
        repository = userRepo;
        this.mapper = mapper;
    }

    public IActionResult GeneratePDF()
    {
        var attendanceOfUser = repository.GetAttendanceOfUser();
        return new ViewAsPdf("~/Views/User/GetAttendanceOfUser.cshtml", mapper.Map<IEnumerable<UserReadDTO>>(attendanceOfUser));
    }
}
