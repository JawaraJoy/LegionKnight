# AetherJump / LegionKnight

## Aturan update Android

- Setiap update wajib menghasilkan **file AAB lebih kecil dari 100.000.000 byte (100 MB desimal)**.
- Tambahan aset/konten game harus dikemas melalui **Unity Cloud Content Delivery (CCD)**; paket lokal dipertahankan untuk bootstrap dan kebutuhan awal yang minimum.
- **Kualitas gambar wajib dipertahankan.** Jangan menurunkan resolusi atau kualitas impor/kompresi tekstur untuk mengejar batas ukuran.
- Untuk integrasi aset baru, perubahan kemasan konten, dan build/rilis Android, baca dan terapkan skill [aetherjump-android-delivery](<C:/Users/Pongo/Documents/ChatGPT/AetherJump AI/.agents/skills/aetherjump-android-delivery/SKILL.md>).

Periksa ukuran file AAB akhir, bukan angka unduhan Google Play, ukuran instalasi, atau `BuildReport.totalSize`. Pastikan scene/katalog CCD memuat perubahan scene sumber terbaru dan versi konten untuk build lama tetap tersedia.
