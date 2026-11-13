---
name: aetherjump-revamp-hero
description: Revamp hero yang sudah playable di AetherJump (Unity LegionKnight) mengikuti pola Kael dan Liura. Gunakan untuk audit kit dan rarity aset hero, redesign ikon skill dan status, penggantian referensi lama di deck, gacha atau prefab, serta integrasi platform battle GHP.
---

# Revamp hero AetherJump

Mulai dari hero yang sudah terpasang dan tahap yang diminta. Untuk “detail ability”, berikan rincian kit dari config. Untuk “buat desain”, serahkan art dan pratinjau. Untuk “pasang”, “replace”, atau revamp visual lengkap, teruskan sampai tampilan yang memakai aset tersebut sudah diperiksa di Unity. Pertahankan identitas dan perilaku kit sebagai dasar brief visual.

## Rarity saat revamp

Baca `HeroUnitConfig.CollectibleField.RarityConfig` sebagai tier hero. Untuk mockup, gunakan tier ini pada label yang ditampilkan. Saat memasang revamp atau menyinkronkan rarity, telusuri skill, ability, platform, kartu deck, dan reward gacha khas hero. Terapkan [aturan rarity aset hero](../aetherjump-create-hero/references/rarity-hero.md) dan perbaiki referensi yang berbeda atau kosong pada aset khas dalam lingkup revamp.

## Baca kit dan tentukan gambar

Buka `Assets/00_Rush/01_Unit/00_Hero/Config/Hero_<Nama>.asset` di LegionKnight. Ikuti `m_Skills` sampai ke ability, proyektil, dan status yang menentukan efek. Ikuti `m_HeroDeckConfig` ke kartu dan `m_UniquePlatforms` ke skill pendaratan normal serta Perfect kiri/kanan. Nama file atau deskripsi lama bisa berbeda dari perilaku yang terpasang.

Untuk rincian kit, urutkan weapon stage, pasif, ultimate, dan platform. Ambil nilai level 1, scaling, biaya charge, durasi atau batas stack dari config yang menghitungnya jika relevan.

Untuk setiap aksi catat pemicu, target, hasil yang terlihat, pemilik efek, dan slot gambar lamanya. Empat weapon stage dapat berbagi satu ikon jika aksinya sama. Beri status ikon tersendiri bila pemain membacanya sebagai tanda, debuff, atau indikator stack. Jumlah ikon mengikuti kit hero.

## Buat aset sesuai slot

- Ambil palet dan bentuk khas dari portrait, chibi, senjata, atau kostum hero. Buat satu motif dominan per aksi. Gunakan outline tegas, cel shading sederhana, dan siluet yang tetap berbeda pada ukuran HUD. Bandingkan seluruh set di latar terang dan gelap pada ukuran tampil sebenarnya.
- Simpan master di workspace pendamping `AetherJump AI/HeroAssets/<Hero>/SkillIcons/V<N>/`. Gunakan nama ekspor `Icon_<Hero>_<Kategori>_<Ability>_v<N>.png`. PhotoCraft dapat dipakai untuk membuang padding berlebih, menyamakan skala optis, dan mengekspor PNG RGBA. Acuan Kael dan Liura memakai 256 × 256 untuk ikon kit, 128 × 128 untuk ikon platform UI, dan 100 PPU. Pasang ikon hero di `Assets/00_Rush/01_Unit/00_Hero/<Hero>/SkillIcons/`. Cocokkan pivot, mode Sprite, dan ukuran impor dengan slot tujuan.
- Untuk pijakan battle, gunakan [skill redesign platform](../aetherjump-redesign-platform/SKILL.md). Hubungkan motif dengan efek pendaratan dan gunakan platform Anomaly Incursion milik Allain yang sedang terpasang sebagai acuan dua rel. Simpan sumber, PNG GHP, dan pratinjau dua baris di `HeroAssets/<Hero>/Platform/`.

## Ganti referensi sesuai pemakaiannya

Sebelum mengganti gambar, telusuri GUID lama, nama sprite, dan alias hero. [Peta pemakai dan contoh Kael–Liura](references/pemakaian-sprite.md) memuat helper pencarian serta kolom yang dibaca deck, kedua tampilan gacha, indikator musuh, dan renderer platform. Untuk sprite atlas, cocokkan juga `fileID`.

Pasangkan gambar baru **per aksi dan per slot**. Satu placeholder dapat mewakili dua aksi berbeda. Jika sprite lama dipakai lintas hero, buat sprite milik hero dengan GUID sendiri dan arahkan referensi yang relevan kepadanya. Periksa override prefab dan scene serta data Legacy yang memang mewakili hero itu. Untuk penggantian langsung atas sprite eksklusif dengan identitas yang sama, pertahankan `.meta` supaya pemakai yang sudah benar ikut terbarui.

## Verifikasi hasil

1. Setelah impor, telusuri lagi referensi lama dan baru. Periksa sprite yang dirender lewat jalur yang terkena perubahan, misalnya empat stage, kartu di deck, pratinjau **dan** hasil gacha, atau gambar dasar dan isian indikator pada musuh.
2. Cek pijakan dengan prefab battle dan `TouchDownSpot` proyek. Ukur badan yang terlihat dan sambungan tumpukan campuran dengan Allain setelah tekstur diimpor. Untuk uji Play Mode yang diminta, ikuti prosedur pada skill platform. Scene pratinjau terisolasi dapat dipakai untuk memeriksa renderer dan UI sambil menjaga edit scene pengguna.
3. Untuk config yang dipasang atau direvisi, cocokkan rarity aset dengan hero asal. Periksa detail skill, kartu deck, serta identitas dan rarity hadiah yang ditampilkan gacha sesuai jalur yang terdampak.
4. Laporkan aset terpasang, lokasi pemakaian yang diganti, pemakaian bersama yang masih sah, dan bukti tampilan. Bedakan pemeriksaan file Legacy, pratinjau Editor, dan uji Play Mode sesuai yang benar-benar dijalankan.

Jika brief juga mengubah gameplay, uji pemicu dan hasil aktual di Play Mode. Gunakan [balancing AetherJump](../aetherjump-balancing/SKILL.md) ketika growth berubah dan [aturan kartu](../aetherjump-roguelike-cards/SKILL.md) ketika akses atau isi deck berubah. Untuk permintaan portrait, rig, dan animasi, gunakan tahapan visual pada [skill pembuatan hero](../aetherjump-create-hero/SKILL.md).
