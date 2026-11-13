# Uji tumpukan di Unity Play Mode

## Aset acuan

Path berikut relatif ke root Unity LegionKnight. Buka `Assets/00_Rush/01_Unit/00_Hero/Allain/Platforms/Platform_Allain_AnomalyIncursion.asset` dan ikuti `m_CollectibleField.m_SplashImage`. Saat uji Liura, referensi itu menunjuk ke `Assets/00_Rush/01_Unit/00_Hero/Allain/Art/Allain_AnomalyIncursion_Platform.png`. Contoh hasil pemasangan ada di `Assets/00_Rush/01_Unit/03_Platform/Configuration/Platform_ManaCharge.asset`.

Untuk membandingkan dua PNG pada ambang alfa yang sama, jalankan perintah berikut dari root proyek. Isi `$ref` dengan path PNG Allain dan `$sprite` dengan path hasil redesign. Pemeriksa memakai Python, Pillow, dan NumPy.

```powershell
python .agents/skills/aetherjump-redesign-platform/scripts/check_ghp.py sprite $ref $sprite
```

## Pengukuran runtime

1. Pilih instance Unity **LegionKnight**. Catat scene dan keadaan Editor. Baca balik referensi sprite, pivot, PPU efektif dan prefab dari kedua config. Build target dapat menurunkan resolusi dan PPU secara proporsional. Pada uji Liura, sumber 1024 px pada 100 PPU diimpor menjadi 256 px pada 25 PPU.
2. Masuk Play Mode. Gunakan `PlatformHandler` dan pool `Base Platform` dengan config game. Susunan **baru, baru, Allain, Allain, baru**, dari bawah ke atas, mencakup keempat pasangan. Untuk uji terkontrol, tentukan urutan config di antrean dan biarkan handler menempatkan instance berikutnya lewat `TouchDownSpot`. Pertahankan perpindahan horizontal yang terjadi saat bermain. Catat posisi Y dan pastikan `SpriteRenderer.sprite` tiap baris sesuai config.
3. Untuk ukuran, render kedua sprite pada skala kamera dan posisi relatif yang sama. Bandingkan tepi gambar pada alfa ≥128. `SpriteRenderer.bounds` mencakup padding transparan. Untuk sambungan, ambil satu tangkapan tumpukan utuh dan satu mask PNG transparan per baris dengan kamera dan transform tetap. Urutkan mask dari bawah ke atas. Ukur semua kolom pada rel datar yang saling menutupi. Kurangi hanya ujung bevel berdasarkan bentuk sprite.
4. Jalankan mode `stack` dengan jarak baris yang diharapkan dalam piksel kamera (`TouchDownSpot` relatif pada skala dunia × piksel per unit kamera). Pada tangkapan Liura, kamera memakai 125 px/unit, pitch 267 px, dan margin 36 px di tiap ujung irisan untuk melewati bevel. Sesuaikan angka tersebut bila skala kamera atau geometri rel berubah.

```powershell
python .agents/skills/aetherjump-redesign-platform/scripts/check_ghp.py stack --pitch-px 267 --bevel-px 36 row-0.png row-1.png row-2.png row-3.png row-4.png
```

Pada skala prefab 0,8, hasil acuan Liura dan Allain adalah **6,72 × 2,152 unit** untuk badan platform dan **2,136 unit** antarbaris. Keempat sambungan memiliki **0 px celah** pada rel yang bertemu. Tepi antialias bisa saling menutupi 1–2 px. Jika ada selisih, periksa posisi badan dan ketebalan rel pada PNG, pivot, PPU, skala renderer, lalu jarak ke titik pendaratan pada prefab.

Sertakan tangkapan Unity dan hasil pengukuran saat melaporkan uji. Simpan bukti di `HeroAssets/<Hero>/Platform/` pada workspace pendamping. Contoh terdahulu ada di `HeroAssets/Liura/Platform/Liura_Allain_Stack_PlayMode.png`. Bersihkan objek uji sementara dan kembalikan Editor ke keadaan awal sambil mempertahankan perubahan scene milik pengguna.
