---
name: aetherjump-roguelike-cards
description: Terapkan aturan kartu Roguelike AetherJump di proyek Unity LegionKnight saat membuat atau mengaudit kartu, menyusun deck hero, atau memperbaiki pool dan equip. Kartu lintas hero hanya boleh mempelajari ultimate dan platform; basic attack serta passive hanya dari deck default hero aktif.
---

# Kartu Roguelike AetherJump

Pedoman ini berlaku untuk proyek Unity LegionKnight. Aturan ditentukan oleh konfigurasi skill dan deck, bukan nama kartu atau hero.

## Aturan pool

- **Deck default hero aktif:** izinkan seluruh kartu yang dirujuk `HeroUnitConfig.HeroDeckConfig`, termasuk basic attack dan passive milik hero tersebut.
- **Kartu tambahan/lintas hero:** skill yang dipelajari hanya boleh berasal dari ultimate atau platform. Basic attack dan passive hero lain tidak boleh masuk pool, meskipun pemain memiliki kartu atau skill itu sudah ada pada unit.
- Periksa **semua** entri `CardPurpose.SkillUp`. Jika satu entri mempelajari basic/passive atau kategori yang belum diizinkan, tolak seluruh kartu di luar pengecualian deck default. Penambahan platform atau ultimate pada kartu yang sama tidak membatalkan pembatasan ini.
- Pertahankan kartu reward umum: stat, currency, Aether Essence, modifier kategori, dan penambahan platform. `CardPurpose.Activation` mengaktifkan efek langsung, sedangkan `SkillUp` mempelajari/menaikkan skill. Jangan mengganti purpose atau kategori basic/passive untuk meloloskan kartu lintas hero.
- Kartu basic/passive tetap boleh menjadi asset dan bagian deck default pemiliknya. Jangan menghapusnya dari semua deck atau menghapus kepemilikan pemain untuk memperbaiki pool.

Contoh Allain:

| Kartu | Sumber | Hero lain | Default Allain |
| --- | --- | --- | --- |
| Parallax Bolt | Basic attack | Tolak | Izinkan |
| Aegis Conversion | Passive | Tolak | Izinkan |
| Anomaly Seeker | Ultimate | Izinkan | Izinkan |
| Anomaly Incursion | Platform | Izinkan | Izinkan |

Allain adalah contoh penerapan, bukan pengecualian khusus. Terapkan aturan yang sama pada hero lama dan baru.

## Konfigurasi dan jalur runtime

Path berikut relatif terhadap root proyek Unity; telusuri referensi terbaru bila struktur berubah.

- `Assets/00_Rush/01_Unit/02_Skill/Scripts/SkillCategoryConfig.cs`: `AllowsCrossHeroLearning` berasal dari field serialized `m_AllowsCrossHeroLearning`, dengan default false. Hanya konfigurasi kategori **Ultimate**, **Platform**, dan **Platform Attack** yang diaktifkan. Kategori baru tidak otomatis diizinkan; skill tanpa referensi kategori juga ditolak untuk pembelajaran lintas hero.
- `Assets/00_Rush/04_RogueLike/Scripts/CardConfig.cs`: `AllowsCrossHeroLearning` memeriksa seluruh entri pembelajaran; `IsAvailableForHero(hero)` menerapkan pengecualian keanggotaan deck default. Gunakan pemeriksaan bersama ini agar pool, equip, dan UI konsisten.
- `RogueLikeHandler.cs` di folder script Roguelike: periksa kartu saat ditambahkan ke custom pool **dan saat draw**. Pemeriksaan draw harus tetap menolak referensi lama, daftar custom yang dimodifikasi langsung, dan kartu yang tidak lagi sesuai setelah berganti hero. Pertahankan pemilihan tanpa duplikat.
- `Assets/00_Rush/Legacy/Scripts/Common/CardDeck.cs`: periksa equip, pemulihan flag equip dari save, persiapan run, dan pemakaian kartu sebelum stok dipotong. Bersihkan equip yang tidak sesuai tanpa mengurangi jumlah kepemilikan. Kartu valid yang memakai salinan terakhir tetap masuk pool run yang sudah dibayar; jangan muncul kembali pada run berikutnya tanpa stok.
- `CardDetailView.cs` di folder script Roguelike: tombol Add mengikuti kelayakan hero. Pemain tetap dapat Remove kartu lama dan Sell kartu yang dimiliki.

Untuk hero baru, susun kartu basic/passive di deck defaultnya. Daftarkan kartu pembelajaran ultimate/platform untuk penggunaan lintas hero sesuai jalur inventory dan reward yang diminta; jangan memasukkan basic/passive sebagai pilihan pembelajaran lintas hero.

## Deskripsi dan growth

Deskripsi kartu dan skill harus mengikuti konfigurasi efek serta level runtime yang ditampilkan. Gunakan jalur `GameplayDescription` dan properti deskripsi konfigurasi; teks manual `m_Description`, `m_SimpleDescription`, atau `m_UpgradeDescription` bukan sumber angka gameplay. Uji preview unlock, upgrade, dan MAX ketika mengubah jalur deskripsi.

Pertahankan **dua versi deskripsi ringkas**. `NormalCard` / `SimpleDescription` menjelaskan efek awal saat ability baru dipelajari. `UpgradeCard` / `UpgradeDescription` menjelaskan **tambahan dari level saat ini ke level berikutnya**, misalnya damage `+15% ATK`, bukan hanya total damage level berikutnya. Hitung selisih memakai evaluator konfigurasi gameplay, termasuk pembulatan, stat summon, dan efek turunan. `BattleCard` memilih per skill berdasarkan kepemilikan runtime pada SkillController, bukan kepemilikan inventory atau riwayat kartu terkumpul. Jika satu kartu memuat skill baru sekaligus skill yang sudah dimiliki, bedakan unlock dan upgrade. Reward Activation tetap menjelaskan bonus yang benar-benar ditambahkan; platform berulang menjelaskan peningkatan frekuensi kemunculan tanpa mengarang kenaikan efek tetap. MAX tidak menjanjikan bonus skill lanjutan.

Kedua versi battle harus singkat dan terbaca pada ukuran font yang tetap. Dahulukan efek utama yang utuh saat ruang terbatas; jangan mengecilkan huruf untuk memuat semua rincian. Pertahankan `Description` lengkap untuk panel detail.

Jika mengubah nilai growth, baca [aetherjump-balancing](../aetherjump-balancing/SKILL.md). Jika membuat atau merombak hero, gunakan [aetherjump-create-hero](../aetherjump-create-hero/SKILL.md) untuk alur produksinya.

## Verifikasi perubahan

Sesuaikan cakupan uji dengan perubahan dan data proyek saat ini:

- Pastikan seluruh kartu default setiap hero tetap dapat ditarik, termasuk basic/passive pemiliknya.
- Uji penolakan basic/passive pada hero lain, penerimaan ultimate/platform, dan keberlangsungan reward umum. Untuk audit pool, periksa seluruh CardConfig terhadap seluruh hero; jangan berasumsi pelanggaran selalu hanya Allain.
- Uji kartu dengan beberapa entri SkillUp, kategori yang hilang/belum diizinkan, pergantian hero, dan daftar custom lama yang langsung berisi kartu terlarang.
- Saat menyentuh inventory, uji equip dan save lama: kartu yang ditolak tidak mengurangi stok; salinan terakhir kartu valid tetap berlaku untuk run saat ini.
- Kompilasi source proyek. Untuk verifikasi Editor dan Play Mode, utamakan Unity MCP bila tersedia. Bedakan hasil regresi source dengan adapter dari pengujian native Unity; laporkan jalur yang belum diuji.

Angka jumlah kartu, hero, dan ukuran deck mengikuti konfigurasi terbaru, bukan jumlah tetap dari audit sebelumnya.
