# Chương 3: Tương tác giữa Controller, View và Model

## Phiên bản thực hành

- Điểm bắt đầu: `chapter-02-completed`
- Phiên bản tham khảo: `chapter-03-completed`

```bash
git switch -c practice-chapter-03 chapter-02-completed
```

Chương 3 tiếp tục trực tiếp trên module Company của Chương 2. Sinh viên củng cố
hợp đồng dữ liệu giữa Controller, strongly typed View và ViewModel; không tạo
project mới và chưa thêm biểu mẫu ghi, cơ sở dữ liệu, Identity hoặc AI.

## Cổng hoàn thành

```bash
dotnet restore AppDemo.slnx
dotnet build AppDemo.slnx --no-restore
dotnet test AppDemo.slnx --no-build
git status --short
```

Chỉ tạo `chapter-03-completed` khi toàn bộ lệnh trên thành công và working tree sạch.
