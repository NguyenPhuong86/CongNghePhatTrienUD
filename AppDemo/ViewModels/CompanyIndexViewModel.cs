using AppDemo.Models;

namespace AppDemo.ViewModels;

public class CompanyIndexViewModel
{
    public CompanyProfile Company { get; set; } = new();
    public List<CompanyService> FeaturedServices { get; set; } = [];
}
