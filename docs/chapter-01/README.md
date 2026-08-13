# THỰC HÀNH CHƯƠNG 1: LÀM QUEN VỚI ASP.NET CORE MVC

## Phiên bản thực hành

- Điểm bắt đầu: `course-start`
- Phiên bản tham khảo: `chapter-01-completed`

```bash
git switch -c practice-chapter-01 course-start
```

Không thực hành trực tiếp trên `main`, vì `main` có thể chứa code của các chương sau.

Tài liệu này hướng dẫn sinh viên tự thực hành Chương 1 của giáo trình *Công nghệ phát triển ứng dụng*. Sau khi hoàn thành, sinh viên sẽ tự tạo được một ứng dụng ASP.NET Core MVC bằng .NET 10, sửa trang chủ, thêm trang giới thiệu, chạy kiểm thử xUnit và lưu phiên bản trên Git/GitHub.

> Quy ước tên trong tài liệu: ứng dụng chính là `CompanyInfo`, project kiểm thử là `CompanyInfo.Tests`. Nếu giảng viên yêu cầu dùng `AppDemo`, hãy thay `CompanyInfo` bằng `AppDemo` trong tên thư mục, project, solution và các lệnh tương ứng.

## 1. Kết quả cần đạt

Sau bài thực hành, sinh viên có thể:

- Giải thích ngắn gọn luồng trình duyệt gửi request và server trả response.
- Phân biệt vai trò của C#, .NET, ASP.NET Core và mô hình MVC.
- Kiểm tra được phiên bản .NET SDK và các công cụ cần thiết.
- Tạo, build và chạy một project ASP.NET Core MVC bằng dòng lệnh.
- Xác định được các thư mục, tệp quan trọng trong project MVC.
- Sửa trang chủ và quan sát thay đổi trên trình duyệt.
- Thêm action, Razor View và liên kết điều hướng cho trang `About`.
- Tạo project xUnit, thêm tham chiếu đến ứng dụng chính và chạy test.
- Khởi tạo Git, commit, push lên GitHub và tạo tag cuối chương.
- Sử dụng AI agent trong phạm vi rõ ràng và tự kiểm chứng kết quả.

## 2. Sản phẩm phải nộp

Sinh viên cần chuẩn bị:

1. URL repository GitHub.
2. Mã commit cuối cùng trên nhánh `main`.
3. Tag `chapter-01-completed`.
4. Ảnh hoặc tệp kết quả `dotnet build` thành công.
5. Ảnh hoặc tệp kết quả `dotnet test` có tất cả test `Passed`.
6. Ảnh trang chủ và trang `/Home/About` trên trình duyệt.
7. Bản trả lời ngắn cho các câu hỏi tự kiểm tra ở cuối tài liệu.

## 3. Kiến thức cần nhớ trước khi làm

### 3.1. Luồng request–response

Khi truy cập một URL:

1. Trình duyệt đóng vai trò **client** và gửi HTTP request.
2. Kestrel nhận request và chuyển vào ứng dụng ASP.NET Core.
3. Routing xác định controller và action phù hợp.
4. Action xử lý yêu cầu và chọn view.
5. Razor View sinh HTML.
6. Server trả HTTP response cho trình duyệt.
7. Trình duyệt dựng HTML/CSS thành giao diện.

Trong bài này, khi mở `/Home/About`, request sẽ đi đến action `About` của `HomeController`, sau đó ASP.NET Core MVC tìm view `Views/Home/About.cshtml`.

### 3.2. GET và POST

- `GET` thường dùng để đọc hoặc hiển thị tài nguyên, chẳng hạn mở trang chủ.
- `POST` thường dùng khi gửi dữ liệu để server xử lý hoặc thay đổi trạng thái.

Chương 1 chủ yếu sử dụng các request `GET`. Form và `POST` sẽ được học sâu hơn ở chương sau.

### 3.3. Vai trò của các thành phần

| Thành phần | Vai trò trong bài thực hành |
|---|---|
| C# | Ngôn ngữ viết controller và logic phía server |
| .NET SDK | Tạo, restore, build, chạy và kiểm thử project |
| ASP.NET Core | Nền tảng chạy ứng dụng web |
| MVC | Tổ chức ứng dụng thành Model, View và Controller |
| Razor | Kết hợp HTML với cú pháp phía server trong tệp `.cshtml` |
| Kestrel | Web server nhận request trên địa chỉ localhost |
| VS Code | Mở, đọc và sửa mã nguồn |
| Trình duyệt | Client gửi request và hiển thị response |

## 4. Chuẩn bị môi trường

### Bước 1. Cài Visual Studio Code

Tải VS Code từ trang chính thức:

<https://code.visualstudio.com/>

Trên Windows, nếu trình cài đặt hiển thị tùy chọn, nên bật:

- Add to PATH.
- Open with Code.
- Register Code as an editor for supported file types.

### Bước 2. Cài .NET 10 SDK

Tải **SDK**, không chỉ tải Runtime:

<https://dotnet.microsoft.com/download>

- Windows 64-bit thông thường: chọn Windows x64.
- Mac Apple Silicon: chọn Arm64.
- Mac dùng chip Intel: chọn x64.

Mở terminal mới và chạy:

```bash
dotnet --version
```

**Kết quả mong đợi:** phiên bản bắt đầu bằng `10.`.

Có thể kiểm tra chi tiết hơn:

```bash
dotnet --info
dotnet --list-sdks
```

Nếu terminal báo không nhận lệnh `dotnet` trên Windows:

1. Đóng và mở lại VS Code/Terminal.
2. Kiểm tra biến môi trường `Path`.
3. Kiểm tra có đường dẫn `C:\Program Files\dotnet\`.
4. Mở terminal mới rồi chạy lại `dotnet --version`.

### Bước 3. Cài extension cho VS Code

Mở khu vực **Extensions** và cài:

- C# Dev Kit của Microsoft.
- C# của Microsoft.
- IntelliCode for C# Dev Kit.
- HTML CSS Support.

Có thể cài thêm Prettier và một icon theme, nhưng đây không phải yêu cầu bắt buộc.

### Bước 4. Kiểm tra lệnh mở VS Code

```bash
code --version
```

Nếu lệnh `code` chưa hoạt động, vẫn có thể mở project bằng **File > Open Folder** và chọn đúng thư mục project.

### Bước 5. Kiểm tra HTTPS development certificate

```bash
dotnet dev-certs https --check
```

Nếu máy chưa tin cậy certificate phát triển, chạy:

```bash
dotnet dev-certs https --trust
```

Trên một số hệ điều hành, hệ thống sẽ yêu cầu xác nhận. Certificate này chỉ dùng cho môi trường phát triển cục bộ.

### Điểm dừng kiểm chứng 1

Chỉ chuyển sang phần tiếp theo khi:

- [ ] `dotnet --version` trả về phiên bản 10.x.
- [ ] VS Code mở bình thường.
- [ ] C# Dev Kit không báo lỗi cài đặt.
- [ ] Có một trình duyệt để mở địa chỉ localhost.

## 5. Tạo workspace, solution và project MVC

### Bước 1. Tạo thư mục làm việc

Mở PowerShell, Command Prompt hoặc Terminal:

```bash
mkdir AspNetPractice
cd AspNetPractice
```

Từ đây, mọi lệnh không ghi chú khác đều được chạy trong thư mục `AspNetPractice`.

### Bước 2. Tạo solution

```bash
dotnet new sln -n CompanyInfo
```

Với .NET 10, solution thường được tạo dưới dạng `CompanyInfo.slnx`. Nếu máy tạo `CompanyInfo.sln`, hãy dùng đúng tên tệp thực tế trong các lệnh phía sau.

### Bước 3. Tạo project ASP.NET Core MVC

```bash
dotnet new mvc -n CompanyInfo --framework net10.0
```

### Bước 4. Thêm project vào solution

```bash
dotnet sln CompanyInfo.slnx add CompanyInfo/CompanyInfo.csproj
```

Nếu solution của máy là `.sln`, thay `CompanyInfo.slnx` bằng `CompanyInfo.sln`.

### Bước 5. Restore và build

```bash
dotnet restore CompanyInfo.slnx
dotnet build CompanyInfo.slnx
```

**Kết quả mong đợi:** xuất hiện dòng `Build succeeded.` và không có error.

> Không sửa tệp trong `bin/` hoặc `obj/`. Đây là kết quả sinh ra khi restore/build, không phải mã nguồn cần chỉnh sửa.

### Bước 6. Mở workspace

```bash
code .
```

Trong Explorer của VS Code, kiểm tra cấu trúc cơ bản:

```text
AspNetPractice/
├── CompanyInfo.slnx
└── CompanyInfo/
    ├── Controllers/
    │   └── HomeController.cs
    ├── Models/
    ├── Views/
    │   ├── Home/
    │   │   └── Index.cshtml
    │   └── Shared/
    │       └── _Layout.cshtml
    ├── wwwroot/
    ├── Program.cs
    ├── appsettings.json
    └── CompanyInfo.csproj
```

### Bước 7. Đọc các tệp quan trọng

Không cần hiểu mọi dòng, nhưng phải trả lời được:

- `Program.cs`: đăng ký dịch vụ, cấu hình HTTP pipeline và khởi động ứng dụng.
- `Controllers/HomeController.cs`: chứa các action xử lý request của trang Home.
- `Views/Home/Index.cshtml`: sinh nội dung HTML của trang chủ.
- `Views/Shared/_Layout.cshtml`: chứa bố cục và thanh điều hướng dùng chung.
- `wwwroot`: chứa tài nguyên tĩnh như CSS, JavaScript và hình ảnh.
- `appsettings.json`: chứa cấu hình không nhạy cảm của ứng dụng.

### Điểm dừng kiểm chứng 2

- [ ] Có solution và project `CompanyInfo`.
- [ ] Project đã được thêm vào solution.
- [ ] `dotnet build` thành công.
- [ ] Sinh viên chỉ ra được năm tệp/thư mục quan trọng ở trên.

## 6. Chạy ứng dụng MVC đầu tiên

### Bước 1. Chạy bằng `dotnet run`

```bash
dotnet run --project CompanyInfo/CompanyInfo.csproj
```

Đọc terminal và tìm dòng dạng:

```text
Now listening on: https://localhost:xxxx
Now listening on: http://localhost:yyyy
```

Số cổng của mỗi máy có thể khác nhau. Hãy mở đúng URL do terminal cung cấp, không tự đoán cổng.

### Bước 2. Quan sát request và response

1. Mở DevTools của trình duyệt bằng `F12`.
2. Chọn tab **Network**.
3. Tải lại trang.
4. Chọn request đầu tiên và quan sát:
   - Request URL.
   - Request Method.
   - Status Code.
   - Response Headers.
   - Nội dung response.

Ghi lại một dòng nhận xét, ví dụ: “Trình duyệt gửi GET đến địa chỉ localhost và server trả status 200 cùng nội dung HTML.”

### Bước 3. Dừng server

Quay lại terminal và nhấn:

```text
Ctrl + C
```

### Bước 4. Chạy ở chế độ theo dõi thay đổi

```bash
dotnet watch --project CompanyInfo/CompanyInfo.csproj run
```

Giữ terminal này hoạt động trong khi sửa view. Khi mã thay đổi, `dotnet watch` có thể Hot Reload hoặc khởi động lại ứng dụng.

### Điểm dừng kiểm chứng 3

- [ ] Kestrel khởi động và hiển thị URL localhost.
- [ ] Trang mặc định mở được.
- [ ] Network hiển thị request `GET` với status `200`.
- [ ] Sinh viên biết dùng `Ctrl + C` để dừng ứng dụng.

## 7. Sửa trang chủ

**Mã nguồn tham khảo:** [https://github.com/NguyenPhuong86/CompanyInfo](https://github.com/NguyenPhuong86/CompanyInfo)

Mở `CompanyInfo/Views/Home/Index.cshtml` và thay nội dung bằng:

```cshtml
@{
    ViewData["Title"] = "Trang chủ";
}

<div class="text-center">
    <h1>Công ty ABC</h1>
    <p>Ứng dụng ASP.NET Core MVC đầu tiên của tôi.</p>
</div>
```

Lưu tệp và tải lại:

- `/`, hoặc
- `/Home/Index`.

**Kết quả mong đợi:** trang hiển thị “Công ty ABC” và đoạn giới thiệu mới.

Nếu nội dung chưa thay đổi:

1. Xác nhận đã sửa đúng `Views/Home/Index.cshtml`.
2. Lưu tệp.
3. Quan sát thông báo của `dotnet watch`.
4. Tải lại trang bằng `Ctrl + F5`.
5. Nếu vẫn lỗi, dừng server và chạy lại.

## 8. Thêm trang Giới thiệu

### Bước 1. Thêm action `About`

Mở `CompanyInfo/Controllers/HomeController.cs`. Trong lớp `HomeController`, thêm:

```csharp
public IActionResult About()
{
    return View();
}
```

Không đặt action này bên ngoài dấu `}` kết thúc lớp.

### Bước 2. Tạo Razor View

Tạo tệp `CompanyInfo/Views/Home/About.cshtml`:

```cshtml
@{
    ViewData["Title"] = "Giới thiệu";
}

<h1>Giới thiệu</h1>
<p>Đây là trang giới thiệu đầu tiên được thêm vào project MVC.</p>
```

### Bước 3. Kiểm tra URL trực tiếp

Mở URL:

```text
/Home/About
```

Ví dụ, nếu ứng dụng chạy tại `https://localhost:7123`, URL đầy đủ sẽ là:

```text
https://localhost:7123/Home/About
```

**Kết quả mong đợi:** trang Giới thiệu xuất hiện mà không báo lỗi “view not found”.

### Bước 4. Thêm liên kết vào thanh điều hướng

Mở `CompanyInfo/Views/Shared/_Layout.cshtml`, tìm danh sách có class `navbar-nav` và thêm mục sau cạnh các liên kết Home/Privacy:

```cshtml
<li class="nav-item">
    <a class="nav-link text-dark"
       asp-area=""
       asp-controller="Home"
       asp-action="About">Giới thiệu</a>
</li>
```

Tải lại trang và bấm liên kết **Giới thiệu**.

Các Tag Helper `asp-controller` và `asp-action` giúp ASP.NET Core tạo URL dựa trên controller/action, thay vì viết cứng URL.

### Bước 5. Giải thích đường đi của request

Hoàn thành câu sau bằng lời của mình:

> Khi người dùng bấm “Giới thiệu”, trình duyệt gửi request GET đến ________. Routing chọn action ________ trong lớp ________. Action trả về view tại ________. View sinh ________ và server gửi kết quả về trình duyệt.

Đáp án kỹ thuật cần có: `/Home/About`, `About`, `HomeController`, `Views/Home/About.cshtml`, HTML.

### Điểm dừng kiểm chứng 4

- [ ] Action `About` nằm trong `HomeController`.
- [ ] View nằm đúng tại `Views/Home/About.cshtml`.
- [ ] Mở trực tiếp `/Home/About` thành công.
- [ ] Link Giới thiệu xuất hiện trên navigation và hoạt động.
- [ ] Sinh viên giải thích được đường đi của request.

## 9. Thực hành HTTPS và HTTP cache ở mức quan sát

Phần này giúp kết nối thực hành với nội dung HTTPS, cache và ranh giới tin cậy của trình duyệt.

### Bước 1. So sánh HTTP và HTTPS

1. Quan sát các URL mà Kestrel cung cấp.
2. Mở URL HTTP và HTTPS nếu cả hai đều có.
3. Trong tab Network, so sánh URL, status code và việc chuyển hướng.

Ghi nhớ: HTTPS bảo vệ kênh truyền, không tự động chứng minh nghiệp vụ, xác thực hoặc phân quyền của ứng dụng là đúng.

### Bước 2. Thay đổi query string

Thử mở:

```text
/Home/About?source=student
```

Sau đó thay `student` bằng một giá trị khác. Giao diện có thể không thay đổi vì action chưa đọc tham số, nhưng request từ client đã thay đổi. Điều này minh họa rằng dữ liệu từ URL do người dùng kiểm soát và server không được mặc định tin cậy.

### Bước 3. Quan sát cache

Trong tab Network:

1. Tải lại một tài nguyên CSS trong `wwwroot`.
2. Quan sát `Cache-Control`, `ETag` hoặc `Last-Modified` nếu có.
3. Tải lại trang và kiểm tra có response `304 Not Modified` hay trạng thái “from memory/disk cache” hay không.

Không bắt buộc mọi máy đều xuất hiện `304`, vì hành vi phụ thuộc header và trình duyệt. Yêu cầu chính là sinh viên xác định được header hoặc trạng thái cache đã quan sát.

## 10. Tạo project kiểm thử xUnit

Đảm bảo server đã dừng trước khi chạy các lệnh kiểm thử.

### Bước 1. Tạo project test

Từ thư mục `AspNetPractice`:

```bash
dotnet new xunit -n CompanyInfo.Tests --framework net10.0
```

### Bước 2. Thêm tham chiếu đến ứng dụng chính

```bash
dotnet add CompanyInfo.Tests/CompanyInfo.Tests.csproj reference CompanyInfo/CompanyInfo.csproj
```

### Bước 3. Thêm project test vào solution

```bash
dotnet sln CompanyInfo.slnx add CompanyInfo.Tests/CompanyInfo.Tests.csproj
```

### Bước 4. Tạo test đầu tiên

Mở tệp test mặc định trong `CompanyInfo.Tests`. Có thể đổi tên thành `TestInfrastructureTests.cs` và thay nội dung bằng:

```csharp
namespace CompanyInfo.Tests;

public class TestInfrastructureTests
{
    [Fact]
    public void TestRunner_WhenExecuted_RunsXunitTest()
    {
        // Arrange: test này không cần dữ liệu đầu vào.

        // Act: test runner thực thi phương thức test.

        // Assert
        Assert.True(true);
    }
}
```

Test này chỉ xác nhận project test, xUnit và test runner hoạt động. Nó chưa kiểm tra quy tắc nghiệp vụ.

### Bước 5. Build và chạy test

```bash
dotnet build CompanyInfo.slnx
dotnet test CompanyInfo.slnx --no-build
```

**Kết quả mong đợi:** tất cả test có trạng thái `Passed`.

### Bước 6. Quan sát một test thất bại có chủ ý

1. Đổi tạm `Assert.True(true)` thành `Assert.True(false)`.
2. Chạy lại:

   ```bash
   dotnet test CompanyInfo.slnx
   ```

3. Đọc tên test, vị trí lỗi, expected/actual hoặc thông báo assertion.
4. Đổi lại thành `Assert.True(true)`.
5. Chạy lại test và xác nhận `Passed`.

Không commit trạng thái test đang thất bại.

### Bước 7. Bài test mở rộng

Mở `CompanyInfo/Models/ErrorViewModel.cs`, đọc property `ShowRequestId` rồi viết ít nhất hai test:

- `RequestId` có giá trị → `ShowRequestId` là `true`.
- `RequestId` rỗng hoặc `null` → `ShowRequestId` là `false`.

Tên test gợi ý:

```text
ShowRequestId_WhenRequestIdHasValue_ReturnsTrue
ShowRequestId_WhenRequestIdIsEmpty_ReturnsFalse
```

### Điểm dừng kiểm chứng 5

- [ ] `CompanyInfo.Tests` tham chiếu `CompanyInfo`.
- [ ] Cả hai project nằm trong solution.
- [ ] Sinh viên đã quan sát một test Failed có chủ ý.
- [ ] Đã hoàn tác và toàn bộ test hiện Passed.
- [ ] Sinh viên phân biệt được test hạ tầng và test hành vi.

## 11. Kiểm tra toàn bộ sản phẩm trước khi dùng Git

Chạy lần lượt:

```bash
dotnet clean CompanyInfo.slnx
dotnet restore CompanyInfo.slnx
dotnet build CompanyInfo.slnx --no-restore
dotnet test CompanyInfo.slnx --no-build
```

Chạy lại ứng dụng:

```bash
dotnet run --project CompanyInfo/CompanyInfo.csproj
```

Kiểm tra thủ công:

- Trang chủ hiển thị đúng nội dung.
- `/Home/About` mở được.
- Link Giới thiệu hoạt động.
- Không có lỗi trong terminal.

Sau đó dừng server bằng `Ctrl + C`.

## 12. Quản lý phiên bản với Git và GitHub

### Bước 1. Kiểm tra Git

```bash
git --version
```

### Bước 2. Kiểm tra `.gitignore`

Template .NET thường tạo `.gitignore` khi được yêu cầu riêng, nhưng không nên giả định. Từ thư mục `AspNetPractice`, nếu chưa có `.gitignore`, chạy:

```bash
dotnet new gitignore
```

Mở `.gitignore` và xác nhận có quy tắc loại trừ ít nhất `bin/`, `obj/`, file người dùng IDE, log và secret cục bộ.

### Bước 3. Khởi tạo repository

```bash
git init
git branch -M main
git status
```

### Bước 4. Xem thay đổi trước khi stage

```bash
git diff
git status
```

Với file mới chưa được Git theo dõi, `git diff` có thể chưa hiển thị nội dung; hãy dùng `git status` để xác định file mới.

### Bước 5. Stage có kiểm soát

```bash
git add .gitignore CompanyInfo CompanyInfo.Tests CompanyInfo.slnx README.md
git status
git diff --cached
```

Nếu solution là `.sln`, thay tên tương ứng. Không tiếp tục nếu staged diff chứa:

- `bin/` hoặc `obj/`.
- Database cục bộ.
- Log.
- Mật khẩu, token hoặc API key.
- Tệp không liên quan đến bài thực hành.

### Bước 6. Commit

```bash
git commit -m "chore(chapter-01): initialize ASP.NET Core MVC project"
```

### Bước 7. Tạo repository GitHub

Trên GitHub, tạo repository trống. Để tránh xung đột lịch sử ở lần push đầu tiên, không yêu cầu GitHub sinh sẵn README, `.gitignore` hoặc license nếu các tệp này đã có ở máy.

Sao chép URL repository rồi chạy:

```bash
git remote add origin <URL_REPOSITORY>
git remote -v
git push -u origin main
```

Thay `<URL_REPOSITORY>` bằng URL thật, không giữ nguyên dấu `<` và `>`.

### Bước 8. Tạo tag sau khi đã kiểm chứng

Chỉ thực hiện khi build và test đều thành công:

```bash
git tag chapter-01-completed
git push origin chapter-01-completed
```

Kiểm tra:

```bash
git status
git log --oneline --decorate -5
git tag
```

**Kết quả mong đợi:** working tree sạch, nhánh `main` đã push và tag `chapter-01-completed` trỏ tới commit đã kiểm chứng.

## 13. Sử dụng AI agent có kiểm soát

Phần này thực hiện **sau khi sinh viên đã tự tạo và hiểu project cơ bản**.

### Nguyên tắc

Một yêu cầu tốt cần nêu:

- Công nghệ và phiên bản.
- Tên project.
- Mục tiêu cụ thể.
- Phạm vi tệp được phép sửa.
- Điều không được làm.
- Lệnh kiểm chứng phải chạy.
- Nội dung cần báo cáo sau khi hoàn thành.

### Prompt 1: rà soát thay đổi hiện tại

```text
Hãy rà soát project CompanyInfo dùng .NET 10 và ASP.NET Core MVC.
Kiểm tra trang chủ, action About, view Views/Home/About.cshtml và liên kết
Giới thiệu trong _Layout.cshtml.

Không thêm database, authentication hoặc package ngoài template mặc định.
Chưa sửa mã nguồn. Trước tiên hãy báo các vấn đề tìm thấy, nêu đúng tệp và lý do.
Sau đó chạy dotnet build và dotnet test để kiểm chứng.
```

### Prompt 2: yêu cầu sửa có giới hạn

```text
Trong project CompanyInfo, hãy sửa trang chủ để hiển thị:
- tên công ty: CompanyInfo;
- một slogan ngắn;
- ba dịch vụ chính.

Chỉ sửa các view cần thiết, không thêm database và không thêm package.
Sau khi sửa, chạy dotnet build và dotnet test.
Báo lại các lệnh đã chạy, các tệp đã sửa và kết quả kiểm tra.
```

### Checklist sau khi dùng AI

- [ ] Tôi biết AI đã chạy lệnh nào.
- [ ] Tôi đã đọc các tệp hoặc diff mà AI thay đổi.
- [ ] Không có tệp ngoài phạm vi bị sửa.
- [ ] Tôi hiểu vì sao code hoạt động.
- [ ] `dotnet build` thành công.
- [ ] `dotnet test` thành công.
- [ ] Tôi đã kiểm tra lại trên trình duyệt.

Không chấp nhận một thay đổi chỉ vì AI nói rằng đã hoàn thành.

## 14. Lỗi thường gặp và cách xử lý

| Hiện tượng | Nguyên nhân thường gặp | Cách kiểm tra/xử lý |
|---|---|---|
| Không nhận lệnh `dotnet` | Chưa cài SDK hoặc terminal chưa nhận PATH mới | Chạy `dotnet --info`, mở terminal mới, kiểm tra PATH |
| `dotnet new mcv` báo lỗi | Gõ sai tên template | Dùng `dotnet new mvc` |
| `dotnet run` không tìm thấy project | Chạy sai thư mục hoặc không chỉ định project | Chạy từ thư mục project hoặc dùng `--project CompanyInfo/CompanyInfo.csproj` |
| Trình duyệt không kết nối | Server chưa chạy hoặc mở sai cổng | Đọc dòng `Now listening on` trong terminal |
| Cảnh báo HTTPS | Development certificate chưa được tin cậy | Chạy `dotnet dev-certs https --trust` |
| Sửa view nhưng giao diện không đổi | Chưa lưu, cache hoặc watch chưa áp dụng thay đổi | Lưu tệp, `Ctrl + F5`, quan sát terminal, chạy lại server |
| `/Home/About` trả 404 | Thiếu action hoặc sai tên URL | Kiểm tra `About()` trong `HomeController` |
| Báo không tìm thấy view | Tạo view sai thư mục/tên | Dùng đúng `Views/Home/About.cshtml` |
| Link không xuất hiện | Sửa sai `_Layout.cshtml` hoặc đặt HTML sai vị trí | Kiểm tra `Views/Shared/_Layout.cshtml` và `navbar-nav` |
| Build lỗi sau khi thêm action | Sai dấu ngoặc hoặc action nằm ngoài lớp | Đọc dòng lỗi đầu tiên và vị trí file/dòng |
| Test project không dùng được lớp ứng dụng | Chưa thêm project reference | Chạy lại `dotnet add ... reference ...` |
| Git push bị `fetch first` | Remote đã có commit riêng | Với bài mới, tạo remote trống; không force push khi chưa hiểu lịch sử |
| Git đưa cả `bin/`, `obj/` vào stage | Thiếu/sai `.gitignore` | Bỏ các tệp khỏi stage, sửa `.gitignore`, kiểm tra lại staged diff |

## 15. Bài tập tự thực hành

### Bài 1. Mở rộng trang chủ

Sửa trang chủ để có:

- Tên công ty.
- Slogan.
- Ba dịch vụ chính trình bày bằng danh sách hoặc ba khối nội dung.
- Liên kết tới trang Giới thiệu.

Yêu cầu: không thêm database và không thêm package.

### Bài 2. Thêm trang Liên hệ

1. Thêm action `Contact` vào `HomeController`.
2. Tạo `Views/Home/Contact.cshtml`.
3. Hiển thị địa chỉ, email minh họa và số điện thoại minh họa.
4. Thêm liên kết “Liên hệ” vào navigation.
5. Giải thích đường đi của request `/Home/Contact`.

Không dùng thông tin cá nhân thật trong repository công khai.

### Bài 3. Kiểm tra quy ước MVC

Đổi tạm tên `About.cshtml` thành một tên sai, mở `/Home/About`, ghi lại lỗi, sau đó khôi phục đúng tên và xác nhận trang hoạt động. Mục tiêu là hiểu mối quan hệ giữa action và vị trí view, không phải chỉ tạo ra lỗi.

### Bài 4. Kiểm thử

Hoàn thiện hai test cho `ErrorViewModel.ShowRequestId` và giải thích vì sao chúng có giá trị hơn `Assert.True(true)`.

## 16. Câu hỏi tự kiểm tra

1. Console application và web application khác nhau thế nào về luồng tương tác?
2. Client, server, request và response có vai trò gì?
3. Vì sao không được tự đoán cổng localhost?
4. GET và POST thường được dùng cho loại thao tác nào?
5. C#, .NET SDK, ASP.NET Core và MVC khác nhau thế nào?
6. `HomeController`, action `About` và `Views/Home/About.cshtml` liên hệ với nhau ra sao?
7. Vì sao sửa file trong `bin/Debug/net10.0` là sai?
8. `dotnet run` và `dotnet watch run` phù hợp với tình huống nào?
9. Vì sao `Assert.True(true)` chỉ kiểm tra hạ tầng test?
10. Working tree, staging area, commit, push và tag khác nhau thế nào?
11. Vì sao phải đọc `git diff --cached` trước khi commit?
12. Tại sao không được xem nội dung do AI tạo ra là đúng nếu chưa build, test và review diff?

## 17. Bảng kiểm hoàn thành chương

### Môi trường

- [ ] .NET 10 SDK hoạt động.
- [ ] VS Code và C# Dev Kit hoạt động.
- [ ] Development certificate đã được kiểm tra.

### Ứng dụng

- [ ] Solution chứa `CompanyInfo` và `CompanyInfo.Tests`.
- [ ] Trang chủ đã được sửa theo yêu cầu.
- [ ] Trang `/Home/About` hoạt động.
- [ ] Navigation có liên kết Giới thiệu.
- [ ] Sinh viên giải thích được luồng request–response.

### Kiểm chứng

- [ ] `dotnet build CompanyInfo.slnx` thành công.
- [ ] `dotnet test CompanyInfo.slnx` có tất cả test Passed.
- [ ] Trang chủ và trang Giới thiệu đã được kiểm tra trên trình duyệt.

### Git/GitHub

- [ ] `.gitignore` loại trừ `bin/`, `obj/`, log và secret.
- [ ] Đã đọc `git status`, `git diff` và `git diff --cached`.
- [ ] Commit có thông điệp rõ ràng.
- [ ] Nhánh `main` đã được push.
- [ ] Tag `chapter-01-completed` đã được tạo và push.
- [ ] Repository không chứa mật khẩu, token hoặc API key.

## 18. Chuỗi lệnh tham khảo nhanh

Các lệnh dưới đây dùng khi thực hiện lại từ đầu. Không chạy máy móc; sau mỗi nhóm lệnh cần đọc kết quả trước khi tiếp tục.

```bash
# Tạo workspace và solution
mkdir AspNetPractice
cd AspNetPractice
dotnet new sln -n CompanyInfo

# Tạo ứng dụng MVC
dotnet new mvc -n CompanyInfo --framework net10.0
dotnet sln CompanyInfo.slnx add CompanyInfo/CompanyInfo.csproj

# Tạo project test
dotnet new xunit -n CompanyInfo.Tests --framework net10.0
dotnet add CompanyInfo.Tests/CompanyInfo.Tests.csproj reference CompanyInfo/CompanyInfo.csproj
dotnet sln CompanyInfo.slnx add CompanyInfo.Tests/CompanyInfo.Tests.csproj

# Kiểm chứng
dotnet restore CompanyInfo.slnx
dotnet build CompanyInfo.slnx --no-restore
dotnet test CompanyInfo.slnx --no-build

# Chạy ứng dụng
dotnet watch --project CompanyInfo/CompanyInfo.csproj run

# Git (sau khi đã có .gitignore và kiểm chứng thành công)
git init
git branch -M main
git add .gitignore CompanyInfo CompanyInfo.Tests CompanyInfo.slnx README.md
git status
git diff --cached
git commit -m "chore(chapter-01): initialize ASP.NET Core MVC project"
git remote add origin <URL_REPOSITORY>
git push -u origin main
git tag chapter-01-completed
git push origin chapter-01-completed
```

---

**Nguyên tắc cuối cùng:** một bài thực hành chỉ được xem là hoàn thành khi sinh viên hiểu thay đổi, ứng dụng build được, test chạy thành công, giao diện đúng yêu cầu và phiên bản đã được lưu bằng Git mà không chứa dữ liệu nhạy cảm.
