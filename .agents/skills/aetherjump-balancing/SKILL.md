---
name: aetherjump-balancing
description: Gunakan saat membuat, menyesuaikan, atau mengaudit growth skill dan kartu upgrade hero AetherJump di proyek Unity LegionKnight. Terapkan kenaikan linear 15% dari nilai level 1 untuk setiap level skill.
---

# AetherJump Balancing

Untuk setiap hero AetherJump, tentukan `B` dari kekuatan efek pada level skill 1. Satu upgrade menambah `0.15 * B`. Untuk level skill `L`, gunakan `nilai(L) = B * (1 + 0.15 * (L - 1))`. Terapkan rumus kepada besaran yang ditingkatkan kartu, misalnya koefisien ATK per hit, jumlah heal, atau persentase buff per stack.

| Contoh efek | Level 1 | Level 2 | Level 3 |
|---|---:|---:|---:|
| Lifesteal per stack | 4% | 4,6% | 5,2% |
| Damage per proyektil | 100% ATK | 115% ATK | 130% ATK |
| Damage ledakan summon | 50% ATK | 57,5% ATK | 65% ATK |

Gunakan nilai dasar milik masing-masing hero, stage, dan efek. Pada lifesteal 4%, satu upgrade menambah **0,6 poin persentase** per stack. Hitung besaran per stack pada level skill tersebut sebelum mengalikan jumlah stack dan menerapkan batas milik efek. Charge, cooldown, durasi, radius, jumlah proyektil, dan ambang stack mengikuti desain skill.

Di Unity, telusuri `HeroUnitConfig` ke `SkillConfig`, `AbilityConfig`, dan kartu `SkillUp` yang dipakai. `AbilityPowerField.AtLevel` / `PowerField.GetFinalPower` di `Assets/00_Rush/01_Unit/02_Skill/Scripts/PowerField.cs` memakai **`BaseAmount + ScaleByLevel * (L - 1)`**. Isi `BaseAmount = B` dan `ScaleByLevel = 0.15 * B`. Jika `B = 1.00`, isinya `1.00` dan `0.15` untuk base dan growth. Level 1 harus menghasilkan BaseAmount tanpa bonus upgrade.

Asset lama dimigrasikan pada 10 November 2026 dengan menggabungkan kontribusi growth pertama ke BaseAmount, tanpa mengubah growth berikutnya. Metadata tersembunyi `m_LevelOnePrecision` mempertahankan sisa presisi float hasil migrasi; ini bukan balancing tambahan dan jangan diisi manual untuk asset baru. Helper mempertahankan batas pembulatan lama untuk efek flat yang tidak menyimpan pecahan. Gunakan `m_KeepFractionalFlatPower` untuk mempertahankan pecahan sampai damage/heal/charge dikirim bila desain efek membutuhkannya. Jalur stat modifier lama memisahkan bonus per stack dan growth per level melalui `GetFinalStackPower`; cek hasil aktual per stack, bukan hanya angka power tanpa konteks.

Saat mengubah balancing, cocokkan level yang dibaca efek turunan dengan skill pemicunya. Uji upgrade melalui kartu pada level 1, 2, dan 3 dengan stat hero yang sama, lalu cocokkan hasil dan deskripsi kartu dengan tabel growth hero tersebut. Terapkan perubahan pada hero yang termasuk dalam permintaan.

## Kartu Weapon Stage

Khusus kartu kategori **Weapon Stage** (`weaponstage`), tulis baris efek upgrade persis `+15% Attack`. Satu upgrade menaikkan semua stage sekaligus. Pada setiap kenaikan level, tambahkan 15% dari damage level skill 1 masing-masing stage.

| Magic Missile Liura | Stage 1 | Stage 2 | Stage 3 | Stage 4 |
|---|---:|---:|---:|---:|
| Level skill 1 | 100% ATK | 200% ATK | 300% ATK | 400% ATK |
| Level skill 2 | 115% ATK | 230% ATK | 345% ATK | 460% ATK |
| Level skill 3 | 130% ATK | 260% ATK | 390% ATK | 520% ATK |
