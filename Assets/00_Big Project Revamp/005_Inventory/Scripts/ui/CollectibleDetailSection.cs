using LegionKnight;
using UnityEngine;

namespace Rush
{
    /// <summary>
    /// Abstract base untuk setiap "section" di dalam CollectibleInformationPanel.
    ///
    /// Pola kerja
    /// ──────────
    ///  CollectibleInformationPanel menyimpan List&lt;CollectibleDetailSection&gt;.
    ///  Saat player memilih sebuah item dan menekan tombol detail, panel memanggil
    ///  Bind(entry) pada setiap section.  Masing-masing section memutuskan sendiri
    ///  apakah dirinya relevan untuk entry tersebut:
    ///    - IsRelevantFor() == true  → Show(), lalu OnBind() dipanggil untuk isi data.
    ///    - IsRelevantFor() == false → Hide(), section tidak terlihat di UI.
    ///
    /// Cara membuat section baru
    /// ─────────────────────────
    ///  1. Turunkan dari CollectibleDetailSection
    ///  2. Override IsRelevantFor() → kembalikan true hanya untuk tipe entry yang sesuai
    ///  3. Override OnBind()        → isi Text/Image komponen dari data entry
    ///  4. Tambahkan ke m_DetailSections di CollectibleInformationPanel via Inspector
    /// </summary>
    public abstract class CollectibleDetailSection : UIView
    {
        /// <summary>
        /// Apakah section ini relevan untuk entry yang diberikan?
        /// Jika false, section akan di-Hide() secara otomatis.
        /// </summary>
        public abstract bool IsRelevantFor(ICollectibleEntry entry);

        /// <summary>
        /// Isi data dari entry ke komponen UI.
        /// Dipanggil hanya setelah IsRelevantFor() mengembalikan true.
        /// </summary>
        protected abstract void OnBind(ICollectibleEntry entry);

        /// <summary>
        /// Panggil dari CollectibleInformationPanel untuk setiap entry yang dipilih.
        /// </summary>
        public void Bind(ICollectibleEntry entry)
        {
            BindInternal(entry);
        }

        protected virtual void BindInternal(ICollectibleEntry entry)
        {
            if (entry == null)
            {
                HideInternal();
                return;
            }

            if (!IsRelevantFor(entry))
            {
                HideInternal();
                return;
            }

            OnBind(entry);
            ShowInternal();
        }
    }
}