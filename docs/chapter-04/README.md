# Chương 4: Kỹ thuật gỡ lỗi

## Phiên bản thực hành

- Điểm bắt đầu: `chapter-03-completed`
- Phiên bản tham khảo: `chapter-04-completed`

```bash
git switch -c practice-chapter-04 chapter-03-completed
```

Chương 4 bổ sung quy trình chẩn đoán lỗi, logging an toàn, breakpoint và kiểm
tra `ModelState` trên code MVC kế thừa. Chương này chưa đưa EF Core, Identity
hoặc AI vào codebase.

Chỉ tạo `chapter-04-completed` sau khi `dotnet build`, `dotnet test` thành công
và `git status --short` không còn thay đổi.
