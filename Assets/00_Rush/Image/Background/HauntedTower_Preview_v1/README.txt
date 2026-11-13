HAUNTED TOWER — PREVIEW V1

Status: aset preview statis, belum dipasang ke gameplay dan belum dibuat animasi Spine.

Buka HauntedTower_Preview.prefab dengan double click untuk melihat susunan di Prefab Mode.
Lihat HauntedTower_Camera_Idle.png dan HauntedTower_Camera_Boost.png untuk preview portrait 540x1170, FOV 60, jarak 37/42.
HauntedTower_Camera_Wide.png hanya memperlihatkan cakupan lebih lebar (rasio 1200:1170), bukan format build.

Aset:
- HauntedTower_Wall_v1.png: modul dinding opaque 1254x1254.
- HauntedTower_Lantern_v1.png: lentera + pengait, alpha, 1024x1536.
- HauntedTower_Banner_v1.png: banner + palang, alpha, 1024x1536.
- HauntedTower_BarredResident_v1.png: penghuni dalam jendela berjeruji, alpha, 1536x1024.
- HauntedTower_ApprovedConcept.png: mockup arah desain.
- Material unlit dan prefab khusus preview.

Prefab berisi 25 modul dinding dan 6 instance dekorasi (masing-masing 2).
Dinding dirangkai untuk menilai tampilan dan cakupan; ketepatan sambungan piksel serta looping runtime belum disertifikasi untuk produksi.
Dekorasi masih berupa sprite utuh; pemisahan bagian, rig, dan animasi Spine belum dibuat.
Original foreground/base, HomeBackground, VerticalLoopView, config dan prefab gameplay tidak diganti.
Preview dirender dalam scene preview terisolasi melalui Unity MCP tanpa Play Mode.
Sumber raster dibuat menggunakan built-in image_gen. Prompt lengkap: generation-prompts.json.
