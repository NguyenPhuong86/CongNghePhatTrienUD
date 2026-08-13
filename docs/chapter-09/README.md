# Chương 9: Tích hợp trí tuệ nhân tạo

## Phiên bản thực hành

- Điểm bắt đầu: `chapter-08-completed`
- Phiên bản tham khảo: `chapter-09-completed`

```bash
git switch -c practice-chapter-09 chapter-08-completed
```

Chương 9 bổ sung dịch vụ AI qua `HttpClient`, Options, timeout/fallback và luồng
tạo bản nháp mô tả để người dùng duyệt trước khi lưu. Test dùng HTTP fixture,
không gọi API thật và không commit API key.

Chỉ tạo `chapter-09-completed` sau khi kiểm tra provider/fallback, xác nhận lỗi
AI không làm hỏng CRUD, toàn bộ solution build/test thành công và working tree sạch.
