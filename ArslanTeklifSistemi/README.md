# Arslan Elektromarket - Kurumsal Teklif ve Stok Otomasyonu

Bu proje, Arslan Elektromarket için geliştirilmiş, WooCommerce API'si ile entegre, anlık kur takibi yapabilen ve tek tıkla A4 formatında teklif dökümanı üreten .NET 10 tabanlı bir full-stack otomasyon sistemidir.

![Teklif Formu Ekran Görüntüsü](arslan-teklif-sistem.png)

## 🛠️ Teknik Özellikler / Technical Features

* **.NET 10 Backend:** Yüksek performanslı ve ölçeklenebilir altyapı.
* **WooCommerce Integration:** Ürün verileri (stok kodu, fiyat, görsel) doğrudan WordPress/WooCommerce üzerinden API ile çekilir.
* **Smart Input Matching:** Stok kodu (SKU) girildiği anda, ürün bilgileri otomatik olarak form alanlarına (input) dolar.
* **TCMB Kur Entegrasyonu:** Türkiye Cumhuriyet Merkez Bankası XML servisinden anlık döviz kurları çekilir.
* **Finansal Matris:** USD/EUR/TL bazlı ürünleri tek formda birleştirip, KDV dahil/hariç otomatik hesaplama yapar.
* **Print-Ready A4 Design:** Web arayüzü, baskı (print) modunda otomatik olarak kurumsal bir A4 teklif formuna dönüşür.

---

## 🇬🇧 English Description

This project is a full-stack automation system designed for Arslan Elektromarket. It integrates with the WooCommerce REST API to fetch product details automatically based on SKU input.

### Key Highlights:
* **.NET 10 Core API:** Powers the backend with asynchronous data fetching.
* **Live Currency Engine:** Integrates with the TCMB XML service to provide real-time exchange rates.
* **Dynamic Form Auto-Fill:** When a user enters a product SKU, the system fetches and populates product details (Price, Name, Currency) automatically.
* **Automated Financial Calculation:** Multi-currency support with real-time VAT (KDV) and total price consolidation.

---

## 🚀 Kurulum / Setup

1. **API Keys:** `appsettings.json` dosyasını oluşturun ve `WooCommerce:ConsumerKey` ile `WooCommerce:SecretKey` alanlarını doldurun.
2. **Database/API:** Proje, WooCommerce API ile iletişim kurar. Bağlantı adresinizi yapılandırmanız yeterlidir.
3. **Run:** Visual Studio 2022+ üzerinde projeyi `.NET 10` ile çalıştırın.

---

*Bu proje, Arslan Elektromarket'in operasyonel hızını artırmak amacıyla geliştirilmiştir.*