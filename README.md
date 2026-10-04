# RegTikTok - TikTok Automation Tool

Dự án công cụ tự động hóa TikTok (Đăng ký tài khoản, Đăng nhập Google, Bật 2FA, Xóa email, Tích hợp HMA & ExpressVPN, Hỗ trợ Change Device qua API).

## 📁 Cấu trúc thư mục

- `WindowsFormsApp2/`: Thư mục Solution chính
  - `WindowsFormsApp2.sln`: File Solution cho Visual Studio (hỗ trợ VS 2019/2022+)
  - `WindowsFormsApp2.slnx`: File Solution XML mới cho VS 2022
  - `packages/`: Chứa toàn bộ các NuGet package đã cài đặt
  - `WindowsFormsApp2/`: Mã nguồn dự án WinForms (.NET Framework 4.7.2)
    - `Form1.cs`: Giao diện và luồng điều khiển thiết bị chính
    - `Tiktok_helper.cs`: Các hàm bổ trợ tự động hóa TikTok
    - `GmailVipService.cs`: Dịch vụ đọc mail / OTP
    - `libs/`: Thư viện phụ thuộc di động (`KAutoHelper.dll`, `Neo.dll`, `Infamous.dll`, `Emgu.CV.*.dll`)
    - `bin/Debug/`: Đã chứa sẵn các tool cần thiết (`adb.exe`, `scrcpy.exe`, native DLLs, template ảnh nhận diện OpenCV trong `Data/`, OCR `tessdata/`)
- `accounts.txt`: Danh sách tài khoản mẫu

## 🚀 Hướng dẫn mở và chạy trên máy khác

1. Mở file `WindowsFormsApp2/WindowsFormsApp2.sln` bằng Visual Studio 2022 (hoặc 2019).
2. Target framework: **.NET Framework 4.7.2**.
3. Cấu hình build: **Debug / Any CPU** hoặc **x86**.
4. Toàn bộ thư viện và reference đã được cấu hình đường dẫn tương đối (`libs/` và `packages/`), có thể build trực tiếp thành công mà không cần cài đặt thêm.
