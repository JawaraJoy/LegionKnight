# Menelusuri sprite dari data sampai layar

Path `Assets/` dihitung dari root Unity LegionKnight. Untuk mencari pemakaian langsung GUID lama atau baru, jalankan helper dari root tersebut dengan Python 3 dan `rg` di PATH. Perintah berikut mencari ikon UI Spray Dagger milik Kael.

```powershell
python ".agents/skills/aetherjump-revamp-hero/scripts/find_sprite_uses.py" "Assets/00_Rush/01_Unit/00_Hero/Kael/SkillIcons/Icon_Kael_Platform_SprayDagger_v1.png"
```

Helper menerima beberapa path aset, path `.meta`, atau GUID sekaligus. Dari workspace pendamping, gunakan path lengkap helper dan `--project` dengan root Unity. Hasil JSON menunjukkan deklarasi GUID dan setiap baris pemakaian di file teks dalam `Assets`, `Packages`, serta `ProjectSettings`. Baris menyertakan `fileID` bila diserialisasi. Cari juga nama sprite, ID konten, serta key Addressables atau `Resources.Load` jika ada pemuatan melalui string. Telusuri referensi antarkonfigurasi untuk menemukan pemakai tidak langsung seperti hadiah yang menunjuk ke kartu.

## Kolom menurut tampilan

| Yang diperiksa | Jalur yang menentukan gambar |
| --- | --- |
| Skill hero | `HeroUnitConfig.m_Skills` → `SkillConfig.m_CollectibleField.m_Icon` dan `m_AbilitySets` → `AbilityConfig.m_Icon`. Baca peran setiap stage dan skill turunan sebelum memetakan ikon. |
| Deck | `m_HeroDeckConfig` → `m_CardConfigs`. Jalankan `DefaultCardItemView.Setup` dengan kartu hero dan periksa `m_CardIcon.sprite`. |
| Gacha kartu | Ikuti `GachaCollectableConfig.m_Collect` ke kartu. `GachaPreviewItemUI` dan `GachaDrawResultCard` mengutamakan `CollectibleField.SplashImage` lalu memakai `Icon` jika kosong. Uji keduanya dengan kartu contoh dari prefab UI yang terpasang. Cocokkan ilustrasi hadiah dengan desain kartu. |
| Platform | Pada `PlatformConfig`, `m_CollectibleField.m_Icon` adalah gambar UI. `Platform2D.IniPlatform()` memasang `m_CollectibleField.m_SplashImage` ke `m_SpriteView` di arena. Gambar hadiah gacha berasal dari data kartu tersendiri. |
| Status | Ikuti `StatusEffectConfig.m_Icon` dan `m_EffectorPrefab` sampai ke `Image.sprite` pada instance. Periksa gambar dasar/isian, tint, dan indikator pada stack yang relevan. Jika prefab dasar dipakai efek lain, pasang override pada varian hero sesuai kebutuhan. |
| Scene dan Legacy | Untuk `objectReference` di override prefab, lihat `propertyPath`, objek target, dan prefab sumber. Cek nama lama hero serta sprite yang dipasang langsung di scene. Jika skrip Legacy tidak dapat dimuat, catat hasil pemeriksaan GUID dan fileID pada YAML. |

## Contoh keputusan dari proyek

- **Kael.** Assault Dagger memakai satu ikon untuk empat stage. Death Sign dan Spray Dagger Break memiliki indikator sendiri. `AbilityView` memilih skill pertama dalam tiap kategori. Periksa judul, ikon, dan deskripsi pasif yang dibuka pemain, terutama bila ada serangan susulan internal dalam kategori yang sama. Kartu Spray Dagger pernah tampak benar di deck tetapi menampilkan gambar pijakan di gacha karena `SplashImage` kartunya masih lama. Platform battle kini menampilkan kipas belati perak yang mengikuti serangan Perfect dan panel V dari masker Kael.
- **Liura.** Empat `Ability_MagicMissileStage` pernah memakai placeholder Fire Ball. Peran proyektil menentukan ikon Magic Missile. Ultimate Fire Ball mendapat ikonnya sendiri. Pita yang naik ke wajik ungu pada platform Mana Charge menggambarkan pemulihan mana saat Perfect. Data karakter Legacy-nya memakai nama `Lily`.
- **Pembanding visual.** Di workspace pendamping lihat `HeroAssets/Kael/Platform/Kael_Allain_Stack_UnityEditor.png` untuk hasil pratinjau Editor dan `HeroAssets/Liura/Platform/Liura_Allain_Stack_PlayMode.png` untuk tangkapan Play Mode. Riwayat pemetaan ikon ada di `HeroAssets/Kael/SkillIcons/V1/Audit_Pemakaian_Ikon_Kael.md` dan `HeroAssets/Liura/SkillIcons/V1/Integration/result.json`.
