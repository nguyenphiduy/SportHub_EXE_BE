# BidaPlatform – Backend API

Hệ thống quản lý câu lạc bộ bida kết hợp thương mại điện tử. Xây dựng trên **ASP.NET Core 8** theo kiến trúc **Clean Architecture + CQRS**.

---

## Mục lục

- [Tính năng](#tính-năng)
- [Kiến trúc](#kiến-trúc)
- [Tech Stack](#tech-stack)
- [Yêu cầu hệ thống](#yêu-cầu-hệ-thống)
- [Cài đặt & Chạy](#cài-đặt--chạy)
- [Cấu hình](#cấu-hình)
- [IoT & Điều khiển bàn billiard](#iot--điều-khiển-bàn-billiard)
- [API Reference](#api-reference)
- [Real-time (SignalR)](#real-time-signalr)
- [Phân quyền](#phân-quyền)

---

## Tính năng

### Quản lý bàn bida
- Thêm / sửa / xoá mềm bàn bida
- Theo dõi trạng thái bàn: `Available`, `Playing`, `Maintenance`
- Bắt đầu / kết thúc phiên chơi, tính tiền theo giờ
- Lịch sử phiên chơi toàn bộ (kèm tên nhân viên, thời gian, tổng tiền)
- Thanh toán: tiền mặt (`Cash`) hoặc chuyển khoản (`BankTransfer`)

### Điều khiển IoT (ESP8266)
- Bật / tắt đèn bàn tự động khi bắt đầu / kết thúc phiên
- Ping thiết bị để kiểm tra trạng thái online/offline
- Hỗ trợ chế độ **Local** (cùng LAN quán) và **Cloud** (tắt IoT, chỉ quản lý session)

### Thương mại điện tử
- Quản lý sản phẩm, danh mục, hình ảnh sản phẩm
- Giỏ hàng, đơn hàng, thanh toán PayOS
- Đánh giá sản phẩm

### Người dùng & Xác thực
- Đăng ký / đăng nhập, JWT Access Token + Refresh Token
- Phân quyền `Admin` / `Staff` / `Customer`
- Mã hoá thông tin nhạy cảm (FullName, Email) theo AES-256

### Thông báo real-time
- SignalR Hub: cập nhật trạng thái bàn ngay lập tức cho tất cả client

---

## Kiến trúc

```
BidaPlatform_BE/
├── BidaPlatform.Domain          # Entities, Enums, Interfaces (không phụ thuộc gì)
├── BidaPlatform.Application     # CQRS UseCases, MediatR Handlers, DTOs, Behaviors
├── BidaPlatform.Infrastructure  # EF Core, Repos, IoT Service, Email, Seed, Jobs
└── BidaPlatform.Presentation    # ASP.NET Core Controllers, Hubs, Middlewares, Program.cs
```

Luồng request: `Controller → MediatR → Handler → Repository → DB`

---

## Tech Stack

| Layer | Công nghệ |
|---|---|
| Framework | ASP.NET Core 8 |
| ORM | Entity Framework Core 8 + PostgreSQL |
| CQRS | MediatR |
| Auth | JWT Bearer, BCrypt |
| Realtime | SignalR |
| IoT | HTTP đến ESP8266 (LAN nội bộ) |
| Jobs | Hangfire |
| Encryption | AES-256 (thông tin cá nhân) |
| Payment | PayOS |
| Email | SMTP Gmail |

---

## Yêu cầu hệ thống

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- PostgreSQL 14+
- (Tuỳ chọn) ESP8266 kết nối cùng LAN để điều khiển đèn bàn

---

## Cài đặt & Chạy

```bash
# 1. Clone về
git clone <repo-url>
cd BidaPlatform_BE

# 2. Tạo file secrets (không commit lên git)
cp src/BidaPlatform.Presentation/appsettings.json \
   src/BidaPlatform.Presentation/appsettings.Secrets.json

# 3. Điền giá trị thực vào appsettings.Secrets.json (xem phần Cấu hình bên dưới)

# 4. Áp dụng migration
dotnet ef database update \
  --project src/BidaPlatform.Infrastructure \
  --startup-project src/BidaPlatform.Presentation

# 5. Chạy
dotnet run --project src/BidaPlatform.Presentation
```

Swagger UI: `https://localhost:PORT/swagger`

---

## Cấu hình

Tất cả giá trị thực đặt trong `appsettings.Secrets.json` (file này đã được `.gitignore`):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=bida;Username=postgres;Password=..."
  },
  "Security": {
    "EncryptionKey": "<32-byte base64 key>"
  },
  "Jwt": {
    "SecretKey": "<random-256-bit-string>"
  },
  "Email": {
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  },
  "PayOS": {
    "ClientId": "...",
    "ApiKey": "...",
    "ChecksumKey": "..."
  }
}
```

---

## IoT & Điều khiển bàn billiard

### Cách hoạt động

Khi bắt đầu/kết thúc phiên, BE gọi HTTP trực tiếp tới ESP8266:
```
GET http://{deviceIpAddress}/on   → Bật đèn
GET http://{deviceIpAddress}/off  → Tắt đèn
GET http://{deviceIpAddress}/     → Ping kiểm tra online
```

Timeout: **5 giây** — nếu thiết bị không phản hồi, API vẫn tạo/kết thúc session bình thường (fail-silent).

### Chế độ Local (tại quán)

Giữ nguyên mặc định `appsettings.json`:
```json
"IoT": { "Enabled": true }
```

BE phải **cùng mạng LAN** với ESP8266. Đèn bật/tắt tự động.

### Chế độ Cloud (quản lý từ xa)

`appsettings.Production.json` đã được cấu hình sẵn:
```json
"IoT": { "Enabled": false }
```

Hoặc set environment variable khi deploy:
```bash
IoT__Enabled=false
```

Khi `Enabled = false`: lệnh điều khiển đèn bị bỏ qua ngay lập tức (không gọi HTTP), toàn bộ tính năng quản lý session/payment/history/orders vẫn hoạt động bình thường qua internet.

### Sơ đồ hoạt động

```
[Admin/Staff - Bất kỳ đâu]
        │  HTTPS
        ▼
[BE Server (Cloud/Local)]
        │
        ├─── Tạo session trong DB          ✅ luôn hoạt động
        ├─── Tính tiền, lưu payment        ✅ luôn hoạt động
        ├─── SignalR notify tất cả client  ✅ luôn hoạt động
        │
        └─── Gửi lệnh HTTP đến ESP8266    ⚠️  chỉ khi cùng LAN
                    │  http (LAN only)
                    ▼
              [ESP8266 tại quán]
                 Bật/Tắt đèn
```

---

## API Reference

### Xác thực
| Method | Endpoint | Mô tả |
|---|---|---|
| POST | `/api/auth/register` | Đăng ký |
| POST | `/api/auth/login` | Đăng nhập, nhận JWT |
| POST | `/api/auth/refresh` | Làm mới Access Token |
| POST | `/api/auth/logout` | Đăng xuất |

### Bàn bida
| Method | Endpoint | Role | Mô tả |
|---|---|---|---|
| GET | `/api/tables` | Admin, Staff | Danh sách bàn |
| POST | `/api/tables` | Admin | Thêm bàn |
| PUT | `/api/tables/{id}` | Admin | Sửa bàn |
| DELETE | `/api/tables/{id}` | Admin | Xoá mềm bàn |
| POST | `/api/tables/{id}/start` | Admin, Staff | Bắt đầu phiên + bật đèn |
| POST | `/api/tables/{id}/stop` | Admin, Staff | Kết thúc phiên + tắt đèn |
| GET | `/api/tables/sessions` | Admin, Staff | Lịch sử tất cả phiên |
| PATCH | `/api/tables/sessions/{id}/payment` | Admin, Staff | Cập nhật thanh toán |
| POST | `/api/tables/{id}/device/ping` | Admin | Ping thiết bị IoT |

### Sản phẩm & Đơn hàng
| Method | Endpoint | Mô tả |
|---|---|---|
| GET | `/api/products` | Danh sách sản phẩm |
| GET/POST/PUT/DELETE | `/api/products` | CRUD sản phẩm (Admin) |
| GET/POST/PUT/DELETE | `/api/categories` | CRUD danh mục (Admin) |
| GET/POST | `/api/orders` | Đặt hàng / xem đơn |
| GET | `/api/revenue` | Báo cáo doanh thu (Admin) |

---

## Real-time (SignalR)

Hub URL: `/hubs/tables`

| Event (client nhận) | Dữ liệu | Khi nào |
|---|---|---|
| `TableStatusChanged` | `{ tableId, tableName, status }` | Khi bàn bắt đầu / kết thúc phiên |

**Frontend kết nối:**
```typescript
const connection = new HubConnectionBuilder()
  .withUrl("/hubs/tables", { accessTokenFactory: () => token })
  .withAutomaticReconnect()
  .build();

connection.on("TableStatusChanged", ({ tableId, status }) => {
  // Cập nhật UI
});
```

---

## Phân quyền

| Tính năng | Admin | Staff | Customer |
|---|---|---|---|
| Thêm / Sửa / Xoá bàn | ✅ | ❌ | ❌ |
| Bắt đầu / Kết thúc phiên | ✅ | ✅ | ❌ |
| Xem lịch sử phiên | ✅ | ✅ | ❌ |
| Quản lý người dùng | ✅ | ❌ | ❌ |
| Báo cáo doanh thu | ✅ | ❌ | ❌ |
| Mua hàng | ✅ | ✅ | ✅ |
