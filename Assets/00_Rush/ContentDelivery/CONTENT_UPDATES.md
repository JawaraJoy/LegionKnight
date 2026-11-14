# Pembaruan konten AetherJump

## Batas dan kompatibilitas

Setiap AAB tetap harus kurang dari **100.000.000 byte**. Gambar mempertahankan sumber, resolusi, dan kualitas impor; aset baru masuk bundle CCD. Guard build Android menolak AAB yang mencapai atau melebihi batas tersebut.

Loader baru memerlukan satu pembaruan aplikasi awal. Setelah terpasang, aplikasi memeriksa manifest channel sebelum membuka scene game. Manifest menunjuk katalog dan bundle versi immutable; hanya pointer channel yang berpindah ke release kompatibel. Katalog tidak diganti ketika game sedang berjalan.

Setiap keluarga aplikasi memiliki ID kompatibilitas sendiri. Penerbitan memeriksa fingerprint kode runtime, plugin, Unity/paket, define Android, arsitektur, serta tipe konten yang disertakan dalam baseline. Kode/mekanik/tipe baru yang belum tersedia pada aplikasi memerlukan AAB baru. Metadata kompatibilitas disimpan di `ContentDelivery/Editor/Compatibility` dan tidak masuk player.

Grup default Addressables diarahkan ke grup remote `CC`, termasuk bundle shader/builtin dan MonoScript bersama. Dengan demikian, katalog konten baru tidak bergantung pada bundle lokal baru yang belum ada pada AAB pemain. Tipe script yang belum dikompilasi dalam player tetap memerlukan update aplikasi.

## Update gambar atau hero yang kompatibel

1. Integrasikan aset dan konfigurasinya. Untuk hero, perbarui daftar `HeroesCollection` pada scene sumber beserta referensi UI, pool/gacha, deck, dan prefab yang dibutuhkan. Tersedianya sebuah bundle saja tidak otomatis menambahkan hero ke daftar permainan.
2. Pertahankan semua ID hero yang sudah diterbitkan agar data pemain tetap terbaca. Nama tampilan atau gambar boleh diperbarui tanpa mengganti ID hero.
3. Jalankan `CcdUpdateWorkflow.PrepareCompatibleContent("content-YYYYMMDD-rN")` dengan nama versi baru. Ini menyalin scene sumber terbaru ke scene remote dan memastikan `UnityService.InitCondition = Build` pada salinan produksi.
4. Jalankan `ScheduleBuild(version)` dan tunggu `Logs/CCD/live-update-status.txt` menunjukkan `Succeeded`. Hindari menjalankan test runner atau build lain bersamaan.
5. Jika tugas mencakup penerbitan, jalankan `Publish(version)` dan tunggu task selesai. Verifikasi URL `content.json`, katalog, dan bundle. Tidak perlu membangun atau mengunggah AAB untuk update yang kompatibel ini.

API Editor berada di namespace `Rush.Editor.ContentDelivery`. `PrepareCompatibleContent` dan `ScheduleBuild` adalah metode instance. `Publish` mengembalikan `Task<string>`. Sumber helper: `Assets/00_Rush/ContentDelivery/Editor/CcdUpdateWorkflow.cs`.

## Keluarga aplikasi baru

Untuk perubahan kode atau tipe yang memerlukan aplikasi baru, gunakan `PreparePlayer(contentVersion, versionCode)` dengan version code baru. Bangun konten baseline terlebih dahulu, lalu AAB. Guard menyimpan hasil AAB yang benar-benar telah diperiksa ukurannya. Setelah itu, terbitkan baseline/channel CCD sebelum mendistribusikan aplikasinya. Pengunggahan ke Play Console dilakukan hanya ketika tugas pengguna mencakup rilis aplikasi.

Simpan kontrak kompatibilitas bersama proyek. Jangan menyunting fingerprint, daftar tipe, atau hasil validasi AAB untuk melewati pemeriksaan. Gunakan baseline yang dibekukan ketika membangun ulang player pada keluarga tersebut.

## Fallback dan rollback

- Manifest tidak kompatibel, tidak tersedia, atau rusak: gunakan katalog terakhir yang berhasil, atau katalog baseline.
- Katalog baru harus lolos hash SHA-256, pemeriksaan scene/label, dan unduhan dependensi sebelum diaktifkan.
- Pointer last-good ditulis setelah scene berhasil dibuka. File backup pointer dipertahankan untuk pemulihan penulisan yang terputus.
- Kegagalan pembaruan dapat memakai konten lama yang seluruh bundlenya masih tersimpan; koneksi untuk autentikasi Play Games tetap mengikuti alur login aplikasi.
- Rollback tidak boleh menghilangkan hero yang sudah diterbitkan. Koreksi konten melalui release baru yang mempertahankan ID hero tersebut.
- Jangan menimpa badge versi immutable atau release yang dipakai aplikasi lama. Channel kompatibel sengaja dapat berpindah; badge versi konten tetap menunjuk snapshot yang sama.

Validasi logika tersedia pada `Rush.Tests.ContentUpdateTests`. Uji perangkat Android tetap diperlukan untuk tampilan, login Play Games, dan perilaku hero aktual.
