# 商品管理資訊系統

以 ASP.NET Core MVC 開發的零售業後台管理系統，涵蓋銷售、庫存、會員與報表等核心功能。

## 功能說明

### 儀表板 Dashboard
- 即時顯示今日營收、本月營收、訂單數量
- 低庫存商品警示
- 本月銷售前五名商品
- 最新 10 筆訂單紀錄
- 近 7 日每日銷售趨勢圖

### 銷售 POS
- 商品搜尋與分類篩選
- 購物車管理（前端 JSON 傳遞）
- 會員查詢：自動帶入等級折扣與點數餘額
- 結帳時支援會員折扣、點數折抵
- 結帳完成自動建立訂單、扣減庫存、更新會員點數與累計消費

### 會員管理
- 會員基本資料 CRUD
- 多級會員制度（依累積點數自動升等）
- 每筆消費依等級累積點數（乘數機制）
- 點數使用與獲得紀錄追蹤

### 商品與分類管理
- 商品 CRUD（含低庫存門檻設定）
- 分類管理

### 庫存管理
- 庫存即時查詢
- 補貨作業（自動建立入庫紀錄）
- 庫存異動歷史紀錄（進出貨追蹤）

### 訂單管理
- 訂單清單與詳細內容檢視
- 顯示折扣金額、最終金額、付款方式

### 報表
- 銷售統計報表

## 技術架構

| 層次 | 技術 |
|------|------|
| 後端框架 | ASP.NET Core MVC (.NET 8) |
| ORM | Entity Framework Core + Code First Migrations |
| 資料庫 | SQL Server (LocalDB) |
| 前端 | Bootstrap 5、jQuery |
| 資料交換 | System.Text.Json（購物車 JSON 傳遞） |

## 專案結構

```
├── Controllers/        # 各功能控制器
│   ├── DashboardController.cs
│   ├── SalesController.cs
│   ├── InventoryController.cs
│   ├── OrdersController.cs
│   ├── CustomersController.cs
│   ├── ProductsController.cs
│   ├── CategoriesController.cs
│   └── ReportsController.cs
├── Models/             # 資料模型
│   ├── Product.cs
│   ├── Customer.cs
│   ├── MembershipLevel.cs
│   ├── Order.cs / OrderDetail.cs
│   ├── StockRecord.cs
│   ├── PointRecord.cs
│   └── ViewModels/
├── Views/              # Razor 頁面
├── Data/               # DbContext
└── Migrations/         # EF Core 資料庫遷移
```

## 執行方式

1. 確認已安裝 .NET 8 SDK 與 SQL Server LocalDB
2. Clone 此專案
3. 執行資料庫遷移：
   ```bash
   dotnet ef database update
   ```
4. 啟動專案：
   ```bash
   dotnet run
   ```

## 開發者

劉于嫙 — 國立XXX大學 資訊管理學系
