HAUNTED TOWER — SPINE PREVIEW V1

Tetap khusus preview. Tidak dipasang ke gameplay, build scene, VerticalLoopView, stage config, atau foreground lama.

Lihat:
- HauntedTower_AnimatedPreview.prefab: susunan dinding dan 6 dekorasi dengan runtime Spine 4.3.
- HauntedLantern_Animated.prefab, HauntedBanner_Animated.prefab, HauntedResident_Animated.prefab: prop terpisah.
- HauntedTower_SpinePreview.gif: render 8 detik dari runtime Spine dalam preview scene Unity terisolasi.
- Pilih salah satu *_SkeletonData.asset, lalu pada Inspector bagian Preview tekan tombol play di samping nama animasi.
- GIF dapat dibuka sebagai file untuk melihat seluruh susunan bergerak tanpa masuk Play Mode.

Klip:
HauntedLantern: Sway (4 detik, loop). Pengait tetap; rantai dan badan berayun.
HauntedBanner: Flutter (4 detik, loop). Palang tetap; deformasi mesh dari kain atas ke ujung.
HauntedResident: Haunt (8 detik, loop), Blink (1 detik), Peek (3 detik).
Bingkai dan jeruji tetap. Penghuni berada di layer belakang, di-clip ke bukaan jendela, bergerak, berkedip, memudar dan kembali.

Format:
SkeletonDataAsset memakai *.source-json.txt (JSON Spine 4.3). Nama sengaja tidak memakai .skel agar importer runtime mengenalinya sebagai JSON.
Binary ekspor *.spine-data.bytes juga tersedia dan sudah divalidasi lewat SkeletonBinary; file ini bukan referensi aktif SkeletonDataAsset.
Atlas memakai *.atlas-data.txt dengan material/texture yang ditautkan langsung agar tidak memicu wizard import atlas otomatis.
Sumber JSON standar, .skel dan .atlas tersedia di workspace pendamping: HeroAssets/Environment/HauntedTower/SpinePreviewV1/Exports.
Bisa diimpor sebagai data skeleton di Spine; ini bukan file proyek .spine.

Verifikasi:
Unity MCP, 325 sampel dari lima klip, 366600 koordinat mesh finite.
Selisih JSON/binary maksimum 0; drift vertex pangkal 0; selisih pose awal/akhir loop 0.
Render 80 frame pada 10 fps. Kamera portrait FOV60, jarak37. Pemeriksaan akhir juga mencakup Boost jarak42.
Rigging, validasi 325 sampel dan render GIF dilakukan pada preview scene dalam Edit Mode. Pemeriksaan akhir kamera Boost/Wide tetap memakai preview scene; saat itu Editor sudah berpindah ke Demo/Play Mode oleh pekerjaan lain. Tugas ini tidak memulai/menghentikan Play Mode atau mengganti scene aktif.

Layer jeruji kosong dan penghuni dibuat menggunakan built-in image_gen. LayerGenerationPrompts.json menyimpan promptnya.

