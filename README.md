# AppDemo – Thực hành ASP.NET Core MVC

Repository chứa project thực hành xuyên suốt 9 chương của giáo trình *Công nghệ phát triển ứng dụng*. Mã nguồn được phát triển liên tục trong một solution; tài liệu từng chương mô tả điểm bắt đầu, kết quả cần đạt và cách kiểm chứng.

## Yêu cầu môi trường

- .NET 10 SDK
- Visual Studio Code
- C# Dev Kit
- SQL Server từ Chương 6

## Bắt đầu nhanh

```bash
dotnet restore AppDemo.slnx
dotnet build AppDemo.slnx --no-restore
dotnet test AppDemo.slnx --no-build
dotnet run --project AppDemo/AppDemo.csproj
```

## Hướng dẫn thực hành

- [Chương 1: Tổng quan ASP.NET Core](docs/chapter-01/README.md)
- [Chương 2: Cấu trúc ứng dụng và MVC](docs/chapter-02/README.md)
- [Chương 3: Tương tác giữa Controller, View và Model](docs/chapter-03/README.md)
- [Chương 4: Kỹ thuật gỡ lỗi](docs/chapter-04/README.md)
- [Chương 5: Biểu mẫu và Model Binding](docs/chapter-05/README.md)
- [Chương 6: Entity Framework Core](docs/chapter-06/README.md)
- [Chương 7: CRUD và truy vấn](docs/chapter-07/README.md)
- [Chương 8: Xác thực và phân quyền](docs/chapter-08/README.md)
- [Chương 9: Tích hợp trí tuệ nhân tạo](docs/chapter-09/README.md)

## Chọn đúng phiên bản để thực hành

Không thực hành trực tiếp trên `main`, vì `main` luôn chứa phiên bản mới nhất và có thể đã bao gồm lời giải của các chương sau. Hãy tạo branch cá nhân từ tag đầu vào của chương:

```bash
git clone <URL_REPOSITORY>
cd AppDemo
git switch -c practice-chapter-02 chapter-01-completed
```

| Chương | Tag bắt đầu | Tag tham khảo hoàn chỉnh |
|---:|---|---|
| 1 | `course-start` | `chapter-01-completed` |
| 2 | `chapter-01-completed` | `chapter-02-completed` |
| 3 | `chapter-02-completed` | `chapter-03-completed` |
| 4 | `chapter-03-completed` | `chapter-04-completed` |
| 5 | `chapter-04-completed` | `chapter-05-completed` |
| 6 | `chapter-05-completed` | `chapter-06-completed` |
| 7 | `chapter-06-completed` | `chapter-07-completed` |
| 8 | `chapter-07-completed` | `chapter-08-completed` |
| 9 | `chapter-08-completed` | `chapter-09-completed` |

Để chỉ xem một phiên bản hoàn chỉnh mà không sửa mã:

```bash
git switch --detach chapter-02-completed
```

Detached HEAD chỉ phù hợp để xem hoặc chạy thử. Muốn tiếp tục thực hành, luôn tạo branch mới từ tag phù hợp.

## Quy ước phát hành

- `main` chứa trạng thái mới nhất đã được review.
- `course-start` là trạng thái trước khi thực hành Chương 1.
- `chapter-XX-completed` là ảnh chụp bất biến sau khi hoàn thành chương tương ứng.
- Không cần tag `chapter-XX-start`, vì nó trùng commit với tag hoàn thành của chương trước.
- Chỉ tạo tag sau khi thay đổi đã merge vào `main`, build thành công, toàn bộ test Passed và working tree sạch.

Quy trình kiểm chứng trước khi tạo tag:

```bash
git switch main
git pull
dotnet restore AppDemo.slnx
dotnet build AppDemo.slnx --no-restore
dotnet test AppDemo.slnx --no-build
git status
git tag chapter-XX-completed
git push origin chapter-XX-completed
```

Không commit secret, database cục bộ, log, `bin/` hoặc `obj/`.
