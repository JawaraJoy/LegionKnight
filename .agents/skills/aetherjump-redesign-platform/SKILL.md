---
name: aetherjump-redesign-platform
description: Rancang ulang sprite platform battle GHP untuk hero AetherJump di proyek Unity LegionKnight. Gunakan saat membuat desain dari ability, menyamakan ukuran dengan Allain, memasang sprite, atau memeriksa kerapatan tumpukan di Play Mode.
---

# Redesign platform hero AetherJump

## Desain dari ability

Telusuri `HeroUnitConfig.m_UniquePlatforms` ke `PlatformConfig`, lalu baca skill untuk pendaratan normal dan Perfect. Lihat ikon ability platform tersebut, portrait hero, dan sprite battle lama. Pilih satu bentuk yang menjelaskan efek utamanya dan satu motif yang dikenali dari hero. Contoh Liura memakai pita mana yang naik ke wajik ungu untuk menggambarkan pemulihan mana saat Perfect. Lekuk sampingnya mengikuti pita topi dan ujung tongkat Liura.

Di `Assets/00_Rush/Image/GHP 1024x1024/`, gunakan `_0005_GHP-Slow.png`, `_0004_GHP-Revive.png`, dan `512x512_0000_Caelis-GHP.png` sebagai referensi relief. Buat tampak depan dengan bidang bertumpuk, lekukan, sorotan, dan bayangan kontak yang terbaca pada skala battle. Bentuk dua rel horizontal sejajar dengan lebar yang seimbang. Tempatkan emblem di antara rel agar pijakan dan alas dapat bersentuhan. Pertahankan lebar rel bawah sampai ke ujung dan hindari siluet yang mengerucut seperti `1024x1024_Normal.png`.

## Ekspor dan perbandingan

Untuk format GHP standar, pakai sprite **Anomaly Incursion** yang sedang terhubung di Unity sebagai pembanding ukuran. Ekspor PNG RGBA **1024 × 1024**. Pada aset Allain yang diuji, badan dengan alfa ≥128 berukuran **840 × 269 px**, dari **x=92–931** dan **y=390–658** inklusif, dihitung dari kiri atas. Jarak antarbaris **267 px**. Pakai posisi kedua rel saat menempatkan gambar. Jaga proporsi emblem ketika menyesuaikan lebar sisi. Ukur piksel badan yang terlihat pada ambang alfa yang sama.

Bandingkan PNG pembanding dan hasil dengan mode `sprite` pada [skrip pemeriksa GHP](scripts/check_ghp.py). Jika acuan proyek berubah, ukur sprite Allain yang kini terhubung. Untuk permintaan desain, simpan gambar sumber dan PNG akhir di `HeroAssets/<Hero>/Platform/` pada workspace pendamping. Pratinjau dua baris dengan hero dan latar battle membantu menilai ukuran serta motif.

## Pemasangan dan uji

Untuk pemasangan, tautkan sprite ke `PlatformConfig.m_CollectibleField.m_SplashImage`, yang dibaca `Platform2D.IniPlatform()`. `m_Icon` dipakai untuk UI. Simpan sprite hero sebagai aset dengan GUID sendiri jika gambar lama juga dipakai konten lain. Cocokkan pengaturan import dengan sprite Allain, termasuk pivot tengah dan PPU sumber 100 untuk format di atas. Verifikasi config menunjuk ke aset yang baru setelah Unity mengimpornya.

Jika diminta menguji di Unity, ukur **badan yang dirender** dan **celah pada rel yang berimpit**. Gunakan pool, prefab, dan posisi pendaratan dari gameplay. Ikuti [uji tumpukan di Play Mode](references/play-mode.md) untuk susunan campuran, cara pengukuran, dan lokasi acuan proyek.
