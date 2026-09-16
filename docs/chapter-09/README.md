# Chương 9: Tích hợp trí tuệ nhân tạo

## Phiên bản thực hành

- Điểm bắt đầu: `chapter-08-completed`
- Phiên bản tham khảo: `chapter-09-completed`

```bash
git switch -c practice-chapter-09 chapter-08-completed
```

Chương 9 bổ sung dịch vụ Gemini qua `HttpClient`, Options, timeout/fallback và luồng
tạo bản nháp mô tả để người dùng duyệt trước khi lưu. Controller chỉ phụ thuộc
`IAiTextService`, nên nghiệp vụ không bị khóa vào nhà cung cấp. Test dùng HTTP fixture,
không gọi API thật và không commit API key.

Demo mặc định dùng `gemini-3.5-flash`. Đặt khóa ngoài repository:

```bash
dotnet user-secrets set "Gemini:ApiKey" "<API_KEY>" --project AppDemo
```

Ứng dụng cũng đọc biến môi trường `GEMINI_API_KEY`. Trước khi thực hành, kiểm tra lại
model, quota và điều kiện free tier trên tài liệu Gemini Developer API vì các thông tin
này có thể thay đổi. Không gửi dữ liệu thật hoặc dữ liệu nhạy cảm vào tài khoản demo.
Xem [Gemini API reference](https://ai.google.dev/api) và
[Gemini Developer API pricing](https://ai.google.dev/gemini-api/docs/pricing).

Chỉ tạo `chapter-09-completed` sau khi kiểm tra provider/fallback, xác nhận lỗi
AI không làm hỏng CRUD, toàn bộ solution build/test thành công và working tree sạch.
