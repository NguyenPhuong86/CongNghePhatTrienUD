# THỰC HÀNH CHƯƠNG 2: CẤU TRÚC ỨNG DỤNG VÀ MÔ HÌNH MVC

## Phiên bản thực hành

- Điểm bắt đầu: `chapter-01-completed`
- Phiên bản tham khảo: `chapter-02-completed`

```bash
git switch -c practice-chapter-02 chapter-01-completed
```

Không thực hành trực tiếp trên `main`, vì `main` có thể chứa code của các chương sau.

Tài liệu này hướng dẫn sinh viên tự thực hành Chương 2 của giáo trình *Công nghệ phát triển ứng dụng*. Bài thực hành tiếp nối project `AppDemo` đã hoàn thành ở Chương 1 và xây dựng module Company gồm ba trang:

- `/Company` – thông tin chung về công ty.
- `/Company/Contact` – thông tin liên hệ.
- `/Company/Services` – danh sách dịch vụ.

Qua module này, sinh viên lần theo đầy đủ luồng: **request → middleware pipeline → routing → controller/action → model hoặc view model → Razor View → HTML response**.

> Tài liệu này thống nhất sử dụng tên project `AppDemo` trong namespace, đường dẫn project, solution và các lệnh tương ứng.

## 1. Kết quả cần đạt

Sau bài thực hành, sinh viên có thể:

- Giải thích vì sao các trang HTML rời rạc khó bảo trì khi website phát triển.
- Đọc `Program.cs` theo hai phần: đăng ký dịch vụ và cấu hình request pipeline.
- Giải thích vai trò của middleware, endpoint và thứ tự xử lý request.
- Phân tích route mặc định `{controller=Home}/{action=Index}/{id?}`.
- Phân biệt routing với quá trình tìm Razor View.
- Tạo controller, model, view model và strongly typed view đúng trách nhiệm.
- Dùng layout, partial view và CSS để tái sử dụng giao diện.
- Khoanh vùng lỗi theo tầng: route, controller, action, model, view hoặc CSS.
- Viết unit test cho kết quả trực tiếp của `CompanyController`.
- Dùng AI agent để rà soát module theo phạm vi và bằng chứng cụ thể.
- Lưu phiên bản đã build/test bằng Git và tag `chapter-02-completed`.

## 2. Sản phẩm phải nộp

1. URL repository đã dùng ở Chương 1.
2. Mã commit hoàn thành Chương 2.
3. Tag `chapter-02-completed`.
4. Ảnh hoặc tệp kết quả `dotnet build` thành công.
5. Ảnh hoặc tệp kết quả các test Company đều `Passed`.
6. Ảnh ba trang `/Company`, `/Company/Contact` và `/Company/Services`.
7. Bảng phân tích đường đi của ba request.
8. Bản trả lời câu hỏi tự kiểm tra cuối tài liệu.

## 3. Điều kiện bắt đầu

Sinh viên cần có sản phẩm Chương 1 với cấu trúc tương tự:

```text
AspNetPractice/
├── AppDemo.slnx
├── AppDemo/
│   ├── Controllers/
│   ├── Models/
│   ├── Views/
│   ├── wwwroot/
│   ├── Program.cs
│   └── AppDemo.csproj
└── AppDemo.Tests/
    └── AppDemo.Tests.csproj
```

Từ thư mục `AspNetPractice`, chạy kiểm tra trạng thái ban đầu:

```bash
git status
dotnet build AppDemo.slnx
dotnet test AppDemo.slnx --no-build
```

Chỉ bắt đầu Chương 2 khi:

- [ ] Working tree không có thay đổi chưa hiểu rõ.
- [ ] Project Chương 1 build thành công.
- [ ] Các test Chương 1 đều Passed.
- [ ] Tag `chapter-01-completed` đã tồn tại.

Kiểm tra tag:

```bash
git tag
```

## 4. Hiểu bài toán trước khi viết code

Nếu tạo ba tệp HTML độc lập, menu, header, dữ liệu và CSS rất dễ bị sao chép. Khi cần đổi tên công ty hoặc kiểu trình bày, lập trình viên phải sửa nhiều nơi và có thể bỏ sót.

MVC chia trách nhiệm như sau:

| Thành phần | Trách nhiệm trong module Company |
|---|---|
| `Program.cs` | Đăng ký MVC, cấu hình pipeline, static assets và route mặc định |
| `CompanyController` | Nhận request đã được route chọn, chuẩn bị dữ liệu và chọn view |
| Model | Mô tả dữ liệu có cấu trúc: hồ sơ, liên hệ, dịch vụ |
| View model | Ghép đúng dữ liệu mà một màn hình cần |
| Razor View | Tạo HTML từ model do controller truyền vào |
| Partial view | Tái sử dụng một mảnh giao diện cục bộ |
| Layout | Tái sử dụng khung trang, menu, CSS và footer |
| `wwwroot` | Chứa CSS, JavaScript, hình ảnh và tài nguyên tĩnh |

Trước khi code, hoàn thành bảng thiết kế:

| URL | Controller | Action | Dữ liệu truyền sang view | View |
|---|---|---|---|---|
| `/Company` | `CompanyController` | `Index` | `CompanyIndexViewModel` | `Views/Company/Index.cshtml` |
| `/Company/Contact` | `CompanyController` | `Contact` | `CompanyContact` | `Views/Company/Contact.cshtml` |
| `/Company/Services` | `CompanyController` | `Services` | `List<CompanyService>` | `Views/Company/Services.cshtml` |

## 5. Tạo nhánh thực hành Chương 2

Việc dùng nhánh riêng giúp sinh viên tách thay đổi của chương mới khỏi phiên bản Chương 1.

```bash
git switch -c chapter-02
git status
```

Nếu giảng viên yêu cầu làm trực tiếp trên `main`, có thể bỏ bước tạo nhánh. Tuy nhiên, sinh viên vẫn phải đọc `git status` và `git diff` trước mỗi commit.

## 6. Đọc `Program.cs` và request pipeline

Mở `AppDemo/Program.cs`. Với template MVC .NET 10, nội dung chính thường có cấu trúc tương tự:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
```

### 6.1. Phần đăng ký dịch vụ

Các dòng trước `builder.Build()` chuẩn bị khả năng cho ứng dụng:

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
```

`AddControllersWithViews()` đăng ký các thành phần để framework có thể tìm controller, tạo controller, chạy action và render Razor View.

### 6.2. Phần pipeline và endpoint

Các dòng sau `builder.Build()` quy định đường đi của request:

- `UseExceptionHandler()` xử lý exception chưa được giải quyết ở môi trường không phải Development.
- `UseHsts()` yêu cầu trình duyệt ưu tiên HTTPS trong các lần truy cập sau.
- `UseHttpsRedirection()` chuyển HTTP sang HTTPS khi cấu hình cho phép.
- `UseRouting()` thực hiện đối chiếu request với endpoint.
- `MapStaticAssets()` ánh xạ tài nguyên tĩnh.
- `MapControllerRoute()` khai báo route MVC theo quy ước.
- `app.Run()` khởi động ứng dụng.

### 6.3. Phân biệt `Use` và `Map`

- Các lệnh `Use...` thêm bước xử lý vào đường đi của request.
- Các lệnh `Map...` khai báo endpoint mà request có thể được ánh xạ tới.

### 6.4. Không sửa `Program.cs` khi chưa cần

Module Company dùng route mặc định nên không cần thêm route riêng. Không bổ sung database, Identity, authentication, authorization hoặc dịch vụ AI ở Chương 2.

### Điểm dừng kiểm chứng 1

Sinh viên phải giải thích được:

- [ ] Dòng nào đăng ký MVC.
- [ ] `builder.Build()` chia `Program.cs` thành hai phần như thế nào.
- [ ] Request đi qua các bước nào trước khi action chạy.
- [ ] Vì sao thêm `CompanyController` không cần sửa route mặc định.

## 7. Thực hành phân tích route mặc định

Route mặc định:

```text
{controller=Home}/{action=Index}/{id?}
```

Ý nghĩa:

| Segment | Ý nghĩa | Giá trị mặc định |
|---|---|---|
| `{controller=Home}` | Tên controller, không gồm hậu tố `Controller` | `Home` |
| `{action=Index}` | Tên action | `Index` |
| `{id?}` | Tham số `id` không bắt buộc | Không có |

Hoàn thành bảng trước khi chạy ứng dụng:

| Request | Controller được chọn | Action được chọn | `id` |
|---|---|---|---|
| `/` | `HomeController` | `Index` | Không có |
| `/Company` | `CompanyController` | `Index` | Không có |
| `/Company/Contact` | `CompanyController` | `Contact` | Không có |
| `/Company/Services` | `CompanyController` | `Services` | Không có |
| `/Speakers/Details/5` | `SpeakersController` | `Details` | `5` |

Ghi nhớ:

```text
/Company/Contact
→ routing chọn CompanyController.Contact()
→ action thực thi
→ action chọn view
→ view discovery tìm Views/Company/Contact.cshtml
→ Razor sinh HTML response
```

URL MVC không phải đường dẫn trực tiếp đến tệp `.cshtml`.

## 8. Tạo các model của module Company

### Bước 1. Tạo `CompanyProfile`

Tạo `AppDemo/Models/CompanyProfile.cs`:

```csharp
namespace AppDemo.Models;

public class CompanyProfile
{
    public string Name { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public int FoundedYear { get; set; }
    public string Address { get; set; } = string.Empty;
}
```

### Bước 2. Tạo `CompanyContact`

Tạo `AppDemo/Models/CompanyContact.cs`:

```csharp
namespace AppDemo.Models;

public class CompanyContact
{
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string WorkingTime { get; set; } = string.Empty;
}
```

### Bước 3. Tạo `CompanyService`

Tạo `AppDemo/Models/CompanyService.cs`:

```csharp
namespace AppDemo.Models;

public class CompanyService
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int EstimatedDays { get; set; }
}
```

### Bước 4. Giải thích thiết kế

- `CompanyProfile` mô tả hồ sơ chung của công ty.
- `CompanyContact` chỉ chứa thông tin trang liên hệ cần.
- `CompanyService` mô tả một dịch vụ.
- `string.Empty` tạo giá trị chuỗi mặc định; đây chưa thay thế validation.
- Chương 2 chưa dùng database nên dữ liệu sẽ được tạo tạm trong controller.

### Điểm dừng kiểm chứng 2

- [ ] Mỗi class nằm trong một tệp đúng thư mục `Models`.
- [ ] Namespace là `AppDemo.Models`.
- [ ] Tên property và kiểu dữ liệu đúng yêu cầu.
- [ ] Không đưa HTML hoặc logic giao diện vào model.

## 9. Tạo view model cho trang Company Index

Trang `/Company` cần đồng thời hồ sơ công ty và hai dịch vụ nổi bật. Tạo thư mục `AppDemo/ViewModels`, sau đó tạo `CompanyIndexViewModel.cs`:

```csharp
using AppDemo.Models;

namespace AppDemo.ViewModels;

public class CompanyIndexViewModel
{
    public CompanyProfile Company { get; set; } = new();
    public List<CompanyService> FeaturedServices { get; set; } = [];
}
```

View model không đại diện cho một bảng trong database. Nó là hợp đồng dữ liệu riêng của một màn hình.

Không dùng `ViewBag` cho bài này vì mục tiêu là thực hành strongly typed view và kiểm tra kiểu dữ liệu rõ ràng.

## 10. Tạo `CompanyController`

Tạo `AppDemo/Controllers/CompanyController.cs`:

```csharp
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
        var services = GetCompanyServices();
        return View(services);
    }

    private static CompanyProfile GetCompanyProfile()
    {
        return new CompanyProfile
        {
            Name = "Công ty ABC",
            Summary = "Cung cấp giải pháp công nghệ thông tin cho cơ quan, doanh nghiệp và trường học.",
            FoundedYear = 2020,
            Address = "Thành phố Thái Nguyên"
        };
    }

    private static List<CompanyService> GetCompanyServices()
    {
        return
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
}
```

### Kiểm tra trách nhiệm của action

| Action | Công việc | Model của view |
|---|---|---|
| `Index()` | Ghép hồ sơ và hai dịch vụ nổi bật | `CompanyIndexViewModel` |
| `Contact()` | Chuẩn bị thông tin liên hệ | `CompanyContact` |
| `Services()` | Chuẩn bị toàn bộ danh sách dịch vụ | `List<CompanyService>` |

Controller đang dùng dữ liệu hard-code có chủ ý để tập trung vào luồng MVC. Đây không phải kiến trúc lưu trữ cuối cùng; dữ liệu bền vững sẽ được học ở chương EF Core.

### Build sớm

```bash
dotnet build AppDemo/AppDemo.csproj
```

Không tạo view trước khi lỗi biên dịch ở model/controller đã được xử lý.

## 11. Cấu hình namespace dùng chung cho Razor View

Mở `AppDemo/Views/_ViewImports.cshtml` và bảo đảm có:

```cshtml
@using AppDemo
@using AppDemo.Models
@using AppDemo.ViewModels
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

Nhờ đó, view có thể viết:

```cshtml
@model CompanyContact
```

thay vì phải ghi đầy đủ `@model AppDemo.Models.CompanyContact`.

Mở `AppDemo/Views/_ViewStart.cshtml` và kiểm tra:

```cshtml
@{
    Layout = "_Layout";
}
```

## 12. Tạo partial view dùng chung

Tạo `AppDemo/Views/Shared/_CompanyHeader.cshtml`:

```cshtml
<div class="company-header">
    <h1>Thông tin công ty</h1>
    <p>Giải pháp công nghệ thông tin cho đơn vị và doanh nghiệp</p>
</div>
```

Tên `_CompanyHeader` rõ phạm vi hơn `_Header`, tránh nhầm với header toàn ứng dụng khi project phát triển.

Partial view tái sử dụng một mảnh giao diện; nó không thay thế layout. Layout cung cấp toàn bộ khung trang, còn partial chỉ cung cấp phần header của module Company.

## 13. Tạo các Razor View

Tạo thư mục `AppDemo/Views/Company`.

### 13.1. View `Index`

Tạo `AppDemo/Views/Company/Index.cshtml`:

```cshtml
@model CompanyIndexViewModel

@{
    ViewData["Title"] = "Thông tin công ty";
}

<partial name="_CompanyHeader" />

<section class="company-section">
    <h2>@Model.Company.Name</h2>
    <p>@Model.Company.Summary</p>
    <p>Thành lập năm: @Model.Company.FoundedYear</p>
    <p>Địa chỉ: @Model.Company.Address</p>
</section>

<section class="company-section">
    <h2>Dịch vụ nổi bật</h2>
    <div class="service-list">
        @foreach (var service in Model.FeaturedServices)
        {
            <article class="service-item">
                <h3>@service.Name</h3>
                <p>@service.Description</p>
            </article>
        }
    </div>
</section>
```

### 13.2. View `Contact`

Tạo `AppDemo/Views/Company/Contact.cshtml`:

```cshtml
@model CompanyContact

@{
    ViewData["Title"] = "Liên hệ";
}

<partial name="_CompanyHeader" />

<section class="company-section">
    <h2>Liên hệ</h2>
    <p>Email: @Model.Email</p>
    <p>Điện thoại: @Model.Phone</p>
    <p>Thời gian làm việc: @Model.WorkingTime</p>
</section>
```

### 13.3. View `Services`

Tạo `AppDemo/Views/Company/Services.cshtml`:

```cshtml
@model List<CompanyService>

@{
    ViewData["Title"] = "Dịch vụ";
}

<partial name="_CompanyHeader" />

<section class="company-section">
    <h2>Dịch vụ</h2>

    @if (Model.Count > 0)
    {
        <div class="service-list">
            @foreach (var service in Model)
            {
                <article class="service-item">
                    <h3>@service.Name</h3>
                    <p>@service.Description</p>
                    <p>Thời gian dự kiến: @service.EstimatedDays ngày</p>
                </article>
            }
        </div>
    }
    else
    {
        <p>Chưa có dịch vụ nào.</p>
    }
</section>
```

### Ranh giới của Razor View

`@foreach` và `@if` phù hợp vì chúng phục vụ render HTML. View không được:

- Tự tạo dữ liệu nghiệp vụ.
- Truy vấn database.
- Tự quyết định route hoặc response.
- Chứa tính toán nghiệp vụ phức tạp.

### Điểm dừng kiểm chứng 3

- [ ] Mỗi view nằm đúng `Views/Company`.
- [ ] Mỗi view khai báo đúng `@model`.
- [ ] Cả ba view dùng partial `_CompanyHeader`.
- [ ] HTML là phần chính; Razor chỉ hỗ trợ render dữ liệu.

## 14. Thêm liên kết vào layout

Mở `AppDemo/Views/Shared/_Layout.cshtml`, tìm danh sách navigation có class `navbar-nav` và thêm:

```cshtml
<li class="nav-item">
    <a class="nav-link text-dark"
       asp-area=""
       asp-controller="Company"
       asp-action="Index">Công ty</a>
</li>
<li class="nav-item">
    <a class="nav-link text-dark"
       asp-area=""
       asp-controller="Company"
       asp-action="Contact">Liên hệ</a>
</li>
<li class="nav-item">
    <a class="nav-link text-dark"
       asp-area=""
       asp-controller="Company"
       asp-action="Services">Dịch vụ</a>
</li>
```

Tag Helper sử dụng routing để sinh URL. Không viết liên kết tới tệp `.cshtml`, ví dụ `href="Views/Company/Services.cshtml"`, vì Razor View không phải tài nguyên được gọi trực tiếp.

## 15. Thêm CSS cho module Company

Mở `AppDemo/wwwroot/css/site.css` và thêm cuối tệp:

```css
.company-header {
    padding: 24px;
    margin-bottom: 20px;
    border-left: 4px solid #0f766e;
    border-radius: 8px;
    background-color: #ecfdf5;
}

.company-header h1 {
    margin: 0 0 8px;
    font-size: 30px;
}

.company-header p {
    margin: 0;
}

.company-section {
    padding: 20px 0;
}

.company-section h2 {
    margin-bottom: 12px;
}

.service-list {
    display: grid;
    gap: 16px;
    grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
}

.service-item {
    padding: 16px;
    border: 1px solid #d6dee8;
    border-radius: 8px;
    background-color: #ffffff;
}

.service-item h3 {
    margin-top: 0;
    font-size: 20px;
}
```

Kiểm tra `_Layout.cshtml` có liên kết đến CSS:

```cshtml
<link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
```

Lưu ý: đơn vị CSS phải viết liền như `24px`, không viết `24 px`.

## 16. Build và chạy module Company

### Bước 1. Build toàn solution

```bash
dotnet build AppDemo.slnx
```

Xử lý lỗi biên dịch trước khi mở trình duyệt.

### Bước 2. Chạy ứng dụng

```bash
dotnet watch --project AppDemo/AppDemo.csproj run
```

Mở đúng URL localhost do terminal cung cấp và kiểm tra lần lượt:

```text
/Company
/Company/Contact
/Company/Services
```

### Bước 3. Ghi bằng chứng

| Request | Status mong đợi | Nội dung cần thấy |
|---|---:|---|
| `/Company` | 200 | Hồ sơ công ty và 2 dịch vụ nổi bật |
| `/Company/Contact` | 200 | Email, điện thoại, thời gian làm việc |
| `/Company/Services` | 200 | 3 dịch vụ và số ngày dự kiến |

### Bước 4. Quan sát bằng DevTools

Trong tab Network:

1. Chọn request `/Company/Services`.
2. Xác nhận method `GET` và status `200`.
3. Tìm request `site.css`.
4. Xác nhận CSS được tải thành công.
5. Quan sát HTML response có tên các dịch vụ.

### Điểm dừng kiểm chứng 4

- [ ] Ba URL đều mở được.
- [ ] Navigation tạo đúng URL.
- [ ] Mỗi trang hiển thị đúng model.
- [ ] Partial xuất hiện trên cả ba trang.
- [ ] CSS được áp dụng và request `site.css` thành công.

## 17. Thực hành khoanh vùng lỗi theo tầng

Không sửa ngẫu nhiên nhiều tệp. Với mỗi lỗi, hãy ghi: **triệu chứng → giả thuyết → tệp kiểm tra → bằng chứng → bản sửa**.

### Tình huống 1. URL trả về 404

Đổi tạm URL thành `/Company/Service`.

Kiểm tra theo thứ tự:

1. Route template trong `Program.cs`.
2. Route value `controller` và `action`.
3. Tên `CompanyController`.
4. Có action `Service()` hay không.

Khôi phục URL đúng `/Company/Services`.

### Tình huống 2. Action chạy nhưng không tìm thấy view

Đổi tạm tên `Services.cshtml` thành `ServiceList.cshtml`, truy cập lại `/Company/Services` và đọc thông báo lỗi.

Kết luận cần nêu: routing đã tìm thấy action, nhưng view discovery không tìm thấy view theo quy ước. Sau đó khôi phục đúng tên `Services.cshtml`.

### Tình huống 3. View nhận sai kiểu model

Đổi tạm dòng đầu của `Services.cshtml` thành:

```cshtml
@model CompanyContact
```

Chạy lại, đọc lỗi kiểu dữ liệu rồi khôi phục `@model List<CompanyService>`.

### Tình huống 4. HTML có dữ liệu nhưng mất định dạng

Đổi tạm class `service-list` trong view thành `services-list`, tải lại trang và quan sát. Kiểm tra:

1. HTML có đúng class không.
2. Selector CSS có khớp không.
3. Layout có tải `site.css` không.
4. Network có trả CSS thành công không.
5. Trình duyệt có dùng cache cũ không.

Khôi phục class đúng sau khi ghi nhận kết quả.

### Ma trận khoanh vùng

| Hiện tượng | Tầng cần kiểm tra trước |
|---|---|
| URL trỏ nhầm trang | Link hoặc Tag Helper |
| Không tìm thấy controller/action | Route, tên controller/action, HTTP method |
| Action chạy nhưng thiếu view | Tên và vị trí tệp trong `Views` |
| View nhận sai kiểu | Dữ liệu từ action và khai báo `@model` |
| HTML đúng nhưng giao diện sai | Layout, class CSS, `site.css`, static assets, cache |

Sau mọi tình huống, chạy:

```bash
git diff
dotnet build AppDemo.slnx
```

Không để lỗi có chủ ý đi vào commit.

## 18. Viết unit test cho `CompanyController`

Unit test trong phần này gọi action như phương thức C#. Vì vậy, chúng kiểm tra loại `IActionResult` và model do action trả về, nhưng **không kiểm tra**:

- Middleware pipeline.
- Routing thực tế.
- HTTP request/response thật.
- Quá trình tìm và render Razor View.
- CSS trên trình duyệt.

### Bước 1. Tạo thư mục test controller

Tạo `AppDemo.Tests/Controllers/CompanyControllerTests.cs`:

```csharp
using AppDemo.Controllers;
using AppDemo.Models;
using AppDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AppDemo.Tests.Controllers;

public class CompanyControllerTests
{
    [Fact]
    public void Index_WhenCalled_ReturnsCompanyIndexViewModel()
    {
        // Arrange
        var controller = new CompanyController();

        // Act
        var result = controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CompanyIndexViewModel>(viewResult.Model);
        Assert.NotNull(model.Company);
        Assert.Equal(2, model.FeaturedServices.Count);
    }

    [Fact]
    public void Contact_WhenCalled_ReturnsCompanyContact()
    {
        // Arrange
        var controller = new CompanyController();

        // Act
        var result = controller.Contact();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CompanyContact>(viewResult.Model);
        Assert.Contains("@", model.Email);
        Assert.False(string.IsNullOrWhiteSpace(model.Phone));
    }

    [Fact]
    public void Services_WhenCalled_ReturnsThreeServices()
    {
        // Arrange
        var controller = new CompanyController();

        // Act
        var result = controller.Services();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<List<CompanyService>>(viewResult.Model);
        Assert.Equal(3, model.Count);
        Assert.All(model, service => Assert.True(service.EstimatedDays > 0));
    }
}
```

### Bước 2. Build trước, test sau

```bash
dotnet build AppDemo.slnx
dotnet test AppDemo.slnx --no-build --filter CompanyControllerTests
```

### Bước 3. Chạy toàn bộ test

```bash
dotnet test AppDemo.slnx --no-build
```

Không chỉ chạy test mới vì thay đổi Chương 2 không được làm hỏng test Chương 1.

### Điểm dừng kiểm chứng 5

- [ ] `Index()` trả `CompanyIndexViewModel` và 2 dịch vụ nổi bật.
- [ ] `Contact()` trả `CompanyContact` có email hợp lệ ở mức cơ bản.
- [ ] `Services()` trả 3 dịch vụ.
- [ ] Mỗi dịch vụ có `EstimatedDays > 0`.
- [ ] Toàn bộ test Chương 1 và Chương 2 đều Passed.
- [ ] Sinh viên nói đúng những gì unit test này không kiểm tra.

## 19. Bài tập mở rộng: trang đội ngũ

Không thực hiện phần này trước khi module cơ bản đã build và test thành công.

Yêu cầu:

1. Tạo model `CompanyMember` gồm `Name`, `Role`, `Summary`.
2. Thêm action `AboutTeam` vào `CompanyController`.
3. Action trả `List<CompanyMember>`.
4. Tạo `Views/Company/AboutTeam.cshtml`.
5. View dùng partial `_CompanyHeader` và layout hiện tại.
6. Thêm link Đội ngũ vào navigation.
7. Viết test xác nhận action trả đúng kiểu và danh sách không rỗng.

Trước khi code, sinh viên phải lập bảng:

| URL | Controller | Action | Model | View | File khác cần sửa |
|---|---|---|---|---|---|
| `/Company/AboutTeam` |  |  |  |  |  |

## 20. Sử dụng AI agent có kiểm soát

### Prompt 1. Chỉ đọc và báo cáo

```text
Hãy đọc project AppDemo và rà soát module Company.
Kiểm tra ba request: /Company, /Company/Contact và /Company/Services.

Với mỗi request, hãy chỉ ra:
- route value;
- controller và action;
- model hoặc view model;
- Razor View;
- partial view;
- layout và CSS liên quan.

Chỉ báo cáo kết quả, chưa sửa tệp và không chạy ứng dụng ngoài phạm vi kiểm tra.
```

### Prompt 2. Kiểm tra lỗi theo tầng

```text
Trong project AppDemo, /Company/Services đang báo không tìm thấy view.
Hãy kiểm tra theo thứ tự:
1. route mặc định trong Program.cs;
2. tên CompanyController;
3. tên action Services;
4. đường dẫn Views/Company/Services.cshtml;
5. kiểu model action truyền và @model của view.

Trước tiên báo nguyên nhân và tệp dự kiến sửa.
Nếu cần sửa, chỉ sửa đúng lỗi tìm thấy, không refactor phần khác,
không thêm database, Identity hoặc package.
Sau đó chạy dotnet build và các test CompanyControllerTests.
```

### Prompt 3. Mở rộng module

```text
Trước khi sửa code, hãy đề xuất thiết kế trang AboutTeam cho module Company:
URL, controller/action, model, view, partial, layout link, CSS và test.
Liệt kê chính xác các tệp sẽ tạo hoặc sửa.
Không sửa Program.cs nếu route mặc định đã đủ.
Chỉ thực hiện sau khi phương án được chấp nhận.
```

### Checklist sau khi dùng AI

- [ ] AI bám đúng project và namespace `AppDemo`.
- [ ] AI nối đúng request với controller/action/view.
- [ ] Không sửa `Program.cs` nếu không cần.
- [ ] Không thêm database, Identity, authentication hoặc package ngoài phạm vi.
- [ ] View nằm trong `Views/Company` và khai báo đúng `@model`.
- [ ] CSS nằm trong `wwwroot/css/site.css`.
- [ ] Sinh viên đã đọc diff.
- [ ] Build và toàn bộ test thành công.

## 21. Kiểm chứng cuối chương

### Bước 1. Kiểm chứng sạch

```bash
dotnet clean AppDemo.slnx
dotnet restore AppDemo.slnx
dotnet build AppDemo.slnx --no-restore
dotnet test AppDemo.slnx --no-build
```

### Bước 2. Kiểm tra giao diện

```bash
dotnet run --project AppDemo/AppDemo.csproj
```

Kiểm tra ba URL, navigation, partial, CSS và nội dung. Sau đó dừng server bằng `Ctrl + C`.

### Bước 3. Đọc phạm vi thay đổi

```bash
git status
git diff --stat
git diff
```

Danh sách file hợp lý thường gồm:

```text
AppDemo/Controllers/CompanyController.cs
AppDemo/Models/CompanyProfile.cs
AppDemo/Models/CompanyContact.cs
AppDemo/Models/CompanyService.cs
AppDemo/ViewModels/CompanyIndexViewModel.cs
AppDemo/Views/Company/Index.cshtml
AppDemo/Views/Company/Contact.cshtml
AppDemo/Views/Company/Services.cshtml
AppDemo/Views/Shared/_CompanyHeader.cshtml
AppDemo/Views/Shared/_Layout.cshtml
AppDemo/Views/_ViewImports.cshtml
AppDemo/wwwroot/css/site.css
AppDemo.Tests/Controllers/CompanyControllerTests.cs
readme-chuong-02.md
```

Nếu có tệp database, Identity, secret hoặc thay đổi không liên quan, phải dừng và kiểm tra trước khi stage.

## 22. Commit, hợp nhất và tạo tag

### Bước 1. Stage đúng phạm vi

Có thể stage theo thư mục/tệp sau khi đã đọc `git diff`:

```bash
git add AppDemo/Controllers/CompanyController.cs
git add AppDemo/Models
git add AppDemo/ViewModels
git add AppDemo/Views/Company
git add AppDemo/Views/Shared/_CompanyHeader.cshtml
git add AppDemo/Views/Shared/_Layout.cshtml
git add AppDemo/Views/_ViewImports.cshtml
git add AppDemo/wwwroot/css/site.css
git add AppDemo.Tests/Controllers/CompanyControllerTests.cs
git add readme-chuong-02.md
```

Kiểm tra staged diff:

```bash
git status
git diff --cached --stat
git diff --cached
```

### Bước 2. Commit

```bash
git commit -m "feat(chapter-02): add Company MVC module"
```

### Bước 3. Push nhánh

```bash
git push -u origin chapter-02
```

Nếu lớp học không dùng pull request và giảng viên cho phép hợp nhất cục bộ:

```bash
git switch main
git merge --no-ff chapter-02
dotnet build AppDemo.slnx
dotnet test AppDemo.slnx --no-build
git push origin main
```

Nếu dùng GitHub Pull Request, tạo PR từ `chapter-02` vào `main`, review thay đổi và chỉ merge khi build/test đạt yêu cầu.

### Bước 4. Tạo tag

Chỉ tạo tag trên commit `main` đã kiểm chứng:

```bash
git switch main
git tag chapter-02-completed
git push origin chapter-02-completed
```

Kiểm tra:

```bash
git status
git log --oneline --decorate -8
git show chapter-02-completed --stat
```

## 23. Lỗi thường gặp

| Hiện tượng | Nguyên nhân thường gặp | Cách xử lý |
|---|---|---|
| `/Company` trả 404 | Sai tên/hậu tố controller | Dùng `CompanyController : Controller` |
| `/Company` chạy nhưng trang khác 404 | Thiếu hoặc sai tên action | Kiểm tra `Contact()` và `Services()` |
| Báo không tìm thấy view | View sai tên/thư mục | Dùng `Views/Company/<Action>.cshtml` |
| View báo sai kiểu model | Action và `@model` không thống nhất | Đối chiếu kiểu object trong `ViewResult.Model` |
| Không nhận ra `CompanyContact` trong view | Thiếu namespace ở `_ViewImports.cshtml` | Thêm `@using AppDemo.Models` |
| Tag Helper sinh sai/không hoạt động | Thiếu `@addTagHelper` hoặc sai controller/action | Kiểm tra `_ViewImports.cshtml` và thuộc tính `asp-*` |
| Partial không tìm thấy | Sai tên/vị trí partial | Đặt tại `Views/Shared/_CompanyHeader.cshtml` |
| CSS không áp dụng | Sai class, selector, link CSS hoặc cache | Kiểm tra HTML, `site.css`, layout và Network |
| Build báo namespace không tồn tại | Namespace trong mã nguồn chưa đồng nhất | Đổi namespace về tên project thực tế `AppDemo` |
| Test không nhận controller/model | Thiếu project reference hoặc `using` | Kiểm tra `.csproj` và namespace |
| Chỉ test mới Passed nhưng test cũ lỗi | Chỉ chạy filter | Chạy lại toàn bộ `dotnet test AppDemo.slnx` |
| Sửa tệp trong `bin/Debug/net10.0` | Nhầm output với source | Chỉ sửa `Controllers`, `Models`, `ViewModels`, `Views`, `wwwroot` |

## 24. Câu hỏi tự kiểm tra

1. Vì sao ba trang HTML rời rạc gây lặp và khó bảo trì?
2. Phần trước và sau `builder.Build()` trong `Program.cs` khác nhau thế nào?
3. `AddControllersWithViews()` đăng ký khả năng gì?
4. Phân biệt các lệnh `Use...` và `Map...`.
5. Route mặc định phân tích `/Company/Services` như thế nào?
6. Vì sao `/Company/Services` không phải đường dẫn tới `Services.cshtml`?
7. Routing và view discovery khác nhau ở bước nào?
8. Vì sao trang Index cần `CompanyIndexViewModel`?
9. Partial view và layout xử lý hai mức tái sử dụng khác nhau thế nào?
10. Khi HTML có dữ liệu nhưng mất style, cần kiểm tra theo thứ tự nào?
11. Vì sao không nên truy vấn database hoặc tạo dữ liệu nghiệp vụ trong view?
12. Unit test gọi trực tiếp `CompanyController.Services()` kiểm tra được gì?
13. Vì sao unit test đó không chứng minh `/Company/Services` được route đúng?
14. Vì sao thêm action mới theo route mặc định thường không cần sửa `Program.cs`?
15. Trước khi tạo tag cuối chương, cần những bằng chứng nào?

## 25. Bảng kiểm hoàn thành Chương 2

### Kiến trúc và Routing

- [ ] Giải thích được request pipeline trong `Program.cs`.
- [ ] Phân tích đúng route mặc định.
- [ ] Phân biệt routing, action execution và view discovery.

### Module Company

- [ ] Có `CompanyController` với `Index`, `Contact`, `Services`.
- [ ] Có ba model và một view model đúng trách nhiệm.
- [ ] Có ba strongly typed view trong `Views/Company`.
- [ ] Có partial `_CompanyHeader` dùng chung.
- [ ] Layout có ba liên kết Company bằng Tag Helper.
- [ ] CSS nằm trong `wwwroot/css/site.css`.

### Kiểm chứng

- [ ] Ba URL trả status 200 và nội dung đúng.
- [ ] CSS được tải thành công.
- [ ] Project build thành công.
- [ ] Các test Company đều Passed.
- [ ] Toàn bộ test Chương 1 và 2 đều Passed.
- [ ] Không còn lỗi có chủ ý trong source.

### Git/GitHub

- [ ] Đã đọc `git status`, `git diff` và staged diff.
- [ ] Commit chỉ chứa module Company, test và tài liệu liên quan.
- [ ] Không có secret, file tạm, `bin/` hoặc `obj/`.
- [ ] Thay đổi đã được merge vào `main`.
- [ ] Đã push tag `chapter-02-completed`.
- [ ] Đã chuẩn bị URL, commit, tag và kết quả test.

## 26. Chuỗi lệnh tham khảo nhanh

Không chạy máy móc. Đọc kết quả sau mỗi nhóm lệnh.

```bash
# Kiểm tra baseline
git status
dotnet build AppDemo.slnx
dotnet test AppDemo.slnx --no-build

# Tạo nhánh thực hành
git switch -c chapter-02

# Sau khi tạo module Company
dotnet build AppDemo.slnx
dotnet test AppDemo.slnx --no-build --filter CompanyControllerTests
dotnet test AppDemo.slnx --no-build

# Chạy ứng dụng
dotnet watch --project AppDemo/AppDemo.csproj run

# Kiểm tra thay đổi
git status
git diff --stat
git diff

# Stage và commit
git add AppDemo/Controllers/CompanyController.cs
git add AppDemo/Models AppDemo/ViewModels AppDemo/Views
git add AppDemo/wwwroot/css/site.css
git add AppDemo.Tests/Controllers/CompanyControllerTests.cs
git add readme-chuong-02.md
git diff --cached
git commit -m "feat(chapter-02): add Company MVC module"
git push -u origin chapter-02

# Sau khi review/merge vào main
git switch main
dotnet build AppDemo.slnx
dotnet test AppDemo.slnx --no-build
git push origin main
git tag chapter-02-completed
git push origin chapter-02-completed
```

---

**Nguyên tắc cuối chương:** không đánh giá module chỉ bằng việc “trang đã mở được”. Sinh viên phải chứng minh request đi đúng route, controller/action chuẩn bị đúng kiểu dữ liệu, view nằm đúng vị trí và dùng đúng model, giao diện tái sử dụng layout/partial/CSS, test phản ánh đúng phạm vi và phiên bản được lưu sau khi build/test thành công.
