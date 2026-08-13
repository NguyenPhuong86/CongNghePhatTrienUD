using AppDemo.Models;
using AppDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AppDemo.Controllers;

public class CompanyController : Controller
{
    public IActionResult Index()
    {
        var viewModel = new CompanyIndexViewModel
        {
            Company = GetCompanyProfile(),
            FeaturedServices = GetCompanyServices().Take(2).ToList()
        };
        return View(viewModel);
    }

    public IActionResult Contact()
    {
        var contact = new CompanyContact
        {
            Email = "contact@company.com",
            Phone = "0208.123.456",
            WorkingTime = "Thứ Hai đến Thứ Sáu, 08:00 - 17:00"
        };
        return View(contact);
    }

    public IActionResult Services()
    {
        return View(GetCompanyServices());
    }

    private static CompanyProfile GetCompanyProfile() => new()
    {
        Name = "Công ty ABC",
        Summary = "Cung cấp giải pháp công nghệ thông tin cho cơ quan, doanh nghiệp và trường học.",
        FoundedYear = 2020,
        Address = "Thành phố Thái Nguyên"
    };

    private static List<CompanyService> GetCompanyServices() =>
    [
        new CompanyService
        {
            Name = "Tư vấn chuyển đổi số",
            Description = "Khảo sát quy trình hiện tại và đề xuất lộ trình ứng dụng công nghệ.",
            EstimatedDays = 5
        },
        new CompanyService
        {
            Name = "Phát triển website",
            Description = "Xây dựng website giới thiệu, cổng thông tin và ứng dụng quản lý nội bộ.",
            EstimatedDays = 15
        },
        new CompanyService
        {
            Name = "Bảo trì hệ thống",
            Description = "Theo dõi, cập nhật và xử lý sự cố cho hệ thống đang vận hành.",
            EstimatedDays = 3
        }
    ];
}
