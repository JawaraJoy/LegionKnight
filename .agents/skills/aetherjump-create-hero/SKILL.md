---
name: aetherjump-create-hero
description: Buat atau revamp hero AetherJump (Unity LegionKnight) dengan alur Allain, dari blueprint dan portrait sampai rig, kit, deck, character selection, gacha, dan uji playable. Gunakan juga untuk satu tahap produksi atau untuk membedakan tampilan, mekanik, dan effort hero Epic dan Legendary.
---

# Membuat hero AetherJump

Mulai dari brief terbaru dan aset yang sudah diterima pengguna. Untuk permintaan satu tahap, kerjakan hasil yang diminta, misalnya portrait chibi saja. Untuk permintaan sampai playable, lanjutkan hingga hero dapat dipilih dan dimainkan dari Home. Saat bekerja lewat Unity MCP, periksa nama dan path proyek pada instance yang aktif. Lokasi aset dan titik pendaftarannya ada di [peta proyek LegionKnight](references/legionknight.md).

## Skala produksi hero Epic dan Legendary

Mulai dari rarity yang dipilih pengguna. Saat menyusun blueprint atau merevisi art dan kit, catat hero pembanding, ciri yang harus terbaca di layar, bagian yang bisa memakai aset atau logic lama, dan bagian yang memerlukan pekerjaan khusus.

| Aspek | Epic | Legendary |
|---|---|---|
| Siluet dan kostum | Gunakan Caelis atau Liura sebagai acuan. Utamakan satu aksen yang mudah dikenali, potongan pakaian sederhana, dan ornamen pendamping yang tenang. | Buat siluet yang lebih menonjol lewat senjata, kostum, atau properti khas yang terlihat pada portrait dan chibi. Beri detail paling banyak pada bagian yang dipakai hero saat beraksi. |
| Kit | Pusatkan permainan pada satu mekanik yang mudah dijelaskan. Weapon, pasif, aktif, dan platform dapat memakai atau mengisi efek yang sama. Manfaatkan pola proyektil, buff, atau pemicu yang sudah ada. | Mekanik khas dapat melibatkan beberapa fase yang saling berhubungan. Tentukan apa yang dilakukan pemain dan bagaimana mereka mengenali perpindahan fase sebelum menambah logic khusus. |
| Rig, animasi, VFX dan SFX | Adaptasi gerakan hero dengan senjata sejenis. Satu gerakan serang bisa melayani beberapa skill. Tempatkan kilatan atau bunyi singkat pada saat efek khas terpicu. | Alokasikan gerak properti, pose, serta VFX dan SFX khusus pada momen yang perlu dibedakan pemain, misalnya saat senjata menancap, ditarik, lalu terlepas. |

Pada **Iona (konsep Epic)**, bilah kaca pucat pada busur kayu menjadi aksen utamanya. Glass Arrow memakai prisma Refraction saat tersedia. Clearshot yang mengalahkan target dan perfect landing di Prism Perch mengisi stok prisma yang sama.

Pada **Vela (konsep Legendary)**, harpun, kabel, dan gulungan di pinggang membentuk siluet sekaligus menjelaskan permainan. Harpun mengikuti target yang tertancap sampai Vela mendarat dan menariknya. **Allain** memberi acuan Legendary yang sudah ada di roster, dengan tanda Anomaly Sign pada musuh dan efek Anomaly Collapse saat tanda terpicu.

Referensi visual di workspace pendamping `AetherJump AI` ada di `HeroAssets/Iona/Portraits/Iona_Chibi_v1.png` dan `HeroAssets/Vela/Portraits/Vela_Chibi_v1.png`.

Bandingkan pada tinggi tampil yang sama, dengan pose dan latar yang sebanding. Kedua rarity memakai standar proyek yang sama untuk proporsi chibi, tangan dan kaki, deformasi, layering, serta keterbacaan serangan. Jika Epic terlihat semeriah Legendary, sederhanakan ornamen sekunder atau luas VFX sambil mempertahankan ukuran tubuh dan ciri utamanya. Tentukan damage dan charge dari peran kit. Gunakan `$aetherjump-balancing` untuk growth kartu.

## Blueprint yang bisa dipakai ulang

| Bagian | Yang perlu diisi |
|---|---|
| Identitas | Nama, class, gender, tinggi (cm), umur, rarity, tema dan senjata. |
| Arah visual | Referensi outfit, palet, siluet, ciri wajah, cara memegang senjata, hero pembanding untuk proporsi dan gerakan. |
| Stat hero | HP, ATK, DEF dan stat kritikal yang relevan pada level hero 1, progres per level hero, breakthrough dan shard sesuai rarity. |
| Weapon Stage 1–4 | Satu nama tampilan, ID unik per stage. Catat sasaran, aksi tiap hit, dan apa yang berubah saat stage naik. |
| Passive | Pemicu, penerima efek, nilai level 1, serta durasi, stack, batas dan aturan refresh bila ada. |
| Skill aktif | Biaya charge, cara memilih target, jumlah hit, damage atau efek per target, serta scaling dari stat hero. |
| Platform | Efek saat perfect landing, posisi atau area efek, interaksi dengan stage, dan apa yang dinaikkan kartu platform. |

Jika pengguna meminta usulan nama atau konfirmasi mekanik, sajikan opsi bersama dampaknya sebelum merakit kit terkait. Bedakan nilai yang telah dipilih dari usulan yang masih terbuka. Gunakan hero pembanding untuk gaya dan struktur. Rarity, senjata, serta mekanik hero baru mengikuti brief-nya sendiri.

## Dari portrait ke animasi

1. Buat **portrait anime** untuk detail wajah, kostum, dan senjata, lalu **chibi full-body** yang tetap terbaca pada ukuran gameplay. Pakai Liura sebagai patokan ukuran kepala terhadap torso, panjang pinggang ke kaki, ukuran tangan/kaki, serta tinggi total. Untuk Allain, wajah laki-laki mengacu ke Kael/Assassin. Jaga ciri visual proyek berupa telapak tangan flat dengan siluet menyatu tanpa detail jari terpisah, serta kaki dan sepatu sederhana.
2. Siapkan sprite transparan beserta bagian yang tersembunyi di pose netral agar sambungan tetap tertutup saat bergerak. PhotoCraft dapat dipakai untuk pekerjaan ini. Di Rigby2D, bandingkan hierarki tulang, pivot kaki, slot wajah, dan titik pegang senjata dengan rig acuan. Simpan sumber `.rigby` yang dapat diedit. Bagi bobot mesh di antara tulang yang bersebelahan pada pinggang, kain, dan anggota tubuh yang perlu melengkung. Pertahankan siluet chibi awal pada pose netral. Untuk Allain, batang staff dan panel jubah depan berada di depan badan, dengan genggaman menutup batang di titik pegang.
3. Ambil pose kunci, timing, easing, dan transisi dari hero pembanding. Siapkan pose hadap depan saat melompat dan ekspresi terpisah untuk idle dan hurt. Cocokkan nama animasi, kapitalisasi, loop, serta clip lanjutan dengan binding yang dibaca prefab. Ekspor skeleton, atlas, dan tekstur, lalu impor sebagai `SkeletonDataAsset`.
4. Bandingkan hero baru dan pembanding di Game View pada kamera, garis pijak, dan fase animasi yang sama. Lihat idle, puncak lompatan, serangan dan hurt. Cocokkan tinggi kepala, letak pinggang, panjang kaki, titik genggam, serta urutan gambar ketika kain dan senjata bergerak.

## Kit, kartu, dan perilisan

- Rakit `HeroUnitConfig` dengan avatar, clip, stat, rarity, empat skill weapon, passive, aktif, dan platform unik. Telusuri setiap `SkillConfig` ke `AbilityConfig` dan prefab yang menyampaikan efek. Gunakan perilaku Rush yang sudah ada bila cocok. Untuk summon, tentukan faction, spawner, HP, health bar, aksi, dan animasi sesuai desain. Minion tanpa serangan seperti Anomaly Entity memakai idle dan death.
- Buat icon hero, icon tiap skill, icon dan sprite platform, serta VFX/SFX sesuai aksi. Periksa ukuran icon di slot UI yang sebenarnya. Uji keterbacaan serangan, tanda di musuh, indikator stack, dan area efek di depan looping background. Jika ada heal, cocokkan teks pemulihan dengan HP yang benar-benar dipulihkan.
- Rakit deck hero dan kartu khususnya. Untuk pola Allain, satu kartu weapon menaikkan keempat stage milik hero tersebut. Bedakan perubahan mekanik antar-stage dari kenaikan level lewat kartu. Ikuti [pedoman kartu Roguelike](../aetherjump-roguelike-cards/SKILL.md): kartu lintas hero hanya boleh mempelajari ultimate dan platform; basic attack serta passive hanya tersedia dari deck default hero aktif. Gunakan `$aetherjump-balancing` untuk besaran yang di-upgrade. Setiap level skill menambah 15% dari nilai efektif level 1. Teks kartu mengikuti bahasa Inggris seperti deck lain; deskripsi tampilan dan angka preview harus dibentuk dari konfigurasi melalui `GameplayDescription`, bukan mengandalkan teks manual serialized.
- Daftarkan hero di character selection dan atur cara membukanya. Untuk perilisan seperti Allain, siapkan splash banner, item hadiah hero, reward pool, dan pity sesuai rarity serta desain banner. Sambungkan banner ke daftar yang dibaca Home.

## Kriteria playable

Di Play Mode, pakai profil uji untuk memilih, memasang, dan memulai run dengan hero dari Home. Periksa empat weapon stage, pemicu passive, biaya dan aktivasi skill aktif, serta perfect landing kiri dan kanan. Ambil kartu lewat jalur gameplay dan cek efek pada level skill 1, 2, dan 3 dengan stat hero yang sama. Uji efek turunan, pergantian atau kedaluwarsa buff, dan reset run jika berlaku. Untuk hero gacha, cek tampilan banner dan pemberian hadiah pada profil uji. Laporkan stat dasar, ringkasan kit dan hasil uji, serta tandai jalur yang masih belum teruji.
