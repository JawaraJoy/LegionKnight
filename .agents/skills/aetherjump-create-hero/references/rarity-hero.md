# Rarity aset hero AetherJump

Untuk hero baru, pasang rarity dari blueprint pada `HeroUnitConfig`. Untuk revamp, ambil `HeroUnitConfig.CollectibleField.RarityConfig` milik hero yang sedang direvisi sebagai acuan. Aset khas hero Epic memakai `Epic.asset` dan aset khas hero Legendary memakai `Legendary.asset` dari `Assets/00_Rush/Configuration/Rarity/`. Pasang referensi `RarityConfig` yang sama saat membuat atau menduplikasi aset khas. Jika pengguna mengubah tier hero, perbarui config hero dan seluruh aset khas yang sudah terhubung, termasuk yang digunakan kembali.

| Aset | Cara memasang dan memeriksa rarity |
|---|---|
| Kit dan efek turunan | Ikuti `m_Skills` ke seluruh weapon stage, pasif, ultimate, skill platform, dan skill susulan. Pada `SkillConfig` isi `m_CollectibleField.m_RarityConfig`. Telusuri `m_AbilitySets` sampai ke ability, status, dan proyektil khas, termasuk `Shooter.m_AmmoConfig` pada prefab penembak. Ketiganya memakai `CombatEffectConfig.m_RarityConfig` langsung pada config efek. |
| Platform dan summon | Ikuti `m_UniquePlatforms` ke `PlatformConfig`, lalu periksa skill pendaratan normal dan Perfect di kiri serta kanan. Platform khas dan unit summon khusus memakai `m_CollectibleField.m_RarityConfig`. Periksa juga skill summon dan efek saat mati jika ada. |
| Kartu deck | Ikuti `m_HeroDeckConfig` ke `m_CardConfigs`. Kenali kartu khas dari skill yang dipelajari atau platform yang ditambahkan. Samakan `CardConfig.CollectibleField.RarityConfig` dengan milik hero asal, termasuk ketika kartu bisa dipakai oleh hero lain. |
| Reward gacha | Ikuti `GachaCollectableConfig.m_Collect` ke hero atau kartu yang benar-benar diberikan. Cocokkan **identitas dan rarity** target dengan hadiah yang ditampilkan. Untuk reward Legacy yang memiliki rarity sendiri, cocokkan juga `m_CollectibleField.m_RarityConfig` dan target `m_GachaItemConfig`. Setiap hadiah lain di banner mengikuti rarity itemnya sendiri. |
| Progres dan profil | Jika tersedia, cocokkan shard duplikat dan breakthrough dengan tier hero. Badge, ikon, serta frame profil eksklusif hero, termasuk varian statis dan animasinya, memakai tier yang sama. |

Periksa pemakai aset sebelum menyamakan rarity. Kartu stat umum mengikuti tier masing-masing. Serangan dan heal dasar platform dipakai juga oleh platform umum. Untuk efek bersama yang mewarisi rarity, biarkan field efek kosong agar runtime membaca rarity skill sumber melalui `AbilityContext`. Peluang dan jumlah hadiah gacha, pity, serta growth skill mengikuti konfigurasi balancing masing-masing.

Setelah impor, bandingkan **referensi objek Unity atau GUID** dengan rarity hero untuk setiap aset khas dalam lingkup pekerjaan. Periksa label atau bingkai di detail skill dan deck. Untuk reward gacha, periksa target serta tampilan di preview, daftar peluang, dan hasil draw. Misalnya, reward kartu Ritual Nexus harus menunjuk ke kartu Ritual Nexus milik Rithma dan menampilkan Legendary.
