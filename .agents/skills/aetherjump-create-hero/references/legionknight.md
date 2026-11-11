# Peta proyek LegionKnight

Path `Assets/...` dihitung dari root Unity. Ikuti referensi serialized pada konfigurasi dan prefab Home yang aktif jika aset berpindah.

## Avatar dan animasi

- Mulai dari `Assets/00_Rush/01_Unit/00_Hero/Config/Hero_Allain.asset`. Field `m_UnitPrefab` menunjuk ke `Assets/00_Rush/01_Unit/00_Hero/Prefab/Unit_Player.prefab` yang juga dipakai Liura dan Kael. Pilih tampilan hero lewat `m_SkeletonDataAsset` pada config masing-masing.
- Rig Liura yang terpakai adalah `Assets/00_Rush/01_Unit/00_Hero/Hero Avatar REvamped/Mage/Mage.json`. Rig wajah Kael yang terpakai adalah `Assets/00_Rush/01_Unit/00_Hero/Hero Avatar REvamped/Assassin/Assasin 1.json`. Ejaan nama file dan spasi di sini memang mengikuti aset yang dirujuk.
- Contoh ekspor yang diimpor ada di `Assets/00_Rush/01_Unit/00_Hero/Hero Avatar REvamped/Allain`. Set itu memuat `Allain.json`, `Allain.atlas.txt`, `Allain.png`, material, atlas asset, dan `Allain_SkeletonData.asset`. Ketiga referensi hero memakai JSON Spine 4.3.x dan skala impor 0.01. Atlas Allain memakai premultiplied alpha. Cocokkan versi, alpha, material, dan skala dengan runtime proyek saat pengerjaan. Sumber Rigby2D yang dapat diedit tersedia di workspace pendamping AetherJump AI pada `HeroAssets/Allain/ProportionsV12/Rig/Allain_v12.rigby`.
- Lihat `Assets/00_Rush/01_Unit/04_Avatar/Spine/Config/AnimationClip` dan field clip pada hero. Contoh nama animasi yang dipakai ialah `Idle`, `Attack`, `Jump`, `Hurt`, `Death`, dan `Emot`. Pada Allain, serangan normal dan perfect berbagi binding `Clip_Hero_Attack`. Periksa binding untuk menentukan animasi yang benar-benar akan diputar.

## Kit, deck, dan platform

- `Assets/00_Rush/01_Unit/00_Hero/Allain` berisi `Skills`, `Abilities`, `Projectiles`, `Status`, `Entities`, `Platforms`, `Prefabs`, `Feedback`, `Art`, `Cards`, dan `Gacha`. Beri aset baru ID dan GUID tersendiri. Pertahankan GUID ketika memperbarui aset yang sudah terhubung.
- `Assets/00_Rush/01_Unit/00_Hero/Allain/Deck_Allain.asset` dirujuk oleh `m_HeroDeckConfig`. Di dalam deck, `Cards/Card_Allain_ParallaxBolt.asset` memiliki empat entri `SkillUp`. Periksa referensi tiap entri setelah menduplikasi kartu. Untuk passive, ikuti `Cards/Card_Allain_AegisConversion.asset` ke `Skills/Skill_Allain_AegisConversion.asset` dan `Abilities/Ability_Allain_AegisConversion.asset`.
- `Platforms/Platform_Allain_AnomalyIncursion.asset` memakai skill perfect yang sama di sisi kiri dan kanan. Untuk skill yang levelnya dikendalikan kartu, lihat `m_SkillsWithIndependentLevels` agar registrasi kedua sisi mempertahankan level awal. Contoh enemy summon ada di `Entities/Minion_AnomalyEntity.asset` dan `Entities/AnomalyEntity.prefab`. Semua path pendek di bagian ini berada di folder hero Allain.

## Character selection, gacha, dan pengujian

| Kebutuhan | Aset dan sambungannya |
|---|---|
| Hero muncul dan bisa dipilih | `Assets/00_Rush/00_Player/Prefabs/Player (Deprecated).prefab` menyimpan daftar `m_HeroUnits` dengan entri `m_HeroConfig`. Periksa status kepemilikan, pemilihan dan pemakaian hero pada profil uji. |
| Banner terlihat dan memberi hero | `Assets/00_Rush/00_Player/Prefabs/Rush Player.prefab` memuat `GachaHandler.m_Banners`. Acuan banner ada di `Assets/00_Rush/01_Unit/00_Hero/Allain/Gacha/Banner_Allain.asset`. Reward `GachaCollect_Allain.asset` mengarahkan `m_Collect` ke hero. Cocokkan reward, pity, dan informasi yang tampil. |
| Jalur pemain | `Assets/00_Rush/SceneController/Scenes/Aether_Home (Used).unity` menggunakan kedua prefab player tersebut. Periksa override instance jika hasil di Home berbeda dari prefab. |
| Uji mekanik terisolasi | `Assets/00_Rush/01_Unit/00_Hero/Allain/Allain_LogicValidation.unity` dan `Scripts/AllainRuntimeValidation.cs` di folder Allain menunjukkan cara menguji targeting, stack, efek turunan, dan upgrade kartu dengan stat tetap. Ambil kasus yang relevan untuk hero baru, lalu verifikasi jalur pemain dari Home. |

